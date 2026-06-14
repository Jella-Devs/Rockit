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
        #region Variables y Campos de Estado

        // Index para movimiento de paginas del menu
        //public int credits = 0;
        int pages;
        int cursor = 0, navigator = 0, doublecheck = 0;
        private bool creditLock = false;
        private bool creditPressed = false;
        bool isTop8Page = false;
        private bool HasEnoughTopArtists()
        {
            return ArtistStore.ListOfArtist != null && ArtistStore.ListOfArtist.Count(a => a.Rp > 0) >= 8;
        }
        string[] letters = new string[] { "★", "A", "B", "C", "D", "E", "F", "G", "H", "I",
            "J", "K" ,"L", "M", "N", "Ñ", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X",
            "Y", "Z" };

        //Modo buscador alfabetico
        bool lettersearchmode = false;

        // Clave para seleccionar artista
        string key = string.Empty;
        private Timer keyresponse, keyresponse2;
        MusicPlayerService playerService = MusicPlayerService.Instance;

        #endregion

        #region Constructor e Inicialización

        public Form1()
        {
            InitializeComponent();
            MusicPlayerService.Instance.SetMainForm(this);
            UIMenuDrawer();
            Loader();
        }
        /// <summary>
        /// Prepara y dibuja los controles UI iniciales en el formulario, 
        /// estableciendo fuentes, fondos y activando el DoubleBuffering.
        /// </summary>
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
            this.ResizeRedraw = true;   // Redibuja al cambiar tama�o
            var idlabels = new List<Label>
            {
                idlabel1, idlabel2, idlabel3, idlabel4,
                idlabel5, idlabel6, idlabel7, idlabel8
            };
            // Fuentes y regiones escaladas dinamicamente por ApplyResponsiveLayout()
            navlabel.BackColor = Color.FromArgb(45, 0, 0, 0);
            ANameLabel.BackColor = Color.FromArgb(65, 0, 0, 0);
            tableLayoutPanel3.BackColor = Color.Transparent;
            tableLayoutPanel3.Paint += TableLayoutPanel3_Paint;
            foreach (var lbl in idlabels)
            {
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
            InitializeCredits();
        }
        /// <summary>
        /// Carga los créditos actuales desde el sistema de archivos o los inicializa en 0.
        /// </summary>
        private void InitializeCredits()
        {
            string filePath = @"C:\Rockit\temp_credits.txt";
            try
            {
                // Asegura que la carpeta exista
                string dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                // Si existe archivo, lo lee
                if (File.Exists(filePath))
                {
                    string contenido = File.ReadAllText(filePath);
                    if (int.TryParse(contenido, out int valor))
                    {
                        Properties.Settings.Default.Credits = valor;
                    }
                }
                else
                {
                    // Si no existe, lo crea con valor 0
                    File.WriteAllText(filePath, Properties.Settings.Default.Credits.ToString());
                }
                creditslabel.Text = $"{Properties.Settings.Default.Credits} creditos";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando creditos: " + ex.Message);
            }
        }

        #endregion

        #region Métodos de Dibujado y UI

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (isTop8Page)
            {
                float scale = (float)this.ClientSize.Width / 1600f;
                float scaleY = (float)this.ClientSize.Height / 900f;
                float fontSize = Math.Max(12f, 32f * scale);

                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                using (Font font = new Font(FontLoader.LoadFont(), fontSize, FontStyle.Bold))
                using (SolidBrush brush = new SolidBrush(Color.AntiqueWhite))
                {
                    // Para orientacion vertical: leemos de abajo hacia arriba
                    // Trasladamos al punto inferior izquierdo donde empezara el texto
                    e.Graphics.TranslateTransform((int)(12 * scale), (int)(800 * scaleY));
                    e.Graphics.RotateTransform(-90);
                    e.Graphics.DrawString("LOS MÁS ESCUCHADOS", font, brush, 0, 0);
                    e.Graphics.ResetTransform();
                }
            }
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            // Definir colores del gradiente (naranja vibrante a morado oscuro profundo)
            Color[] colors = new Color[]
            {
                 ColorTranslator.FromHtml("#14081C"), // Naranja vibrante
                 ColorTranslator.FromHtml("#300F3C"), // Carmesí
                 ColorTranslator.FromHtml("#6D174A"), // Magenta oscuro
                 ColorTranslator.FromHtml("#300F3C"), // Morado profundo
                 ColorTranslator.FromHtml("#14081C")  // Casi negro/morado
            };

            // Crear rectángulo para cubrir todo el formulario
            Rectangle rect = this.ClientRectangle;

            // Crear gradiente lineal diagonal (45 grados)
            using (LinearGradientBrush brush = new LinearGradientBrush(
                rect,
                colors[0],
                colors[^1],
                45f)) // 45 grados: superior izquierda a inferior derecha
            {
                // Configurar mezcla de colores personalizada
                ColorBlend blend = new ColorBlend
                {
                    Colors = colors,
                    Positions = new float[]
                    {
                    0.0f, 0.25f, 0.5f, 0.75f, 1.0f
                    }
                };

                brush.InterpolationColors = blend;
                e.Graphics.FillRectangle(brush, rect);
            }
        }

        private void titlePanel_Paint(object sender, PaintEventArgs e)
        {
            FontFamily leagueSpartan = FontLoader.LoadFont();
            // Escalar fuente proporcional a la resolucion de diseño (1600x900 base)
            float scale = Math.Min(
                (float)this.ClientSize.Width / 1600f,
                (float)this.ClientSize.Height / 900f);
            float fontSize = Math.Max(16f, 58f * scale);
            string[] lines = { "Billares", "La Quinta" };
            Font font = new Font(leagueSpartan, fontSize, FontStyle.Bold);
            float lineHeight = font.GetHeight(e.Graphics) - 4;

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            float paddingX = Math.Max(10f, 24f * scale);
            float paddingY = Math.Max(12f, 36f * scale);
            float y = paddingY;
            foreach (var line in lines)
            {
                e.Graphics.DrawString(line, font, Brushes.AntiqueWhite, new PointF(paddingX, y));
                y += lineHeight;
            }
        }
        private void ApplyRoundedRegion(Control ctrl, int radius)
        {
            if (ctrl.Width <= 0 || ctrl.Height <= 0) return;
            int r = Math.Max(1, radius);
            int diameter = r * 2;
            var rect = new Rectangle(0, 0, ctrl.Width, ctrl.Height);
            var draw = new System.Drawing.Drawing2D.GraphicsPath();

            draw.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            draw.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            draw.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            draw.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            draw.CloseAllFigures();

            ctrl.Region = new Region(draw);
        }
        private void SetRoundedPictureBox(PictureBox pb, int radius)
        {
            ApplyRoundedRegion(pb, radius);
        }
        private void SetRoundedLabel(Label lbl, int radius)
        {
            ApplyRoundedRegion(lbl, radius);
        }
        private void SetRoundedTableLayoutUpperCorners(TableLayoutPanel panel, int radius)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            Rectangle bounds = panel.ClientRectangle;
            if (bounds.Width <= 0 || bounds.Height <= 0) return;
            int diameter = radius * 2;
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddLine(bounds.Right, bounds.Y + radius, bounds.Right, bounds.Bottom);
            path.AddLine(bounds.Right, bounds.Bottom, bounds.X, bounds.Bottom);
            path.AddLine(bounds.X, bounds.Bottom, bounds.X, bounds.Y + radius);
            path.CloseFigure();
            panel.Region = new Region(path);
        }
        private void AdjustLabelToText(Label lbl)
        {
            using (Graphics g = lbl.CreateGraphics())
            {
                SizeF textSize = g.MeasureString(lbl.Text, lbl.Font);
                lbl.Width = (int)textSize.Width + 10;  // margen adicional
                lbl.Height = (int)textSize.Height + 4;
                //MessageBox.Show(textSize.ToString() + "," + lbl.Width + "," + lbl.Height);

                lbl.Margin = new Padding(lbl.Width + 100, 0, lbl.Width + 100, 0);
            }
        }
        private void AdjustLabelNameToText(Label lbl)
        {
            using (Graphics g = lbl.CreateGraphics())
            {
                SizeF textSize = g.MeasureString(lbl.Text, lbl.Font);
                lbl.Width = (int)textSize.Width + 10;  // margen adicional
                lbl.Height = (int)textSize.Height + 4;
                //MessageBox.Show(textSize.ToString() + "," + lbl.Width + "," + lbl.Height);

                lbl.Margin = new Padding(lbl.Width - 100, 0, lbl.Width - 100, 0);
            }
        }

        #endregion

        #region Eventos de Teclado

        public void FeederMenu()
        {
            Feeder feeder = new Feeder();
            feeder.Show();
        }
        private async void Form1_KeyDown(object sender, KeyEventArgs e)
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
                if (lettersearchmode)
                {
                    lettersearch("back");
                }
                else
                {
                    if (!isTop8Page && cursor == 0 && HasEnoughTopArtists())
                    {
                        isTop8Page = true;
                        ClearMenu();
                    }
                    else if ((int)Math.Ceiling((double)cursor / 8) > 0)
                        if ((int)Math.Ceiling((double)cursor / 8) > 0)
                        {
                            cursor = cursor - 8;
                            ClearMenu();
                        }
                    ButtonVisibility();
                    RefreshMenu();
                }
            }
            else if (e.KeyCode == Keys.Add)
            {
                if (lettersearchmode)
                {
                    lettersearch("forward");
                }
                else
                {
                    if (isTop8Page)
                    {
                        isTop8Page = false;
                        cursor = 0;
                        ClearMenu();
                    }
                    else if ((int)Math.Ceiling((double)cursor / 8) != pages - 1)
                        if ((int)Math.Ceiling((double)cursor / 8) != pages - 1)
                        {
                            cursor = cursor + 8;
                            ClearMenu();
                        }
                    ButtonVisibility();
                    RefreshMenu();
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (lettersearchmode)
                {
                    lettersearch("select");
                }
            }
            else if (e.KeyCode == Keys.Multiply)
            {
                //MessageBox.Show("Search");
            }
            else if (e.KeyCode == Keys.K)
            {
                playerService.ClearPlaylist();
            }
            /*
            else if (e.KeyCode == Keys.Divide)
            {
                //playerService.Skip();
                FormController formController = new FormController();
                formController.ShowDialog();
                e.Handled = true;           // Marca el evento como manejado
                e.SuppressKeyPress = true;  //  Suprime la tecla para el sistema
            }*/
            else if (e.KeyCode == Keys.Decimal)
            {
                lettersearchmode = true;
                lettersearch("forward");
            }
            else if (e.KeyCode == Keys.F5)
            {
                Loader();
                RefreshMenu();
                MessageBox.Show("El menu se ha actualizado");
            }
            else if (e.KeyCode == Keys.F6)
            {
                Loader();
            }
            //Monedero + 2
            else if (e.KeyCode == Keys.Z)
            {
                if (creditLock) return; // bloquea si esta en delay
                if (creditPressed) return;

                creditPressed = true;
                creditLock = true;

                Properties.Settings.Default.Credits += 2;
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

                await Task.Delay(1200);
                creditLock = false;
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

                    // Valor numrico que quieras escribir

                    creditslabel.Text = $"{Properties.Settings.Default.Credits} creditos";

                    // Crea o sobreescribe el archivo con el nuevo valor
                    File.WriteAllText(filePath, Properties.Settings.Default.Credits.ToString());
                }
                catch (Exception err) { MessageBox.Show(err.ToString()); }
            }
            else if (e.KeyCode == Keys.C)
            {
                Properties.Settings.Default.Credits = 0;
                string filePath = @"C:\Rockit\temp_credits.txt";
                try
                {
                    // Asegura que el directorio exista
                    string dir = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    // Valor numrico que quieras escribir

                    creditslabel.Text = $"{Properties.Settings.Default.Credits} creditos";

                    // Crea o sobreescribe el archivo con el nuevo valor
                    File.WriteAllText(filePath, Properties.Settings.Default.Credits.ToString());
                }
                catch (Exception err) { MessageBox.Show(err.ToString()); }
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
            else if (e.KeyCode == Keys.S)
            {
                playerService.Skip();

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

        #endregion

        #region Lógica de Negocio y Navegación

        /// <summary>
        /// Inicializa el estado de la aplicación. 
        /// Verifica si existen artistas en BD, los carga en memoria o importa desde archivos de texto local.
        /// </summary>
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

                    // Verifica si ya est� en memoria
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
            isTop8Page = HasEnoughTopArtists();
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
                PlaylistStore.playlist = new List<PlayListItem>(); // por seguridad, aseguramos una lista vac�a
            }
        }

        private void SelectArtist()
        {
            if (int.TryParse(key, out int parsedKey))
            {
                var existingArtistID = new HashSet<int>(ArtistStore.ListOfArtist.Select(artist => artist.ArtistId));
                if (existingArtistID.Contains(parsedKey))
                {
                    ArtistMenu artistMenu = new ArtistMenu(key, navlabel, creditslabel);
                    artistMenu.Show();
                }
                key = "";
            }
        }
        private void NavigatorSelArt()
        {
            if (letters[navigator] == "★")
            {
                if (!HasEnoughTopArtists()) return;
                isTop8Page = true;
                cursor = 0;
                ClearMenu();
                ButtonVisibility();
                RefreshMenu();
                return;
            }
            isTop8Page = false;
            var artist = ArtistStore.ListOfArtist.FirstOrDefault(a => a.Name.StartsWith(letters[navigator], StringComparison.OrdinalIgnoreCase));
            if (artist != null)
            {
                int artistPos = ArtistStore.ListOfArtist.IndexOf(artist);
                artistPos = ((int)Math.Ceiling((double)artistPos / 8));
                cursor = ((artistPos - 1) * 8);

                ClearMenu();
                ButtonVisibility();
                RefreshMenu();
            }
        }
        private void ButtonVisibility()
        {
            int currentPage = (int)Math.Ceiling((double)cursor / 8);
            pictureNext.Visible = isTop8Page ? (pages > 0) : (currentPage < pages - 1);
            picturePrev.Visible = !isTop8Page;
        }

        /// <summary>
        /// Actualiza la vista de cuadrícula de portadas de artistas basándose en la posición del cursor.
        /// </summary>
        public void RefreshMenu()
        {
            if (isTop8Page && !HasEnoughTopArtists()) { isTop8Page = false; cursor = 0; }
            if (ArtistStore.ListOfArtist.Count > 0)
            {
                pages = (int)Math.Ceiling((double)ArtistStore.ListOfArtist.Count / 8);
                if (isTop8Page) pagelabel.Text = "Pág: ★ Top 8 ★";
                else pagelabel.Text = "Pág: " + ((int)Math.Ceiling((double)cursor / 8) + 1) + "/" + pages;
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
                var topArtists = isTop8Page ?
                    ArtistStore.ListOfArtist.OrderByDescending(a => a.Rp).Take(8).ToList() :
                    ArtistStore.ListOfArtist.Skip(cursor).Take(8).ToList();

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
            if (playerService.isPlaying && PlaylistStore.playlist.Count > playerService.currentIndex)
            {
                LegendLabel.Visible = true;
                
                var currentSong = PlaylistStore.playlist[playerService.currentIndex];
                string fullpath = currentSong.SongPath;
                
                // Extraer el nombre de la canción estrictamente del archivo
                string songName = Path.GetFileNameWithoutExtension(fullpath);

                // Obtener el artista exacto de la base de datos
                string artistName = "Desconocido";
                var songFromDb = SongStore.ListOfSongs.FirstOrDefault(s => string.Equals($"{s.Path}\\{s.Name}", fullpath, StringComparison.OrdinalIgnoreCase) || string.Equals(s.Path + "/" + s.Name, fullpath, StringComparison.OrdinalIgnoreCase));
                
                if (songFromDb != null && !string.IsNullOrWhiteSpace(songFromDb.ArtistName))
                {
                    artistName = songFromDb.ArtistName;
                }
                else
                {
                    // Fallback si no se encuentra en memoria por alguna razón
                    string parentFolder = new DirectoryInfo(Path.GetDirectoryName(fullpath)).Name;
                    var matchingArtist = ArtistStore.ListOfArtist.FirstOrDefault(a => fullpath.IndexOf($"\\{a.Name}\\", StringComparison.OrdinalIgnoreCase) >= 0 || fullpath.IndexOf($"/{a.Name}/", StringComparison.OrdinalIgnoreCase) >= 0);
                    if (matchingArtist != null)
                    {
                        artistName = matchingArtist.Name;
                    }
                    else
                    {
                        artistName = parentFolder.Split('-')[0].Trim(); // Ej. "Grupo Frontera - Agosto 2025" -> "Grupo Frontera"
                    }
                }

                // Evitar duplicar el nombre del artista si la canción ya lo incluye
                string result;
                if (songName.StartsWith(artistName, StringComparison.OrdinalIgnoreCase))
                {
                    result = songName.Trim();
                }
                else
                {
                    result = $"{artistName.Trim()} - {songName.Trim()}";
                }

                ANameLabel.Text = result;
                tableLayoutPanel3.Visible = true;
            }
            else
            {
                LegendLabel.Visible = false;
                ANameLabel.Text = "";
                tableLayoutPanel3.Visible = false;
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
        private void lettersearch(string direction)
        {
            if (direction == "back")
            {
                if (doublecheck == 0)
                {
                    navlabel.Visible = true;
                    doublecheck++;
                    startkeyresponse2();
                }
                else if ((navigator > 0))
                {
                    navigator--;
                    navlabel.Text = letters[navigator];
                    navlabel.Visible = true;
                    startkeyresponse2();
                }
                else
                {
                    navigator = letters.Length - 1;
                    navlabel.Text = letters[navigator];
                    navlabel.Visible = true;
                    startkeyresponse2();
                }
                //LetterNavigatorForm.Instance.Showing(navigator);
            }
            else if (direction == "forward")
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
                //LetterNavigatorForm.Instance.Showing(navigator);
            }
            else if (direction == "select")
            {
                navlabel.Visible = false;
                NavigatorSelArt();
                doublecheck = 0;
                lettersearchmode = false;
                //LetterNavigatorForm.Instance.Hiding();
            }
        }

        #endregion

        #region Manejo de Timers

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
            lettersearchmode = false;
            //LetterNavigatorForm.Instance.Hiding();
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

        #endregion

        #region Clases Anidadas y Utilidades

        /// <summary>
        /// Carga y expone las fuentes personalizadas del proyecto (LeagueSpartan y DSEG7).
        /// </summary>
        public static class FontLoader
        {
            private static PrivateFontCollection _fontCollection = new PrivateFontCollection();
            private static PrivateFontCollection _dsegCollection = new PrivateFontCollection();
            private static FontFamily? _leagueSpartan = null;
            private static FontFamily? _dseg7 = null;

            /// <summary>Retorna la familia LeagueSpartan Bold.</summary>
            public static FontFamily LoadFont()
            {
                if (_leagueSpartan != null) return _leagueSpartan;

                string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\Fonts\LeagueSpartan-Bold.ttf"));
                byte[] data = File.ReadAllBytes(path);
                IntPtr ptr = Marshal.AllocCoTaskMem(data.Length);
                Marshal.Copy(data, 0, ptr, data.Length);
                _fontCollection.AddMemoryFont(ptr, data.Length);
                Marshal.FreeCoTaskMem(ptr);
                _leagueSpartan = _fontCollection.Families[0];
                return _leagueSpartan;
            }

            /// <summary>Retorna la familia DSEG7Classic (estilo 7 segmentos).</summary>
            public static FontFamily LoadDSEG7()
            {
                if (_dseg7 != null) return _dseg7;

                string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\Fonts\DSEG7Classic-Regular.ttf"));
                byte[] data = File.ReadAllBytes(path);
                IntPtr ptr = Marshal.AllocCoTaskMem(data.Length);
                Marshal.Copy(data, 0, ptr, data.Length);
                _dsegCollection.AddMemoryFont(ptr, data.Length);
                Marshal.FreeCoTaskMem(ptr);
                _dseg7 = _dsegCollection.Families[0];
                return _dseg7;
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

        #endregion

        #region Diseño Responsivo y Ciclo de Vida Adicional

        private void Form1_Load(object sender, EventArgs e)
        {
            ApplyResponsiveLayout();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.ClientSize.Width > 0 && this.ClientSize.Height > 0)
            {
                ApplyResponsiveLayout();
                this.Invalidate();
                titlePanel?.Invalidate();
            }
        }

        /// <summary>
        /// Aplica cálculos matemáticos basados en la resolución nativa de pantalla 
        /// para escalar dimensiones, fuentes y márgenes dinámicamente.
        /// </summary>
        private void ApplyResponsiveLayout()
        {
            float scaleX = (float)this.ClientSize.Width / 1600f;
            float scaleY = (float)this.ClientSize.Height / 900f;
            float scale = Math.Min(scaleX, scaleY);

            FontFamily lf = FontLoader.LoadFont();
            FontFamily dseg = FontLoader.LoadDSEG7();

            // --- HEADER ---
            // keylabel con fuente DSEG7 (estilo display 7-segmentos)
            keylabel.Font = new Font(dseg, Math.Max(24f, 64f * scale), FontStyle.Regular);
            keylabel.ForeColor = Color.White;
            keylabel.TextAlign = ContentAlignment.MiddleCenter;

            // pagelabel, navlabel, player
            pagelabel.Font = new Font(lf, Math.Max(6f, 12f * scale));
            navlabel.Font = new Font(lf, Math.Max(24f, 38f * scale));
            ANameLabel.Font = new Font(lf, Math.Max(6f, 12f * scale));
            LegendLabel.Font = new Font(lf, Math.Max(6f, 16f * scale));

            int navSize = Math.Max(40, (int)(80 * scale));
            navlabel.Size = new Size(navSize, navSize);

            // creditslabel: compacto estilo "17 creditos" alineado top-right
            creditslabel.Font = new Font(lf, Math.Max(12f, 22f * scale), FontStyle.Bold);
            creditslabel.TextAlign = ContentAlignment.MiddleRight;
            creditslabel.Text = $"{Properties.Settings.Default.Credits} creditos";
            creditslabel.Padding = new Padding(0, 0, (int)(40 * scaleX), 0);

            // Padding del keylabel (centrado verticalmente en el panel)
            int hPad = Math.Max(10, (int)(240 * scaleX));
            int vPad = Math.Max(5, (int)(50 * scaleY));
            flowLayoutPanel1.Padding = new Padding(hPad, vPad, hPad, (int)(40 * scaleY));

            // --- BOTONES NEXT/PREV ---
            int btnSize = Math.Max(32, (int)(64 * scale));
            int midY = (int)(this.ClientSize.Height * 0.55f);
            pictureNext.Size = new Size(btnSize, btnSize);
            picturePrev.Size = new Size(btnSize, btnSize);
            pictureNext.Location = new Point(this.ClientSize.Width - btnSize - 4, midY - btnSize / 2);
            picturePrev.Location = new Point(4, midY - btnSize / 2);
            pictureNext.BringToFront();
            picturePrev.BringToFront();

            // --- TARJETAS DE ARTISTAS ---
            var pictureBoxes = new List<PictureBox> { picArtist1, picArtist2, picArtist3, picArtist4, picArtist5, picArtist6, picArtist7, picArtist8 };
            var labels = new List<Label> { label1, label2, label3, label4, label5, label6, label7, label8 };
            var idlabels = new List<Label> { idlabel1, idlabel2, idlabel3, idlabel4, idlabel5, idlabel6, idlabel7, idlabel8 };

            int artistMargin = (int)(85 * scaleX);
            int picRadius = Math.Max(8, (int)(20 * scale));
            int badgeSize = Math.Max(32, (int)(48 * scale));
            float badgeFontSize = Math.Max(8f, 13f * scale);

            for (int i = 0; i < pictureBoxes.Count; i++)
            {
                var pb = pictureBoxes[i];
                pb.Margin = new Padding(artistMargin, 0, artistMargin, 0);
                SetRoundedPictureBox(pb, picRadius);

                // Nombre de artista: LeagueSpartan Bold centrado
                labels[i].Font = new Font(lf, Math.Max(6f, 12f * scale), FontStyle.Bold);
                labels[i].Margin = new Padding(artistMargin + 5, 0, artistMargin + 5, 0);
                SetRoundedLabel(labels[i], Math.Max(8, (int)(20 * scale)));

                // --- BADGE ID FLOTANTE sobre la imagen (Custom Paint) ---
                var badge = idlabels[i];
                badge.Visible = false; // Ocultamos el label real porque lo dibujaremos manualmente

                pb.Tag = badge; // Guardamos la referencia para el evento Paint

                // Asegurar un solo handler
                pb.Paint -= Pb_Paint_Badge;
                pb.Paint += Pb_Paint_Badge;
            }

            SetRoundedPictureBox(playerPic1, Math.Max(8, (int)(32 * scale)));
            SetRoundedLabel(navlabel, Math.Max(4, (int)(8 * scale)));
            // El redondeo de tableLayoutPanel3 ahora se hace nativamente en su evento Paint
        }

        private void TableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {
            float scaleX = (float)this.ClientSize.Width / 1600f;
            float scaleY = (float)this.ClientSize.Height / 900f;
            float scale = Math.Min(scaleX, scaleY);
            int radius = Math.Max(8, (int)(32 * scale));

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = new GraphicsPath())
            {
                int d = radius * 2;
                Rectangle rect = tableLayoutPanel3.ClientRectangle;
                // Arriba izquierda
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                // Arriba derecha
                path.AddArc(rect.Right - d - 1, rect.Y, d, d, 270, 90);
                // Abajo derecha
                path.AddLine(rect.Right - 1, rect.Y + radius, rect.Right - 1, rect.Bottom);
                // Abajo izquierda
                path.AddLine(rect.Right - 1, rect.Bottom, rect.X, rect.Bottom);
                path.AddLine(rect.X, rect.Bottom, rect.X, rect.Y + radius);
                path.CloseFigure();

                using (SolidBrush brush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                {
                    e.Graphics.FillPath(brush, path);
                }
            }
        }

        private void Pb_Paint_Badge(object sender, PaintEventArgs e)
        {
            if (sender is PictureBox pb && pb.Tag is Label badgeLbl && !string.IsNullOrEmpty(badgeLbl.Text))
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                float scaleX = (float)this.ClientSize.Width / 1600f;
                float scale = Math.Min(scaleX, (float)this.ClientSize.Height / 900f);

                int badgeSize = Math.Max(32, (int)(48 * scale));
                float badgeFontSize = Math.Max(8f, 13f * scale);
                int offset = (int)(8 * scaleX);

                Rectangle rect = new Rectangle(offset, offset, badgeSize, badgeSize);

                // Fondo circular dorado semitransparente
                using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(200, 180, 145, 40)))
                {
                    e.Graphics.FillEllipse(bgBrush, rect);
                }

                // Texto centrado
                using (Font font = new Font(FontLoader.LoadFont(), badgeFontSize, FontStyle.Bold))
                using (SolidBrush textBrush = new SolidBrush(Color.White))
                using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    e.Graphics.DrawString(badgeLbl.Text, font, textBrush, rect, sf);
                }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z)
            {
                creditPressed = false;
            }
        }

        #endregion
    }
}
