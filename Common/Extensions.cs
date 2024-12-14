using System.ComponentModel;
using System.Numerics;

namespace Common;

public static class Extensions {
    public static Pos[] Adjacent(this Pos initial) => [
        initial.Move(Orthogonal.N),
        initial.Move(Orthogonal.E),
        initial.Move(Orthogonal.S),
        initial.Move(Orthogonal.W)
    ];
    // directions are copied from Day6/Attempt2.cs. Given Grid<> uses it's parsing code it should be the same
    public static (Orthogonal, Pos)[] KeyedAdjacent(this Pos initial) => [
        (Orthogonal.N, initial.Move(Orthogonal.N)),
        (Orthogonal.E, initial.Move(Orthogonal.E)),
        (Orthogonal.S, initial.Move(Orthogonal.S)),
        (Orthogonal.W, initial.Move(Orthogonal.W))
    ];

    public static Pos Move(this Pos pos, Orthogonal dir) {
        return dir switch {
            Orthogonal.N => pos with {X = pos.X - 1},
            Orthogonal.E => pos with {Y = pos.Y + 1},
            Orthogonal.S => pos with {X = pos.X + 1},
            Orthogonal.W => pos with {Y = pos.Y - 1},
            _ => throw new InvalidEnumArgumentException()
        };
    }

    public static Orthogonal Inverse(this Orthogonal dir) {
        return dir switch {
            Orthogonal.N => Orthogonal.S,
            Orthogonal.E => Orthogonal.W,
            Orthogonal.S => Orthogonal.N,
            Orthogonal.W => Orthogonal.E,
            _ => throw new InvalidEnumArgumentException()
        };
    }

    public static Orthogonal[] Perpendicular(this Orthogonal dir) {
        return dir switch {
            Orthogonal.N => [Orthogonal.E, Orthogonal.W],
            Orthogonal.E => [Orthogonal.N, Orthogonal.S],
            Orthogonal.S => [Orthogonal.E, Orthogonal.W],
            Orthogonal.W => [Orthogonal.N, Orthogonal.S],
            _ => throw new InvalidEnumArgumentException()
        };
    }
    
    public static Pos[] IndicesOf<T>(this Grid<T> grid, T val) where T: IEquatable<T> {
        return grid.ToIndexedEnumerable().Where(obj => obj.val.Equals(val)).Select(obj => obj.pos).ToArray();
    } 
}
