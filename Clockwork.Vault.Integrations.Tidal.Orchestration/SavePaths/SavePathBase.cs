using System;
using System.Collections.Generic;
using System.Linq;
using Clockwork.Vault.Integrations.Tidal.Orchestration.SaveCommands;
using OpenTidl.Models;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration.SavePaths
{
    internal abstract class SavePathBase
    {
        protected readonly SaveTidalEntityHandler _saveTidalEntityHandler;

        protected SavePathBase(SaveTidalEntityHandler saveTidalEntityHandler)
        {
            _saveTidalEntityHandler = saveTidalEntityHandler;
        }

        /// <summary>
        /// Returns true if saved, false if already in insertedArtists
        /// </summary>
        protected bool SaveArtist(ICollection<ArtistModel> insertedArtists, ArtistModel item)
        {
            if (insertedArtists == null)
                throw new ArgumentNullException(nameof(insertedArtists));
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (insertedArtists.All(a => a.Id != item.Id))
            {
                _saveTidalEntityHandler.MapAndInsertArtist(item);
                insertedArtists.Add(item);
                return true;
            }

            return false;
        }
    }
}