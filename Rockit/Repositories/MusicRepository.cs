using Rockit.Data;
using Rockit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockit.Repositories
{
    public class MusicRepository
    {
        private readonly RockolaDbContext _context;

        public MusicRepository()
        {
            _context = new RockolaDbContext();
        }
        public void LoadDataToMemory()
        {
            var artists = _context.Artists.ToList();
            var songs = _context.Songs.ToList();

            ArtistStore.ListOfArtist = artists;
            SongStore.ListOfSongs = songs;
        }
        public void SaveDataToDatabase()
        {
            foreach (var artist in ArtistStore.ListOfArtist)
            {
                if (!_context.Artists.Any(a => a.ArtistId == artist.ArtistId))
                {
                    _context.Artists.Add(artist);
                }
            }

            foreach (var song in SongStore.ListOfSongs)
            {
                if (!_context.Songs.Any(s => s.SongId == song.SongId))
                {
                    _context.Songs.Add(song);
                }
            }

            _context.SaveChanges();
        }
        public void UpdateSongPlayCount(int songId)
        {
            var song = _context.Songs.FirstOrDefault(s => s.SongId == songId);
            if (song != null)
            {
                song.Rp++;
                _context.SaveChanges();
            }
        }

        public void UpdateArtistPlayCount(int artistId)
        {
            var artist = _context.Artists.FirstOrDefault(a => a.ArtistId == artistId);
            if (artist != null)
            {
                artist.Rp++;
                _context.SaveChanges();
            }
        }
        public bool ExistsArtist(int artistId)
        {
            return _context.Artists.Any(a => a.ArtistId == artistId);
        }

        public void AddArtist(Artist artist)
        {
            _context.Artists.Add(artist);
            _context.SaveChanges();
        }

        public bool ExistsSong(int songId)
        {
            return _context.Songs.Any(s => s.SongId == songId);
        }

        public void AddSong(Song song)
        {
            _context.Songs.Add(song);
            _context.SaveChanges();
        }

        public List<Song> GetSongsByArtistName(string artistName)
        {
            return _context.Songs
                .Where(s => s.ArtistName == artistName)
                .ToList();
        }

        public List<Artist> GetAllArtists() => _context.Artists.ToList();
        public List<Song> GetAllSongs() => _context.Songs.ToList();

        public void DeleteArtist(Artist artist) => _context.Artists.Remove(artist);
        public void DeleteSong(Song song) => _context.Songs.Remove(song);

        public bool HasArtists() => _context.Artists.Any();
        public bool HasSongs() => _context.Songs.Any();

        public void SaveChanges() => _context.SaveChanges();
    }

}
