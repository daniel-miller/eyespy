using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace EyeSpy
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsyncMethodNamingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SPY03";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Async method name must end with 'Async'",
            messageFormat: "Method '{0}' is async but does not end with 'Async'",
            category: "Naming",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Async method names should end with 'Async' to follow convention.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
        }

        private static void AnalyzeMethod(SymbolAnalysisContext context)
        {
            var method = (IMethodSymbol)context.Symbol;

            // Skip non-async or non-ordinary methods (e.g., constructors, operators)
            if (!method.IsAsync || method.MethodKind != MethodKind.Ordinary)
                return;

            // Skip compiler-generated methods (e.g., top-level program entry points)
            if (method.Name.StartsWith("<") || method.IsImplicitlyDeclared)
                return;

            // Check if the method name ends with "Async"
            if (!method.Name.EndsWith("Async", StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(Rule, method.Locations[0], method.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
