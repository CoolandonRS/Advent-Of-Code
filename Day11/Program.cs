namespace Day11;

class Program {
    private static readonly IRule[] rules = [new ToOneRule(), new InHalfRule(), new Mul2024Rule()];
    
    static void Main(string[] args) {
        var input = File.ReadAllText("../../../input.txt").Split(' ').Select(long.Parse).ToArray();
     
        // Console.WriteLine(Solve(input, 25));
        Console.WriteLine(Solve(input, 75));
    }


    static long Solve(long[] input, int cap) {
        var memo = new Dictionary<(long, long), long>();
        return input.Sum(n => Solve(n, 1, cap, ref memo));
    } 
    // the idea of using a dictionary to memoize results is wholeheartedly taken from the reddit after I ran out of ideas of how to optimize.
    // I might at some point make a [Memoize] attribute to make this sort of thing easier. If I do, that's why it's not here yet.
    static long Solve(long n, int count, int cap, ref Dictionary<(long val, long count), long> memo) {
        if (count > cap) return 1;
        if (memo.TryGetValue((n, count), out var memod)) return memod;
        if (n == 0) {
            var oneSolve = Solve(1, count + 1, cap, ref memo);
            memo.TryAdd((n, count), oneSolve);
            return oneSolve;
        }
        var str = n.ToString();
        if (str.Length % 2 != 0) {
            var mulSolve = Solve(n * 2024, count + 1, cap, ref memo);
            memo.TryAdd((n, count), mulSolve);
            return mulSolve;
        };
        var pivot = str.Length / 2;
        var splitSolve = Solve(long.Parse(str[..pivot]), count + 1, cap, ref memo) + Solve(long.Parse(str[pivot..]), count + 1, cap, ref memo);
        memo.TryAdd((n, count), splitSolve);
        return splitSolve;
    }

    // the sad, sad remains of my initial attempt.
    static void Iterate(ref List<long> input, int count) {
        for (var i = 0; i < count; i++) {
            Console.WriteLine($"iter {i}");
            var max = input.Count;
            for (var j = 0; j < max; j++) {
                foreach (var rule in rules) {
                    if (rule.CanRunOn(input[j])) {
                        rule.Modify(ref input, ref j, ref max);
                        break;
                    }
                }
            }
        }
    }
}

interface IRule {
    bool CanRunOn(long input);
    void Modify(ref List<long> ints, ref int idx, ref int cap);
}

struct ToOneRule: IRule {
    public bool CanRunOn(long input) => input == 0;

    public void Modify(ref List<long> ints, ref int idx, ref int cap) {
        ints[idx] = 1;
    }
}

struct InHalfRule: IRule {
    public bool CanRunOn(long input) => input.ToString().Length % 2 == 0;
    public void Modify(ref List<long> ints, ref int idx, ref int cap) {
        var str  = ints[idx].ToString();
        var pivot = str.Length / 2;
        var str1 = str[..pivot];
        var str2 = str[pivot..];
        ints[idx] = long.Parse(str1);
        // the longest calls
        ints.Insert(++idx, long.Parse(str2));
        cap += 1;
    }
}

struct Mul2024Rule: IRule {
    public bool CanRunOn(long input) => true;

    public void Modify(ref List<long> ints, ref int idx, ref int cap) {
        ints[idx] *= 2024;
    }
}