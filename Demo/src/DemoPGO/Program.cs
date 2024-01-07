using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Runtime.CompilerServices;

[DisassemblyDiagnoser]
[HideColumns("Error", "StdDev", "Median", "RatioSD")]
public partial class Program
{
    static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}



[HideColumns("Error", "StdDev", "Median", "RatioSD")]
[DisassemblyDiagnoser(maxDepth: 0)]
public class Tests
{
    internal interface IValueProducer
    {
        int GetValue();
    }

    class Producer42 : IValueProducer
    {
        public int GetValue() => 42;
    }
    private IValueProducer? _valueProducer;
    private readonly int _factor = 2;

    [GlobalSetup]
    public void Setup() => _valueProducer = new Producer42();


    // Una delle principali ottimizzazioni dei feed PGO dinamici è la capacità di devirtualizzare le chiamate virtuali
    // e di interfaccia per sito di chiamata. Come notato, il JIT tiene traccia dei tipi concreti utilizzati e quindi può
    // generare un percorso rapido per il tipo più comune; questo è noto come devirtualizzazione protetta (GDV).

    [Benchmark]
    public int GetValue() => _valueProducer!.GetValue() * _factor;

    [Benchmark]
    public int GetValueDGV1() => DGV1();

    [Benchmark]
    public int GetValueDGV2() => DGV2();

    // .NET 8 introduce un nuovo meccanismo utilizzato dal JIT il Non-GC Heap (un'evoluzione del vecchio concetto di "Frozen Segments"
    // utilizzato da Native AOT). Il JIT può garantire che gli oggetti rilevanti siano allocati sull'heap non GC, che, come suggerisce il nome,
    // non è gestito dal GC ed è destinato a memorizzare oggetti in cui il JIT può dimostrare che l'oggetto non ha riferimenti di cui 
    // GC deve essere a conoscenza e sarà rootato per tutta la durata del processo

    [Benchmark]
    public string GetPrefix() => "https://www.microsoft.com";

    [Benchmark]
    public Type GetTestsType() => typeof(Tests);


    private int DGV1()
    {
        int result = _valueProducer!.GetType() == typeof(Producer42) ?
                        Unsafe.As<Producer42>(_valueProducer).GetValue() :
                        _valueProducer.GetValue();

        return result * _factor;
    }

    private int DGV2()
    {
        int result = _valueProducer!.GetType() == typeof(Producer42) ?
                        42 :
                        _valueProducer.GetValue();

        return result * _factor;
    }
}
