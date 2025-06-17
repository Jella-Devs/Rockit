using Microsoft.VisualBasic;
using Rockit.Models;
using Rockit.Repositories;
using Rockit.Services;
using System;
using System.Collections;
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
using static Rockit.Form1;
using static Rockit.Forms.ToastForms.FormConfirmToast;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Rockit.Forms
{
    public partial class ArtistMenu : Form
    {
        string artistkey;
        private MusicRepository _musicRepository;
        int currentIndex = 0;
        MusicPlayerService playerService = MusicPlayerService.Instance;
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
            RoundLeftCorners(pictureBox1);
            RoundRightCorners(listView1);
            var artist = _musicRepository.GetArtistById(Int32.Parse(artistkey)); // Asumiendo que este método carga los datos del artista en la UI
            keylabel.Text = artistkey + " - " + artist.Name;
            listView1.Font = new Font(leagueSpartan, 14f);
            listView1.Dock = DockStyle.Fill;
            listView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            //listView1.Columns[0].Width = 0;
            foreach (ColumnHeader column in listView1.Columns)
            {
                column.Width = -2; // Auto-ajustar al contenido
            }

            if (!File.Exists(artist.Picture))
            {
                artist.Picture = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\Images\portadadefault.png"));
            }
            LoadPictureBox(artist.Picture);
            var SongsByArtist = _musicRepository.GetSongsByArtistName(artist.Name);
            loadSongs(SongsByArtist);// Asumiendo que este método obtiene las canciones del artista
            this.BackColor = Color.FromArgb(50, 45, 45);
            this.DoubleBuffered = true; // Para reducir parpadeo
            ApplyRoundedCorners(30); // Radio del borde

            keylabel.Font = new Font(leagueSpartan, 28f);
        }
        private void loadSongs(List<Song> Songs)
        {
            foreach (var song in Songs)
            {
                //var item = new ListViewItem(song.SongId.ToString());
                //var item = new ListViewItem((listView1.Items.Count + 1).ToString());
                var item = new ListViewItem((""));
                item.SubItems.Add(Path.GetFileNameWithoutExtension(song.Name));
                // Guardar "Path + Nombre" como valor oculto en Tag
                item.Tag = $"{song.Path}\\{song.Name}";


                listView1.Items.Add(item);
            }
            // Seleccionar y enfocar el primer ítem
            if (listView1.Items.Count > 0)
            {
                listView1.ListViewItemSorter = new ListViewItemComparer(1, true); // true = ascendente
                listView1.Sort();
                listView1.Items[0].Selected = true;
                listView1.Select();
                listView1.Focus();
            }
        }
        class ListViewItemComparer : IComparer
        {
            private int col;
            private bool ascending;

            public ListViewItemComparer(int column, bool ascending = true)
            {
                this.col = column;
                this.ascending = ascending;
            }

            public int Compare(object x, object y)
            {
                string itemX = ((ListViewItem)x).SubItems[col].Text;
                string itemY = ((ListViewItem)y).SubItems[col].Text;

                return ascending ? string.Compare(itemX, itemY) : string.Compare(itemY, itemX);
            }
        }
        private async void ArtistMenu_KeyDown(object sender, KeyEventArgs e)
        {
            if (listView1.Items.Count == 0) return;

            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Back)
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
                    e.SuppressKeyPress = true; // evita beep y propagación
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
                    e.SuppressKeyPress = true; // evita beep y propagación
                }
            }
            if (e.KeyCode == Keys.Enter && listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                string tagValue = selectedItem.Tag?.ToString();

                if (ToastHelper.MostrarConfirmacion(selectedItem.SubItems[1].Text))
                {
                    playerService.AgregarCancionAPlaylist(selectedItem.SubItems[1].Text, tagValue);
                }
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
        private void RoundLeftCorners(PictureBox pictureBox)
        {
            int radio = 20; // Radio de las esquinas redondeadas
            GraphicsPath path = new GraphicsPath();

            // Esquina superior izquierda
            path.AddArc(0, 0, radio * 2, radio * 2, 180, 90);

            // Borde superior derecho (recto)
            path.AddLine(radio, 0, pictureBox.Width, 0);

            // Borde derecho (recto)
            path.AddLine(pictureBox.Width, 0, pictureBox.Width, pictureBox.Height);

            // Borde inferior derecho (recto)
            path.AddLine(pictureBox.Width, pictureBox.Height, radio, pictureBox.Height);

            // Esquina inferior izquierda
            path.AddArc(0, pictureBox.Height - radio * 2, radio * 2, radio * 2, 90, 90);

            // Borde izquierdo (entre esquinas)
            path.CloseFigure();

            pictureBox.Region = new Region(path);
        }
        private void RoundRightCorners(ListView listView)
        {
            int radio = 20; // Radio de las esquinas redondeadas
            GraphicsPath path = new GraphicsPath();

            // Borde superior izquierdo (recto)
            path.AddLine(0, 0, listView.Width - radio, 0);

            // Esquina superior derecha
            path.AddArc(listView.Width - radio * 2, 0, radio * 2, radio * 2, 270, 90);

            // Borde derecho (entre esquinas)
            path.AddLine(listView.Width, radio, listView.Width, listView.Height - radio);

            // Esquina inferior derecha
            path.AddArc(listView.Width - radio * 2, listView.Height - radio * 2, radio * 2, radio * 2, 0, 90);

            // Borde inferior izquierdo (recto)
            path.AddLine(listView.Width - radio, listView.Height, 0, listView.Height);

            // Borde izquierdo
            path.AddLine(0, listView.Height, 0, 0);

            path.CloseFigure();
            listView.Region = new Region(path);
        }
        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            ApplyRoundedCorners(30); // Asegura que se mantenga al cambiar de tamaño
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

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Add || e.KeyCode == Keys.Subtract)
            {
                e.SuppressKeyPress = true; // Suprime el beep del sistema
            }
        }
    }
}
