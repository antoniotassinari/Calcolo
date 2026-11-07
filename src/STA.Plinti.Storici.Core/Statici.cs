using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace STA_Dimensionamento_Plinti.generali
{
    class Statici
    {
        public static XmlDocument xmldoc = new XmlDocument();
    }


    public enum TipoControllo
    {
        /// <summary>
        /// Per un alemento nuovo
        /// </summary>
        Aggiunta,
        /// <summary>
        /// per modificare un elemento
        /// </summary>
        Modifica
    }
}
