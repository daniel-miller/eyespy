using System.Threading.Tasks;

namespace TestDiagnostics;

public class UnawaitedAsync
{
    public void Execute()
    {
        // This line should trigger SPY05 because the await operator is missing.
        ExecuteAsync();
    }

    public async Task ExecuteAsync()
    {
        await Task.CompletedTask;
    }
}
