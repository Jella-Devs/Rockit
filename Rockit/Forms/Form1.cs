using Rockit.Forms;
using Rockit.Models;
using System.Windows.Forms;

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
            Loader();
            this.KeyPreview = true;
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
            }
        }
        public void Loader()
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

                // Parsear datos
                //if (!int.TryParse(parts[0], out string artistId)) continue;

                string name = subParts[0];
                string picture = subParts[1];

                // Crear el artista y agregarlo a la lista
                var artist = new Artist
                {
                    ArtistId = parts[0],
                    Name = name,
                    Picture = picture
                };

                ArtistStore.ListOfArtist.Add(artist);
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
                    string id = topArtists[i].ArtistId;

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
                            path = Path.Combine(projectPath,"Resources", "portadadefault.png");
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
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
