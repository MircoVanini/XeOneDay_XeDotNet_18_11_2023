using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Text;


public partial class Program
{
    static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

    // The Project Gutenberg eBook of The Adventures of Sherlock Holmes, by Arthur Conan Doyle
    //
    private static readonly string s_haystack = new HttpClient().GetStringAsync("http://aleph.gutenberg.org/1/6/6/1661/1661-0.txt").Result;

    [Benchmark]
    [Arguments("Sherlock")]
    [Arguments("elementary")]
    public int Count(string needle)
    {
        ReadOnlySpan<char> haystack = s_haystack;
        int count = 0, pos;
        while ((pos = haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase)) >= 0)
        {
            haystack = haystack[(pos + needle.Length)..];
            count++;
        }
        return count;
    }


    [Params(StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase)]
    public StringComparison Comparison { get; set; }

    [Params("elementary")]
    public string Needle { get; set; } = string.Empty;

    [Benchmark]
    public int CountComparison()
    {
        int count = 0, pos = 0;
        while ((pos = s_haystack.IndexOf(Needle, pos, Comparison)) >= 0)
        {
            pos += Needle.Length;
            count++;
        }

        return count;
    }

    private readonly byte[] _data = new byte[98];
    [Benchmark]
    public bool Contains() => _data.AsSpan().Contains((byte)1);


    private readonly int[] _dataInt = new int[10240];
    [Benchmark]
    public int IndexOf() => _dataInt.AsSpan().IndexOf(42);


    const string Sonnet = """
    Shall I compare thee to a summer's day?
    Thou art more lovely and more temperate:
    Rough winds do shake the darling buds of May, And summer's lease hath all too short a date; Sometime too hot the eye of heaven shines,
    And often is his gold complexion dimm'd;
    And every fair from fair sometime declines,
    By chance or nature's changing course untrimm'd; But thy eternal summer shall not fade,
    Nor lose possession of that fair thou ow'st;
    Nor shall death brag thou wander'st in his shade, When in eternal lines to time thou grow'st:
    So long as men can breathe or eyes can see,
    So long lives this, and this gives life to thee. 
    """;

    private readonly StringBuilder _builder = new(Sonnet);
    [Benchmark]
    public void Replace()
    {
        _builder.Replace('?', '!');
        _builder.Replace('!', '?');
    }

    [Benchmark]
    [Arguments("http://microsoft.com")]
    public bool StartsWith(string text) =>
        text.StartsWith("https://",
            StringComparison.OrdinalIgnoreCase);

    [Benchmark]
    [Arguments("http://microsoft.com")]
    public bool OpenCoded(string text) =>
        text.Length >= 8 &&
        (text[0] | 0x20) == 'h' &&
        (text[1] | 0x20) == 't' &&
        (text[2] | 0x20) == 't' &&
        (text[3] | 0x20) == 'p' &&
        (text[4] | 0x20) == 's' &&
        text[5] == ':' &&
        text[6] == '/' &&
        text[7] == '/';

}