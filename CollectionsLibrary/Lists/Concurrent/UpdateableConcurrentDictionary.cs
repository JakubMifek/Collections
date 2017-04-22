﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CollectionsLibrary.Updateable;

namespace CollectionsLibrary.Concurrent
{
    public interface IConcurrentUpdatebleDictionary<in THash, TKey, TValue> : IUpdateable<TValue>, IDictionary<TKey, TValue>
    {
        KeyValuePair<TKey, TValue> this[THash hash] { get; set; }
        KeyValuePair<TKey, TValue> this[int index] { get; }
        KeyValuePair<TKey, TValue>[] GetViewBetween(int lower, int upper);
    }

    public class SortedConcurrentUpdateableDictionary<THash, TKey, TValue> : IConcurrentUpdatebleDictionary<THash, TKey, TValue> where TKey : IComparable<TKey>
    {
        public SortedConcurrentUpdateableDictionary()
        {
            OnUpdate += (sender, args) => { lock (_sync) _sorted = new KeyValuePair<TKey, TValue>[0]; };
        }

        ConcurrentDictionary<TKey, TValue> dict = new ConcurrentDictionary<TKey, TValue>();

        public KeyValuePair<TKey, TValue> this[int index]
        {
            get
            {
                if (0 <= index && index < Count)
                    return GetViewBetween(index, index + 1)[0];
                else throw new InvalidOperationException("Index out of bounds of dictionary.");
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                return dict[key];
            }

            set
            {
                dict[key] = value;
                OnUpdate?.Invoke(this, new UpdateEventArgs<TValue>(Change.Edit, dict[key]));
            }
        }

        public KeyValuePair<TKey, TValue> this[THash hash]
        {
            get
            {
                foreach (var item in dict)
                {
                    if (item.Key.ToString() == hash.ToString())
                        return item;
                }
                return new KeyValuePair<TKey, TValue>(default(TKey), default(TValue));
            }

            set
            {
                foreach (var item in dict)
                {
                    if (item.Key.ToString() == hash.ToString())
                    {
                        dict[item.Key] = value.Value;
                        return;
                    }
                }
            }
        }

        public int Count => dict.Count;

        public bool IsReadOnly => false;

        public ICollection<TKey> Keys
        {
            get
            {
                var list = new List<TKey>(dict.Keys);
                list.Sort();
                return list;
            }
        }

        private class KeyValuePairComparer : IComparer<KeyValuePair<TKey, TValue>>
        {
            static KeyValuePairComparer _instance;
            public static KeyValuePairComparer Instance => _instance ?? (_instance = new KeyValuePairComparer());

            public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            {
                return x.Key.CompareTo(y.Key);
            }
        }

        private class ValueEnumerator : IEnumerable<TValue>
        {
            KeyValuePair<TKey, TValue>[] array;

            public ValueEnumerator(KeyValuePair<TKey, TValue>[] array)
            {
                this.array = array;
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                foreach (var item in array)
                    yield return item.Value;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                foreach (var item in array)
                    yield return item.Value;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                var array = dict.ToArray();
                Array.Sort(array, KeyValuePairComparer.Instance);
                return new List<TValue>(new ValueEnumerator(array));
            }
        }

        public event UpdateHandler<TValue> OnUpdate;

        protected void Update(UpdateEventArgs<TValue> args)
        {
            OnUpdate?.Invoke(this, args);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(TKey key, TValue value)
        {
            dict.TryAdd(key, value);
            OnUpdate?.Invoke(this, new UpdateEventArgs<TValue>(Change.Add, value));
        }

        public void Clear()
        {
            dict.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            foreach (var smtg in dict)
                if (smtg.Key.CompareTo(item.Key) == 0)
                    return true;

            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return dict.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var item in dict)
                array[arrayIndex++] = item;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        KeyValuePair<TKey, TValue>[] _sorted = new KeyValuePair<TKey, TValue>[0];
        readonly object _sync = new object();

        public KeyValuePair<TKey, TValue>[] GetViewBetween(int lower, int upper)
        {
            if (upper < lower) throw new InvalidOperationException();
            if (upper == lower) return new KeyValuePair<TKey, TValue>[0];

            KeyValuePair<TKey, TValue>[] output;

            lock (_sync)
            {
                if (_sorted.Length == 0 || _sorted.Length != Count)
                {
                    _sorted = dict.ToArray();
                    Array.Sort(_sorted, KeyValuePairComparer.Instance);
                }

                var size = Math.Min(upper - lower, Count);
                output = new KeyValuePair<TKey, TValue>[size];
                Array.ConstrainedCopy(_sorted, lower, output, 0, size);
            }

            return output;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool Remove(TKey key)
        {
            TValue removed;
            var ret = dict.TryRemove(key, out removed);
            OnUpdate?.Invoke(this, new UpdateEventArgs<TValue>(Change.Remove, removed));
            return ret;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dict.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dict.GetEnumerator();
        }
    }
}
