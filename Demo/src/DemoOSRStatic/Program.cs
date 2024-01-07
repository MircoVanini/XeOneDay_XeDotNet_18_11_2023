using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[DisassemblyDiagnoser]
[HideColumns("Error", "StdDev", "Median", "RatioSD")]
public partial class Program
{
    static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

    private static readonly int s_year = DateTime.UtcNow.Year;
    
    [Benchmark]
    public int Compute()
    {
        int result = 0;

        for (int i = 0; i < 1_000_000; i++)
        {
            result += i;
            if (s_year == 2021)
            {
                result += i;
            }
        }

        return result; 
    }
}