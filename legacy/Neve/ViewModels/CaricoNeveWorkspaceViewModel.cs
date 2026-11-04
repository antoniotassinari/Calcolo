using System.ComponentModel;
using STA.Common;
using System.Collections.Generic;
using STA.Carichi.Neve.Commands;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;


namespace STA.Carichi.Neve.ViewModels
{
    [XmlRoot("CaricoDiNeve")]
    public class CaricoNeveWorkspaceViewModel : ViewModelBase
    {
        #region Membri

        CaricoNeve _CaricoNeve;

        #endregion

        #region Membri pubblici

        public CaricoNeve CaricoDiNeve
        {
            get { return _CaricoNeve; }
            set { _CaricoNeve = value; }
        }


        public List<ZoneCaricoNeve> ListaZone
        {
            get { return CaricoDiNeve.ListaZone; }
        }

        public List<ClasseTopografia> ListaClassiTopografia
        {
            get { return CaricoDiNeve.ListaClassiTopografia; }
        }

        public ZoneCaricoNeve ZonaScelta
        {
            get { return CaricoDiNeve.ZonaScelta; }
            set
            {
                CaricoDiNeve.ZonaScelta = value;
                RaisePropertyChangedEvent("ZonaScelta");
                AggiornaProprietà();
            }
        }

        public ClasseTopografia ClasseTopografiaScelta
        {
            get { return CaricoDiNeve.ClasseTopografiaScelta; }
            set
            {
                CaricoDiNeve.ClasseTopografiaScelta = value;
                RaisePropertyChangedEvent("ClasseTopografiaScelta");
                AggiornaProprietà();
            }
        }

        /// <summary>
        /// Altezza Topografica del sito
        /// </summary>
        public double AltezzaTopografia
        {
            get { return CaricoDiNeve.AltezzaTopografia; }
            set
            {
                CaricoDiNeve.AltezzaTopografia = value;
                AggiornaProprietà();
            }
        }

        public double Qsk
        { get { return CaricoDiNeve.Qsk; } }

        public double InclinazioneFalda
        {
            get { return CaricoDiNeve.InclinazioneFalda; }
            set
            {
                CaricoDiNeve.InclinazioneFalda = value;
                AggiornaProprietà();
            }
        }

        public double CoefficienteM1
        {
            get
            {
                return CaricoDiNeve.CoefficienteM1;
            }
        }

        #endregion

        #region Comandi
        private SaveCaricoNeveToXMLFileCommand _SaveCommand { get; set; }

        public SaveCaricoNeveToXMLFileCommand SaveCommand
        {
            get { return _SaveCommand; }
            set
            {
                _SaveCommand = value;
                RaisePropertyChangedEvent("SaveCommand");
            }
        }

        private LoadCaricoNeveFromXMLFileCommand _LoadCommand { get; set; }

        public LoadCaricoNeveFromXMLFileCommand LoadCommand
        {
            get { return _LoadCommand; }
            set
            {
                _LoadCommand = value;
                RaisePropertyChangedEvent("LoadCommand");
            }
        }

        #endregion

        #region costruttore
        /// <summary>
        /// Costruttore di una istanza default
        /// </summary>
        public CaricoNeveWorkspaceViewModel() : this(new CaricoNeve())
        {

        }

        /// <summary>
        /// Costruttore che accetta un carico neve esistente
        /// </summary>
        /// <param name="cariconeve"></param>
        public CaricoNeveWorkspaceViewModel(CaricoNeve cariconeve)
        {
            _CaricoNeve = cariconeve;
            SaveCommand = new SaveCaricoNeveToXMLFileCommand(this);
            LoadCommand = new LoadCaricoNeveFromXMLFileCommand(this);
            AggiornaProprietà();
        }

        public void AggiornaProprietà()
        {

            RaisePropertyChangedEvent("ClasseTopografiaScelta");
            RaisePropertyChangedEvent("AltezzaTopografia");
            RaisePropertyChangedEvent("Qsk");
            RaisePropertyChangedEvent("InclinazioneFalda");
            RaisePropertyChangedEvent("CoefficienteM1");
        }
        

        #endregion

        #region Metodi

        public void SerilizeToXML()
        {

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.ShowDialog();
            if (sfd.FileName != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CaricoNeve));
                TextWriter textWriter = new StreamWriter(sfd.FileName);
                serializer.Serialize(textWriter, CaricoDiNeve);
                textWriter.Close();
            }
        }

        public void DeserializeFromXML()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            switch (ofd.ShowDialog())
            {
                case DialogResult.OK:
                    FileStream fs = new FileStream(ofd.FileName, FileMode.Open);
                    XmlSerializer serializer = new XmlSerializer(typeof(CaricoNeve));
                    CaricoDiNeve = (CaricoNeve)serializer.Deserialize(fs);
                    break;
                default:
                    break;
            }
            AggiornaProprietà();
        }


        #endregion
    }
}
