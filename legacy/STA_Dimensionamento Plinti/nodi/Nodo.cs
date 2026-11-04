using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Word = Microsoft.Office.Interop.Word;


namespace STA_Dimensionamento_Plinti
{
    /// <summary>
    /// Classe per la gestione dell'elenco di nodi.
    /// </summary>
    public partial class ListaNodi : List<Nodo>
    {
        /// <summary>
        /// Metodo per salvare il nodo XML
        /// </summary>
        /// <returns>nodo Xml in uscita</returns>
        public XmlNode salvaXML()
        {
            XmlNode nodoUscita = generali.Statici.xmldoc.CreateNode(XmlNodeType.Element, "Nodi", "");

            foreach (Nodo nodoProv in this)
            {
                nodoUscita.AppendChild(nodoProv.salvaXML());
            }

            return nodoUscita;
        }

        /// <summary>
        /// Metodo per leggere la lista dei nodi da XML
        /// </summary>
        /// <param name="nodoIn">Nodo XML in ingresso</param>
        public void apriXML(XmlNode nodoIn)
        {
            foreach (XmlNode nodoNodo in nodoIn)
            {
                this.Add(new Nodo(nodoNodo));
            }
        }

        /// <summary>
        /// Effettua il calcolo dei nodi
        /// </summary>
        /// <param name="plintiCheck"></param>
        public void calcolaNodi(ListaPlinti plintiCheck)
        {
            foreach (Nodo nodoCheck in this)
            {
                nodoCheck.calcolaNodo(plintiCheck);
            }
        }

        public void aggiungiTabelleNodi(ref Word.Document docWord)
        {
            object oMissing = System.Reflection.Missing.Value;
            
            object endOfDoc = "\\endofdoc";
            //Word.Paragraph paraPlinti;
            Word.Range rng01 = docWord.Bookmarks.get_Item(ref endOfDoc).Range;



            //Prima le tabelle dei plinti su pali

            Word.Table tabellaNodi;
            //Word.Table tabellaPlintiSuPali;

            if (this.Count != 0)
            {
                tabellaNodi = docWord.Tables.Add(rng01, this.Count+1, 1, ref oMissing, ref oMissing);
                int i = 1;
                foreach (Nodo nodo in this)
                {

                    tabellaNodi.Cell(i, 1).Range.Text = nodo.Nome;
                    i++;
                }
            }
        }
    }

    public class Nodo
    {


        #region proprietà private
        //dichiarazione variabili protette
        private string sNome {get; set;}
        private List<Reazione> sReazioni = new List<Reazione>();
        private ListaPlinti sPlintiVerificati { get; set; }
        private double sPesoPannelli { get; set; }
        private double sPesoTerrenoSopra { get; set; }

        #endregion
        
        /// <summary>
        /// RESTITUISCE la List delle reazioni
        /// </summary>
        public List<Reazione> Reazioni
        {
            get
            {
                return sReazioni;
            }
        }

        /// <summary>
        /// Lista dei plinti per cui effettuare la verifica associata al nodo
        /// </summary>
        public ListaPlinti PlintiDiCalcolo { get; set; }

        /// <summary>
        /// Il plinto verificato associato alla reazione
        /// </summary>
        public ListaPlinti PlintiVerificati { get { return sPlintiVerificati; } }

        /// <summary>
        /// Resituisce il nome del Nodo
        /// </summary>
        public string Nome
        {
            get
            {
                return sNome;
            }
        }
            
        /// <summary>
        /// TreeNode associato al nodo per popolare la maschera.
        /// </summary>
        public TreeNode tnNodo
        {
            get
            {
                TreeNode nodoMaster = new TreeNode(Nome);
                TreeNode[] nodiReazioni = new TreeNode[Reazioni.Count];
                int i = 0;
                foreach (Reazione inReazione in Reazioni)
                {
                    nodiReazioni[i] = inReazione.Nodo;
                    i++;
                }
                nodoMaster.Nodes.AddRange(nodiReazioni);
                return nodoMaster;
            }
        }

       

        /// <summary>
        /// Metodo per aggiungere una reazione alla lista del nodo
        /// </summary>
        /// <param name="nuovaReazione">Reazione in ingresso</param>
        public void aggiungiReazione(Reazione nuovaReazione)
        {
            sReazioni.Add(nuovaReazione);
            
        }
        #region Costruttori
        /// <summary>
        /// costruttore 
        /// </summary>
        /// <param name="inLinea"></param>
        public Nodo(string[] inLinea)
        {
            sNome = inLinea[0];

        }

        /// <summary>
        /// Costruttore da un nodo XML
        /// </summary>
        /// <param name="nodoIn">nodo XML in ingresso</param>
        public Nodo(XmlNode nodoIn)
        {
            sNome = nodoIn.Attributes["Nome"].Value;
            foreach (XmlNode nodoReazione in nodoIn.ChildNodes)
            {
                if (nodoReazione.Name == "Combinazione")
                {
                    sReazioni.Add(new Reazione(nodoReazione));
                }
            }
        }

        /// <summary>
        /// Costruttore Standard. Richiede una stringa per il nome
        /// </summary>
        /// <param name="inNome">Nome del nodo</param>
        public Nodo(string inNome)
        {
            sNome = inNome;
            sPesoTerrenoSopra = 0;
            sPesoPannelli = 0;
        }
        #endregion


        /// <summary>
        /// metodo per salvare in XML.
        /// </summary>
        /// <returns>Nodo XML in uscita</returns>
        public XmlNode salvaXML()
        {
            XmlNode xmlNodo = generali.Statici.xmldoc.CreateNode(XmlNodeType.Element, "Nodo", "");

            XmlAttribute nodoNome = generali.Statici.xmldoc.CreateAttribute("Nome");
            nodoNome.Value = this.Nome;
            xmlNodo.Attributes.Append(nodoNome);

            foreach (Reazione rea in this.Reazioni)
            {
                xmlNodo.AppendChild(rea.salvaXml());
            }

            return xmlNodo;
        }

        /// <summary>
        /// Effettua i calcoli delle pressioni mediante i Plinti
        /// </summary>
        /// <param name="plintiCheck">Lista di plinti in ingresso</param>
        public void calcolaNodo(ListaPlinti plintiCheck)
        {
            foreach (Reazione reaCheck in Reazioni)
            {
                reaCheck.verificaPlinti(plintiCheck);

            }

            
        }



        public override string ToString()
        {
            return Nome;
        }



        
    }
}
