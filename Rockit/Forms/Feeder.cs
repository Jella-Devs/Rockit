using Rockit.Models;
using Rockit.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rockit.Forms
{
    public partial class Feeder : Form
    {
        public Feeder()
        {
            InitializeComponent();
            this.KeyPreview = true;
        }
        private void btnCargar_Click(object sender, EventArgs e)
        {
            SelectSongsFolder();
            string pathFiles = Properties.Settings.Default.SongsFolderPath;
            try
            {
                var directories = Directory.GetDirectories(pathFiles);
                getRawSongs(directories);
            }
            catch (Exception ex) { MessageBox.Show("Ocurrió algo inesperado: " + ex); }
            this.Close();
        }
        private void getRawSongs(String[] directories)
        {
            //Inicializar variables temporales
            int TempArtistId = ArtistStore.ListOfArtist.Count() + 1;
            int TempSongsId = SongStore.ListOfSongs.Count() + 1;
            var newArtists = new List<Artist>();
            var newSongs = new List<Song>();
            
            // Inicializar listas si son nulas
            ArtistStore.ListOfArtist ??= new List<Artist>();
            SongStore.ListOfSongs ??= new List<Song>();

            // Crear índices para búsqueda rápida
            var existingArtistNames = new HashSet<string>(
                ArtistStore.ListOfArtist.Select(a => a.Name)
            );

            var existingSongKeys = new HashSet<string>(
                SongStore.ListOfSongs.Select(s => $"{s.ArtistName}|{s.Name}")
            );

            // Preparar rutas de salida
            string finderFolder = Properties.Settings.Default.FindFolderPath;
            string pathFinderResultSongs = Path.Combine(finderFolder, "FinderResultSongs.txt");
            string pathFinderResultArtist = Path.Combine(finderFolder, "FinderResultArtist.txt");

            if (!File.Exists(pathFinderResultSongs))
            {
                using FileStream fs = File.Create(pathFinderResultSongs);
                MessageBox.Show("pathFinderResultSong Creado");
            }

            if (!File.Exists(pathFinderResultArtist))
            {
                using FileStream fa = File.Create(pathFinderResultArtist);
                MessageBox.Show("pathFinderResultSong Creado");
            }

            // Procesar directorios
            foreach (var dir in directories)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                string artistName = dirInfo.Name;

                if (artistName.Contains(" - "))
                    artistName = artistName.Split(" -")[0];

                // Verificar si es un artista nuevo
                if (!existingArtistNames.Contains(artistName))
                {
                    string picturePath = Path.Combine(dir, "portada.jpg");
                    Artist newArtist = AddArtist(artistName, TempArtistId, picturePath);
                    ArtistStore.ListOfArtist.Add(newArtist);
                    newArtists.Add(newArtist);

                    existingArtistNames.Add(artistName); // actualizar índice
                    TempArtistId++; // Avanza el ID 
                }
                // Procesar canciones dentro del directorio
                foreach (var file in dirInfo.GetFiles("*.mp3"))
                {
                    string songKey = $"{artistName}|{file.Name}";

                    if (!existingSongKeys.Contains(songKey))
                    {
                        Song newSong = AddSong(file.Name, TempSongsId, dir, artistName);
                        SongStore.ListOfSongs.Add(newSong);
                        newSongs.Add(newSong);
                       
                        existingSongKeys.Add(songKey); // actualizar índice
                        TempSongsId++;
                    }
                }
            }
            // UPDATE
            if (newArtists.Any())
            {
                using var artistAdd = new StreamWriter(pathFinderResultArtist, append: true);
                foreach (var artist in newArtists)
                {
                    artistAdd.WriteLine(artist.ArtistId + "|" + artist.Name + "$" + artist.Picture);
                }
            }
            if (newSongs.Any())
            {
                using var songsWriter = new StreamWriter(pathFinderResultSongs, append: true);
                foreach (var song in newSongs)
                {
                    songsWriter.WriteLine(song.SongId + "|" + song.Path+ "$" + song.Name + "%" + song.ArtistName);
                }
            }

            Cleaner(directories, pathFinderResultArtist, pathFinderResultSongs);

            Properties.Settings.Default.ArtistCount = ArtistStore.ListOfArtist.Count();
            Properties.Settings.Default.Save();
            CounterShow("artist");
            CounterShow("songs");
            MessageBox.Show("Carga completa");
            var repo = new MusicRepository();
            repo.SaveDataToDatabase();
        }
        private Song AddSong(string name, int id, string path, string artist)
        {
            Song song = new Song();
            song.Name = name;
            song.SongId = id;
            song.Path = path;
            song.ArtistName = artist;

            return song;
        }
        private Artist AddArtist(string name,int id, string picture)
        {
            Artist artist = new Artist();
            artist.ArtistId = id;
            artist.Name = name;
            artist.Picture = picture;

            return artist;
        }
        private void CounterShow(string n)
        {
            switch (n)
            {
                case "artist":
                    MessageBox.Show(ArtistStore.ListOfArtist != null ? ArtistStore.ListOfArtist.Count.ToString() : "No hay artistas aun");
                    break;
                case "songs":
                    MessageBox.Show(SongStore.ListOfSongs != null ? SongStore.ListOfSongs.Count.ToString() : "No hay artistas aun");
                    break;
            }
        }
        // DELETE
        private void Cleaner(string[] directories, string pathFinderResultArtist, string pathFinderResultSongs)
        {
            var currentArtistNames = new HashSet<string>(
                directories.Select(dir =>
                {
                    var name = new DirectoryInfo(dir.ToString()).Name;
                    return name.Contains(" - ") ? name.Split(" -")[0] : name;
                })
            );
            ArtistStore.ListOfArtist.RemoveAll(a => !currentArtistNames.Contains(a.Name));

            using var artistWriter = new StreamWriter(pathFinderResultArtist, append: false);
            foreach (var artist in ArtistStore.ListOfArtist)
            {
                artistWriter.WriteLine($"{artist.ArtistId}|{artist.Name}${artist.Picture}");
            }
            var validArtistNames = ArtistStore.ListOfArtist.Select(a => a.Name).ToHashSet();
            SongStore.ListOfSongs.RemoveAll(s => !validArtistNames.Contains(s.ArtistName));
            
            // INSERT
            using var songsWriter = new StreamWriter(pathFinderResultSongs, append: true);
            foreach (var song in SongStore.ListOfSongs)
            {
                songsWriter.WriteLine(song.SongId + "|" + song.Path + "$" + song.Name + "%" + song.ArtistName);
            }
        }
        private void SelectSongsFolder()
        {
            string folderPath = Properties.Settings.Default.SongsFolderPath;
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Selecciona la carpeta de donde cargar musica";
                dialog.SelectedPath = folderPath;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.SongsFolderPath = dialog.SelectedPath;
                    Properties.Settings.Default.Save();
                }
            }
        }
        private void Feeder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
