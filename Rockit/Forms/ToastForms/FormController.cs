using Rockit.Repositories;
using Rockit.Services;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Rockit.Forms.ToastForms
{
    public partial class FormController : Form
    {
        public FormController()
        {
            InitializeComponent();
            MusicPlayerService playerService = MusicPlayerService.Instance;
            this.Focus();
            UIMenuDrawer();
        }
        private void UIMenuDrawer()
        {
            this.KeyPreview = true;
            this.DoubleBuffered = true; // Para evitar parpadeo
            FontFamily leagueSpartan = FontLoader.LoadFont();
            /*
            label1.Font = new Font(leagueSpartan, 12f);
            label2.Font = new Font(leagueSpartan, 12f);
            label3.Font = new Font(leagueSpartan, 12f);*/
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.BackColor = Color.FromArgb(175, 93, 99);
            this.Opacity = 0.88;
            this.ShowInTaskbar = false;
            this.DoubleBuffered = true; // Para reducir parpadeo
            ApplyRoundedCorners(30); // Radio del borde
        }
        private void ApplyRoundedCorners(int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();
            Rectangle bounds = new Rectangle(0, 0, this.Width, this.Height);

            // Esquinas redondeadas
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90); // Top-left
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90); // Top-right
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90); // Bottom-right
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90); // Bottom-left

            path.CloseFigure(); // Asegura que se cierre correctamente la figura

            this.Region = new Region(path);
        }

        private void FormController_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Add)
            {
                MusicPlayerService.VolumeController.IncreaseVolume();
            }
            else if (e.KeyCode == Keys.Subtract)
            {
                MusicPlayerService.VolumeController.DecreaseVolume();
            }
            else if (e.KeyCode == Keys.Divide)
            {
                MusicPlayerService.Instance.Skip();
            }
            else if (e.KeyCode == Keys.Decimal)
            {
                MusicPlayerService.Instance.ClearPlaylist();
            }
            else if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Enter)
            {
                this.Close();
            }
        }
    }
}
