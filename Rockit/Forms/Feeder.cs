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
        List<Song> ListOfSongs;
        List<Artist> ListOfArtist;
        public Feeder()
        {
            InitializeComponent();
        }
        private void btnCargar_Click(object sender, EventArgs e)
        {
            String pathFiles = "D:\\Music\\Testing Music";
            try
            {
                var directories = Directory.GetDirectories(pathFiles);
                getRawSongs(directories);
            }
            catch (Exception ex) { MessageBox.Show("Ocurrió algo inesperado: " + ex); }
        }
        private void getRawSongs(String[] directories)
        {
            if (ListOfArtist == null && ListOfSongs == null)
            {
                ListOfSongs = new List<Song>();
                ListOfArtist = new List<Artist>();
            }

            String songsFolder = "D:\\Music\\Testing Music";
            string pathFinderResultSong = Path.Combine(songsFolder, "FinderResultSongs.txt");
            string pathFinderResultArtist = Path.Combine(songsFolder, "FinderResultArtist.txt");

            using FileStream fs = File.Create(pathFinderResultSong);
            using FileStream fa = File.Create(pathFinderResultArtist);
            using var sr = new StreamWriter(fs);
            using var sa = new StreamWriter(fa);
            bool artistnew, songnew;
            string dataasstring, artiststring;
            for (int i = 0; i < directories.Length; i++)
            {
                artistnew = true;
                songnew = true;

                DirectoryInfo dirfile = new DirectoryInfo(directories[i]);
                artiststring = dirfile.Name;
                if (artiststring.Contains(" - "))
                {
                    artiststring = artiststring.Split(" -")[0];
                }
                foreach (Artist querya in ListOfArtist)
                {
                    if (querya.Name == artiststring)
                    {
                        artistnew = false;
                    }
                }
                if (artistnew)
                {
                    Artist artist = AddArtist(artiststring, directories[i] + "\\portada.jpg");
                    ListOfArtist.Add(artist);

                    artiststring = artist.ToString();
                    sa.WriteLine(artist.Name);
                }
                foreach (var file in dirfile.GetFiles("*.mp3"))
                {
                    foreach (Song queryb in ListOfSongs)
                    {
                        if (queryb.Name == file.Name && queryb.ArtistName.Equals(artiststring))
                        {
                            songnew = false;
                        }
                    }
                    if (songnew)
                    {
                        Song song = AddSong(file.Name, directories[i], artiststring);

                        ListOfSongs.Add(song);
                        dataasstring = file.ToString();
                        sr.WriteLine(dataasstring);
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
                    MessageBox.Show(ListOfArtist != null ? ListOfArtist.Count.ToString() : "No hay artistas aun");
                    break;
                case "songs":
                    MessageBox.Show(ListOfSongs != null ? ListOfSongs.Count.ToString() : "No hay artistas aun");
                    break;
            }
        }
    }
}
