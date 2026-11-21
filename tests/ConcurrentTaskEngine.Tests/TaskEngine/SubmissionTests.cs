using ConcurrentTaskEngine;
using Xunit;

namespace ConcurrentTaskEngine.Tests.TaskEngineTests;


public class SubmissionTests
{
    [Fact]
    public void CounterIncrementation()
    {
        var engine = new TaskEngine(workerCount: Environment.ProcessorCount);
        int counter = 0;
        int count = 1000;
        
        for (int i = 0; i < count; i++)
        {
            engine.Submit(() =>
            {
                Interlocked.Increment(ref counter);
            });
        }
        engine.Stop();
        
        Assert.Equal(count, counter);
    }
}
