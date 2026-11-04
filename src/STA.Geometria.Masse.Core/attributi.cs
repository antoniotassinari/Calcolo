using System;

namespace Geometria.Attributes
{
    /// <summary>
    /// Attributo da assegnare per distinugere che una proprietà si input e non calcolata, mi servirà per gestire l'interfaccia
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple=false, Inherited=true)]
    public class InputPropertyAttribute : Attribute
    {
        private string _Name;

        /// <summary>
        /// Costruttore che richiede una stringa di immissione
        /// </summary>
        /// <param name="inputName">stringa del nome</param>
        public InputPropertyAttribute(string inputName)
        {
            _Name = inputName;
        }

        /// <summary>
        /// restituisce il nome associato all'attributo
        /// </summary>
        public string Name { get { return _Name; } }

    }
}