using System.Collections.Generic;
using System.Linq;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration
{
    public static class Extensions
    {
        public static IList<T> ProjectToList<T>(this IQueryable<T> x) where T : class => x.Select(i => i).ToList();
    }
}