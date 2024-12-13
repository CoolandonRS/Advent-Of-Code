namespace Day9;

static class Program {
    static void Main(string[] args) {
        var input = File.ReadAllText("../../../input.txt");

        var disk = ParseDisk(input);
        // disk = SortDisk(disk);
        disk = SortDiskGroupedBySector(disk);
        Console.WriteLine(CalcChecksum(disk));
    }

    static long CalcChecksum(List<int?> disk) {
        return disk.Select((n, idx) => n is null ? 0L : n.Value * idx).Sum();
    }

    static List<int?> SortDisk(List<int?> disk) {
        while (!DoneSorting(disk)) {
            var firstEmpty = disk.IndexOf(null);
            var lastFull = disk.FindLastIndex(n => n is not null);
            disk.Swap(firstEmpty, lastFull);
        }

        return disk;
    }

    static List<int?> SortDiskGroupedBySector(List<int?> disk) {
        var max = disk.Max()!.Value;
        for (var sector = max; sector >= 0; sector--) {
            var idx = disk.IndexOf(sector);
            var len = disk.SequenceLength(idx, sector);
            foreach (var (val, i) in disk[..idx].Select((n, i) => (n, i))) {
                if (val is not null) continue;
                var size = disk.SequenceLength(i, null);
                if (size >= len) {
                    disk.SwapSequences(idx, i, len);
                    break;
                }
            }
        }
        return disk;
    }

    [Obsolete("Not what was asked")]
    static List<int?> SortDiskGroupedByEmpty(List<int?> disk) {
        var max = disk.Max()!.Value;
        var indices = disk.IncongruentIndicesOf(null);
        var idxPointer = 0;
        var arrPointer = indices[0];
        while (true) {
            var size = disk.SequenceLength(arrPointer, null);
            var look = max + 1; // plus one because loop will immediately decrement it
            var lookable = disk[arrPointer..];
            var found = false;
            var goalSize = 0;
            while (look-- > 0) {
                var firstIdx = lookable.IndexOf(look);
                if (firstIdx == -1) continue;
                goalSize = lookable.SequenceLength(firstIdx, look);
                if (goalSize <= size) {
                    found = true;
                    break;
                }
            }

            if (found) {
                var actualFirst = disk.IndexOf(look);
                disk.SwapSequences(arrPointer, actualFirst, goalSize);

                if (size - goalSize > 0) {
                    arrPointer += goalSize;
                    continue;
                }
            }

            try {
                arrPointer = indices[++idxPointer];
            } catch (IndexOutOfRangeException) {
                break;
            }
        }
        return disk;
    }

    static bool DoneSorting(List<int?> disk) {
        var firstEmpty = disk.IndexOf(null);
        return disk[firstEmpty..].All(n => n is null);
    }

    static List<int?> ParseDisk(string input) {
        // inefficiently 1-to-1 simulate it because why not
        List<int?> disk = [];
        var sector = 0;
        var empty = false;
        foreach (var c in input) {
            var count = int.Parse(c.ToString());
            int? newSector = empty ? null : sector++;
            while (count-- > 0) {
                disk.Add(newSector);
            }
            empty = !empty;
        }
        disk.TrimExcess();
        return disk;
    }

    static void Swap<T>(this List<T> list, int idx1, int idx2) {
        // the IDE told me to do it
        (list[idx1], list[idx2]) = (list[idx2], list[idx1]);
    }

    static void SwapSequences<T>(this List<T> list, int idx1, int idx2, int len) {
        for (var i = 0; i < len; i++) {
            list.Swap(idx1 + i, idx2 + i);
        }
    }

    static int[] IncongruentIndicesOf(this List<int?> list, int? item) {
        List<int> indices = [];
        var found = false;
        foreach (var (obj, idx) in list.Select((obj, idx) => (obj, idx))) {
            if (obj == item) {
                if (!found) {
                    indices.Add(idx);
                    found = true;
                }
            } else if (found) found = false;
        }
        return indices.ToArray();
    }

    static int SequenceLength(this List<int?> list, int startIdx, int? item) {
        var count = 0;
        for (var i = startIdx; i < list.Count; i++) {
            if (list[i] == item) count++; else break;
        }
        return count;
    }
}