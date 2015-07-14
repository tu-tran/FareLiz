namespace SkyDean.FareLiz.Core.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// The data queue.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public class DataQueue<T> : IDisposable
    {
        /// <summary>The _data.</summary>
        private readonly List<T> _data = new List<T>();

        /// <summary>The _sync lock.</summary>
        private readonly ReaderWriterLockSlim _syncLock = new ReaderWriterLockSlim();

        /// <summary>The disposed.</summary>
        private bool disposed;

        /// <summary>Initializes a new instance of the <see cref="DataQueue{T}" /> class.</summary>
        public DataQueue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataQueue{T}"/> class.
        /// </summary>
        /// <param name="initialData">
        /// The initial data.
        /// </param>
        public DataQueue(IEnumerable<T> initialData)
            : this()
        {
            this._data.AddRange(initialData);
        }

        /// <summary>Gets the count.</summary>
        public int Count
        {
            get
            {
                var result = 0;
                this._syncLock.EnterReadLock();
                result = this._data.Count;
                this._syncLock.ExitReadLock();
                return result;
            }
        }

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The enqueue.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void Enqueue(T item)
        {
            this._syncLock.EnterWriteLock();
            this._data.Add(item);
            this._syncLock.ExitWriteLock();
        }

        /// <summary>
        /// The enqueue.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public void Enqueue(IEnumerable<T> items)
        {
            this._syncLock.EnterWriteLock();
            this._data.AddRange(items);
            this._syncLock.ExitWriteLock();
        }

        /// <summary>
        /// The dequeue.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public IList<T> Dequeue(int count)
        {
            this._syncLock.EnterWriteLock();
            var result = new List<T>();
            var taking = count > this._data.Count ? this._data.Count : count;
            for (var i = 0; i < taking; i++)
            {
                var item = this._data[i];
                result.Add(item);
            }

            this._data.RemoveRange(0, taking);
            this._syncLock.ExitWriteLock();
            return result;
        }

        /// <summary>
        /// The clear.
        /// </summary>
        /// <param name="dispose">
        /// The dispose.
        /// </param>
        public void Clear(bool dispose)
        {
            this._syncLock.EnterWriteLock();
            if (this._data.Count > 0)
            {
                if (dispose && typeof(IDisposable).IsAssignableFrom(typeof(T)))
                {
                    foreach (var item in this._data)
                    {
                        ((IDisposable)item).Dispose();
                    }
                }

                this._data.Clear();
            }

            this._syncLock.ExitWriteLock();
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void Remove(T item)
        {
            this._syncLock.EnterWriteLock();
            this._data.Remove(item);
            this._syncLock.ExitWriteLock();
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public void Remove(IEnumerable<T> items)
        {
            this._syncLock.EnterWriteLock();
            foreach (var item in items)
            {
                this._data.Remove(item);
            }

            this._syncLock.ExitWriteLock();
        }

        /// <summary>
        /// The transfer.
        /// </summary>
        /// <param name="targetQueue">
        /// The target queue.
        /// </param>
        public void Transfer(DataQueue<T> targetQueue)
        {
            this._syncLock.EnterWriteLock();
            if (this._data.Count > 0)
            {
                targetQueue._data.AddRange(this._data);
                this._data.Clear();
            }

            this._syncLock.ExitWriteLock();
        }

        /// <summary>The to list.</summary>
        /// <returns>The <see cref="List" />.</returns>
        public List<T> ToList()
        {
            this._syncLock.EnterReadLock();
            var result = new List<T>(this._data.Count);
            if (this._data.Count > 0)
            {
                result.AddRange(this._data);
            }

            this._syncLock.ExitReadLock();
            return result;
        }

        /// <summary>The to array.</summary>
        /// <returns>The <see cref="T[]" />.</returns>
        public T[] ToArray()
        {
            T[] result;
            this._syncLock.EnterReadLock();
            result = this._data.ToArray();
            this._syncLock.ExitReadLock();
            return result;
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this._syncLock != null)
                    {
                        this._syncLock.Dispose();
                    }
                }

                this.disposed = true;
            }
        }
    }
}