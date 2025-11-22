using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using STA.Carichi.Sisma;

namespace STA.Modules.CaricoSisma
{
    /// <summary>
    /// Module initialization for Seismic Load
    /// </summary>
    public class ModuloCaricoSisma
    {
        public static Sisma SismaAssociato = new Sisma(12.19944, 44.41778);

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
