using System.ComponentModel.DataAnnotations;

namespace Clockwork.Vault.Dao.Models
{
    public abstract class AppGeneratedEntityBase
    {
        [Key]
        public int AppGeneratedId { get; set; }
    }
}