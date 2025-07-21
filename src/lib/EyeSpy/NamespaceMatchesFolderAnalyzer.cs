using System;
using System.Collections.Immutable;
using System.IO;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace EyeSpy
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NamespaceMatchesFolderAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SPY04";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Namespace does not match folder structure",
            messageFormat: "Namespace '{0}' does not match folder structure, expected '{1}'",
            category: "Naming",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The declared namespace must match the folder structure. Partial match is allowed."
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeNamespace, SyntaxKind.NamespaceDeclaration, SyntaxKind.FileScopedNamespaceDeclaration);
        }

        private static void AnalyzeNamespace(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (BaseNamespaceDeclarationSyntax)context.Node;

            var declaredNamespace = namespaceDeclaration.Name.ToString();

            var syntaxTree = context.Node.SyntaxTree;

            var filePath = syntaxTree.FilePath;

            if (string.IsNullOrEmpty(filePath) || !Path.IsPathRooted(filePath))
                return;

            var projectDirectory = Path.GetDirectoryName(context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue("build_property.ProjectDir", out var dir) ? dir : null);

            if (projectDirectory == null || !filePath.StartsWith(projectDirectory))
                return;

            string folderName = Path.GetFileName(projectDirectory);

            var relativePath = Path.GetDirectoryName(
                filePath.Substring(projectDirectory.Length).TrimStart(Path.DirectorySeparatorChar)
                );

            var expectedNamespace = folderName;

            if (!string.IsNullOrEmpty(relativePath))
                expectedNamespace += "." + relativePath.Replace(Path.DirectorySeparatorChar, '.');

            if (!expectedNamespace.StartsWith(declaredNamespace))
            {
                var diagnostic = Diagnostic.Create(
                    descriptor: Rule,
                    location: namespaceDeclaration.Name.GetLocation(),
                    messageArgs: new[] { declaredNamespace, expectedNamespace }
                );
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
