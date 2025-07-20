# EyeSpy C# Code Analyzer

This is a small set of custom Roslyn analyzers to enforce coding standards for C# projects. These analyzers provide compile-time errors to help maintain code quality and consistency.

## Analyzers Included



## Installation

1. **Add as NuGet Package Reference:**
   Add the analyzer project as a package reference to your target project:
   ```xml
   <PackageReference Include="EyeSpy" Version="1.0.0">
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
[SuppressMessage("Naming", "SPY02:Avoid 'ID' suffix; use 'Id'", 
    Justification = "Legacy API compatibility")]
public string LegacyUserID { get; set; }
```

## Requirements

- **.NET Standard 2.0** or higher
- **Microsoft.CodeAnalysis.CSharp 4.13.0**
- **Microsoft.CodeAnalysis.Analyzers 3.11.0**

## Development

To extend or modify these analyzers:

1. Clone the repository
2. Open in Visual Studio or your preferred IDE
3. Add new analyzer classes following the existing patterns
4. Update diagnostic IDs (use SPYXX format)
5. Add corresponding tests
6. Update this README with new analyzer documentation

## Testing

Test your analyzers using the Microsoft.CodeAnalysis.Testing framework or by adding them to a test project and verifying the expected diagnostics are generated.

## License

This project is licensed under the MIT License - see the LICENSE file for details.