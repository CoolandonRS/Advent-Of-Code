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

    public Grid(char[][] grid, Func<char, T> transform) {
        var xLen = grid.Length;
        var yLen = grid[0].Length;
        var map = new T[xLen, yLen];
        for (var x = 0; x < xLen; x++) {
            for (var y = 0; y < yLen; y++) {
                map[x, y] = transform(grid[x][y]);
            }
        }
        _grid = map;
    }
    
    public Grid(string[][] grid, Func<string, T> transform) {
        var xLen = grid.Length;
        var yLen = grid[0].Length;
        var map = new T[xLen, yLen];
        for (var x = 0; x < xLen; x++) {
            for (var y = 0; y < yLen; y++) {
                map[x, y] = transform(grid[x][y]);
            }
        }
        _grid = map;
    }
    
    public Grid(string[] grid, Func<char, T> transform): this(grid.Select(str => str.ToCharArray()).ToArray(), transform) {
    }
}
