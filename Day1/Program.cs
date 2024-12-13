namespace HistorianHysteria;

// Historian Hysteria
class Program {
    static void Main(string[] args) {
        var first = new List<int>();
        var second = new List<int>();

        foreach (var line in File.ReadLines("../../../input.txt")) {
            var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            first.Add(int.Parse(split[0]));
            second.Add(int.Parse(split[1]));
        }

        // Console.WriteLine(SolvePart1(first, second));
        Console.WriteLine(SolvePart2(first, second));
    }

    static long SolvePart1(List<int> first, List<int> second) {
        var sum = 0L;

        foreach (var nums in first.Order().Zip(second.Order())) {
            sum += long.Abs(nums.First - nums.Second);
        }

        return sum;
    }

    static long SolvePart2(List<int> first, List<int> second) {
        var counts = second.Aggregate(new Dictionary<int, long>(), (dict, cur) => {
            var count = dict.GetValueOrDefault(cur, 0L);
            dict[cur] = count + 1;
            return dict;
        });
        
        var score = 0L;

        foreach (var num in first) {
            score += num * counts.GetValueOrDefault(num, 0L);
        }

        return score;
    }
}