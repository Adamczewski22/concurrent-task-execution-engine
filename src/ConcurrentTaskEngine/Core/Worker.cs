using ConcurrentTaskEngine.Scheduling;

namespace ConcurrentTaskEngine.Core;


internal class Worker
{
    private static long _nextId = 0;
    public long Id { get; }

    private readonly SharedWorkQueueSP _workQueue;

    public Worker(SharedWorkQueueSP workQueue)
    {
        _workQueue = workQueue;
        Id = Interlocked.Increment(ref _nextId);
    }

    public void Run()
    {
        while (true)
        {
            WorkItem item = _workQueue.Dequeue();
            
            // Stop in case of a poison pill
            if (item.Work == null) break;

            item.Work();
        }
    }
}
