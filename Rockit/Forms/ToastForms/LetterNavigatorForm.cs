using NAudio.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Rockit.Form1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Rockit.Forms.ToastForms
{
    public partial class LetterNavigatorForm : Form
    {
        private static LetterNavigatorForm? _instance;
        int navigator;
        string[] letters = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I",
            "J", "K" ,"L", "M", "N", "Ñ", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X",
            "Y", "Z" };

        public static LetterNavigatorForm Instance
        => _instance ??= new LetterNavigatorForm();
        
        public LetterNavigatorForm()
        {
            InitializeComponent();
            this.FormClosed += LetterNavigatorForm_FormClosed;
            FontFamily leagueSpartan = FontLoader.LoadFont();

            this.StartPosition = FormStartPosition.Manual; // controlamos la posición manualmente
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.None; // estilo tipo herramienta (opcional)
            this.ShowInTaskbar = false;
            this.TopMost = false; // no confundir con SetWindowPos (lo controlamos manualmente)
            tableLayoutPanel1.BackColor = Color.Black;
            //this.TransparencyKey = Color.Black;
            this.Opacity = 0.92;

            lblLetter.Font = new Font(leagueSpartan, 38f);
            lblLetter.ForeColor = Color.White;
            lblLetter.TextAlign = ContentAlignment.MiddleCenter;
            ControlPosition();

        }
        protected override bool ShowWithoutActivation => true;

        private void ControlPosition()
        {
            // Obtiene el área de trabajo completa (todos los monitores)
            var pantalla = Screen.PrimaryScreen.WorkingArea;

            // Ancho del formulario
            int ancho = this.Width;
            int alto = this.Height;

            // Cálculo perfecto:
            int left = pantalla.Left + (pantalla.Width - ancho) / 2;     // ← centrado horizontal
            int top = pantalla.Top + 20;                                // ← 20 píxeles desde arriba (ajústalo si quieres más/menos)

            this.Location = new Point(left, top);
        }

        public void Showing(int navigator)
        {
            this.navigator = navigator;
            lblLetter.Text = letters[navigator];
            this.Show();
        }
        public void Hiding()
        {
            this.Hide();
        }
        private void LetterNavigatorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _instance = null;
        }
    }
}
