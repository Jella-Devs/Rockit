using Microsoft.EntityFrameworkCore;
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
            try
            {
                _context = new RockolaDbContext();

                // Probar conexión de inmediato
                _context.Database.OpenConnection();
                _context.Database.CloseConnection();
            }
            catch (Npgsql.NpgsqlException ex)
            {
                MessageBox.Show($"[PostgreSQL] Error al conectar con la base de datos:\n{ex.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[General] Error inesperado:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
        public void LoadDataToMemory()
        {
            var artists = _context.Artists.OrderBy(a => a.Name).ToList();
            var songs = _context.Songs.OrderBy(a => a.Name).ToList();

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
        public void RegisterSongPlay(string songPath)
        {
            var memSong = SongStore.ListOfSongs.FirstOrDefault(s => s.Path == songPath || $"{s.Path}\\{s.Name}" == songPath);
            if (memSong != null)
            {
                var song = _context.Songs.FirstOrDefault(s => s.SongId == memSong.SongId);
                if (song != null)
                {
                    song.Rp++;
                    _context.Songs.Update(song);
                    _context.SaveChanges();

                    var artist = _context.Artists.FirstOrDefault(a => a.Name == song.ArtistName);
                    if (artist != null)
                    {
                        artist.Rp = _context.Songs.Where(s => s.ArtistName == song.ArtistName).Sum(s => s.Rp);
                        _context.Artists.Update(artist);
                        _context.SaveChanges();
                    }

                    // Sincronizar con la memoria
                    memSong.Rp = song.Rp;
                    var memArtist = ArtistStore.ListOfArtist.FirstOrDefault(a => a.Name == song.ArtistName);
                    if (memArtist != null && artist != null) memArtist.Rp = artist.Rp;
                }
            }
        }

        public Artist GetArtistById(int artistId)
        {
            return _context.Artists.FirstOrDefault(a => a.ArtistId == artistId);
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

        public void ClearCatalog()
        {
            _context.Songs.RemoveRange(_context.Songs);
            _context.Artists.RemoveRange(_context.Artists);
            _context.SaveChanges();
        }

        public void AddToPlaylist(string name, string path)
        {
            var item = new PlayListItem
            {
                SongName = name,
                SongPath = path
            };
            _context.PlaylistItems.Add(item);
            _context.SaveChanges();
        }

        public void RemoveToPlaylist(PlayListItem song)
        {
            var existing = _context.PlaylistItems.FirstOrDefault(p => p.SongName == song.SongName);
            if (existing != null)
            {
                _context.PlaylistItems.Remove(existing);
                _context.SaveChanges();
            }
        }

        public List<PlayListItem> GetPlaylist()
        {
            return _context.PlaylistItems
                .OrderBy(p => p.AddedAt)
                .ToList();
        }

        public void ClearPlaylist()
        {
            _context.PlaylistItems.RemoveRange(_context.PlaylistItems);
            _context.SaveChanges();
        }

    }

}
