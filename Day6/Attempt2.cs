using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Day6;

using Map = ITile[,];
using Pos = (int x, int y);
using Guard = ((int x, int y) Pos, Direction Dir);

// TODO: refactor to using Common.Grid<T> for legibility. Most of Grid<T>'s code comes from here anyway.

// the reason this one has so much fun printing code is because I was stuck on it and I didn't know what else to do.
// It should also be noted that the reason Common isn't used here is that it was born on Day 10.
public static class Attempt2 {
    private const bool Example     =                false;
    private const bool ShouldPrint = Example     || false;
    private const bool PrintFinal  = ShouldPrint || false;

    static void Main(string[] args) {
        if (ShouldPrint || PrintFinal) Console.OutputEncoding = Encoding.Unicode;
        if (ShouldPrint || PrintFinal) AnsiConsole.Cursor.Hide();
        if (ShouldPrint || PrintFinal) AnsiConsole.Clear();
        // AnsiConsole.Profile.Encoding = Encoding.Unicode;
        
        var input = File.ReadAllLines($"../../../{(Example ? "example_" : "")}input.txt").Select(str => str.ToCharArray()).ToArray();
        var (map, guard) = ParseMap(input);
        
        if (ShouldPrint) map.PrintBursted();
        
        // Console.WriteLine(TurnsUntilEscape(map, guard));
        Console.WriteLine(PotentialParadoxes(map, guard));
    }
    
    // Don't have to count `VisitedParadoxTile`s since they aren't added by default.
    static int TurnsUntilEscape(Map map, Guard guard) {
        var finalMap = WalkMap(map, guard, (_, _) => {}, ShouldPrint);
        if (PrintFinal) {
            AnsiConsole.Clear();
            finalMap.PrintBursted();
        }
        return CountTiles(finalMap, tile => tile is VisitedTile or StartingTile) + 1; // plus one because exit tile is never counted
    }
    
    static int PotentialParadoxes(Map initialMap, Guard initialGuard) {
        var finalMap = WalkMap(initialMap, initialGuard, (map, guard) => {
            if (CanMakeParadox(map, guard)) map.Set(guard.MovePos(), new ParadoxTile());
        }, ShouldPrint);
        if (PrintFinal) {
            AnsiConsole.Clear();
            finalMap.PrintBursted();
        }
        return CountTiles(finalMap, tile => tile is IParadoxTile);
    }

    static bool CanMakeParadox(Map map, Guard guard) {
        var iterLimit = map.GetLength(0) * map.GetLength(1);
        var potentialWallPos = guard.MovePos();
        if (map.Get(potentialWallPos) is not UnvisitedTile) return false;
        var simulGuard = guard.RotateRight();
        var iter = 0;
        while (true) {
            if (iter++ > iterLimit) {
                return true;
            }
            try {
                var simulPos = simulGuard.MovePos();
                var simulTile = map.Get(simulPos);
                // this `|| simulPos == potentialWallPos` literally took me like 2-3 days to figure out AAAAAAAAAAAAAAAA
                if (simulTile is BlockedTile || simulPos == potentialWallPos) {
                    simulGuard = simulGuard.RotateRight();
                    continue;
                } 
                if (simulTile is VisitedTile simulVisited) {
                    if (simulVisited.Directions.Contains(simulGuard.Dir)) return true;
                }
                simulGuard = simulGuard.Move(); 
            } catch (IndexOutOfRangeException) {
                return false;
            }
        }
    }

    static int CountTiles(Map map, Func<ITile, bool> predicate) {
        var count = 0;
        foreach (var tile in map) if (predicate(tile)) count++;
        return count;
    }
    
    // step is called:
    //   AFTER the guard is rotated
    //   BEFORE the guard is moved AFTER the map is updated 
    static Map WalkMap(Map map, Guard guard, Action<Map, Guard> step, bool print = false) {
        while (true) {
            try {
                if (print) {
                    map.PrintDiff(guard);
                    Thread.Sleep(1);
                    // AnsiConsole.Cursor.SetPosition(0, 0); // needed for PrintBursted, not for PrintDiff
                }
                var nextPos = guard.MovePos();
                var nextTile = map.Get(nextPos);
                if (nextTile is BlockedTile) {
                    guard = guard.RotateRight();
                    step(map, guard);
                    continue;
                }

                var curTile = map.Get(guard);
                HashSet<Direction> visitDirs = [guard.Dir];
                ITile? newTile = null;
                if (curTile is VisitedTile visited) {
                    visitDirs.UnionWith(visited.Directions);
                    newTile = curTile switch {
                        VisitedParadoxTile => new VisitedParadoxTile(visitDirs),
                        StartingTile => new StartingTile(visitDirs),
                        _ => null
                    };
                } else if (curTile is ParadoxTile) { // WalkMap will NEVER create `ParadoxTile`s by itself. However, it will change them into `VisitedParadoxTile`s when it walks over one.
                    newTile = new VisitedParadoxTile(visitDirs);
                }
                map.Set(guard, newTile ?? new VisitedTile(visitDirs));
                step(map, guard);
                guard = guard.Move();
            } catch (IndexOutOfRangeException) {
                break;
            }
        }

        return map;
    }
    
    static (Map, Guard) ParseMap(char[][] rawMap) {
        var xLen = rawMap.Length;
        var yLen = rawMap[0].Length;
        var map = new ITile[xLen, yLen];
        Guard? guard = null;
        for (var x = 0; x < rawMap.Length; x++) {
            for (var y = 0; y < yLen; y++) {
                switch (rawMap[x][y]) {
                    case '#': 
                        map[x, y] = new BlockedTile(); 
                        break;
                    case '.': 
                        map[x, y] = new UnvisitedTile(); 
                        break;
                    case '^': 
                        map[x, y] = new StartingTile([Direction.N]);
                        guard = ((x, y), Direction.N);
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected tile: {rawMap[x][y]}");
                }
            }
        }
        if (guard is null) throw new InvalidOperationException();
        return (map, guard.Value);
    }

    static ITile Get(this Map map, Pos pos) => map[pos.x, pos.y];
    static void Set(this Map map, Pos pos, ITile val) => map[pos.x, pos.y] = val;
    static ITile Get(this Map map, Guard guard) => map.Get(guard.Pos);
    static void Set(this Map map, Guard guard, ITile val) => map.Set(guard.Pos, val);


    static Pos Move(this Pos pos, Direction dir) {
        return dir switch {
            // I do not know why these are the changes for the directions. It looks wrong. It feels wrong. But it works right.
            Direction.N => (pos.x - 1, pos.y),
            Direction.E => (pos.x, pos.y + 1),
            Direction.S => (pos.x + 1, pos.y),
            Direction.W => (pos.x, pos.y - 1),
            _ => throw new InvalidEnumArgumentException()
        };
    }

    static Pos MovePos(this Guard guard, Direction dir) => guard.Pos.Move(dir);
    static Guard Move(this Guard guard, Direction dir) => (guard.MovePos(dir), dir);
    static Pos MovePos(this Guard guard) => guard.MovePos(guard.Dir);
    static Guard Move(this Guard guard) => guard.Move(guard.Dir);

    static Direction RotateRight(this Direction dir) {
        return dir switch {
            Direction.N => Direction.E,
            Direction.E => Direction.S,
            Direction.S => Direction.W,
            Direction.W => Direction.N,
            _ => throw new InvalidEnumArgumentException()
        };
    }

    static Direction RotateDirRight(this Guard guard) => RotateRight(guard.Dir);
    static Guard RotateRight(this Guard guard) => (guard.Pos, guard.RotateDirRight());

    public static void PrintDiff(this Map map, Guard guard) {
        var pos = guard.Pos;
        AnsiConsole.Cursor.SetPosition(pos.y + 1, pos.x + 1);
        AnsiConsole.Write(new Markup(guard.GetRawMarkup()));
        var behindPos = guard.RotateRight().RotateRight().MovePos();
        AnsiConsole.Cursor.SetPosition(behindPos.y + 1, behindPos.x + 1);
        AnsiConsole.Write(new Markup(map.Get(behindPos).GetRawMarkup()));
    }
    
    public static void PrintBursted(this Map map, Guard? guard = null) {
        var doGuard = guard is not null;
        var builder = new StringBuilder();
        for (var x = 0; x < map.GetLength(0); x++) {
            for (var y = 0; y < map.GetLength(1); y++) {
                if (doGuard && (x, y) == guard!.Value.Pos) {
                    builder.Append(guard.Value.GetRawMarkup());
                    continue;
                }
                builder.Append(map[x, y].GetRawMarkup());
            }
            builder.Append('\n');
        }
        AnsiConsole.Write(new Markup(builder.ToString()));
    }

    public static string GetRawMarkup(this Guard guard) {
        return "[yellow]" + guard.Dir switch {
            Direction.N => '^',
            Direction.E => '>',
            Direction.S => 'v',
            Direction.W => '<',
            _ => throw new InvalidEnumArgumentException()
        } + "[/]";
    }

    public static string GetRawMarkup(this ITile tile) {
        return tile switch {
            BlockedTile => "[red]#[/]",
            UnvisitedTile => "[blue].[/]",
            ParadoxTile => "[purple]*[/]",
            VisitedTile visited => visited.GetRawDirectionalMarkup(VisitedHeathenLines),
            _ => throw new InvalidEnumArgumentException()
        };
    }

    private static readonly ImmutableArray<char> VisitedLines =        [' ','╴','╷','┐','╶','─','┌','┬','╵','┘','│','┤','└','┴','├','┼'];
    private static readonly ImmutableArray<char> VisitedArrows =       [' ','←','↓','↙','→','↔','↘','⇩','↑','↖','↕','⇦','↗','⇧','⇨','O'];
    private static readonly ImmutableArray<char> VisitedHeathenLines = [' ','─','│','┼','─','?','┼','?','│','┼','?','?','┼','?','?','?'];
    
    public static string GetRawDirectionalMarkup(this VisitedTile visited, ImmutableArray<char>? charset = null) {
        var key = 0;
        if (visited.Directions.Contains(Direction.N)) key |= 0b1000;
        if (visited.Directions.Contains(Direction.E)) key |= 0b0100;
        if (visited.Directions.Contains(Direction.S)) key |= 0b0010;
        if (visited.Directions.Contains(Direction.W)) key |= 0b0001;
        return visited switch {
            StartingTile => "[yellow]",
            VisitedParadoxTile => "[cyan]",
            _ => "[green]"
        } + (charset ?? VisitedArrows)[key] + "[/]";
    }

    public static string GetRawMarkup(this VisitedTile visited) {
        return visited switch {
            StartingTile => "[yellow]o[/]",
            VisitedParadoxTile => "[cyan]*[/]",
            _ => "[green].[/]"
        };
    }
}

public enum Direction {
    N, E, S, W
}

public interface ITile;
public interface IParadoxTile: ITile;
public record UnvisitedTile: ITile;
public record BlockedTile: ITile;
public record ParadoxTile: IParadoxTile;
public record VisitedTile(HashSet<Direction> Directions): ITile;
public record StartingTile(HashSet<Direction> Directions): VisitedTile(Directions);
public record VisitedParadoxTile(HashSet<Direction> Directions): VisitedTile(Directions), IParadoxTile;
