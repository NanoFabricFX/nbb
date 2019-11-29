using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NBB.MultiTenant.EntityFramework.Extensions
{
    public static class IEnumerableExtensions
    {
        public static List<TSuper> ConvertListType<TSuper>(this IEnumerable<object> list) where TSuper : class
        {
            var result = new List<TSuper>();
            foreach (var x in list)
            {
                result.Add(x as TSuper);
            }
            return result;
        }

        public static async Task<List<TSuper>> ToListWithConvertListTypeAsync<TSuper>(this IQueryable<object> query, CancellationToken cancellationToken = default) where TSuper : class
        {
            var list = await query.ToListAsync(cancellationToken);
            var result = new List<TSuper>();
            foreach (var x in list)
            {
                result.Add(x as TSuper);
            }
            return result;
        }
    }
}