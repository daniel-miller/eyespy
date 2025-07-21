using System.Threading.Tasks;

namespace TestDiagnostics;

public class ThirdClass
{
    public static void Foo() { }
}

// This should trigger SPY01 because FourthClass must be moved to a separate file.
public class FourthClass
{
    public static async Task BarAsync() { }
}