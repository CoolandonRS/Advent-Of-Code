using System.Collections.Frozen;
using System.ComponentModel;
using System.Numerics;
using System.Text;

namespace Day6.old;

// If I went back now I could probably fix this up, but at the time I had no clue what was going wrong, so I decided just to rewrite the whole thing and hope it worked the next time. Hence Attempt2
static class Attempt1 {
    static void OldMain(string[] args) {
        
        var pos = (0, 0);
        
        var map = File.ReadAllLines("../../../input.txt").Select((str, row) => {
            return str.Select((c, col) => {
                switch (c) {
                    case '#': return (ITile) new BlockedTile();
                    case '.': return (ITile) new UnvisitedTile();
                    case '^':
                        pos = (row, col);
                        return (ITile) new UnvisitedTile();
                    default:
                        throw new InvalidOperationException();
                }
            }).ToArray();
        }).ToArray();

        var direction = Direction.Left; // start left because the way we process input rotates and mirrors the map
        
       // Console.WriteLine(TurnsUntilEscape(map, pos, direction, false));
       Console.WriteLine(PotentialParadoxes(map, pos, direction, false));
    }

    public static int PotentialParadoxes(ITile[][] initialMap, (int row, int col) initialPos, Direction initialDirection, bool print = false) {
        HashSet<(int, int)> paradoxes = [initialPos];
        WalkMap(initialMap, initialPos, initialDirection, (map, pos, direction) => {
            var rotatedDir = direction.RotateLeft();
            (int row, int col) potentialWall = pos.Move(direction);
            (int row, int col) rotatedPos = pos;
            var depth = 0;
            while (true) {
                depth++;
                if (depth > map.Length * map[0].Length * 100L) {
                    Console.Error.WriteLine("ASSUMING PARADOX!!!!");
                    if (!paradoxes.Add(potentialWall)) {
                        Console.WriteLine("Assumed paradox already existed!?!?!?");
                    }
                    break;
                }
                if (print) Console.BackgroundColor = ConsoleColor.Black;
                try {
                    
                    rotatedPos = rotatedPos.Move(rotatedDir);
                    var rotatedTile = map[rotatedPos.row][rotatedPos.col];
                    if (rotatedTile is VisitedTile visited && visited.Directions.Contains(rotatedDir)) {
                        paradoxes.Add(potentialWall);
                        if (print) Console.BackgroundColor = ConsoleColor.DarkGray;
                        break;
                    } else if (rotatedTile is BlockedTile) {
                        rotatedDir = rotatedDir.RotateLeft();
                    }
                } catch (IndexOutOfRangeException) {
                    break;
                }
            }
        }, print);
        return paradoxes.Count - 1;
    }

    public static int TurnsUntilEscape(ITile[][] map, (int row, int col) pos, Direction direction, bool print = false) => WalkMap(map, pos, direction, (_, _, _) => {}, print);

    public static int WalkMap(ITile[][] map, (int row, int col) pos, Direction direction, Action<ITile[][], (int row, int col), Direction> during, bool print = false) {
        while (true) {
            if (print) {
                // Console.ForegroundColor = ConsoleColor.Magenta;
                // Console.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
                map.Print(pos, direction);
                Thread.Sleep(100);
                Console.Clear();
            }
            try {
                (int row, int col) move = pos.Move(direction);
                var newTile = map[move.row][move.col];
                if (newTile is BlockedTile) {
                    direction = direction.RotateLeft();
                    during(map, pos, direction);
                    continue;
                }
                
                HashSet<Direction> visitedDirections = [direction];
                if (newTile is VisitedTile visited) visitedDirections.UnionWith(visited.Directions);
                
                map[move.row][move.col] = new VisitedTile(visitedDirections);
                during(map, pos, direction);
                pos = move;
            } catch (IndexOutOfRangeException) {
                break;
            }
        }
        
        // we start with 1 so we count the exit tile. // or not? after ITile refactor it appears to no longer be the case
        return map.Aggregate(0, (count, tiles) => count + tiles.Count(tile => tile is VisitedTile));
    }

    public static (int, int) Move(this (int row, int col) pos, Direction direction) {
        return direction switch {
            Direction.Up => (pos.row, pos.col - 1),
            Direction.Right => (pos.row + 1, pos.col),
            Direction.Down => (pos.row, pos.col + 1),
            Direction.Left => (pos.row - 1, pos.col),
            _ => throw new InvalidEnumArgumentException()
        };
    }

    // We rotate left instead of right because the way we process input rotates and flips the map
    public static Direction RotateLeft(this Direction direction) {
        return direction switch {
            Direction.Up => Direction.Left,
            Direction.Right => Direction.Up,
            Direction.Down => Direction.Right,
            Direction.Left => Direction.Down,
            _ => throw new InvalidEnumArgumentException()
        };
    }

    

    // TODO: Consider rotating/mirroring displayed output so it matches input
    // This todo ended up being ignored because in the Attempt2 Rewrite it isn't mirrored.
    public static void Print(this ITile[][] map, (int row, int col) pos, Direction direction) {
        for (var col = 0; col < map[0].Length; col++) {
            for (var row = 0; row < map.Length; row++) {
                if ((row, col) == pos) {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(direction switch {
                        Direction.Up => '^',
                        Direction.Right => '>',
                        Direction.Down => 'v',
                        Direction.Left => '<',
                        _ => throw new InvalidEnumArgumentException()
                    });
                    continue;
                }
                switch (map[row][col]) {
                    case BlockedTile:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write('#');
                        break;
                    case UnvisitedTile:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write('.');
                        break;
                    case VisitedTile visited:
                        // TODO consider displaying what directions have been visited
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write('.');
                        break;
                    default: throw new InvalidEnumArgumentException();
                }
            }
            Console.WriteLine();
        }
        Console.ForegroundColor = ConsoleColor.White;
    }
}

// public enum Tile {
//     Unvisited, Visited, Blocked
// }

public interface ITile;

public struct BlockedTile: ITile;
public struct UnvisitedTile: ITile;
public readonly record struct VisitedTile(HashSet<Direction> Directions): ITile;

public enum Direction {
    Up, Down, Left, Right
}