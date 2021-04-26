using System;
using System.Collections;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("../readFromBlob")]

namespace Company.Function
{
    public static class Activities
    {
        public static Func<IEnumerable, IEnumerable> eachMaker(Action<object>? callback = null)
        {
            IEnumerable returnFunc(IEnumerable source)
            {
                foreach (object r in source)
                {
                    if (callback != null)
                    {
                        callback(r);
                    }
                    yield return r;
                }
            };
            return returnFunc;
        }

        public static Func<IEnumerable, IEnumerable> filterMaker<T>(Func<T, bool> callback)
        {
            IEnumerable returnFunc(IEnumerable source)
            {
                foreach (T r in source)
                {
                    if (callback(r))
                    {
                        yield return r;
                    }
                }
            };
            return returnFunc;
        }

        public static Func<IEnumerable, IEnumerable> mapMaker<T, U>(Func<T, U> callback)
        {
            IEnumerable returnFunc(IEnumerable source)
            {
                foreach (T r in source)
                {
                    yield return callback(r);
                }
            };
            return returnFunc;
        }

        public static Func<IEnumerable, IEnumerable> pipelineMaker(params Func<IEnumerable, IEnumerable>[] a)
        {
            IEnumerable returnFunc(IEnumerable source)
            {
                IEnumerable tail = source;
                foreach (Func<IEnumerable, IEnumerable> activity in a)
                {
                    tail = activity.Invoke(tail);
                }
                return tail;
            }
            return returnFunc;
        }
    }
}
