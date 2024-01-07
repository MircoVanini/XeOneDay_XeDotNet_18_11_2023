
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Reflection;

[DisassemblyDiagnoser]
[HideColumns("Error", "StdDev", "Median", "RatioSD")]
public partial class Program
{
    static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

    private MethodInfo _zeroArgs = typeof(Program)!.GetMethod(nameof(ZeroArgsMethod))!;
    private MethodInfo _oneArg = typeof(Program)!.GetMethod(nameof(OneArgMethod))!;
    private object[] _args = new object[] { 42 };

    [Benchmark] public void InvokeZero() => _zeroArgs.Invoke(null, null);
    [Benchmark] public void InvokeOne() => _oneArg.Invoke(null, _args);

    public static void ZeroArgsMethod() { }
    public static void OneArgMethod(int i) { }

}
