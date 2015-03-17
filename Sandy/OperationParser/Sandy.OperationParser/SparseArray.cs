using System;
using System.Collections;
using System.Collections.Generic;

namespace AV.Cyclone.Sandy.OperationParser
{
    public class SparseArray<T> : IEnumerable<KeyValuePair<int, T>>
    {
        private readonly Dictionary<int, T> array = new Dictionary<int, T>();
        private readonly Func<int, T> activator;

        public SparseArray(Func<int, T> activator)
        {
            this.activator = activator;
        }

        public T this[int index]
        {
            get
            {
                T value;
                if (array.TryGetValue(index, out value)) return value;
                value = activator(index);
                array[index] = value;
                return value;
            }
        }

        public int Count
        {
            get { return array.Count; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
        {
            return array.GetEnumerator();
        }
    }
}