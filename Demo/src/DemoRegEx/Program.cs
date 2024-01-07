
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Reflection;
using System.Text.RegularExpressions;

[DisassemblyDiagnoser]
[HideColumns("Error", "StdDev", "Median", "RatioSD")]
public partial class Program
{
    static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

    private static Regex s_regex = new Regex(@"[a-z]shing", RegexOptions.Compiled);

    private static string s_text = new HttpClient().GetStringAsync(@"https://github.com/rust-leipzig/regex-performance/blob/13915c5182f2662ed906cde557657037c0c0693e/3200.txt").Result;

    [Benchmark]
    public int SubstringSearch()
    {
        int count = 0;
        Match m = s_regex.Match(s_text);
        while (m.Success)
        {
            count++;
            m = m.NextMatch();
        }
        return count;
    }

    private static Regex s_email = new Regex(@"[\w.+-]+@[\w.-]+.[\w.-]+", RegexOptions.Compiled);

    [Benchmark]
    public int Email()
    {
        int count = 0;
        Match m = s_email.Match(s_text);
        while (m.Success)
        {
            count++;
            m = m.NextMatch();
        }
        return count;
    }
}
