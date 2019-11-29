using System.Collections.Generic;

namespace NBB.MultiTenant.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNullOrDefault<T>(this T obj)
        {
            return EqualityComparer<T>.Default.Equals(obj, default);            
        }
    }
}
