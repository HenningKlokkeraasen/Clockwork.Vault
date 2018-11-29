using System;
using Clockwork.Vault.Dao.Models.Core;

namespace Clockwork.Vault.Dao.Models.Master
{
    public class FavoriteBase : AppGeneratedEntityBase
    {
        public DateTime Created { get; set; }

        public SourceEnum Source { get; set; }
    }
}