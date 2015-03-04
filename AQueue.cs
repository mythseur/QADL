using System;
using System.Collections.Generic;

namespace QAD
{
    /// <summary>
    /// A Queue that represents a Queue that will be downloaded asynchronously
    /// </summary>
    public class AQueue
    {
        /// <summary>
        /// Private Queue, stores the files to be downloaded
        /// </summary>
        private Queue<File> queue = new Queue<File>();

        /// <summary>
        /// Files actually in the queue
        /// </summary>
        public int Count { get { return queue.Count; } }


        /// <summary>
        /// Constructor
        /// </summary>
        public AQueue()
        {

        }

        /// <summary>
        /// A virtual implementation to enable changes and new event.
        /// Enqueue a file the queue.
        /// </summary>
        /// <param name="item">Item to be stored</param>
        public virtual void Enqueue(File item)
        {
            queue.Enqueue(item);
        }

        /// <summary>
        /// Dequeue a file from the queue. Virtual method for further changes
        /// </summary>
        /// <returns>The file that has been dequeued</returns>
        public virtual File Dequeue()
        {
            return queue.Dequeue();
        }


        /// <summary>
        /// Check if there is any file in the queue.
        /// </summary>
        /// <returns>True if there is any file presents in the queue</returns>
        public virtual Boolean Any()
        {
            return (queue.Count > 0);
        }

        /// <summary>
        /// Used in the foreach implementation
        /// </summary>
        /// <returns>An Enumerator on the queue files.</returns>
        public virtual Queue<File>.Enumerator GetEnumerator()
        {
            return queue.GetEnumerator();
        }
    }
}
