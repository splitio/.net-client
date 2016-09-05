using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Splitio.Services.Impressions.Classes
{
    public class BlockingQueue<T>
    {
        private ConcurrentQueue<T> queue = new ConcurrentQueue<T>();
        private readonly int maxSize;

        public BlockingQueue(int maxSize)
        {
            this.maxSize = maxSize;
        }

        public bool HasReachedMaxSize()
        {
            return queue.Count >= maxSize;
        }

        public ConcurrentQueue<T> FetchAllAndClear()
        {
            lock (queue)
            {
                var existingItems = new ConcurrentQueue<T>(queue);
                queue = new ConcurrentQueue<T>();
                return existingItems;
            }
        }

        public void Enqueue(T item)
        {
            lock (queue)
            {
                if (!HasReachedMaxSize())
                {
                    queue.Enqueue(item);
                }
            }
        }
        public T Dequeue()
        {
            lock (queue)
            {
                T item;
                queue.TryDequeue(out item);
                return item;
            }
        }
    }
}
