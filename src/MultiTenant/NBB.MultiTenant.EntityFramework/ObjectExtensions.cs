using System;
using System.Collections.Generic;
using System.Text;

namespace NBB.MultiTenant.EntityFramework
{
    public static class ObjectExtensions
    {
        public static bool IsNullOrDefault<T>(this T value)
        {
            return ((object)default(T)) == null ?
                ((object)value) == null :
                default(T).Equals(value);
        }
    }
}
