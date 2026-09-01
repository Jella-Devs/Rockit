namespace Rockit.Forms
{
    partial class Feeder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnCargar = new Button();
            btnCargaRapida = new Button();
            progressBar1 = new ProgressBar();
            SuspendLayout();
            // 
            // btnCargar
            // 
            btnCargar.Location = new Point(135, 489);
            btnCargar.Name = "btnCargar";
            btnCargar.Size = new Size(102, 60);
            btnCargar.TabIndex = 0;
            btnCargar.Text = "Cargar";
            btnCargar.UseVisualStyleBackColor = true;
            btnCargar.Click += btnCargar_Click;
            // 
            // btnCargaRapida
            // 
            btnCargaRapida.Location = new Point(247, 489);
            btnCargaRapida.Name = "btnCargaRapida";
            btnCargaRapida.Size = new Size(102, 60);
            btnCargaRapida.TabIndex = 2;
            btnCargaRapida.Text = "Carga rápida";
            btnCargaRapida.UseVisualStyleBackColor = true;
            btnCargaRapida.Click += btnCargaRapida_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(54, 422);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(269, 43);
            progressBar1.TabIndex = 1;
            // 
            // Feeder
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 561);
            Controls.Add(progressBar1);
            Controls.Add(btnCargaRapida);
            Controls.Add(btnCargar);
            Name = "Feeder";
            Text = "Feeder";
            KeyDown += Feeder_KeyDown;
            ResumeLayout(false);
        }

        #endregion

        private Button btnCargar;
        private Button btnCargaRapida;
        private ProgressBar progressBar1;
    }
}
