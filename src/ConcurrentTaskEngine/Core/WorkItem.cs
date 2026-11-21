namespace ConcurrentTaskEngine.Core;

internal class WorkItem
{
    private static long _nextId = 0;
    public long Id { get; }

    public Action? Work { get; set; }


    public WorkItem(Action? work)
    {
        Id = Interlocked.Increment(ref _nextId);
        Work = work;
    }
}
