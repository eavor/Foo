using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foo
{
    public static class CommonExtension
    {
        public static bool IsNull<T>(this List<T> obj) => obj == null || obj.Count() == 0;
    }
}
