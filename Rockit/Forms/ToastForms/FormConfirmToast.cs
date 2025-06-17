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

namespace Rockit.Forms.ToastForms
{
    public partial class FormConfirmToast : Form
    {
        public bool Confirmed { get; private set; } = false;
        public FormConfirmToast(string message)
        {
            InitializeComponent();
            FontFamily leagueSpartan = FontLoader.LoadFont();
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.BackColor = Color.FromArgb(175, 93, 99);
            this.Opacity = 0.98;
            this.ShowInTaskbar = false;
            label1.Font = new Font(leagueSpartan, 20f);
            label2.Font = new Font(leagueSpartan, 10f);
            label3.Font = new Font(leagueSpartan, 10f);
            button1.Font = new Font(leagueSpartan, 12f);
            button2.Font = new Font(leagueSpartan, 12f);
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 0;
            button1.BackColor = Color.FromArgb(218, 247, 166);
            button2.BackColor = Color.FromArgb(108, 117, 125);
            ApplyRoundedCorners(30);
            Label label = new Label()
            {
                Text = message,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(leagueSpartan, 14f),
                ForeColor = Color.White,
            };
            this.Controls.Add(label);

            this.Width = 420;
            this.Height = 200;
            this.KeyPreview = true;
            this.KeyDown += ToastConfirmForm_KeyDown;
        }
        private void ToastConfirmForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Confirmed = true;
                this.Close();
            }
            else if (e.KeyCode == Keys.Back)
            {
                Confirmed = false;
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Confirmed = true;
            this.Close();
        }
        public static class ToastHelper
        {
            public static bool MostrarConfirmacion(string mensaje)
            {
                using (var toast = new FormConfirmToast(mensaje))
                {
                    // ShowDialog bloquea hasta que el usuario responda
                    DialogResult result = toast.ShowDialog();
                    return toast.Confirmed; // o result == DialogResult.OK
                }
            }
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
        private void ApplyRoundedButton(Button button, int radius)
        {
            Rectangle bounds = new Rectangle(0, 0, button.Width, button.Height);
            int diameter = radius * 2;

            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90); // Top-left
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90); // Top-right
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90); // Bottom-right
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90); // Bottom-left
            path.CloseFigure();

            button.Region = new Region(path);
        }
    }
}
