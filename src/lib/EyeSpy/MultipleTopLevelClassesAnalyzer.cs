using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace EyeSpy
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MultipleTopLevelClassesAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SPY01";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            "Multiple top-level classes in file",
            "File contains multiple top-level class declarations",
            "Structure",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
        }

        private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);

            var topLevelClasses = root.Members
                .OfType<NamespaceDeclarationSyntax>()
                .SelectMany(ns => ns.Members.OfType<ClassDeclarationSyntax>())
                .Concat(
                    root.Members
                        .OfType<ClassDeclarationSyntax>() // for global-scope classes
                )
                .Where(cls => cls.Parent is NamespaceDeclarationSyntax || cls.Parent is CompilationUnitSyntax)
                .ToList();

            if (topLevelClasses.Count > 1)
            {
                var location = topLevelClasses[1].GetLocation(); // flag the second class

                context.ReportDiagnostic(Diagnostic.Create(Rule, location));
            }
        }
    }
}
