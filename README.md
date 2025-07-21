# EyeSpy: A static code analysis library for C#

This is a small set of custom Roslyn analyzers to enforce coding standards for C# projects. These analyzers provide compile-time errors to help maintain code quality and consistency.

## Diagnostic Analyzers

Here is a list of the analyzers implemented in this library.

---

### SPY01: Disallow multiple top-level classes in the same file

Each C# file should contain **only one top-level class declaration**. Placing multiple classes in a single file can lead to maintainability issues and violates widely accepted conventions.

**Example:**

```csharp
public class FirstClass { }

public class SecondClass { } // ❌ Move SecondClass to a separate file
```

Keeping one class per file improves traceability and simplifies version control diffs—especially in large teams or open-source projects.

---

### SPY02: Disallow "ID" as an identifier name suffix

Use `Id` instead of `ID` to comply with .NET naming conventions. This enhances readability and aligns with idiomatic C# standards.

**Examples:**

```csharp
public int UserID { get; set; }        // ❌ Should be UserId
private string customerID;             // ❌ Should be customerId
void ProcessOrder(int orderID) { }     // ❌ Should be orderId
```

This rule is cosmetic but contributes to cleaner, more uniform code—especially beneficial in APIs, domain models, and shared libraries.

---

### SPY03: Async method names must have Async suffix

Async methods **must end with the `Async` suffix**. This is a standard convention that clearly signals asynchronous behavior to developers.

**Examples:**

```csharp
public async Task ProcessData() { }      // ❌ Should be ProcessDataAsync
public async Task<string> GetUser() { }  // ❌ Should be GetUserAsync
```

This convention helps prevent confusion when synchronous and asynchronous methods coexist.

---

### SPY04: Namespace hierarchy must align to folder structure

A file’s namespace should reflect its folder structure to support intuitive project organization and navigation.

**Examples:**

Assume the file path is:

```
C:\Projects\MyApp\Controllers\Api\UserController.cs
```

Valid namespaces:

```csharp
namespace MyApp.Controllers.Api    // ✅
namespace MyApp.Controllers        // ✅
namespace MyApp                    // ✅
```

Invalid examples:

```csharp
namespace Controllers.Api          // ❌ Missing project root
namespace Foo.Bar                  // ❌ Entirely mismatched
namespace MyApp.Services           // ❌ Diverges from folder structure
```

This alignment improves onboarding, searchability, and long-term maintainability. While tools like VS Code and Visual Studio support similar rules (e.g., IDE0130), this rule allows for more flexibility in shallow namespace declarations.

---

### SPY05: Async calls must be awaited

Un-awaited async calls are a common source of bugs. Always `await` async methods to ensure proper exception handling and task completion.

**Example:**

```csharp
public async Task ProcessDataAsync()
{
    WorkAsync();               // ❌ Should be: await WorkAsync();
    var result = WorkAsync();  // ❌ Should be: var result = await WorkAsync();
}
```

This rule reduces the likelihood of dropped tasks and makes the async control flow explicit and reliable.

---

## Installation

1. **Add as NuGet Package Reference:**
   Add the analyzer project as a package reference to your target project:
   ```xml
   <PackageReference Include="EyeSpy" Version="1.0.5">
     <PrivateAssets>all</PrivateAssets>
     <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
   </PackageReference>
   ```

2. **Add as Project Reference:**
   Reference the analyzer project directly:
   ```xml
   <ProjectReference Include="path/to/EyeSpy.csproj">
     <OutputItemType>Analyzer</OutputItemType>
   </ProjectReference>
   ```

3. **Add as Analyzer Reference:**
   Reference the compiled analyzer assembly:
   ```xml
   <Analyzer Include="path/to/EyeSpy.dll" />
   ```

## Configuration

### Disabling Specific Analyzers

You can disable individual analyzers using the diagnostic ID:

**In .editorconfig:**
```ini
[*.cs]
dotnet_diagnostic.SPY01.severity = none  # Disable multiple classes check
dotnet_diagnostic.SPY02.severity = suggestion  # Change to suggestion
```

**In project file:**
```xml
<PropertyGroup>
  <NoWarn>$(NoWarn);SPY01;SPY02</NoWarn>
</PropertyGroup>
```

**Using pragma directives:**
```csharp
#pragma warning disable SPY01
public class FirstClass { }
public class SecondClass { } // No error
#pragma warning restore SPY01
```

### Suppressing Warnings

Use `SuppressMessage` attribute for specific suppressions:
```csharp
[SuppressMessage("Naming", "SPY02:Avoid 'ID' suffix; use 'Id'", Justification = "Legacy compatibility")]
public string LegacyUserID { get; set; }
```

## Requirements

- **.NET Standard 2.0** or higher
- **Microsoft.CodeAnalysis.CSharp 4.14.0**
- **Microsoft.CodeAnalysis.Analyzers 4.14.0**

## Development

To extend or modify these analyzers:

1. Clone the repository
2. Open in VS Code or your preferred IDE
3. Add new analyzer classes following the existing patterns
4. Update diagnostic codes (use SPYXX format)
5. Add corresponding tests
6. Update this README with new analyzer documentation

## Testing

Test your analyzers using the Microsoft.CodeAnalysis.Testing framework or by adding them to the TestDiagnostics project to verify the expected diagnostics are triggered.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.