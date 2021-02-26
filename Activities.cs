using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("readFromBlob")]

namespace Company.Function
{
    public static class Activities
    {
        public static IEnumerable<T> passthrough<T>(IEnumerable<T> ip)
        {
            foreach (T r in ip)
            {
                yield return r;
            }
        }
        public static IEnumerable<Row> greenEyesOnlyFilter(IEnumerable<Row> ip)
        {
            foreach (Row r in ip)
            {
                if (r.eyeColor == "green")
                {
                    yield return r;
                }
            }
        }
    }
}