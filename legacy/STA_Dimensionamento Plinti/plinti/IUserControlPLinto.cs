using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STA_Dimensionamento_Plinti.plinti
{
    interface IUserControlPLinto
    {
        event EventHandler<AggiuntoPlintoValido> PlintoValidato;

    }

    interface IUserControlPropPlinto
    {

    }
}
