using System.Runtime.InteropServices.JavaScript;

namespace Day7;

class Program {
    static void Main(string[] args) {
        (long ans, long[] longs)[] input = File.ReadAllLines("../../../input.txt").Select(line => {
            var split = line.Split(':');
            var longs = split[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
            return (long.Parse(split[0]), ints: longs);
        }).ToArray();

        var sum = 0L;
        
        foreach (var (ans, longs) in input) {
            // if (IsSolvable(ans, longs, Operations1)) sum += ans;
            if (IsSolvable(ans, longs, Operations2)) sum += ans;
        }
        
        Console.WriteLine(sum);
    }

    public static bool IsSolvable(long ans, long[] longs, Func<long, long, long>[] ops) {
        return IsSolvable(ans, longs[1..], ops, longs[0]);
    }
    public static bool IsSolvable(long ans, long[] longs, Func<long, long, long>[] ops, long runningTotal) {
        if (runningTotal > ans) return false;
        if (longs.Length == 0) return runningTotal == ans;

        return ops.Any(op => IsSolvable(ans, longs[1..], ops, op(runningTotal, longs[0])));
    }

    public static readonly Func<long, long, long>[] Operations1 = [ 
        (x, y) => x + y, 
        (x, y) => x * y
    ];
    public static readonly Func<long, long, long>[] Operations2 = [
        (x, y) => x + y,
        (x, y) => x * y,
        (x, y) => long.Parse($"{x}{y}")
    ];
}
