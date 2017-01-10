﻿using CollectionsLibrary.Updateable;
using System.Collections;
using System.Collections.Generic;

namespace CollectionsLibrary.Concurrent
{
    public interface IConcurrentUpdatableList<T> : IList<T>, IUpdateable<T>
    {
    }

    public class ConcurrentUpdateableList<T> : IConcurrentUpdatableList<T>
    {
        List<T> _list = new List<T>();
        object _sync = new object();

        public T this[int index]
        {
            get
            {
                lock (_sync)
                    return _list[index];
            }

            set
            {
                lock (_sync)
                    _list[index] = value;

                OnUpdate?.Invoke(this, new UpdateEventArgs<T>(Change.Edit, value));
            }
        }

        public int Count
        {
            get
            {
                lock (_sync)
                    return _list.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public event UpdateHandler<T> OnUpdate;

        public void Add(T item)
        {
            lock (_sync)
                _list.Add(item);

            OnUpdate?.Invoke(this, new UpdateEventArgs<T>(Change.Add, item));
        }

        public void Clear()
        {
            lock (_sync)
            {
                _list.Clear();
                OnUpdate?.Invoke(this, new UpdateEventArgs<T>(Change.Remove, default(T)));
            }
        }

        public bool Contains(T item)
        {
            lock (_sync)
                return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_sync)
                _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            List<T> copy;
            lock (_sync)
                copy = new List<T>(this);

            return copy.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            lock (_sync)
                return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            lock (_sync)
            {
                _list.Insert(index, item);
                OnUpdate?.Invoke(this, new UpdateEventArgs<T>(Change.Add, item));
            }
        }

        public bool Remove(T item)
        {
            lock (_sync)
            {
                OnUpdate?.Invoke(this, new UpdateEventArgs<T>(Change.Remove, item));
                return _list.Remove(item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_sync)
            {
                var item = _list[index];
                _list.RemoveAt(index);
                OnUpdate?.Invoke(this, new UpdateEventArgs<T>(Change.Remove, item));
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
