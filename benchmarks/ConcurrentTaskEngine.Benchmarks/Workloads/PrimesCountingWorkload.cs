namespace ConcurrentTaskEngine.Benchmarks.Workloads;


public static class PrimesCountingWorkload
{
    public static int Execute(int range)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(range);

        int primeCount = 0;
        bool[] notCandidates = new bool[range];

        for (int i = 2; i <= range; i++)
        {
            if (notCandidates[i - 1]) continue;

            primeCount++;

            if (i > range / i) continue;

            for (int j = i * i; j <= range; j += i)
            {
                notCandidates[j - 1] = true;
            }
        }
        return primeCount;
    }
}
