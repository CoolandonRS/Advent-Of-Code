using System.ComponentModel;
using System.Numerics;

namespace Common;

using Pos = (int x, int y);
using LongPos = (long x, long y);



public static class Extensions {
    // int pos logic
    public static Pos Mul(this Pos pos, int other) => (pos.x * other, pos.y * other);
    public static Pos Mul(this Pos pos, Pos other) => (pos.x * other.x, pos.y * other.y);
    public static Pos Add(this Pos pos, int other) => (pos.x + other, pos.y + other);
    public static Pos Add(this Pos pos, Pos other) => (pos.x + other.x, pos.y + other.y);
    public static Pos Sub(this Pos pos, int other) => (pos.x - other, pos.y - other);
    public static Pos Sub(this Pos pos, Pos other) => (pos.x - other.x, pos.y - other.y);
    
    public static bool EitherGreater(this Pos pos, int other) => pos.x > other || pos.y > other;
    public static bool EitherGreater(this Pos pos, Pos other) => pos.x > other.x || pos.y > other.y;
    // long pos logic
    public static LongPos Mul(this LongPos pos, long other) => (pos.x * other, pos.y * other);
    public static LongPos Mul(this LongPos pos, LongPos other) => (pos.x * other.x, pos.y * other.y);
    public static LongPos Add(this LongPos pos, long other) => (pos.x + other, pos.y + other);
    public static LongPos Add(this LongPos pos, LongPos other) => (pos.x + other.x, pos.y + other.y);
    public static LongPos Sub(this LongPos pos, long other) => (pos.x - other, pos.y - other);
    public static LongPos Sub(this LongPos pos, LongPos other) => (pos.x - other.x, pos.y - other.y);
    
    public static bool EitherGreater(this LongPos pos, long other) => pos.x > other || pos.y > other;
    public static bool EitherGreater(this LongPos pos, LongPos other) => pos.x > other.x || pos.y > other.y;
    // other
    
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
            Orthogonal.N => (pos.x - 1, pos.y),
            Orthogonal.E => (pos.x, pos.y + 1),
            Orthogonal.S => (pos.x + 1, pos.y),
            Orthogonal.W => (pos.x, pos.y - 1),
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
