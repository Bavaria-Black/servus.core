namespace Servus.Core.Collections;

public class CircularQueue<T>(int capacity)
{
    private readonly Queue<T> _queue = new(capacity);

    public void Enqueue(T item)
    {
        if (_queue.Count >= capacity)
            _queue.Dequeue(); // Remove oldest
        
        _queue.Enqueue(item);
    }

    public bool TryDequeue(out T item) => _queue.TryDequeue(out item!);
    public int Count => _queue.Count;
    public int Capacity => capacity;
    public IEnumerable<T> Items => _queue;
}