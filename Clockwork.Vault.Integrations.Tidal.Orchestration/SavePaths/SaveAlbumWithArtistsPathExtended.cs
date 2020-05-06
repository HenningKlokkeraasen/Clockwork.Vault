using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clockwork.Vault.Dao;
using Clockwork.Vault.Integrations.Tidal.Orchestration.SaveCommands;
using OpenTidl.Models;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration.SavePaths
{
    /// <summary>
    /// Intended for use when provided album is a lesser model and/or without UPC information 
    /// </summary>
    internal class SaveAlbumWithArtistsPathExtended : SavePathBase
    {
        private readonly TidalIntegrator _tidalIntegrator;
        private readonly VaultContext _vaultContext;

        internal SaveAlbumWithArtistsPathExtended(SaveTidalEntityHandler saveTidalEntityHandler,
            TidalIntegrator tidalIntegrator, VaultContext vaultContext)
            : base(saveTidalEntityHandler)
        {
            _tidalIntegrator = tidalIntegrator;
            _vaultContext = vaultContext;
        }

        internal async Task<(AlbumModel, IList<string>)> Run(ICollection<AlbumModel> insertedAlbums,
            ICollection<ArtistModel> insertedArtists, AlbumModel item)
        {
            var log = new List<string>();
            if (insertedAlbums.Any(a => a.Id == item.Id))
                return (null, log);

            var (albumIsAlreadyInDbAndHasUpc, fullAlbum, getLog) = await GetFromDbOrFullAlbumFromTidal(item);
            log.AddRange(getLog);
            if (albumIsAlreadyInDbAndHasUpc)
                return (null, log);

            if (fullAlbum == null)
                return (null, log);

            insertedAlbums.Add(fullAlbum);
            var albumLog = new SaveAlbumWithArtistsPath(_saveTidalEntityHandler)
                .Run(fullAlbum, insertedArtists);
            log.AddRange(albumLog);

            return (fullAlbum, log);
        }

        private async Task<(bool, AlbumModel, IList<string>)> GetFromDbOrFullAlbumFromTidal(AlbumModel item)
        {
            var log = new List<string>();

            var (albumIsAlreadyInDbAndHasUpc, dbTestLog) = AlbumIsAlreadyInDbAndHasUpc(item);
            log.AddRange(dbTestLog);
            if (albumIsAlreadyInDbAndHasUpc)
                return (true, null, log);

            var (fullAlbum, albumLog) = await GetFullAlbum(item);
            log.Add(albumLog);

            return (false, fullAlbum, log);
        }

        private (bool, IList<string>) AlbumIsAlreadyInDbAndHasUpc(AlbumModel album)
        {
            var log = new List<string>();

            var existingRecord = _vaultContext.TidalAlbums.FirstOrDefault(p => p.Id == album.Id);
            if (existingRecord == null)
                return (false, log);

            log.Add($"Album exists in DB: {album.Id} {album.Title}");

            if (existingRecord.Upc == null)
            {
                log.Add($"    album does not have UPC in DB: {album.Id} {album.Title}");
                return (false, log);
            }

            log.Add("    album has UPC - will not get album or insert album or album artists");

            return (true, log);
        }

        private async Task<(AlbumModel, string)> GetFullAlbum(AlbumModel lesserAlbumModel)
        {
            var log = string.Empty;

            var albumResult = await _tidalIntegrator.GetAlbum(lesserAlbumModel.Id);
            if (albumResult == null)
            {
                log = $"WARN Could not get album {lesserAlbumModel.Id} {lesserAlbumModel.Title} - inserting lesser album model";
                albumResult = lesserAlbumModel;
            }

            return (albumResult, log);
        }
    }
}