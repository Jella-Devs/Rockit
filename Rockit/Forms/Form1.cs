using Rockit.Forms;
using Rockit.Forms.ToastForms;
using Rockit.Models;
using Rockit.Repositories;
using Rockit.Services;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Rockit
{
    public partial class Form1 : Form
    {
        // Index para movimiento de paginas del menu
        int pages;
        int cursor = 0, navigator = 0, doublecheck = 0;
        string[] letters = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I",
            "J", "L", "M", "N", "Ñ", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X",
            "Y", "Z" };

        // Clave para seleccionar artista
        string key = string.Empty;
        private Timer keyresponse, keyresponse2;
        MusicPlayerService playerService = MusicPlayerService.Instance;

        public Form1()
        {
            InitializeComponent();
            MusicPlayerService.Instance.SetMainForm(this);
            UIMenuDrawer();
            Loader();
        }
        private void UIMenuDrawer()
        {
            // Inicializador de Timers
            keyresponse = new Timer();
            keyresponse2 = new Timer();
            keyresponse.Interval = 2000;
            keyresponse2.Interval = 2000;
            keyresponse.Tick += Keyresponse_Tick;
            keyresponse2.Tick += Keyresponse2_Tick;

            this.KeyPreview = true;
            this.DoubleBuffered = true; // Para evitar parpadeo
            this.ResizeRedraw = true;   // Redibuja al cambiar tamaño
            FontFamily leagueSpartan = FontLoader.LoadFont();
            var pictureBoxes = new List<PictureBox>
            {
                picArtist1, picArtist2, picArtist3, picArtist4,
                picArtist5, picArtist6, picArtist7, picArtist8
            };
            var labels = new List<Label>
                {
                    label1,label2,label3,label4,label5, label6, label7, label8
                };
            var idlabels = new List<Label>
                {
                    idlabel1,idlabel2,idlabel3,idlabel4,idlabel5, idlabel6, idlabel7, idlabel8
                };
            keylabel.Font = new Font(leagueSpartan, 24f);
            pagelabel.Font = new Font(leagueSpartan, 12f);
            navlabel.Font = new Font(leagueSpartan, 38f);
            ANameLabel.Font = new Font(leagueSpartan, 12f);
            LegendLabel.Font = new Font(leagueSpartan, 16f);
            navlabel.BackColor = Color.FromArgb(45, 0, 0, 0);
            ANameLabel.BackColor = Color.FromArgb(65, 0, 0, 0);
            tableLayoutPanel3.BackColor = Color.FromArgb(20, 0, 0, 0);
            SetRoundedLabel(navlabel, 8);
            SetRoundedTableLayoutUpperCorners(tableLayoutPanel3, 32);
            foreach (var pb in pictureBoxes)
            {
                SetRoundedPictureBox(pb, 20);
            }
            SetRoundedPictureBox(playerPic1, 32);
            foreach (var lbl in labels)
            {
                lbl.Font = new Font(leagueSpartan, 12f);
                SetRoundedLabel(lbl, 20);
            }
            foreach (var lbl in idlabels)
            {
                lbl.Font = new Font(leagueSpartan, 13f);
                SetRoundedLabel(lbl, 10);
                lbl.ForeColor = Color.White;
                lbl.TextAlign = ContentAlignment.MiddleCenter;
            }
            typeof(TableLayoutPanel)
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(tableLayoutPanel1, true, null);
            typeof(TableLayoutPanel)
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(tableLayoutPanel2, true, null);
            typeof(TableLayoutPanel)
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(tableLayoutPanel3, true, null);
            typeof(TableLayoutPanel)
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(tableLayoutPanel4, true, null);
            typeof(TableLayoutPanel)
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(tableLayoutPanel5, true, null);
            typeof(FlowLayoutPanel)
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(flowLayoutPanel1, true, null);
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            // Definir colores del gradiente
            Color[] colors = new Color[]
            {
                 ColorTranslator.FromHtml("#5F4B8B"),
                 ColorTranslator.FromHtml("#826E93"),
                 ColorTranslator.FromHtml("#FFB694"),
                 ColorTranslator.FromHtml("#D77464"),
                 ColorTranslator.FromHtml("#AF5D63")
            };

            // Crear rectángulo para cubrir todo el formulario
            Rectangle rect = this.ClientRectangle;

            // Crear gradiente lineal diagonal
            using (LinearGradientBrush brush = new LinearGradientBrush(
                rect,
                colors[0],
                colors[^1],
                90f)) // 90 grados: superior derecha a inferior izquierda
            {
                // Configurar mezcla de colores personalizada
                ColorBlend blend = new ColorBlend
                {
                    Colors = colors,
                    Positions = new float[]
                    {
                    0.0f, 0.2f, 0.5f, 0.7f, 1.0f
                    }
                };

                brush.InterpolationColors = blend;
                e.Graphics.FillRectangle(brush, rect);
            }
        }

        private void titlePanel_Paint(object sender, PaintEventArgs e)
        {
            FontFamily leagueSpartan = FontLoader.LoadFont();
            string[] lines = { "Salón", "Presidencial" };
            Font font = new Font(leagueSpartan, 46f);
            float lineHeight = font.GetHeight(e.Graphics) - 2; // reduce espacio

            // Activar anti-aliasing para texto
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            float paddingX = 12f; // Padding izquierdo
            float paddingY = 26f; // Padding superior
            float y = paddingY;
            foreach (var line in lines)
            {
                e.Graphics.DrawString(line, font, Brushes.AntiqueWhite, new PointF(paddingX, y));
                y += lineHeight;
            }
        }
        private void SetRoundedPictureBox(PictureBox pb, int radius)
        {
            var rect = new Rectangle(0, 0, pb.Width, pb.Height);
            var draw = new System.Drawing.Drawing2D.GraphicsPath();

            int diameter = radius * 2;
            draw.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);                          // esquina superior izquierda
            draw.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);          // esquina superior derecha
            draw.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);  // esquina inferior derecha
            draw.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);           // esquina inferior izquierda
            draw.CloseAllFigures();

            pb.Region = new Region(draw);
        }
        private void SetRoundedLabel(Label lbl, int radius)
        {
            var rect = new Rectangle(0, 0, lbl.Width, lbl.Height);
            var draw = new System.Drawing.Drawing2D.GraphicsPath();

            int diameter = radius * 2;
            draw.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);                          // esquina superior izquierda
            draw.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);          // esquina superior derecha
            draw.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);  // esquina inferior derecha
            draw.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);           // esquina inferior izquierda
            draw.CloseAllFigures();

            lbl.Region = new Region(draw);
        }
        private void SetRoundedTableLayoutUpperCorners(TableLayoutPanel panel, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            Rectangle bounds = panel.ClientRectangle;

            // Esquinas superiores redondeadas
            path.AddArc(bounds.X, bounds.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(bounds.Right - radius * 2, bounds.Y, radius * 2, radius * 2, 270, 90);

            // Lados
            path.AddLine(bounds.Right, bounds.Y + radius, bounds.Right, bounds.Bottom);
            path.AddLine(bounds.Right, bounds.Bottom, bounds.X, bounds.Bottom);
            path.AddLine(bounds.X, bounds.Bottom, bounds.X, bounds.Y + radius);

            path.CloseFigure();

            panel.Region = new Region(path);
        }
        public void FeederMenu()
        {
            Feeder feeder = new Feeder();
            feeder.Show();
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1 && (e.Alt || e.Control || e.Shift))
            {
                FeederMenu();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Subtract)
            {
                if ((int)Math.Ceiling((double)cursor / 8) > 0)
                {
                    cursor = cursor - 8;
                    ClearMenu();
                }
                ButtonVisibility();
                RefreshMenu();
            }
            else if (e.KeyCode == Keys.Add)
            {
                if ((int)Math.Ceiling((double)cursor / 8) != pages - 1)
                {
                    cursor = cursor + 8;
                    ClearMenu();
                }
                ButtonVisibility();
                RefreshMenu();
            }
            else if (e.KeyCode == Keys.Multiply)
            {
                //MessageBox.Show("Search");
            }
            else if (e.KeyCode == Keys.K)
            {

                playerService.ClearPlaylist();


            }
            else if (e.KeyCode == Keys.Divide)
            {
                //playerService.Skip();
                FormController formController = new FormController();
                formController.ShowDialog();
                e.Handled = true;           // Marca el evento como manejado
                e.SuppressKeyPress = true;  //  Suprime la tecla para el sistema
            }
            else if (e.KeyCode == Keys.Decimal)
            {
                if (doublecheck == 0)
                {
                    navlabel.Visible = true;
                    doublecheck++;
                    startkeyresponse2();
                }
                else if ((navigator < letters.Length - 1))
                {
                    navigator++;
                    navlabel.Text = letters[navigator];
                    navlabel.Visible = true;
                    startkeyresponse2();
                }
                else
                {
                    navigator = 0;
                    navlabel.Text = letters[navigator];
                    navlabel.Visible = true;
                    startkeyresponse2();
                }
            }
            else if (e.KeyCode == Keys.F5)
            {
                RefreshMenu();
            }
            else if (e.KeyCode == Keys.F6)
            {
                Loader();
            }
            else if (e.KeyCode == Keys.F7)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < PlaylistStore.playlist.Count; i++)
                {
                    sb.AppendLine($"{i + 1}. {PlaylistStore.playlist[i].SongName}");
                }

                MessageBox.Show(sb.ToString(), "Canciones en la Playlist");
            }
            else if (e.KeyCode == Keys.Back)
            {
                if (key.Length > 0)
                {
                    key = key.Substring(0, key.Length - 1);
                    keylabel.Text = key;
                    startkeyresponse();
                }
                else
                {
                    keyresponse.Stop();
                }

            }
            else if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)
            {
                int numPressed = e.KeyCode - Keys.NumPad0;
                if (key.Length < 4) //Parametro
                {
                    key = key + (int)numPressed;
                    keylabel.Text = key;
                    startkeyresponse();
                }
            }
        }
        public void Loader()
        {
            var repo = new MusicRepository();

            if (repo.HasArtists() && repo.HasSongs())
            {
                // Si hay datos en la base, simplemente carga a memoria
                repo.LoadDataToMemory();
                //MessageBox.Show("Datos cargados desde la base de datos");
            }
            else
            {
                string finderFolder = Properties.Settings.Default.FindFolderPath;
                string pathFinderResultArtist = Path.Combine(finderFolder, "FinderResultArtist.txt");

                if (!File.Exists(pathFinderResultArtist))
                    return;

                var lines = File.ReadAllLines(pathFinderResultArtist);

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Separar ID de nombre+imagen
                    var parts = line.Split('|');
                    if (parts.Length != 2) continue;

                    // Separar nombre de imagen
                    var subParts = parts[1].Split('$');
                    if (subParts.Length != 2) continue;

                    int artistId = Int32.Parse(parts[0]);
                    string name = subParts[0];
                    string picture = subParts[1];

                    // Verifica si ya está en memoria
                    if (ArtistStore.ListOfArtist.Any(a => a.ArtistId == artistId))
                        continue;

                    // Crear el artista y agregarlo a la lista
                    var artist = new Artist
                    {
                        ArtistId = artistId,
                        Name = name,
                        Picture = picture
                    };

                    ArtistStore.ListOfArtist.Add(artist);

                    // Guardar en la BD si no existe
                    if (!repo.ExistsArtist(artistId))
                    {
                        repo.AddArtist(artist);
                    }
                }
            }
            CargarPlaylistDesdeBaseDeDatos();
            RefreshMenu();
        }
        private void CargarPlaylistDesdeBaseDeDatos()
        {
            var repo = new MusicRepository();
            var playlistDesdeBD = repo.GetPlaylist();

            if (playlistDesdeBD != null && playlistDesdeBD.Count > 0)
            {
                PlaylistStore.playlist = playlistDesdeBD;
                playerService.isPlaying = true;
                StatusPlayerinLabels();
                playerService.Play();
            }
            else
            {
                PlaylistStore.playlist = new List<PlayListItem>(); // por seguridad, aseguramos una lista vacía
            }
        }

        private void SelectArtist()
        {
            if (int.TryParse(key, out int parsedKey))
            {
                var existingArtistID = new HashSet<int>(ArtistStore.ListOfArtist.Select(artist => artist.ArtistId));
                if (existingArtistID.Contains(parsedKey))
                {
                    ArtistMenu artistMenu = new ArtistMenu(key);
                    artistMenu.Show();
                }
                key = "";
            }
        }
        private void NavigatorSelArt()
        {
            var artist = ArtistStore.ListOfArtist.FirstOrDefault(a => a.Name.StartsWith(letters[navigator], StringComparison.OrdinalIgnoreCase));
            if (artist != null)
            {
                int artistId = artist.ArtistId;
                artistId = ((int)Math.Ceiling((double)artistId / 8));
                cursor = ((artistId - 1) * 8);

                ClearMenu();
                ButtonVisibility();
                RefreshMenu();
            }
        }
        private void ButtonVisibility()
        {
            int currentPage = (int)Math.Ceiling((double)cursor / 8);
            pictureNext.Visible = currentPage < pages - 1;
            picturePrev.Visible = currentPage > 0;
        }
        public void RefreshMenu()
        {
            if (ArtistStore.ListOfArtist.Count > 0)
            {
                pages = (int)Math.Ceiling((double)ArtistStore.ListOfArtist.Count / 8);
                pagelabel.Text = "Pág: " + ((int)Math.Ceiling((double)cursor / 8) + 1) + "/" + pages;

                // Declarar elementos dinamicos
                var pictureBoxes = new List<PictureBox>
                {
                    picArtist1, picArtist2, picArtist3, picArtist4,
                    picArtist5, picArtist6, picArtist7, picArtist8
                };
                var labels = new List<Label>
                {
                    label1,label2,label3,label4,label5, label6, label7, label8
                };
                var idlabels = new List<Label>
                {
                    idlabel1,idlabel2,idlabel3,idlabel4,idlabel5, idlabel6, idlabel7, idlabel8
                };

                // Tomar datos de la lista dependiendo la posicion del cursor
                var topArtists = ArtistStore.ListOfArtist.Skip(cursor).Take(8).ToList();

                // Asignar cada portada a su PictureBox
                for (int i = 0; i < topArtists.Count; i++)
                {
                    string path = topArtists[i].Picture;
                    string name = topArtists[i].Name;
                    int id = topArtists[i].ArtistId;

                    try
                    {
                        labels[i].Text = name;
                        labels[i].BackColor = Color.FromArgb(26, 0, 0, 0);
                        idlabels[i].BackColor = Color.FromArgb(46, 0, 0, 0);
                        idlabels[i].Text = id.ToString();
                        if (System.IO.File.Exists(path))
                        {
                            LoadPictureBox(pictureBoxes[i], path);
                        }
                        else
                        {
                            string imagepath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\Images\portadadefault.png"));
                            LoadPictureBox(pictureBoxes[i], imagepath);
                        }
                    }
                    catch
                    {
                        labels[i].Text = null;
                        pictureBoxes[i].Image = null;
                    }
                }
            }
        }
        public void StatusPlayerinLabels()
        {
            if (playerService.isPlaying)
            {
                LegendLabel.Visible = true;
                string fullpath = PlaylistStore.playlist[0].SongPath;
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullpath);
                string parentFolderName = new DirectoryInfo(Path.GetDirectoryName(fullpath)).Name;
                //string result = $"{parentFolderName} - {fileNameWithoutExtension}";
                string result = $"{parentFolderName.Trim()} - {fileNameWithoutExtension.Trim()}";
                ANameLabel.Text = (result);
                tableLayoutPanel3.Visible = true;
                //string imagepath = ArtistStore.ListOfArtist[int.Parse(key)-1].Picture;
                //LoadPictureBox(playerPic1, imagepath);
            }
            else
            {
                LegendLabel.Visible = false;
                ANameLabel.Text = "";
                tableLayoutPanel3.Visible = false;
                //playerPic1.Image = null;
            }
        }
        private void ClearMenu()
        {
            var pictureBoxes = new List<PictureBox>
            {
                picArtist1, picArtist2, picArtist3, picArtist4,
                picArtist5, picArtist6, picArtist7, picArtist8
            };

            var labels = new List<Label>
            {
                label1, label2, label3, label4,
                label5, label6, label7, label8,
                idlabel1, idlabel2, idlabel3, idlabel4,
                idlabel5, idlabel6, idlabel7, idlabel8,

            };

            foreach (var pic in pictureBoxes)
                pic.Image = null;

            foreach (var lbl in labels)
            {
                lbl.Text = string.Empty;
                lbl.BackColor = Color.FromArgb(0, 0, 0, 0);
            }
        }
        private void Keyresponse_Tick(object sender, EventArgs e)
        {
            keyresponse.Stop();
            keylabel.Text = "";
            SelectArtist();
        }

        private void Keyresponse2_Tick(object sender, EventArgs e)
        {
            keyresponse2.Stop();
            navlabel.Visible = false;
            NavigatorSelArt();
            doublecheck = 0;
        }
        private void startkeyresponse()
        {
            if (keyresponse.Enabled)
            {
                keyresponse.Stop();
                keyresponse.Start();
            }
            else
            {
                keyresponse.Start();
            }
        }
        private void startkeyresponse2()
        {
            if (keyresponse2.Enabled)
            {
                keyresponse2.Stop();
                keyresponse2.Start();
            }
            else
            {
                keyresponse2.Start();
            }
        }
        public static class FontLoader
        {
            public static PrivateFontCollection FontCollection = new PrivateFontCollection();

            public static FontFamily LoadFont()
            {
                string leaguespartan = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\Fonts\LeagueSpartan-Bold.ttf"));
                string bungee = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\Fonts\Bungee-Regular.ttf"));

                FontCollection.AddFontFile(leaguespartan);

                if (FontCollection.Families.Length > 0)
                {
                    byte[] fontData = File.ReadAllBytes(leaguespartan);
                    IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
                    Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
                    FontCollection.AddMemoryFont(fontPtr, fontData.Length);
                    Marshal.FreeCoTaskMem(fontPtr);
                }
                else
                {
                    MessageBox.Show("Algo inesperado ocurrio, error en FONT");
                }

                return FontCollection.Families[0];
            }
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
        private async void LoadPictureBox(PictureBox pic, string path)
        {
            Image img = await LoadImageAsync(path);
            pic.Image = img;
            pic.SizeMode = PictureBoxSizeMode.StretchImage;
        }
    }
}
