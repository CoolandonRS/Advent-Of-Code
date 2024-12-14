using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Common;
using Spectre.Console;

namespace Day14;

using Robot = (Pos pos, Pos velocity);

static class Program {
    private const bool Example = false;
    
    private static readonly Regex roboRgx = new(@"p=(?<px>\d+),(?<py>\d+) v=(?<vx>-?\d+),(?<vy>-?\d+)");
    
    static void Main(string[] args) {
        var bounds = Example ? new Pos(11, 7) : new Pos(101, 103);
        
        IEnumerable<Robot> robotEnumerable = roboRgx.Matches(File.ReadAllText($"../../../{(Example ? "example_" : "")}input.txt")).Select(match => (
            new Pos(match, "p"),
            new Pos(match, "v")
        ));

        
        // Console.WriteLine(Part1(robotEnumerable, bounds));
        Part2(robotEnumerable.ToArray(), bounds);
    }

    
    public static int Part1(IEnumerable<Robot> robots, Pos bounds) {
        for (var i = 0; i < 100; i++) {
            robots = robots.Select(robot => robot.Move(bounds));
        }
        return SafetyScore(robots, bounds);
    }

    [DoesNotReturn]
    // It was spoiled to me that looking for min score was a way to find the easter egg easily, but it was pretty fun to solve nonetheless. 
    public static void Part2(Robot[] robots, Pos bounds) {
        var minScore = int.MaxValue;
        var idx = 0;
        while (true) {
            idx++;
            robots = robots.Select(robot => robot.Move(bounds)).ToArray();
            var newScore = SafetyScore(robots, bounds);
            if (newScore >= minScore) continue;
            minScore = newScore;
            AnsiConsole.Clear();
            Print(robots, bounds);
            AnsiConsole.Cursor.SetPosition(Convert.ToInt32(bounds.Y + 1), Convert.ToInt32(bounds.X));
            AnsiConsole.WriteLine(idx);
            Console.ReadKey();
        }
    }

    public static int SafetyScore(IEnumerable<Robot> robots, Pos bounds) {
        var quads = (XY: 0, Xy: 0, xY: 0, xy: 0);
        var halfBounds = bounds / 2;
        foreach (var robot in robots.Select(robot => robot.pos)) {
            // this could be made a bit more efficient if I cared enough.
            if (robot.X > halfBounds.X && robot.Y > halfBounds.Y) quads.XY++;
            else if (robot.X > halfBounds.X && robot.Y < halfBounds.Y) quads.Xy++;
            else if (robot.X < halfBounds.X && robot.Y > halfBounds.Y) quads.xY++;
            else if (robot.X < halfBounds.X && robot.Y < halfBounds.Y) quads.xy++;
        }

        return quads.XY * quads.Xy * quads.xY * quads.xy;
    }

    public static Robot Move(this Robot robot, Pos bounds) {
        var newPos = (robot.pos + robot.velocity) % bounds; // modulo to wrap upper half
        // if statements to wrap lower half
        if (newPos.X < 0) newPos = newPos with {X = bounds.X + newPos.X}; // add because newPos is known to be negative
        if (newPos.Y < 0) newPos = newPos with {Y = bounds.Y + newPos.Y};
        return (newPos, robot.velocity);
    }

    public static void Print(Robot[] robots, Pos bounds) {
        AnsiConsole.WriteLine(string.Join('\n',Enumerable.Repeat(new string('.', Convert.ToInt32(bounds.X)), Convert.ToInt32(bounds.Y))));
        foreach (var robot in robots.Select(robot => robot.pos)) {
            AnsiConsole.Cursor.SetPosition(Convert.ToInt32(robot.X), Convert.ToInt32(robot.Y));
            AnsiConsole.Write(new Text("#", Color.Green));
        }
    }
}
