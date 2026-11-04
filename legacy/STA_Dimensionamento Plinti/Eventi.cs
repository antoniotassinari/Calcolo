using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STA_Dimensionamento_Plinti
{
    class Eventi
    {
    }

    /// <summary>
    /// Classe argomenti evento per passare le caratteristiche del plinto
    /// </summary>
    public class AggiuntoPlintoValido : EventArgs
    {
        private IPlinto _plinto;

        public AggiuntoPlintoValido(IPlinto nuovo)
        {
            _plinto = nuovo;
        }

        public IPlinto plinto
        {
            get { return _plinto; }
        }
    }

    /// <summary>
    /// Argomenti di tipo nodo per il passaggio delle caratteristiche
    /// </summary>
    public class ArgomentoNodo : EventArgs
    {
        private Nodo _nodo;

        public ArgomentoNodo(Nodo nodo)
        {
            _nodo = nodo;
        }

        public Nodo nodo { get { return _nodo; } }
    }

}
