using System.Text.RegularExpressions;

namespace Day3;

// Mull It Over
class Program {
    private static readonly Regex mulRgx = new(@"mul\((\d+),(\d+)\)");
    private static readonly Regex doRgx  = new(@"(?<command>(?:do)|(?:don't)|(?:mul))\((?:(?<n1>\d+),(?<n2>\d+))?\)");
    
    static void Main(string[] args) {
        var input = File.ReadAllText("../../../input.txt");

        // Console.WriteLine(IgnoringDo(input));
        Console.WriteLine(UsingDo(input));
    }

    static int IgnoringDo(string input) {
        var matches = mulRgx.Matches(input);

        var sum = 0;
        
        foreach (Match match in matches) {
            sum += int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value);
        }

        return sum;
    }

    static int UsingDo(string input) {
        var matches = doRgx.Matches(input);

        var enabled = true;
        var sum = 0;

        foreach (Match match in matches) {
            switch (match.Groups["command"].Value) {
                case "do": {
                    if (!match.Groups["n1"].Success) enabled = true;
                    continue;
                }
                case "don't": {
                    if (!match.Groups["n1"].Success) enabled = false;
                    continue;
                }
                case "mul": {
                    if (!enabled) continue;
                    if (!match.Groups["n1"].Success) continue;
                    sum += int.Parse(match.Groups["n1"].Value) * int.Parse(match.Groups["n2"].Value);
                    continue;
                }
                default: throw new InvalidOperationException("erm");
            }
        }

        return sum;
    }
}
