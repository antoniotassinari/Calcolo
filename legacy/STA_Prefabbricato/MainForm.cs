using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using STA_Prefabbricato.data;
using System.IO;

namespace STA_Prefabbricato
{
    public partial class MainForm : Form
    {
        public static XmlDocument xmldoc = new XmlDocument();

        public MainForm()
        {
            InitializeComponent();
            
        }

        private void esciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void salvaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            salvaXml();
        }

        /// <summary>
        /// metodo per il salvataggio del file
        /// </summary>
        /// <returns></returns>
        private bool salvaXml()
        {
            try
            {
                xmldoc = new XmlDocument();

                SaveFileDialog of = new SaveFileDialog();
                of.Filter = "File XML Tassinari|*.staxml";
                of.AddExtension = true;
                of.DefaultExt = "staxml";
                of.FilterIndex = 0;
                of.ShowDialog();

                

                XmlComment commenti = xmldoc.CreateComment("File di salvataggio Tassinari - data: " + DateTime.Now.ToString());

                XmlNode nodoDichiarazione = xmldoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");

                xmldoc.AppendChild(nodoDichiarazione);

                xmldoc.AppendChild(commenti);

                XmlElement nodoPrincipale = xmldoc.CreateElement("", "Dati", "");

                XmlText testo = xmldoc.CreateTextNode("Questi sono i dati principali");

                nodoPrincipale.AppendChild(testo);

                nodoPrincipale.AppendChild(ucDatiImputSezione1.salvaXml());

                nodoPrincipale.AppendChild(ucDatiInputTrefoli1.salvaXml());

                

                xmldoc.AppendChild(nodoPrincipale);

                File.Delete(of.FileName);

                xmldoc.Save(of.FileName);





                return true;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                return false;
            }
        }

        private void leggiXml()
        {
            try
            {
                OpenFileDialog of = new OpenFileDialog();
                of.Filter = "File XML Tassinari|*.staxml";
                of.DefaultExt = "staxml";
                of.FilterIndex = 0;
                of.ShowDialog();

                xmldoc.Load(of.FileName);


                ucDatiImputSezione1.leggiXml(xmldoc.DocumentElement["Sezioni"]);
                ucDatiInputTrefoli1.leggiXml(xmldoc.DocumentElement["Trefoli"]);



            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);

            }
        }

        private void apriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            leggiXml();
        }

    }
}
