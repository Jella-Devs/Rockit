using Rockit.Forms;

namespace Rockit
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
    }
}
