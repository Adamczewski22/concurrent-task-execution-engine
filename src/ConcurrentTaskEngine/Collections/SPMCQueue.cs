using System.Threading;

using ConcurrentTaskEngine.Core;

namespace ConcurrentTaskEngine.Collections;


internal class SPMCQueue : IConcurrentQueue
{
    private sealed class Node
    {
        public WorkItem? WorkItem {get; }
        public Node? Next {get; set; }

        public Node (WorkItem? workItem)
        {
            WorkItem = workItem;
            Next = null;
        }
    }

    private readonly Lock _dequeue_lock;

    private Node _head;
    private Node _tail;

    public SPMCQueue () {
        _dequeue_lock = new Lock();
        
        Node dummy = new Node(null);
        _head = dummy;
        _tail = dummy;
    }

    public void Enqueue(WorkItem item)
    {
        Node newNode = new Node(item);
        _tail.Next = newNode;
        _tail = newNode;
    }

    public WorkItem Dequeue()
    {
        lock (_dequeue_lock)
        {
            if (_head == _tail)
            {
                throw new InvalidOperationException("The queue is empty.");
            }
            _head = _head.Next;
            return _head.WorkItem;
        }
    }
}
