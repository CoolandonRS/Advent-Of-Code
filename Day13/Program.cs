using System.Diagnostics;
using System.Numerics;
using System.Text.RegularExpressions;
using Common;
using Pos = (long x, long y);

namespace Day13;

class Program {
    // I love regex
    private static readonly Regex inputRgx = new(@"X\+(?<ax>\d+), Y\+(?<ay>\d+)\n.+?X\+(?<bx>\d+), Y\+(?<by>\d+)\n.+?X=(?<gx>\d+), Y=(?<gy>\d+)");
    
    static void Main(string[] args) {
        var input = File.ReadAllText("../../../input.txt");

        var tokens = 0L;

        var addToGoal = 10000000000000; // part1: 0, part2: 10000000000000

        var matches = inputRgx.Matches(input);
        foreach (var (match, idx) in matches.Select((match, idx) => (match, idx))) {
            tokens += SolveAlgebra(
                (long.Parse(match.Groups["ax"].Value), long.Parse(match.Groups["ay"].Value)),
                (long.Parse(match.Groups["bx"].Value), long.Parse(match.Groups["by"].Value)),
                (long.Parse(match.Groups["gx"].Value) + addToGoal, long.Parse(match.Groups["gy"].Value) + addToGoal)
            ) ?? 0;
        }
        Console.WriteLine(tokens);
    }
    
    // first attempt. Then I went "can I divide to get bFactor?"
    static long? SolveIterate(Pos aDiff, Pos bDiff, Pos goal) {
        var bFactor = 0;
        while (goal.EitherGreater(bDiff.Mul(++bFactor)));
        if (bDiff.Mul(bFactor) == goal) return bFactor;
        while (--bFactor > 0) {
            var newB = bDiff.Mul(bFactor);
            var aFactor = 0;
            while (goal.EitherGreater(newB.Add(aDiff.Mul(++aFactor))));
            if (newB.Add(aDiff.Mul(aFactor)) == goal) return bFactor + (aFactor * 3);
        }
        return null;
    }

    // Thus SolveMath was born. Looking at this though, I couldn't shake the feeling that this could be done with pure math.
    static long? SolveMath(Pos aDiff, Pos bDiff, Pos goal) {
        var bFactor = (long) Math.Ceiling(Math.Max((double) goal.x / bDiff.x, (double) goal.y / bDiff.y));
        if (bDiff.Mul(bFactor) == goal) return bFactor;
        while (--bFactor > 0) {
            var newGoal = goal.Sub(bDiff.Mul(bFactor));
            var aFactor = (long) Math.Ceiling(Math.Max((double) newGoal.x / aDiff.x, (double) newGoal.y / aDiff.y));
            if (aDiff.Mul(aFactor) == newGoal) return bFactor + (aFactor * 3);
        }
        return null;
    }

    // https://stackoverflow.com/a/4543530
    // Then with a bit of help from a friend and StackOverflow, we got this.
    static long? SolveAlgebra(Pos aDiff, Pos bDiff, Pos goal) {
        var delta = ((double) aDiff.x * bDiff.y) - ((double) aDiff.y * bDiff.x);
        // Console.WriteLine(delta);
        if (delta == 0) return null;

        var x = ((((double) bDiff.y * goal.x) - ((double) bDiff.x * goal.y)) / delta);
        var y = ((((double) aDiff.x * goal.y) - ((double) aDiff.y * goal.x)) / delta);
        if (x != Math.Floor(x) || y != Math.Floor(y)) return null;
        return ((long) x * 3) + (long) y;
    }
}
