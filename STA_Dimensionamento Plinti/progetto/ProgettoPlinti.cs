using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace STA_Dimensionamento_Plinti.progetto
{
    public class ProgettoPlinti
    {
        #region proprietà interne

        private ListaPlinti sPlinti { get; set; }
        private ListaNodi sNodi { get; set; }
        private double sPressioneLimiteTerreno { get; set; }
        private string sPercorsoFileRelazione { get; set; }

        #endregion


        #region proprietà pubbliche

        /// <summary>
        /// Plinti presenti nel progetto
        /// </summary>
        public ListaPlinti Plinti { get { return sPlinti; } }
        /// <summary>
        /// Nodi presenti nel progetto
        /// </summary>
        public ListaNodi Nodi { get { return sNodi; } }
        /// <summary>
        /// pressione limite del terreno
        /// </summary>
        public double PressioneLimiteTerreno { get { return sPressioneLimiteTerreno; } }
        /// <summary>
        /// Perscorso del file aperto per la relazione
        /// </summary>
        public string PercorsoFileRelazione { get { return sPercorsoFileRelazione; } }
        
        #endregion

        #region Costruttori

        public ProgettoPlinti()
        {
            sPlinti = new ListaPlinti();
            sNodi = new ListaNodi();
        }



        #endregion

        #region metodi

        public void aggiungiPlinto(Plinto plintoIn)
        {
            sPlinti.Add(plintoIn);
        }

        /// <summary>
        /// metodo per assegnare la lista di plinti
        /// </summary>
        /// <param name="listaIn"></param>
        public void assumiPlintiPresenti(ListaPlinti plintiIn)
        {
            sPlinti = plintiIn;
            sPlinti.Sort();
        }


        /// <summary>
        /// Metodo per la lettura delle righe
        /// </summary>
        /// <param name="filePath">string del perscorso del file</param>
        public void leggiFile(string filePath)
        {
            sNodi = new ListaNodi();
            StreamReader sr = new StreamReader(filePath);
            string line = sr.ReadLine();
            while (line != null)
            {
                try
                {
                    leggiRigaDivisa(rigaDivisa(line));
                }
                catch
                {
                }
                line = sr.ReadLine();
            }

        }

        /// <summary>
        /// controlla matrice di stringhe in entrata ed aggiunge i nodi alla lista se non esiste già un nodo con quel numero, altrimenti aggiunge le reazioni al nodo
        /// </summary>
        /// <param name="inRigaDivisa">array di stringhe in entrata</param>
        private void leggiRigaDivisa(string[] inRigaDivisa)
        {
            if (inRigaDivisa.Length == 0)
            {
                return;
            }
            if (Nodi.Count == 0)
            {
                Nodo nuovo = new Nodo(inRigaDivisa[0]);
                nuovo.aggiungiReazione(new Reazione(inRigaDivisa[1], Convert.ToDouble(inRigaDivisa[2]), Convert.ToDouble(inRigaDivisa[3]), Convert.ToDouble(inRigaDivisa[4]), Convert.ToDouble(inRigaDivisa[5]), Convert.ToDouble(inRigaDivisa[6]), Convert.ToDouble(inRigaDivisa[7])));
                sNodi.Add(nuovo);
            }
            else
            {
                int i = 0;
                foreach (Nodo nodoCheck in Nodi)
                {
                    if (nodoCheck.Nome == inRigaDivisa[0])
                    {
                        nodoCheck.aggiungiReazione(new Reazione(inRigaDivisa[1], Convert.ToDouble(inRigaDivisa[2]), Convert.ToDouble(inRigaDivisa[3]), Convert.ToDouble(inRigaDivisa[4]), Convert.ToDouble(inRigaDivisa[5]), Convert.ToDouble(inRigaDivisa[6]), Convert.ToDouble(inRigaDivisa[7])));
                        i++;
                    }

                }
                if (i == 0)
                {
                    Nodo nuovo = new Nodo(inRigaDivisa[0]);
                    nuovo.aggiungiReazione(new Reazione(inRigaDivisa[1], Convert.ToDouble(inRigaDivisa[2]), Convert.ToDouble(inRigaDivisa[3]), Convert.ToDouble(inRigaDivisa[4]), Convert.ToDouble(inRigaDivisa[5]), Convert.ToDouble(inRigaDivisa[6]), Convert.ToDouble(inRigaDivisa[7])));
                    sNodi.Add(nuovo);
                }
            }
        }

        /// <summary>
        /// restituisce l'array di stringhe con le proprietà della reazione di nodo
        /// </summary>
        /// <param name="rigaLetta">riga txt intera in ingresso</param>
        /// <returns>Restituisce l'array di stringhe</returns>
        private string[] rigaDivisa(string rigaLetta)
        {

            string[] primoPassaggio = rigaLetta.Split('|');
            List<string> listaStringhe = new List<string>();
            foreach (string riga in primoPassaggio)
            {
                string stringaPulita = riga.Trim();
                if (stringaPulita != "")
                {
                    listaStringhe.Add(stringaPulita);
                }
            }
            string[] secondoPassaggio = new string[listaStringhe.Count];
            int i = 0;
            foreach (string stringaIn in listaStringhe)
            {
                secondoPassaggio[i] = stringaIn;
                i++;
            }
            return secondoPassaggio;

        }

        /// <summary>
        /// metodo per salvare la relazion in XML
        /// </summary>
        public void salvaXML()
        {
            try
            {
                //Assegna all'istanza statica un nuovo file pulito
                generali.Statici.xmldoc = new XmlDocument();

                System.Windows.Forms.SaveFileDialog of = new System.Windows.Forms.SaveFileDialog();
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

                nodoPrincipale.AppendChild(Plinti.salvaXML());

                nodoPrincipale.AppendChild(Nodi.salvaXML());

                generali.Statici.xmldoc.AppendChild(nodoPrincipale);

                File.Delete(of.FileName);

                generali.Statici.xmldoc.Save(of.FileName);

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        /// <summary>
        /// Metodo per la lettura del file XML
        /// </summary>
        public void apriXML()
        {

                System.Windows.Forms.OpenFileDialog of = new System.Windows.Forms.OpenFileDialog();
                of.Filter = "File XML Tassinari|*.staxml";
                of.DefaultExt = "staxml";
                of.FilterIndex = 0;
                of.ShowDialog();
                try
                {
                generali.Statici.xmldoc.Load(of.FileName);
                }
            catch {}
                try
                {
                    sPlinti.apriXML(generali.Statici.xmldoc.DocumentElement["Plinti"]);
                    sPlinti.Sort();
                    sNodi.apriXML(generali.Statici.xmldoc.DocumentElement["Nodi"]);
                }
                catch
                {
                    MessageBox.Show("Formato del file non valido!");
                }


       }
        
        /// <summary>
        /// Effettua tutti i calcoli dei plinti
        /// </summary>
        public void calcola()
        {
            sNodi.calcolaNodi(Plinti);
        }

        /// <summary>
        /// Metodo per scrivere la relazione in Word
        /// </summary>
        public void scrivirelazioneWord()
        {
            

            Word.Application word_app = new Word.Application();
            word_app.Visible = true;
            object endOfDoc = "\\endofdoc";

            object missing = Type.Missing;
            Word.Document doc = word_app.Documents.Add(ref missing, ref missing, ref missing, ref missing);
            Word.Paragraph para = doc.Paragraphs.Add(ref missing);
            para.Range.Text = Plinti.Count.ToString();
            para.Range.Font.Name = "Lucida Sans Unicode";
            para.Range.InsertParagraphAfter();

            Word.Paragraph oPara2;
            object oRng = doc.Bookmarks.get_Item(ref endOfDoc).Range;
            oPara2 = doc.Content.Paragraphs.Add(ref oRng);
            oPara2.Range.Text = "Finito?";
            oPara2.Range.InsertParagraphAfter();

            Plinti.aggiungiTabellePlinti(ref doc);

            oRng = doc.Bookmarks.get_Item(ref endOfDoc).Range;
            oPara2.Range.InsertParagraphAfter();

            Nodi.aggiungiTabelleNodi(ref doc);

        }

        #endregion
    }
}
