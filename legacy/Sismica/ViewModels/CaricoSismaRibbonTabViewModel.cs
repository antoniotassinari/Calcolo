using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STA.Carichi.Sisma.ViewModels;
using System.ComponentModel;
using STA.Common;
using System.Windows.Forms;
using System.IO;

using STA.Carichi.Sisma.Commands;

namespace STA.Carichi.Sisma.ViewModels
{
    public class CaricoSismaRibbonTabViewModel : ViewModelBase
    {
        #region membri

        Sisma _sisma {get; set;}

        public Sisma sisma
        {
            get { return _sisma; }
            set
            {
                _sisma = value;
            }
        }

        #endregion

        #region costr

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="sismaIn"></param>
        public CaricoSismaRibbonTabViewModel(Sisma sismaIn)
        {
            _sisma = sismaIn;
            SalvaPerEtabsComando = new CaricoSismaSaveToSAPCommand(this);
        }

        /// <summary>
        /// Coastruttore di base
        /// </summary>
        public CaricoSismaRibbonTabViewModel() : this(new Sisma())
        {
        }

        #endregion

        #region Comandi

        CaricoSismaSaveToSAPCommand _SalvaPerEtabsComando { get; set; }

        public CaricoSismaSaveToSAPCommand SalvaPerEtabsComando
        {
            get { return _SalvaPerEtabsComando; }
            set
            {
                _SalvaPerEtabsComando = value;
                RaisePropertyChangedEvent("SalvaPerEtabsComando");
            }
        }

        #endregion

        #region Metodi

        /// <summary>
        /// Metodo per salvare per etabs
        /// </summary>
        public void SalvaPerEtabs()
        {
            //Apre un file per il salvataggio dei file in una cartella
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            ///Vaglia i casi di selezione
            switch (fbd.ShowDialog())
            {
                    ///nel caso di una corretta selezione
                case DialogResult.OK:
                    
                    ///Richiama il metodo proprio della classe Sisma
                    _sisma.salvaFilesEtabs(fbd.SelectedPath);
                    break;


                default:
                    break;
            }
            
        }

        #endregion

    }
}
