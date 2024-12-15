using Common;

namespace Day12;

class Program {
    static void Main(string[] args) {
        var grid = new Grid<char>(File.ReadAllLines("../../../input.txt"), (c, _) => c);

        var claimed = new HashSet<Pos>();

        var cost = 0;
        
        foreach (var (pos, c) in grid.ToIndexedEnumerable()) {
            if (claimed.Contains(pos)) continue;
            // var result = Claim(grid, pos, ref claimed);
            var result = ClaimSided(grid, pos, ref claimed);
            
            // Item1 is used instead of the name so its agnostic between Claim() and ClaimSided()
            // Console.WriteLine($"{c}: {result.area} * {result.Item1}");
            cost += result.Item1 * result.area;
        }
        
        Console.WriteLine(cost);
    }

    private static (int perim, int area) Claim(Grid<char> grid, Pos pos, ref HashSet<Pos> claimed) {
        if (!claimed.Add(pos)) throw new InvalidOperationException();
        var target = grid[pos];
        var stack = new Stack<Pos>(pos.Adjacent());
        var perim = 0;
        var area = 1;
        while (stack.Count > 0) {
            var cur = stack.Pop();
            try {
                if (grid[cur] == target) {
                    if (!claimed.Add(cur)) continue;
                    area++;
                    foreach (var adj in cur.Adjacent()) stack.Push(adj);
                    continue;
                }
            } catch (IndexOutOfRangeException) {}
            perim++;
        }

        return (perim, area);
    }

    private static (int sides, int area) ClaimSided(Grid<char> grid, Pos pos, ref HashSet<Pos> claimed) {
        if (!claimed.Add(pos)) throw new InvalidOperationException();
        var walls = new Dictionary<Pos, Orthogonal[]>();
        var target = grid[pos];
        var stack = new Stack<(Orthogonal dir, Pos pos)>(pos.KeyedAdjacent());
        var area = 1;
        while (stack.Count > 0) {
            var cur = stack.Pop();
            try {
                if (grid[cur.pos] == target) {
                    if (!claimed.Add(cur.pos)) continue;
                    area++;
                    foreach (var adj in cur.pos.KeyedAdjacent()) stack.Push(adj);
                    continue;
                }
            } catch (IndexOutOfRangeException) {}
            var origin = cur.pos.Move(cur.dir.Inverse());
            var existing = walls.GetValueOrDefault(origin, []);
            walls[origin] = existing.Union([cur.dir]).ToArray();
        }

        return (CountSides(walls), area);
    }

    private static int CountSides(Dictionary<Pos, Orthogonal[]> walls) {
        var processed = new HashSet<(Pos, Orthogonal)>();
        
        var sides = 0;
        
        foreach (var (pos, orthos) in walls) {
            foreach (var dir in orthos) {
                if (processed.Contains((pos, dir))) continue;
                Process(pos, dir);
                sides++;
            }
        }
        
        return sides;

        void Process(Pos pos, Orthogonal dir) {
            var possible = dir.Perpendicular().Select(dir => pos.Move(dir)).ToArray();
            foreach (var cur in possible) {
                if (walls.TryGetValue(cur, out var curOrthos) && curOrthos.Contains(dir) && processed.Add((cur, dir))) {
                    Process(cur, dir);
                }
            }
        }
    }
}
