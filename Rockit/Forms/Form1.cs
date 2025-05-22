using Rockit.Forms;
using Rockit.Models;

namespace Rockit
{
    public partial class Form1 : Form
    {
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
                MessageBox.Show("Next");
            }
            else if (e.KeyCode == Keys.Multiply)
            {
                MessageBox.Show("Search");
                FeederMenu();
            }
        }

        private void rshButton_Click(object sender, EventArgs e)
        {
            if (ArtistStore.ListOfArtist.Count > 0)
            {
                MessageBox.Show("Hay carga " + ArtistStore.ListOfArtist.Count);
                // Obtener los primeros 8 artistas (o menos si hay menos de 8)
                var topArtists = ArtistStore.ListOfArtist
                    .Take(8)
                    .Select(a => a.Name);

                // Unirlos en un solo string con saltos de línea
                string message = string.Join(Environment.NewLine, topArtists);

                // Mostrar en un MessageBox
                MessageBox.Show(message, "Primeros 8 Artistas");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
