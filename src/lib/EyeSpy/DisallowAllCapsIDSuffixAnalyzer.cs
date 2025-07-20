using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace EyeSpy
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DisallowAllCapsIDSuffixAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SPY02";
        private const string Title = "Avoid 'ID' suffix; use 'Id'";
        private const string MessageFormat = "Identifier '{0}' ends with 'ID'; use 'Id' instead";
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId, Title, MessageFormat, Category,
            DiagnosticSeverity.Error, isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            // Fields, parameters, and properties can still be analyzed as symbols
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Field, SymbolKind.Parameter, SymbolKind.Property);

            // Locals must be analyzed via syntax
            context.RegisterSyntaxNodeAction(AnalyzeLocalVariable, SyntaxKind.VariableDeclarator);
        }

        private static void AnalyzeLocalVariable(SyntaxNodeAnalysisContext context)
        {
            var declarator = (VariableDeclaratorSyntax)context.Node;

            var symbol = context.SemanticModel.GetDeclaredSymbol(declarator);

            if (symbol != null && symbol.Name.EndsWith("ID"))
            {
                var diagnostic = Diagnostic.Create(Rule, declarator.GetLocation(), symbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var name = context.Symbol.Name;

            if (name.EndsWith("ID"))
            {
                var diagnostic = Diagnostic.Create(Rule, context.Symbol.Locations[0], name);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}