using System.Diagnostics;
using System.Numerics;
using System.Text.RegularExpressions;
using Common;

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
                new Pos(match, "a"),
                new Pos(match, "b"),
                new Pos(match, "g") + addToGoal
            ) ?? 0;
        }
        Console.WriteLine(tokens);
    }
    
    // first attempt. Then I went "can I divide to get bFactor?"
    static long? SolveIterate(Pos aDiff, Pos bDiff, Pos goal) {
        var bFactor = 0;
        while (goal > (bDiff * ++bFactor));
        if ((bDiff * bFactor) == goal) return bFactor;
        while (--bFactor > 0) {
            var newB = bDiff * bFactor;
            var aFactor = 0;
            while (goal > newB + (aDiff * ++aFactor));
            if (newB + (aDiff * aFactor) == goal) return bFactor + (aFactor * 3);
        }
        return null;
    }

    // Thus SolveMath was born. Looking at this though, I couldn't shake the feeling that this could be done with pure math.
    static long? SolveMath(Pos aDiff, Pos bDiff, Pos goal) {
        var bFactor = (long) Math.Ceiling(Math.Max((double) goal.X / bDiff.X, (double) goal.Y / bDiff.Y));
        if ((bDiff * bFactor) == goal) return bFactor;
        while (--bFactor > 0) {
            var newGoal = goal - (bDiff * bFactor);
            var aFactor = (long) Math.Ceiling(Math.Max((double) newGoal.X / aDiff.X, (double) newGoal.Y / aDiff.Y));
            if ((aDiff * aFactor) == newGoal) return bFactor + (aFactor * 3);
        }
        return null;
    }

    // https://stackoverflow.com/a/4543530
    // Then with a bit of help from a friend and StackOverflow, we got this.
    static long? SolveAlgebra(Pos aDiff, Pos bDiff, Pos goal) {
        var delta = ((double) aDiff.X * bDiff.Y) - ((double) aDiff.Y * bDiff.X);
        // Console.WriteLine(delta);
        if (delta == 0) return null;

        var x = ((((double) bDiff.Y * goal.X) - ((double) bDiff.X * goal.Y)) / delta);
        var y = ((((double) aDiff.X * goal.Y) - ((double) aDiff.Y * goal.X)) / delta);
        if (x != Math.Floor(x) || y != Math.Floor(y)) return null;
        return ((long) x * 3) + (long) y;
    }
}
