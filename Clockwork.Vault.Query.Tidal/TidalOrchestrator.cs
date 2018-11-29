using System.Collections.Generic;
using System.Linq;
using Clockwork.Vault.Dao.Models.Tidal;
using Clockwork.Vault.Query.Tidal.ViewModels;

namespace Clockwork.Vault.Query.Tidal
{
    public class TidalOrchestrator
    {
        private readonly TidalRepository _tidalRepository;

        public TidalOrchestrator()
        {
            _tidalRepository = new TidalRepository();
        }

        public IList<TidalArtist> Artists => _tidalRepository.Artists;

        public IList<TidalAlbum> Albums => _tidalRepository.Albums;

        public IList<TidalPlaylist> Playlists => _tidalRepository.Playlists;

        public IList<TidalTrack> Tracks => _tidalRepository.Tracks;

        public TidalArtist GetArtist(int id) => _tidalRepository.GetArtist(id);

        public TidalAlbum GetAlbum(int id) => _tidalRepository.GetAlbum(id);

        public TidalPlaylist GetPlaylist(string id) => _tidalRepository.GetPlaylist(id);

        public TidalTrack GetTrack(int id) => _tidalRepository.GetTrack(id);

        public IList<TidalTrackArtist> GetArtists(TidalTrack track) => _tidalRepository.GetArtists(track);

        public IList<TidalAlbumArtist> GetArtists(TidalAlbum album) => _tidalRepository.GetArtists(album);

        public IList<TidalAlbumTrack> GetTracks(TidalAlbum album) => _tidalRepository.GetTracks(album);

        public TidalTrackExpanded GetTrackExpanded(int id)
        {
            var track = _tidalRepository.GetTrack(id);
            if (track == null)
                return null;

            var trackArtists = _tidalRepository.GetArtists(track)
                .Select(a => a.Artist)
                .ToList();

            return new TidalTrackExpanded
            {
                Track = track,
                Artists = trackArtists
            };
        }

        public TidalAlbumExpanded GetAlbumExpanded(int id)
        {
            var album = _tidalRepository.GetAlbum(id);
            if (album == null)
                return null;

            var albumArtists = _tidalRepository.GetArtists(album)
                .Select(a => a.Artist)
                .ToList();

            var tracks = _tidalRepository.GetTracks(album)
                .Select(t => t.TrackId)
                .Select(GetTrackExpanded)
                .ToList();

            return new TidalAlbumExpanded
            {
                Album = album,
                Artists = albumArtists,
                Tracks = tracks
            };
        }

        public TidalPlaylistExpanded GetPlaylistExpanded(string id)
        {
            var playlist = _tidalRepository.GetPlaylist(id);
            if (playlist == null)
                return null;

            var tracks = _tidalRepository.GetTracks(playlist)
                .Select(t=>t.TrackId)
                .Select(GetTrackExpanded)
                .ToList();

            return new TidalPlaylistExpanded
            {
                Playlist = playlist,
                Tracks = tracks
            };
        }
    }
}