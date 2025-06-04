using Rockit.Models;
using Rockit.Repositories;
using Rockit.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;
using static Rockit.Form1;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Rockit.Forms
{
    public partial class ArtistMenu : Form
    {

        string artistkey;
        private MusicRepository _musicRepository;
        int currentIndex = 0;
        public ArtistMenu(string key)
        {
            InitializeComponent();
            _musicRepository = new MusicRepository();
            artistkey = key;
            this.Focus();
            UIMenuDrawer();

        }
        private void UIMenuDrawer()
        {
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.Columns.Clear();
            listView1.Columns.Add("ID", 50);
            listView1.Columns.Add("Nombre", 200);
            listView1.MultiSelect = false;
            listView1.Items.Clear();
            this.KeyPreview = true;
            this.DoubleBuffered = true; // Para evitar parpadeo
            FontFamily leagueSpartan = FontLoader.LoadFont();
         
            var artist = _musicRepository.GetArtistById(Int32.Parse(artistkey)); // Asumiendo que este método carga los datos del artista en la UI
            keylabel.Text = artistkey + " - " + artist.Name;
            LoadPictureBox(artist.Picture);
            var SongsByArtist = _musicRepository.GetSongsByArtistName(artist.Name);
            loadSongs(SongsByArtist);// Asumiendo que este método obtiene las canciones del artista
            this.BackColor = Color.FromArgb(157, 117, 119);
            this.DoubleBuffered = true; // Para reducir parpadeo
            ApplyRoundedCorners(30); // Radio del borde

            keylabel.Font = new Font(leagueSpartan, 32f);
        }
        private void loadSongs(List<Song> Songs)
        {
            

            foreach (var song in Songs)
            {
                var item = new ListViewItem(song.SongId.ToString());
                item.SubItems.Add(song.Name);

                // Guardar "Path + Nombre" como valor oculto en Tag
                item.Tag = $"{song.Path}\\{song.Name}";

                listView1.Items.Add(item);
            }



        }
        private void ArtistMenu_KeyDown(object sender, KeyEventArgs e)
        {
            if (listView1.Items.Count == 0) return;

           

            // Busca el elemento seleccionado actualmente
            

            if (e.KeyCode == Keys.Divide)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            
            if (e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.Add) // Tecla "+"
            {
                if (currentIndex < listView1.Items.Count - 1)
                {
                    currentIndex++;
                    listView1.Items[currentIndex].Selected = true;
                    listView1.Items[currentIndex].Focused = true;
                    listView1.EnsureVisible(currentIndex);

                }
            }
            else if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract) // Tecla "-"
            {
                if (currentIndex > 0)
                {
                    currentIndex--;
                    listView1.Items[currentIndex].Selected = true;
                    listView1.Items[currentIndex].Focused = true;
                    listView1.EnsureVisible(currentIndex);
                }
            }
            if (e.KeyCode == Keys.Enter && listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                string tagValue = selectedItem.Tag?.ToString();

                MusicPlayerService.AgregarCancionAPlaylist(selectedItem.SubItems[1].Text, tagValue);
            }

        }
        private void ApplyRoundedCorners(int radius)
        { 
            GraphicsPath path = new GraphicsPath();
            Rectangle bounds = new Rectangle(0, 0, this.Width, this.Height);

            int diameter = radius * 2;
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90); // Top-left
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90); // Top-right
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90); // Bottom-right
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90); // Bottom-left
            path.CloseAllFigures();

            this.Region = new Region(path);
        }
        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            ApplyRoundedCorners(30); // Asegura que se mantenga al cambiar de tamaño
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void LoadPictureBox(string path)
        {
            Image img = await LoadImageAsync(path);
            pictureBox1.Image = img;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }
        private async Task<Image> LoadImageAsync(string path)
        {
            return await Task.Run(() =>
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (var ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    return Image.FromStream(ms);
                }
            });
        }
    }
}
