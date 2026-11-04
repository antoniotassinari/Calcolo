using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace STA_Dimensionamento_Plinti
{
    public partial class plintoSingoloForm : Form
    {
        

        public plintoSingoloForm()
        {
            InitializeComponent();
        }

        private void toolStripButtonApri_Click(object sender, EventArgs e)
        {
            apriXml();
        }

        private void apriXml()
        {
            try
            {
                OpenFileDialog of = new OpenFileDialog();
                of.Filter = "File XML Tassinari|*.staxml";
                of.DefaultExt = "staxml";
                of.FilterIndex = 0;
                of.ShowDialog();

                generali.Statici.xmldoc.Load(of.FileName);

                ucCalcoloPlintoSingolo1.apriXml(generali.Statici.xmldoc.DocumentElement["Calcolo_Plinto"]);

            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message);
            }


        }

        private void salvaXml()
        {
            try
            {
                generali.Statici.xmldoc = new XmlDocument();

                SaveFileDialog of = new SaveFileDialog();
                of.Filter = "File XML Tassinari|*.staxml";
                of.AddExtension = true;
                of.DefaultExt = "staxml";
                of.FilterIndex = 0;
                of.ShowDialog();

                XmlComment commenti = generali.Statici.xmldoc.CreateComment("File di salvataggio Tassinari - data: " + DateTime.Now.ToString());

                XmlNode nodoDichiarazione = generali.Statici.xmldoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");

                generali.Statici.xmldoc.AppendChild(nodoDichiarazione);

                XmlNode nodoStile = generali.Statici.xmldoc.CreateProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"http://server.studio.ingtassinari.it/software/staStyle.xsl\"");

                generali.Statici.xmldoc.AppendChild(nodoStile);

                generali.Statici.xmldoc.AppendChild(commenti);

                XmlElement nodoPrincipale = generali.Statici.xmldoc.CreateElement("", "Dati", "");

                XmlText testo = generali.Statici.xmldoc.CreateTextNode("Questi sono i dati principali");

                nodoPrincipale.AppendChild(testo);

                nodoPrincipale.AppendChild(ucCalcoloPlintoSingolo1.salvaXml());



                generali.Statici.xmldoc.AppendChild(nodoPrincipale);

                File.Delete(of.FileName);

                generali.Statici.xmldoc.Save(of.FileName);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void toolStripButtonSalva_Click(object sender, EventArgs e)
        {
            salvaXml();
        }
    }
}
