namespace Day2;

// Red-Nosed Reports
class Program {
    static void Main(string[] args) {
        var input = File.ReadAllLines("../../../input.txt");

        var safe = 0;

        foreach (var line in input) {
            var ints = line.Split(' ').Select(int.Parse).ToArray();
            if (IsSafe(ints)) safe++;
            // if (IsSafeDampened(ints)) safe++;
        }
        Console.WriteLine(safe);
    }

    static Direction GetDirection(int[] array) {
        if (array[0] > array[1]) return Direction.Greater;
        if (array[0] < array[1]) return Direction.Less;
        return Direction.Equal;
    }

    static bool IsSafe(int[] array) {
        var direction = GetDirection(array);
        if (direction == Direction.Equal) return false;

        var prev = array[0];
        foreach (var n in array[1..]) {
            var absDiff = int.Abs(n - prev);
            if (absDiff is < 1 or > 3) return false;
            if (prev > n && direction != Direction.Greater) return false;
            if (prev < n && direction != Direction.Less) return false;
            prev = n;
        }
        return true;
    }

    static bool IsSafeDampened(int[] array) {
        if (IsSafe(array)) return true;
        return GenerateAlts(array).Any(IsSafe);
    }

    static int[][] GenerateAlts(int[] arr) {
        var list = new List<int[]>();
        for (var i = 0; i < arr.Length; i++) {
            var alt = new List<int>();
            for (var j = 0; j < arr.Length; j++) {
                if (i == j) continue;
                alt.Add(arr[j]);
            }
            list.Add(alt.ToArray());
        }
        return list.ToArray();
    }
}

enum Direction {
    Greater, Less, Equal
}