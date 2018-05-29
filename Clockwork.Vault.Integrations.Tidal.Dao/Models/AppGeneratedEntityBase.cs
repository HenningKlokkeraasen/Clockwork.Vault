using System.ComponentModel.DataAnnotations;

namespace Clockwork.Vault.Integrations.Tidal.Dao.Models
{
    public abstract class AppGeneratedEntityBase
    {
        [Key]
        public int AppGeneratedId { get; set; }
    }
}