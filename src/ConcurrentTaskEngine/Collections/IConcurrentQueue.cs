using ConcurrentTaskEngine.Core;

namespace ConcurrentTaskEngine.Collections;

internal interface IConcurrentQueue
{
    public void Enqueue(WorkItem item);
    public WorkItem Dequeue();
}
