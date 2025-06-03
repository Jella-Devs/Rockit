using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Rockit.Form1;

namespace Rockit.Forms
{
    public partial class ArtistMenu : Form
    {
        string artistkey;
        public ArtistMenu(string key)
        {
            InitializeComponent();
            artistkey = key;
            this.Focus();
            UIMenuDrawer();

        }
        private void UIMenuDrawer()
        {
            this.KeyPreview = true;
            this.DoubleBuffered = true; // Para evitar parpadeo
            FontFamily leagueSpartan = FontLoader.LoadFont();
            keylabel.Text = artistkey;
            this.BackColor = Color.FromArgb(157, 117, 119);
            this.DoubleBuffered = true; // Para reducir parpadeo
            ApplyRoundedCorners(30); // Radio del borde

            keylabel.Font = new Font(leagueSpartan, 32f);
        }

        private void ArtistMenu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Divide)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
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
    }
}
