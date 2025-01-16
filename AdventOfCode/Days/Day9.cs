namespace AdventOfCode.Days;

public static class Day9
{
    private record DiskBlock(int Count, int Value);

    public static ulong Part1(string input)
    {
        DiskBlock[] blocks = [];
        DiskBlock[] buffer = [];

        List<DiskBlock> diskItems = [];
        List<DiskBlock> defrag = [];

        var free = false;
        var id = 0;

        using var file = File.OpenRead(input);
        using var reader = new StreamReader(file);
        while (!reader.EndOfStream)
        {
            var cr = (char)reader.Read();
            var digit = int.Parse(cr.ToString());

            diskItems.Add(new DiskBlock(digit, free ? -1 : id++));
            free = !free;

            blocks = [.. diskItems];
            buffer = new DiskBlock[blocks.Length];
        }

        Array.Copy(blocks, buffer, blocks.Length);

        var (left, right) = (0, buffer.Length - 1);

        while (left <= right)
            if (buffer[left].Value != -1)
            {
                defrag.Add(buffer[left]);
                left++;
            }
            else if (buffer[right].Value == -1)
            {
                right--;
            }
            else
            {
                var count = Math.Min(buffer[left].Count, buffer[right].Count);
                defrag.Add(new DiskBlock(count, buffer[right].Value));

                if (buffer[left].Count == count)
                    left++;
                else
                    buffer[left] = new DiskBlock(buffer[left].Count - count, buffer[left].Value);

                if (buffer[right].Count == count)
                    right--;
                else
                    buffer[right] = new DiskBlock(buffer[right].Count - count, buffer[right].Value);
            }

        return CheckSum(defrag);
    }

    public static ulong Part2(string input)
    {
        DiskBlock[] blocks = [];
        List<DiskBlock> diskItems = [];

        var free = false;
        var id = 0;

        using var file = File.OpenRead(input);
        using var reader = new StreamReader(file);
        while (!reader.EndOfStream)
        {
            var cr = (char)reader.Read();
            var digit = int.Parse(cr.ToString());

            diskItems.Add(new DiskBlock(digit, free ? -1 : id++));
            free = !free;

            blocks = [.. diskItems];
        }

        var buffer = new List<DiskBlock>(blocks);

        for (var tail = buffer.Count - 1; tail >= 0; tail--)
        {
            if (buffer[tail].Value == -1)
                continue;

            for (var head = 0; head < tail; head++)
                if (buffer[head].Value == -1 && buffer[head].Count >= buffer[tail].Count)
                {
                    var count = buffer[tail].Count;
                    var diff = buffer[head].Count - count;

                    buffer[head] = new DiskBlock(count, buffer[tail].Value);
                    buffer[tail] = new DiskBlock(count, -1);

                    if (diff > 0)
                    {
                        buffer.Insert(head + 1, new DiskBlock(diff, -1));
                        tail++;
                    }

                    break;
                }
        }

        return CheckSum(buffer);
    }

    private static ulong CheckSum(IEnumerable<DiskBlock> disk)
    {
        return disk.Aggregate((sum: 0UL, index: 0), (acc, b) =>
            (acc.sum + CheckSum(b, acc.index), acc.index + b.Count)).sum;
    }

    private static ulong CheckSum(DiskBlock block, int index)
    {
        return block.Value == -1
            ? 0UL
            : (ulong)((index + (index + block.Count - 1)) * block.Count) / 2UL * (ulong)block.Value;
    }
}