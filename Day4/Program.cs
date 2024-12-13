using System.ComponentModel;

namespace Day4;

static class Program {
    static void Main(string[] args) {
        var input = File.ReadAllLines("../../../input.txt").Select(str => str.ToCharArray()).ToArray();

        // Console.WriteLine(FindAll(input, 'X', FindXmas));
        Console.WriteLine(FindAll(input, 'A', FindMas));
    }

    static int FindAll(char[][] input, char search, Func<(int,int), char[][], int> find) {
        var found = 0;
        
        for (var row = 0; row < input.Length; row++) {
            for (var col = 0; col < input[0].Length; col++) {
                if (input[row][col] != search) continue;
                found += find((row, col), input);
            }
        }

        return found;
    }

    static int FindXmas((int row, int col) pos, char[][] grid) {
        if (grid[pos.row][pos.col] != 'X') return 0;

        var found = 0;
        
        foreach (Direction dir in Enum.GetValuesAsUnderlyingType<Direction>()) {
            try {
                var success = true;
                var newPos = pos;
                foreach (var c in new[] {'M', 'A', 'S'}) {
                    newPos = newPos.Move(dir);
                    if (grid[newPos.row][newPos.col] != c) {
                        success = false;
                        break;
                    }
                }
                if (success) found++;
            } catch (IndexOutOfRangeException) {}
        }

        return found;
    }


    private static readonly Direction[] cardinals = [Direction.N, Direction.E, Direction.S, Direction.W]; 
    static int FindMas((int row, int col) pos, char[][] grid) {
        if (grid[pos.row][pos.col] != 'A') return 0;

        try {
            var ne = grid.Get(pos.Move(Direction.NE));
            var se = grid.Get(pos.Move(Direction.SE));
            var nw = grid.Get(pos.Move(Direction.NW));
            var sw = grid.Get(pos.Move(Direction.SW));
            
            if (ne == se && nw == sw) {
                if (ne == 'M' && nw == 'S') return 1;
                if (ne == 'S' && nw == 'M') return 1;
            }

            if (ne == nw && se == sw) {
                if (ne == 'M' && se == 'S') return 1;
                if (ne == 'S' && se == 'M') return 1;
            }

            return 0;
        } catch (IndexOutOfRangeException) { return 0; }
    }

    private static char Get(this char[][] grid, (int row, int col) pos) => grid[pos.row][pos.col];

    private static (int row, int col) Move(this (int row, int col) pos, Direction dir) {
        return dir switch {
            Direction.NW => (pos.row + 1, pos.col - 1),
            Direction.N  => (pos.row + 1, pos.col),
            Direction.NE => (pos.row + 1, pos.col + 1),
            Direction.W  => (pos.row, pos.col - 1),
            Direction.E  => (pos.row, pos.col + 1),
            Direction.SW => (pos.row - 1, pos.col - 1),
            Direction.S  => (pos.row - 1, pos.col),
            Direction.SE => (pos.row - 1, pos.col + 1),
            _ => throw new InvalidEnumArgumentException()
        };
    }
}

enum Direction {
    NW, N, NE,
    W,     E,
    SW, S, SE
}