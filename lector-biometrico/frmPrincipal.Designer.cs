namespace lector_biometrico
{
    partial class frmPrincipal
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPrincipal));
            this.lblEstadoHuella = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripRegistrarHuella = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripRegistrarCliente = new System.Windows.Forms.ToolStripMenuItem();
            this.lbDatosCliente = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblEstadoHuella
            // 
            this.lblEstadoHuella.AutoSize = true;
            this.lblEstadoHuella.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstadoHuella.Location = new System.Drawing.Point(324, 110);
            this.lblEstadoHuella.Name = "lblEstadoHuella";
            this.lblEstadoHuella.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblEstadoHuella.Size = new System.Drawing.Size(92, 31);
            this.lblEstadoHuella.TabIndex = 3;
            this.lblEstadoHuella.Text = "label1";
            this.lblEstadoHuella.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEstadoHuella.Click += new System.EventHandler(this.lblEstadoHuella_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripRegistrarHuella,
            this.toolStripRegistrarCliente});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Margin = new System.Windows.Forms.Padding(30);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(887, 28);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Image = global::lector_biometrico.Properties.Resources.girar;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(158, 24);
            this.toolStripMenuItem1.Text = "Refrescar clientes";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Image = global::lector_biometrico.Properties.Resources.enlace_roto;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(170, 24);
            this.toolStripMenuItem2.Text = "Cambiar conexción";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripRegistrarHuella
            // 
            this.toolStripRegistrarHuella.Image = global::lector_biometrico.Properties.Resources.escaneo_de_huellas_digitales;
            this.toolStripRegistrarHuella.Name = "toolStripRegistrarHuella";
            this.toolStripRegistrarHuella.Size = new System.Drawing.Size(146, 24);
            this.toolStripRegistrarHuella.Text = "Registrar huella";
            this.toolStripRegistrarHuella.Click += new System.EventHandler(this.toolStripRegistrarHuella_Click);
            // 
            // toolStripRegistrarCliente
            // 
            this.toolStripRegistrarCliente.Image = global::lector_biometrico.Properties.Resources.nueva_cuenta;
            this.toolStripRegistrarCliente.Name = "toolStripRegistrarCliente";
            this.toolStripRegistrarCliente.Size = new System.Drawing.Size(150, 24);
            this.toolStripRegistrarCliente.Text = "Registrar cliente";
            this.toolStripRegistrarCliente.Click += new System.EventHandler(this.toolStripRegistrarCliente_Click);
            // 
            // lbDatosCliente
            // 
            this.lbDatosCliente.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbDatosCliente.AutoSize = true;
            this.lbDatosCliente.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDatosCliente.Location = new System.Drawing.Point(340, 163);
            this.lbDatosCliente.Name = "lbDatosCliente";
            this.lbDatosCliente.Size = new System.Drawing.Size(19, 25);
            this.lbDatosCliente.TabIndex = 7;
            this.lbDatosCliente.Text = "-";
            this.lbDatosCliente.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(887, 465);
            this.Controls.Add(this.lbDatosCliente);
            this.Controls.Add(this.lblEstadoHuella);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Biplo - Registrar huella digital";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblEstadoHuella;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripRegistrarHuella;
        private System.Windows.Forms.ToolStripMenuItem toolStripRegistrarCliente;
        private System.Windows.Forms.Label lbDatosCliente;
    }
}

