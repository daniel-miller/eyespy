using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace EyeSpy
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsyncMethodAwaitAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SPY05"; // Mimic CS4014

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Async method call must be awaited",
            messageFormat: "Method '{0}' returns a Task but the await operator is missing",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Async method calls that return Task or Task<T> should be awaited to ensure proper async behavior.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            var semanticModel = context.SemanticModel;

            // Get the symbol info for the invoked method
            var symbolInfo = semanticModel.GetSymbolInfo(invocation);
            if (!(symbolInfo.Symbol is IMethodSymbol method))
                return;

            // Check if the method returns Task or Task<T>
            if (!IsTaskType(method.ReturnType))
                return;

            // Check if this invocation is already awaited
            if (IsAwaited(invocation))
                return;

            // Check if this is in a context where awaiting is not possible/necessary
            if (IsInValidNonAwaitContext(invocation, semanticModel))
                return;

            // Report diagnostic
            var methodName = GetMethodDisplayName(method);
            var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation(), methodName);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsTaskType(ITypeSymbol returnType)
        {
            if (returnType == null)
                return false;

            // Check for Task
            if (returnType.Name == "Task" && returnType.ContainingNamespace?.ToDisplayString() == "System.Threading.Tasks")
                return true;

            // Check for Task<T>
            if (returnType is INamedTypeSymbol namedType &&
                namedType.IsGenericType &&
                namedType.Name == "Task" &&
                namedType.ContainingNamespace?.ToDisplayString() == "System.Threading.Tasks")
                return true;

            return false;
        }

        private static bool IsAwaited(InvocationExpressionSyntax invocation)
        {
            var parent = invocation.Parent;

            // Direct await
            if (parent is AwaitExpressionSyntax)
                return true;

            // Check for method chaining (e.g., await SomeAsync().ConfigureAwait(false))
            while (parent is MemberAccessExpressionSyntax memberAccess && memberAccess.Expression == invocation)
            {
                parent = memberAccess.Parent;
                if (parent is InvocationExpressionSyntax chainedInvocation)
                {
                    parent = chainedInvocation.Parent;
                    if (parent is AwaitExpressionSyntax)
                        return true;
                }
            }

            return false;
        }

        private static bool IsInValidNonAwaitContext(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            var parent = invocation.Parent;

            // Check if it's being returned directly from an async method
            if (parent is ReturnStatementSyntax returnStatement)
            {
                var containingMethod = returnStatement.FirstAncestorOrSelf<MethodDeclarationSyntax>();
                if (containingMethod != null)
                {
                    var methodSymbol = semanticModel.GetDeclaredSymbol(containingMethod);
                    if (methodSymbol is IMethodSymbol method && IsTaskType(method.ReturnType))
                        return true;
                }

                // Also check for local functions and lambda expressions
                var containingLocalFunction = returnStatement.FirstAncestorOrSelf<LocalFunctionStatementSyntax>();
                if (containingLocalFunction != null)
                {
                    var localFunctionSymbol = semanticModel.GetDeclaredSymbol(containingLocalFunction);
                    if (localFunctionSymbol is IMethodSymbol localMethod && IsTaskType(localMethod.ReturnType))
                        return true;
                }
            }

            // Check if it's being assigned to a Task variable (fire-and-forget scenarios)
            if (parent is EqualsValueClauseSyntax equalsValue &&
                equalsValue.Parent is VariableDeclaratorSyntax variableDeclarator)
            {
                var variableDeclaration = variableDeclarator.Parent as VariableDeclarationSyntax;
                if (variableDeclaration != null)
                {
                    var typeInfo = semanticModel.GetTypeInfo(variableDeclaration.Type);
                    if (IsTaskType(typeInfo.Type))
                        return true;
                }
            }

            // Check if it's being passed as an argument where Task is expected
            if (parent is ArgumentSyntax argument)
            {
                var argumentList = argument.Parent as ArgumentListSyntax;
                var invocationParent = argumentList?.Parent as InvocationExpressionSyntax;
                if (invocationParent != null)
                {
                    var symbolInfo = semanticModel.GetSymbolInfo(invocationParent);
                    if (symbolInfo.Symbol is IMethodSymbol parentMethod)
                    {
                        var argumentIndex = argumentList.Arguments.IndexOf(argument);
                        if (argumentIndex >= 0 && argumentIndex < parentMethod.Parameters.Length)
                        {
                            var parameterType = parentMethod.Parameters[argumentIndex].Type;
                            if (IsTaskType(parameterType))
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        private static string GetMethodDisplayName(IMethodSymbol method)
        {
            if (method.ContainingType != null)
                return $"{method.ContainingType.Name}.{method.Name}";

            return method.Name;
        }
    }
}