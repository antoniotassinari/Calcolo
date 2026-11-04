using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using STA.Carichi.Neve;
using STA.Carichi.Neve.ViewModels;

namespace STA.Modules.CaricoNeve
{
    /// <summary>
    /// Module initialization for Snow Load
    /// </summary>
    public class ModuloCaricoNeve
    {
        public static STA.Carichi.Neve.CaricoNeve CaricoAssociato = new Carichi.Neve.CaricoNeve();
        public static STA.Carichi.Neve.ViewModels.CaricoNeveWorkspaceViewModel _modelView = new Carichi.Neve.ViewModels.CaricoNeveWorkspaceViewModel(CaricoAssociato);

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
