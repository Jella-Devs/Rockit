using Rockit.Models;
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
        //List<Song> ListOfSongs;

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
            string pathFinderResultSong = Path.Combine(finderFolder, "FinderResultSongs.txt");
            string pathFinderResultArtist = Path.Combine(finderFolder, "FinderResultArtist.txt");

            using FileStream fs = File.Create(pathFinderResultSong);
            using FileStream fa = File.Create(pathFinderResultArtist);
            using var songWriter = new StreamWriter(fs);
            using var artistWriter = new StreamWriter(fa);

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
                    Artist newArtist = AddArtist(artistName, picturePath);
                    ArtistStore.ListOfArtist.Add(newArtist);
                    artistWriter.WriteLine(newArtist.Name);
                    existingArtistNames.Add(artistName); // actualizar índice
                }

                // Procesar canciones dentro del directorio
                foreach (var file in dirInfo.GetFiles("*.mp3"))
                {
                    string songKey = $"{artistName}|{file.Name}";

                    if (!existingSongKeys.Contains(songKey))
                    {
                        Song newSong = AddSong(file.Name, dir, artistName);
                        SongStore.ListOfSongs.Add(newSong);

                        songWriter.WriteLine(file.Name);
                        existingSongKeys.Add(songKey); // actualizar índice
                    }
                }

            }
            CounterShow("artist");
            CounterShow("songs");
            MessageBox.Show("Carga completa");
        }
        private Song AddSong(string name, string path, string artist)
        {
            Song song = new Song();
            song.Name = name;
            song.Path = path;
            song.ArtistName = artist;

            return song;
        }
        private Artist AddArtist(string name, string picture)
        {
            Artist artist = new Artist();
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
