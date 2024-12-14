using System.Text.RegularExpressions;

namespace Common;

public record struct Pos(long X, long Y) {
    public static Pos operator +(Pos pos, long other) => new(pos.X + other, pos.Y + other);
    public static Pos operator +(Pos pos, Pos other) => new(pos.X + other.X, pos.Y + other.Y);
    public static Pos operator -(Pos pos, long other) => new(pos.X - other, pos.Y - other);
    public static Pos operator -(Pos pos, Pos other) => new(pos.X - other.X, pos.Y - other.Y);
    public static Pos operator *(Pos pos, long other) => new(pos.X * other, pos.Y * other);
    public static Pos operator *(Pos pos, Pos other) => new(pos.X * other.X, pos.Y * other.Y);
    public static Pos operator /(Pos pos, long other) => new(pos.X / other, pos.Y / other);
    public static Pos operator /(Pos pos, Pos other) => new(pos.X / other.X, pos.Y / other.Y);
    public static Pos operator %(Pos pos, long other) => new(pos.X % other, pos.Y % other);
    public static Pos operator %(Pos pos, Pos other) => new(pos.X % other.X, pos.Y % other.Y);

    public static bool operator >(Pos pos, long other) => pos.X > other || pos.Y > other;
    public static bool operator >(Pos pos, Pos other) => pos.X > other.X || pos.Y > other.Y;
    public static bool operator <(Pos pos, long other) => pos.X < other || pos.Y < other;
    public static bool operator <(Pos pos, Pos other) => pos.X < other.X || pos.Y < other.Y;
    
    public static implicit operator Pos((int x, int y) pos) => new(pos.x, pos.y);
    public static implicit operator Pos((long x, long y) pos) => new(pos.x, pos.y);
    public static implicit operator (int, int)(Pos pos) => (Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y));
    public static implicit operator (long, long)(Pos pos) => (pos.X, pos.Y);

    public Pos(Match match, string prefix): this(long.Parse(match.Groups[prefix + 'x'].Value), long.Parse(match.Groups[prefix + 'y'].Value)) {
    }
}
