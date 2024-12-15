using System.Text;
using Spectre.Console;

namespace Common;

public class Grid<T> {
    private T[,] _grid;

    public T this[int x, int y] {
        get => _grid[x, y];
        set => _grid[x, y] = value;
    }

    public T this[Pos pos] {
        get => _grid[pos.X, pos.Y];
        set => _grid[pos.X, pos.Y] = value;
    }

    public IEnumerable<(Pos pos, T val)> ToIndexedEnumerable() {
        var xLen = _grid.GetLength(0);
        var yLen = _grid.GetLength(1);
        for (var x = 0; x < xLen; x++) {
            for (var y = 0; y < yLen; y++) {
                yield return ((x, y), this[x, y]);
            }
        }
    }
    
    // more or less stolen from day 6
    public void Print(Func<T, string> convert, Pos? pointer = null, string? pointerStr = null) {
        if ((pointer is null) != (pointerStr is null)) throw new InvalidOperationException("pointer and pointStr nulls must match");
        var doPointer = pointer is not null && pointerStr is not null;
        var builder = new StringBuilder();
        for (var x = 0; x < _grid.GetLength(0); x++) {
            for (var y = 0; y < _grid.GetLength(1); y++) {
                if (doPointer && pointer == (x, y)) {
                    builder.Append(pointerStr);
                    continue;
                }
                builder.Append(convert(this[x, y]));
            }
            builder.Append('\n');
        }
        AnsiConsole.Write(new Markup(builder.ToString()));
    }
    
    public void PrintDiff(Func<T, string> convert, Pos pointer, string pointerStr, Orthogonal backprintDir) {
        // we add one to positions in this method because console indices are 1-based while arrays are 0-based.
        AnsiConsole.Cursor.SetPosition(Convert.ToInt32(pointer.Y + 1), Convert.ToInt32(pointer.X + 1));
        AnsiConsole.Write(new Markup(pointerStr));
        var backprintPos = pointer.Move(backprintDir);
        AnsiConsole.Cursor.SetPosition(Convert.ToInt32(backprintPos.Y + 1), Convert.ToInt32(backprintPos.X + 1));
        AnsiConsole.Write(new Markup(convert(this[backprintPos])));
    }

    public Grid(char[][] grid, Func<char, Pos, T> transform) {
        var xLen = grid.Length;
        var yLen = grid[0].Length;
        var map = new T[xLen, yLen];
        for (var x = 0; x < xLen; x++) {
            for (var y = 0; y < yLen; y++) {
                map[x, y] = transform(grid[x][y], (x, y));
            }
        }
        _grid = map;
    }
    
    public Grid(string[][] grid, Func<string, Pos, T> transform) {
        var xLen = grid.Length;
        var yLen = grid[0].Length;
        var map = new T[xLen, yLen];
        for (var x = 0; x < xLen; x++) {
            for (var y = 0; y < yLen; y++) {
                map[x, y] = transform(grid[x][y], (x, y));
            }
        }
        _grid = map;
    }
    
    public Grid(string[] grid, Func<char, Pos, T> transform): this(grid.Select(str => str.ToCharArray()).ToArray(), transform) {
    }
}
