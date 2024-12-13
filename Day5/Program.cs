using System.Collections.Immutable;

namespace Day5;

class Program {
    static void Main(string[] args) {
        var input = File.ReadAllLines("../../../input.txt");

        var split = input.Select((str, idx) => (str, idx)).First(val => string.IsNullOrWhiteSpace(val.str)).idx;
        var rules = input[..split].Select(str => {
            var split = str.Split('|');
            return new Rule(int.Parse(split[0]), int.Parse(split[1]));
        }).ToArray();
        var updates = input[(split + 1)..].Select(str => {
            return str.Split(',').Select(int.Parse).ToImmutableList();
        }).ToArray();
        
        // Console.WriteLine(SumCorrectRuleCenters(rules, updates));
        Console.WriteLine(SumIncorrectRuleCenters(rules, updates));
        
    }

    static int SumIncorrectRuleCenters(Rule[] rules, ImmutableList<int>[] updates) {
        var sum = 0;

        foreach (var update in updates) {
            if (rules.All(rule => rule.SatisfiedBy(update))) continue;
            var count = update.Count;
            if (count % 2 != 1) Console.Error.WriteLine("erm non-odd updates?");
            sum += SortUpdate(rules, update)[count / 2];
        }

        return sum;
    }
    
    // a sorta scuffed insertion sort? 
    static ImmutableList<int> SortUpdate(Rule[] rules, IList<int> update) {
        var mutable = update.ToList();
        while (true) {
            try {
                var broken = rules.First(rule => !rule.SatisfiedBy(mutable));
                
                var first = mutable.IndexOf(broken.First);
                var second = mutable.IndexOf(broken.Second);
            
                mutable.Insert(second, broken.First);
                mutable.RemoveAt(first + 1); // plus one because we know first is after second and we just inserted
            } catch {
                return mutable.ToImmutableList();
            }
        }
    }

    static int SumCorrectRuleCenters(Rule[] rules, IList<int>[] updates) {
        var sum = 0;

        foreach (var update in updates) {
            if (!rules.All(rule => rule.SatisfiedBy(update))) continue;
            var count = update.Count;
            if (count % 2 != 1) Console.Error.WriteLine("erm non-odd updates?");
            sum += update[count / 2];
        }

        return sum;
    }
}

internal readonly record struct Rule(int First, int Second) {
    public bool SatisfiedBy(IList<int> ints) {
        var first = ints.IndexOf(First);
        if (first == -1) return true;
        var second = ints.IndexOf(Second);
        if (second == -1) return true;
        return first < second;
    }
}
