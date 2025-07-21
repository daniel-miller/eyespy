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

            // Get all top-level classes
            var topLevelClasses = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(IsTopLevelClass)
                .ToList();

            // Report diagnostic for each class after the first one
            if (topLevelClasses.Count > 1)
            {
                for (int i = 1; i < topLevelClasses.Count; i++)
                {
                    var cls = topLevelClasses[i];
                    var location = cls.Identifier.GetLocation();
                    context.ReportDiagnostic(Diagnostic.Create(Rule, location));
                }
            }
        }

        private static bool IsTopLevelClass(ClassDeclarationSyntax classDeclaration)
        {
            // A class is top-level if its parent is:
            // 1. CompilationUnitSyntax (directly in file, no namespace)
            // 2. NamespaceDeclarationSyntax (traditional block namespace)
            // 3. FileScopedNamespaceDeclarationSyntax (file-scoped namespace)
            return classDeclaration.Parent is CompilationUnitSyntax ||
                   classDeclaration.Parent is NamespaceDeclarationSyntax ||
                   classDeclaration.Parent is FileScopedNamespaceDeclarationSyntax;
        }
    }
}