using System.Threading;

using ConcurrentTaskEngine.Core;
using ConcurrentTaskEngine.Scheduling;
using ConcurrentTaskEngine.Collections;

namespace ConcurrentTaskEngine;


public class TaskEngine
{
    public int WorkerCount {get; }

    private readonly SharedWorkQueueSP _workQueue;
    private readonly List<Thread> _threads;
    private readonly Lock _submission_lock;
    private bool _stopped;

    public TaskEngine(int workerCount)
    {
        if (workerCount < 1) throw new ArgumentOutOfRangeException("workerCount must be positive");
        WorkerCount = workerCount;

        // Init work queue
        IConcurrentQueue queue = new SPMCQueue();
        _workQueue = new SharedWorkQueueSP(queue, workerCount);

        // Init worker threads
        _threads = [];

        _submission_lock = new Lock();
        _stopped = false;
        
        for (int i = 0; i < workerCount; i++)
        {
            Worker worker = new(_workQueue);
            Thread t = new(new ThreadStart(worker.Run));
            t.IsBackground = true;
            t.Start();

            _threads.Add(t);
        }
    }

    public void Submit(Action work)
    {   
        ArgumentNullException.ThrowIfNull(work);

        if (_threads.Contains(Thread.CurrentThread)) throw new InvalidOperationException("A worker thread must not call engine methods.");

        lock (_submission_lock)
        {
            if (_stopped) throw new InvalidOperationException("Cannot submit after the task engine has been stopped.");
            WorkItem item = new(work);
            _workQueue.Enqueue(item);
        }
    }

    public void Stop()
    {
        if (_threads.Contains(Thread.CurrentThread)) throw new InvalidOperationException("A worker thread must not call engine methods.");

        lock (_submission_lock)
        {
            if (!_stopped)
            {
                // Stop the queue
                _workQueue.RequestStop();
                _stopped = true;
            }
        }

        // Wait for the threads
        foreach (var thread in _threads)
        {
            thread.Join();
        }
    }
}
