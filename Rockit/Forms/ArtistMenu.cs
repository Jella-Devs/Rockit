using Microsoft.VisualBasic;
using Rockit.Forms.ToastForms;
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
using Timer = System.Windows.Forms.Timer;

namespace Rockit.Forms
{
    public partial class ArtistMenu : Form
    {
        string artistkey;
        Label navlabel, creditslabel;
        //int credits;
        private Timer keyresponse, keyresponse2;
        private MusicRepository _musicRepository;
        int currentIndex = 0;
        MusicPlayerService playerService = MusicPlayerService.Instance;
        public ArtistMenu(string key, Label label,Label clabel)
        {
            InitializeComponent();
            _musicRepository = new MusicRepository();
            artistkey = key;
            navlabel = label;
            creditslabel = clabel;
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
            RoundLeftCorners(pictureBox1);
            RoundRightCorners(listView1);
            var artist = _musicRepository.GetArtistById(Int32.Parse(artistkey)); // Asumiendo que este método carga los datos del artista en la UI
            keylabel.Text = artistkey + " - " + artist.Name;
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
            ApplyResponsiveLayout();
        }
        private void loadSongs(List<Song> Songs)
        {
            var topSongs = Songs.Where(s => s.Rp > 0).OrderByDescending(s => s.Rp).Take(15).ToList();
            var allSongs = Songs.OrderBy(s => Path.GetFileNameWithoutExtension(s.Name)).ToList();

            // Separador inicial si hay canciones en Top 15
            if (topSongs.Count > 0)
            {
                var topSeparator = new ListViewItem("");
                topSeparator.SubItems.Add("――――― TOP CANCIONES ―――――");
                topSeparator.Tag = "SEPARATOR";
                topSeparator.ForeColor = Color.Gray;
                listView1.Items.Add(topSeparator);
            }

            // Agregar Top 15
            foreach (var song in topSongs)
            {
                var item = new ListViewItem("");
                item.SubItems.Add(Path.GetFileNameWithoutExtension(song.Name));
                item.Tag = $"{song.Path}\\{song.Name}";
                listView1.Items.Add(item);
            }

            // Separador antes del orden alfabético
            if (topSongs.Count > 0 && allSongs.Count > 0)
            {
                var separator = new ListViewItem("");
                separator.SubItems.Add("――――― ORDEN ALFABÉTICO ―――――");
                separator.Tag = "SEPARATOR";
                separator.ForeColor = Color.Gray;
                listView1.Items.Add(separator);
            }

            // Agregar todas alfabeticamente
            foreach (var song in allSongs)
            {
                var item = new ListViewItem("");
                item.SubItems.Add(Path.GetFileNameWithoutExtension(song.Name));
                item.Tag = $"{song.Path}\\{song.Name}";
                listView1.Items.Add(item);
            }

            // Seleccionar y enfocar el primer ítem que no sea separador
            if (listView1.Items.Count > 0)
            {
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    if (listView1.Items[i].Tag?.ToString() != "SEPARATOR")
                    {
                        currentIndex = i;
                        break;
                    }
                }
                listView1.Items[currentIndex].Selected = true;
                listView1.Select();
                listView1.Focus();
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
            else if (e.KeyCode == Keys.Divide)
            {
                this.Close();
            }
            //Monedero + 2
            else if (e.KeyCode == Keys.Z)
            {
                Properties.Settings.Default.Credits = Properties.Settings.Default.Credits + 2;
                string filePath = @"C:\Rockit\temp_credits.txt";
                try
                {
                    // Asegura que el directorio exista
                    string dir = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    // Valor numérico que quieras escribir

                        creditslabel.Text = $"{Properties.Settings.Default.Credits} creditos";

                    // Crea o sobreescribe el archivo con el nuevo valor
                    File.WriteAllText(filePath, Properties.Settings.Default.Credits.ToString());
                }
                catch (Exception err) { MessageBox.Show(err.ToString()); }
            }
            //Monedero + 3
            else if (e.KeyCode == Keys.X)
            {
                Properties.Settings.Default.Credits = Properties.Settings.Default.Credits + 3;
                string filePath = @"C:\Rockit\temp_credits.txt";
                try
                {
                    // Asegura que el directorio exista
                    string dir = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    // Valor numérico que quieras escribir

                        creditslabel.Text = $"{Properties.Settings.Default.Credits} creditos";

                    // Crea o sobreescribe el archivo con el nuevo valor
                    File.WriteAllText(filePath, Properties.Settings.Default.Credits.ToString());
                }
                catch (Exception err) { MessageBox.Show(err.ToString()); }
            }
            if (e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.Add) // Tecla "+"
            {
                if (currentIndex < listView1.Items.Count - 1)
                {
                    currentIndex++;
                    if (listView1.Items[currentIndex].Tag?.ToString() == "SEPARATOR" && currentIndex < listView1.Items.Count - 1)
                    {
                        currentIndex++;
                    }
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
                    if (listView1.Items[currentIndex].Tag?.ToString() == "SEPARATOR" && currentIndex > 0)
                    {
                        currentIndex--;
                    }
                    listView1.Items[currentIndex].Selected = true;
                    listView1.Items[currentIndex].Focused = true;
                    listView1.EnsureVisible(currentIndex);
                    e.SuppressKeyPress = true; // evita beep y propagación
                }
            }
            if (e.KeyCode == Keys.Enter && listView1.SelectedItems.Count > 0 && Properties.Settings.Default.Credits > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                string tagValue = selectedItem.Tag?.ToString();

                if (tagValue == "SEPARATOR") return;

                if (ToastHelper.MostrarConfirmacion(selectedItem.SubItems[1].Text))
                {
                    Properties.Settings.Default.Credits = Properties.Settings.Default.Credits - 1;
                    string filePath = @"C:\Rockit\temp_credits.txt";
                    try
                    {
                        // Asegura que el directorio exista
                        string dir = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }

                        // Valor numérico que quieras escribir

                            creditslabel.Text = $"{Properties.Settings.Default.Credits} creditos";

                        // Crea o sobreescribe el archivo con el nuevo valor
                        File.WriteAllText(filePath, Properties.Settings.Default.Credits.ToString());
                    }
                    catch (Exception err) { MessageBox.Show(err.ToString()); }
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

        private void ApplyResponsiveLayout()
        {
            float scaleX = (float)Screen.PrimaryScreen.Bounds.Width / 1600f;
            float scaleY = (float)Screen.PrimaryScreen.Bounds.Height / 900f;
            
            // Limitar para que no sea demasiado pequeña en resoluciones muy bajas
            scaleX = Math.Max(0.5f, scaleX);
            scaleY = Math.Max(0.5f, scaleY);

            // Escalar tamaño base de la ventana
            this.Size = new Size((int)(1149 * scaleX), (int)(562 * scaleY));
            this.CenterToScreen();

            // Escalar fuentes
            FontFamily leagueSpartan = FontLoader.LoadFont();
            keylabel.Font = new Font(leagueSpartan, Math.Max(14f, 28f * scaleX));
            listView1.Font = new Font(leagueSpartan, Math.Max(9f, 14f * scaleX));

            // Escalar márgenes dinámicos del layout original de diseñador
            pictureBox1.Margin = new Padding((int)(40 * scaleX), (int)(60 * scaleY), 0, (int)(40 * scaleY));
            listView1.Margin = new Padding(0, 0, (int)(40 * scaleX), (int)(38 * scaleY));
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
