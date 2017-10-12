using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsLibrary.Lists
{
    public class LazyFunctionEnumerator<T> : IEnumerable<T>
    {
        private readonly Func<T> _func;

        public LazyFunctionEnumerator(Func<T> function)
        {
            _func = function;
        }

        public IEnumerator<T> GetEnumerator()
        {
            while (true)
                yield return _func.Invoke();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
