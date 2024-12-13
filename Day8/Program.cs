using System.Collections.Frozen;
using System.Numerics;
using System.Runtime.Intrinsics;

namespace Day8;

// the reason Common isn't used here is that Common was born on Day 10.
public static class Program {
    public static void Main(string[] args) {
        var input = File.ReadAllLines("../../../input.txt");
        var (dishes, bounds) = ParseInput(input);
        
        // Console.WriteLine(FindAntinodes(dishes, bounds, false));
        Console.WriteLine(FindAntinodes(dishes, bounds, true));
    }

    public static int FindAntinodes(FrozenDictionary<char, (int x, int y)[]> dishes, (int, int) bounds, bool resonance) {
        var antinodes = new HashSet<(int x, int y)>();
        
        // this is one of the few were I just modified the logic for part 2, and then modified it again so it could still do part1
        foreach (var (key, locs) in dishes) {
            foreach (var loc1 in locs) {
                foreach (var loc2 in locs) {
                    if (loc1 == loc2) continue;
                    if (resonance) antinodes.Add(loc2);
                    var diff = loc2.Sub(loc1);
                    var doubled = loc2.Add(diff);
                    while (doubled.IsInBounds(bounds)) {
                        antinodes.Add(doubled);
                        doubled = doubled.Add(diff);
                        if (!resonance) break;
                    }
                }
            }
        }

        return antinodes.Count;
    }

    public static (int, int) Sub(this (int, int) a, (int, int) b) {
        return (a.Item1 - b.Item1, a.Item2 - b.Item2);
    }

    public static (int, int) Add(this (int, int) a, (int, int) b) {
        return (a.Item1 + b.Item1, a.Item2 + b.Item2);
    }

    public static bool IsInBounds(this (int, int) pos, (int, int) bounds) {
        return pos is {Item1: >= 0, Item2: >= 0} && pos.Item1 <= bounds.Item1 && pos.Item2 <= bounds.Item2;
    }

    public static (FrozenDictionary<char, (int x, int y)[]> dishes, (int, int) bounds) ParseInput(string[] input) {
        var xLen = input.Length;
        var yLen = input[0].Length;
        var temp = new Dictionary<char, List<(int, int)>>();
        for (int x = 0; x < xLen; x++) {
            for (int y = 0; y < yLen; y++) {
                var c = input[x][y];
                if (c == '.') continue;
                var list = temp.GetValueOrDefault(c, []);
                list.Add((x, y));
                temp[c] = list;
            }
        }
        
        // subtract one from bounds because 0-indexed
        return (temp.ToFrozenDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray()), (xLen - 1, yLen - 1));
    }
}
