using Common;

namespace Day10;

class Program {
    static void Main(string[] args) {
        var grid = new Grid<int>(File.ReadAllLines("../../../input.txt"), c => int.Parse(c.ToString()));
        
        var trailheads = grid.IndicesOf(0);
        
        // Console.WriteLine(trailheads.Sum(trailhead => ReachablePeaks(trailhead, grid)));
        Console.WriteLine(trailheads.Sum(trailhead => ReachablePeaks(trailhead, grid, null)));
    }
    
    static int ReachablePeaks(Pos cur, Grid<int> grid) => ReachablePeaks(cur, grid, new());
    // like everyone else on this day, I solved Part 2 first. I initially just removed the offending code to get the answer, but afterward retrofitted it to make the HashSet nullable.
    static int ReachablePeaks(Pos cur, Grid<int> grid, HashSet<Pos>? visitedPeaks) {
        var n = grid[cur];
        if (n is 9) return (visitedPeaks?.Add(cur) ?? true) ? 1 : 0;
        return cur.Adjacent().Where(pos => {
            try {
                return grid[pos] == n + 1;
            } catch (IndexOutOfRangeException) {
                return false;
            }
        }).Sum(pos => ReachablePeaks(pos, grid, visitedPeaks));
    }
}
