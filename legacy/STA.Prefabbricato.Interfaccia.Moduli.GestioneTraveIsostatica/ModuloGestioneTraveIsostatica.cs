using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using STA.Prefabbricato.TraveIsostatica;
using STA.Prefabbricato.TraveIsostatica.ViewModels;

namespace STA.Prefabbricato.Interfaccia.Moduli.GestioneTraveIsostatica
{
    /// <summary>
    /// Module initialization for Trave Isostatica management
    /// </summary>
    public class ModuloGestioneTraveIsostatica
    {
        private static TraveIsostatica.TraveIsostatica Trave = new TraveIsostatica.TraveIsostatica();
        public static TraveIsostaticaViewModel traveIsostaticaViewModel = new TraveIsostaticaViewModel(Trave);

        /// <summary>
        /// Initializes the module
        /// </summary>
        public void Initialize()
        {
            // Module initialization logic - Prism dependency removed
            // Register views and viewmodels as needed for your application
        }
    }
}
