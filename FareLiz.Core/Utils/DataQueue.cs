using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace SkyDean.FareLiz.Core.Utils
{
    [DebuggerDisplay("Count = {Count}")]
    public class DataQueue<T> : IDisposable
    {
        private readonly ReaderWriterLockSlim _syncLock = new ReaderWriterLockSlim();
        private readonly List<T> _data = new List<T>();

        public DataQueue() { }
        public DataQueue(IEnumerable<T> initialData)
            : this()
        {
            _data.AddRange(initialData);
        }

        public void Enqueue(T item)
        {
            _syncLock.EnterWriteLock();
            _data.Add(item);
            _syncLock.ExitWriteLock();
        }

        public void Enqueue(IEnumerable<T> items)
        {
            _syncLock.EnterWriteLock();
            _data.AddRange(items);
            _syncLock.ExitWriteLock();
        }

        public IList<T> Dequeue(int count)
        {
            _syncLock.EnterWriteLock();
            var result = new List<T>();
            int taking = count > _data.Count ? _data.Count : count;
            for (int i = 0; i < taking; i++)
            {
                var item = _data[i];
                result.Add(item);
            }
            _data.RemoveRange(0, taking);
            _syncLock.ExitWriteLock();
            return result;
        }

        public void Clear(bool dispose)
        {
            _syncLock.EnterWriteLock();
            if (_data.Count > 0)
            {                
                if (dispose && typeof(IDisposable).IsAssignableFrom(typeof(T)))
                {
                    foreach (var item in _data)
                        ((IDisposable)item).Dispose();
                }
                _data.Clear();                
            }
            _syncLock.ExitWriteLock();
        }

        public void Remove(T item)
        {
            _syncLock.EnterWriteLock();
            _data.Remove(item);
            _syncLock.ExitWriteLock();
        }

        public void Remove(IEnumerable<T> items)
        {
            _syncLock.EnterWriteLock();
            foreach (var item in items)
            {
                _data.Remove(item);
            }
            _syncLock.ExitWriteLock();
        }

        public void Transfer(DataQueue<T> targetQueue)
        {
            _syncLock.EnterWriteLock();
            if (_data.Count > 0)
            {
                targetQueue._data.AddRange(_data);
                _data.Clear();
            }
            _syncLock.ExitWriteLock();
        }

        public int Count
        {
            get
            {
                int result = 0;
                _syncLock.EnterReadLock();
                result = _data.Count;
                _syncLock.ExitReadLock();
                return result;
            }
        }

        public List<T> ToList()
        {
            _syncLock.EnterReadLock();
            var result = new List<T>(_data.Count);
            if (_data.Count > 0)
                result.AddRange(_data);
            _syncLock.ExitReadLock();
            return result;
        }

        public T[] ToArray()
        {
            T[] result;
            _syncLock.EnterReadLock();
            result = _data.ToArray();
            _syncLock.ExitReadLock();
            return result;
        }

        bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_syncLock != null)
                        _syncLock.Dispose();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
