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
                MessageBox.Show("Prev");
            }
            else if (e.KeyCode == Keys.Add)
            {   
                if ((int)Math.Ceiling((double)cursor / 8) != pages - 1)
                {
                    cursor = cursor != 0 ? cursor + 8 : (cursor + 1) * 8;
                    ClearMenu();
                }
                RefreshMenu();
            }
            else if (e.KeyCode == Keys.Multiply)
            {
                MessageBox.Show("Search");
                FeederMenu();
            }
        }

        private void rshButton_Click(object sender, EventArgs e)
        {
           RefreshMenu();
        }
        private void RefreshMenu()
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

                // Tomar datos de la lista dependiendo la posicion del cursor
                var topArtists = ArtistStore.ListOfArtist.Skip(cursor).Take(8).ToList();

                // Asignar cada portada a su PictureBox
                for (int i = 0; i < topArtists.Count; i++)
                {
                    string path = topArtists[i].Picture;
                    string name = topArtists[i].Name;

                    try
                    {
                        if (System.IO.File.Exists(path))
                        {
                            labels[i].Text = name;
                            pictureBoxes[i].Image = Image.FromFile(path);
                            pictureBoxes[i].SizeMode = PictureBoxSizeMode.StretchImage; // Ajuste al tamaño del PictureBox
                        }
                        else
                        {
                            labels[i].Text = null;
                            pictureBoxes[i].Image = null;
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
                label5, label6, label7, label8
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
