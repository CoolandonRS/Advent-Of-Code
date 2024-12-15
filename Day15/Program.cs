using System.Linq.Expressions;
using Common;
using Spectre.Console;

namespace Day15;

static class Program {
    private const bool ShouldPrint = false;
    
    static void Main(string[] args) {
        var input = File.ReadAllLines("../../../input.txt");
        var pivot = input.ToList().FindIndex(string.IsNullOrWhiteSpace);
        var (robot, grid) = MakeGrid(input[..pivot]);
        (_, grid) = WalkGrid(robot, grid, string.Join("", input[pivot..]).ToCharArray());

        var sum = 0L;
        foreach (var (pos, tile) in grid.ToIndexedEnumerable()) {
            if (tile is not Box) continue;
            sum += (pos.X * 100) + pos.Y;
        }
        
        Console.WriteLine(sum);
    }

    static (Pos robot, Grid<ITile> grid) WalkGrid(Pos robot, Grid<ITile> grid, char[] instructions) {
        foreach (var instr in instructions.Select(c => c.ToOrthogonal())) {
            if (ShouldPrint) {
                AnsiConsole.Clear();
                grid.Print(tile => tile.ToRawMarkup(), robot, "[yellow]@[/]");
                Console.ReadKey();
            }

            var simul = robot;
            bool? canMove = null;
            bool willPush = false;
            while (true) {
                simul = simul.Move(instr);
                switch (grid[simul]) {
                    case Wall:
                        canMove = false;
                        break;
                    case Empty:
                        canMove = true;
                        break;
                    case Box:
                        willPush = true;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
                if (canMove is not null) break;
            }

            if (!canMove.Value) continue;
            
            // this line will be needed whether or not we push. Since we don't reference robot after this, it's safe to do now.
            robot = robot.Move(instr);
            
            if (!willPush) continue;

            var back = instr.Inverse();
            grid[simul] = new Box();
            while (grid[simul] is Box) simul = simul.Move(back);
            grid[simul.Move(instr)] = new Empty();
        }

        return (robot, grid);
    }

    static (Pos robot, Grid<ITile> grid) MakeGrid(string[] rawGrid) {
        Pos? robot = null;
        var grid = new Grid<ITile>(rawGrid, (c, pos) => {
            switch (c) {
                case '.': return new Empty();
                case 'O': return new Box();
                case '#': return new Wall();
                case '@':
                    robot = pos;
                    return new Empty();
                default: throw new InvalidOperationException($"Unexpected tile {c}");
            }
        });
        return (robot!.Value, grid);
    }

    static string ToRawMarkup(this ITile tile) {
        return tile switch {
            Box => "[green]O[/]",
            Wall => "[red]#[/]",
            Empty => "[blue].[/]",
            _ => "[pink]?[/]"
        };
    }
}

public interface ITile;
public readonly record struct Empty: ITile;
public readonly record struct Wall: ITile;
public readonly record struct Box: ITile;