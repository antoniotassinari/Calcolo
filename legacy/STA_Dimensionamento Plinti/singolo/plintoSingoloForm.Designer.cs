namespace STA_Dimensionamento_Plinti
{
    partial class plintoSingoloForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(plintoSingoloForm));
            this.toolStripFile = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonApri = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSalva = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainerGenerale = new System.Windows.Forms.ToolStripContainer();
            this.elementHostGestore = new System.Windows.Forms.Integration.ElementHost();
            this.ucCalcoloPlintoSingolo1 = new STA_Dimensionamento_Plinti.ucCalcoloPlintoSingolo();
            this.toolStripFile.SuspendLayout();
            this.toolStripContainerGenerale.ContentPanel.SuspendLayout();
            this.toolStripContainerGenerale.TopToolStripPanel.SuspendLayout();
            this.toolStripContainerGenerale.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripFile
            // 
            this.toolStripFile.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripFile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonApri,
            this.toolStripButtonSalva});
            this.toolStripFile.Location = new System.Drawing.Point(3, 0);
            this.toolStripFile.Name = "toolStripFile";
            this.toolStripFile.Size = new System.Drawing.Size(188, 25);
            this.toolStripFile.TabIndex = 2;
            this.toolStripFile.Text = "toolStrip1";
            // 
            // toolStripButtonApri
            // 
            this.toolStripButtonApri.Image = global::STA_Dimensionamento_Plinti.Properties.Resources.folder;
            this.toolStripButtonApri.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonApri.Name = "toolStripButtonApri";
            this.toolStripButtonApri.Size = new System.Drawing.Size(86, 22);
            this.toolStripButtonApri.Text = "Apri da File";
            this.toolStripButtonApri.Click += new System.EventHandler(this.toolStripButtonApri_Click);
            // 
            // toolStripButtonSalva
            // 
            this.toolStripButtonSalva.Image = global::STA_Dimensionamento_Plinti.Properties.Resources.accept;
            this.toolStripButtonSalva.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSalva.Name = "toolStripButtonSalva";
            this.toolStripButtonSalva.Size = new System.Drawing.Size(90, 22);
            this.toolStripButtonSalva.Text = "Salva su File";
            this.toolStripButtonSalva.ToolTipText = "Salva su File";
            this.toolStripButtonSalva.Click += new System.EventHandler(this.toolStripButtonSalva_Click);
            // 
            // toolStripContainerGenerale
            // 
            // 
            // toolStripContainerGenerale.ContentPanel
            // 
            this.toolStripContainerGenerale.ContentPanel.Controls.Add(this.elementHostGestore);
            this.toolStripContainerGenerale.ContentPanel.Size = new System.Drawing.Size(740, 523);
            this.toolStripContainerGenerale.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainerGenerale.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainerGenerale.Name = "toolStripContainerGenerale";
            this.toolStripContainerGenerale.Size = new System.Drawing.Size(740, 548);
            this.toolStripContainerGenerale.TabIndex = 3;
            this.toolStripContainerGenerale.Text = "toolStripContainer1";
            // 
            // toolStripContainerGenerale.TopToolStripPanel
            // 
            this.toolStripContainerGenerale.TopToolStripPanel.Controls.Add(this.toolStripFile);
            // 
            // elementHostGestore
            // 
            this.elementHostGestore.AutoSize = true;
            this.elementHostGestore.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHostGestore.Location = new System.Drawing.Point(0, 0);
            this.elementHostGestore.Name = "elementHostGestore";
            this.elementHostGestore.Size = new System.Drawing.Size(740, 523);
            this.elementHostGestore.TabIndex = 0;
            this.elementHostGestore.Text = "elementHostGestore";
            this.elementHostGestore.Child = this.ucCalcoloPlintoSingolo1;
            // 
            // plintoSingoloForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(740, 548);
            this.Controls.Add(this.toolStripContainerGenerale);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "plintoSingoloForm";
            this.Text = "Calcolo di plinto singolo";
            this.toolStripFile.ResumeLayout(false);
            this.toolStripFile.PerformLayout();
            this.toolStripContainerGenerale.ContentPanel.ResumeLayout(false);
            this.toolStripContainerGenerale.ContentPanel.PerformLayout();
            this.toolStripContainerGenerale.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainerGenerale.TopToolStripPanel.PerformLayout();
            this.toolStripContainerGenerale.ResumeLayout(false);
            this.toolStripContainerGenerale.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripFile;
        private System.Windows.Forms.ToolStripButton toolStripButtonApri;
        private System.Windows.Forms.ToolStripButton toolStripButtonSalva;
        private System.Windows.Forms.ToolStripContainer toolStripContainerGenerale;
        private System.Windows.Forms.Integration.ElementHost elementHostGestore;
        private ucCalcoloPlintoSingolo ucCalcoloPlintoSingolo1;

    }
}