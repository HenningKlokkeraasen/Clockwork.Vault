using System.ComponentModel.DataAnnotations;

namespace Clockwork.Vault.Dao.Models.Tidal.ManualData
{
    public class TidalPlaylistCreatorManual
    {
        [Key]
        public int AppGeneratedId { get; set; }

        public int CreatorId { get; set; }
        public virtual TidalCreator Creator { get; set; }

        /// <summary>
        /// Entered manually
        /// </summary>
        public string Name { get; set; }
    }
}