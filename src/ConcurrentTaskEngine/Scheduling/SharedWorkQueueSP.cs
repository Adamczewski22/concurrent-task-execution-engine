using System.Threading;

using ConcurrentTaskEngine.Collections;
using ConcurrentTaskEngine.Core;

namespace ConcurrentTaskEngine.Scheduling;


internal class SharedWorkQueueSP
{
    private readonly SemaphoreSlim _semaphore;
    private readonly IConcurrentQueue _queue;
    private readonly int _workerCount;
    private bool _stopped;

    public SharedWorkQueueSP(IConcurrentQueue queue, int workerCount)
    {
        _semaphore = new SemaphoreSlim(0);
        _queue = queue;
        _workerCount = workerCount;
        _stopped = false;
    }

    public void Enqueue(WorkItem item)
    {
        if (_stopped) throw new InvalidOperationException("The work queue has been stopped.");

        _queue.Enqueue(item);
        _semaphore.Release();
    }

    public WorkItem Dequeue()
    {
        _semaphore.Wait();
        return _queue.Dequeue();
    }

    public void RequestStop()
    {
        if (_stopped) return;
        _stopped = true;

        // Enqueue a poison pill for each worker
        for (int i = 0; i < _workerCount; i++)
        {
            EnqueuePoisonPill();
        }
    }

    private void EnqueuePoisonPill()
    {
        WorkItem poisonPill = new(null);
        _queue.Enqueue(poisonPill);
        _semaphore.Release();
    }
}
