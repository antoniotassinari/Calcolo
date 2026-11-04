namespace Template_01
{
    [System.ComponentModel.ToolboxItemAttribute(false)]
    partial class ActionsPaneControlPlinti
    {
        /// <summary> 
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Liberare le risorse in uso.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.ucProgettoPlintoNew1 = new STA_Dimensionamento_Plinti.progetto.ucProgettoPlintoNew();
            this.button1 = new System.Windows.Forms.Button();
            this.ofdReazioni = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // elementHost1
            // 
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(302, 314);
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.ucProgettoPlintoNew1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(224, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ofdReazioni
            // 
            this.ofdReazioni.FileName = "openFileDialog1";
            this.ofdReazioni.Filter = "File di testo (*.txt)|*.txt";
            // 
            // ActionsPaneControlPlinti
            // 
            this.Controls.Add(this.button1);
            this.Controls.Add(this.elementHost1);
            this.Name = "ActionsPaneControlPlinti";
            this.Size = new System.Drawing.Size(302, 314);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private STA_Dimensionamento_Plinti.progetto.ucProgettoPlintoNew ucProgettoPlintoNew1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog ofdReazioni;
    }
}
