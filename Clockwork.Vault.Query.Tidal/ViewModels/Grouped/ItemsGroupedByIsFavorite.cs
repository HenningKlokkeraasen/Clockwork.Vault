using System.Collections.Generic;

namespace Clockwork.Vault.Query.Tidal.ViewModels.Grouped
{
    public class ItemsGroupedByIsFavorite<T>
    {
        public IList<T> Items { get; set; }
        public bool IsFavorite { get; set; }
    }
}