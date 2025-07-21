using System.Threading.Tasks;

namespace TestDiagnostics;

public class MissingAsyncSuffix
{
    public static async Task Execute()
    {
        await Task.CompletedTask;
    }
}
