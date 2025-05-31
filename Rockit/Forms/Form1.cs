using Rockit.Forms;
using Rockit.Models;
using Rockit.Repositories;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;

namespace Rockit
{
    public partial class Form1 : Form
    {
        //Index para movimiento de paginas del menu
        int pages;
        int cursor = 0;

        public Form1()
        {
            InitializeComponent();
            UIMenuDrawer();
            Loader();
        }
        private void UIMenuDrawer()
        {
            this.KeyPreview = true;
            this.DoubleBuffered = true; // Para evitar parpadeo
            this.ResizeRedraw = true;   // Redibuja al cambiar tamaño
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            // Definir colores del gradiente
            Color[] colors = new Color[]
            {
                ColorTranslator.FromHtml("#405f8f"),
                ColorTranslator.FromHtml("#69607d"),
                ColorTranslator.FromHtml("#f19e72"),
                ColorTranslator.FromHtml("#cd856e"),
                ColorTranslator.FromHtml("#9d7577")
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
            string[] lines = { "Billares", "La Quinta" };
            Font font = new Font(leagueSpartan, 56f);
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
                RefreshMenu();
            }
            else if (e.KeyCode == Keys.Add)
            {
                if ((int)Math.Ceiling((double)cursor / 8) != pages - 1)
                {
                    cursor = cursor + 8;
                    ClearMenu();
                }
                RefreshMenu();
            }
            else if (e.KeyCode == Keys.Multiply)
            {
                MessageBox.Show("Search");
                FeederMenu();
            }
            else if (e.KeyCode == Keys.F5)
            {
                RefreshMenu();
            }
            else if (e.KeyCode == Keys.F6)
            {
                Loader();
                //LoadData();
            }
        }
        public void Loader()
        {
            var repo = new MusicRepository();

            if (repo.HasArtists() && repo.HasSongs())
            {
                // Si hay datos en la base, simplemente carga a memoria
                repo.LoadDataToMemory();
                MessageBox.Show("Datos cargados desde la base de datos");
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

            MessageBox.Show(Properties.Settings.Default.ArtistCount.ToString());
            MessageBox.Show("Carga completa");
            RefreshMenu();
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
        public void RefreshMenu()
        {
            if (ArtistStore.ListOfArtist.Count > 0)
            {
                pages = (int)Math.Ceiling((double)ArtistStore.ListOfArtist.Count / 8);

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
                        idlabels[i].Text = id.ToString();
                        if (System.IO.File.Exists(path))
                        {
                            pictureBoxes[i].Image = Image.FromFile(path);
                            pictureBoxes[i].SizeMode = PictureBoxSizeMode.StretchImage; // Ajuste al tamaño del PictureBox
                        }
                        else
                        {
                            string projectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");
                            path = Path.Combine(projectPath, "Resources", "portadadefault.png");
                            string fullpath = Path.GetFullPath(path);
                            pictureBoxes[i].Image = Image.FromFile(fullpath);
                            pictureBoxes[i].SizeMode = PictureBoxSizeMode.StretchImage; // Ajuste al tamaño del PictureBox
                        }
                    }
                    catch
                    {
                        // Puedes dejarlo en blanco o poner una imagen por defecto
                        labels[i].Text = null;
                        pictureBoxes[i].Image = null;
                    }
                }
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
                lbl.Text = string.Empty;

        }
        public static class FontLoader
        {
            public static PrivateFontCollection FontCollection = new PrivateFontCollection();

            public static FontFamily LoadFont()
            {
                string leaguespartan = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Resources\Fonts\LeagueSpartan-SemiBold.ttf"));

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
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
