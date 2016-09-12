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
        private object lockingObject = new object();

        private readonly int maxSize;

        public BlockingQueue(int maxSize)
        {
            this.maxSize = maxSize;
        }

        public bool HasReachedMaxSize()
        {
            if (maxSize == -1)
            {
                return false;
            }
            return queue.Count >= maxSize;
        }

        public ConcurrentQueue<T> FetchAllAndClear()
        {
            lock (lockingObject)
            {
                var existingItems = new ConcurrentQueue<T>(queue);
                queue = new ConcurrentQueue<T>();
                return existingItems;
            }
        }

        public void Enqueue(T item)
        {
            lock (lockingObject)
            {
                if (!HasReachedMaxSize())
                {
                    queue.Enqueue(item);
                }
            }
        }
        public T Dequeue()
        {
            lock (lockingObject)
            {
                T item;
                queue.TryDequeue(out item);
                return item;
            }
        }
    }
}
