
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

[DisassemblyDiagnoser]
[HideColumns("Error", "StdDev", "Median", "RatioSD")]
public partial class Program
{
    static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

    private JsonSerializerOptions _options = new JsonSerializerOptions(); 
    private MyAmazingClass _instance = new MyAmazingClass();
    public class MyAmazingClass
    {
        public int Value { get; set; }
    }

    [Benchmark(Baseline = true)]
    public string ImplicitOptions() => JsonSerializer.Serialize(_instance);
    
    [Benchmark]
    public string WithCached() => JsonSerializer.Serialize(_instance, _options);
    
    [Benchmark]
    public string WithoutCached() => JsonSerializer.Serialize(_instance, new JsonSerializerOptions());   
}