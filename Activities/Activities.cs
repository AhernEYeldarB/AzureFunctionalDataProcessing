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
            IEnumerable returnFunc(IEnumerable ip)
            {
                foreach (object r in ip)
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
            IEnumerable returnFunc(IEnumerable ip)
            {
                foreach (T r in ip)
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
            IEnumerable returnFunc(IEnumerable ip)
            {
                foreach (T r in ip)
                {
                    yield return callback(r);
                }
            };
            return returnFunc;
        }
        public static Func<IEnumerable, IEnumerable> pipelineMaker(params Func<IEnumerable, IEnumerable>[] a)
        {
            IEnumerable returnFunc(IEnumerable ip)
            {
                IEnumerable tail = ip;
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
