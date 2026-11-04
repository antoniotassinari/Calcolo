using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using STA.Common;
using System.Xml.Serialization;

namespace STA.Carichi.Neve
{
    /// <summary>
    /// Classe per la gestione delle proprietà della zona di carico di neve
    /// </summary>
    [XmlRoot("CaricoDiNeve")]
    public class ZoneCaricoNeve
    {
        #region proprietà

        #region interne

        string _Nome { get; set; }
        double _asmin { get; set; }
        double _qskmin { get; set; }
        double _coeff1 { get; set; }
        double _coeff2 { get; set; }

        #endregion

        /// <summary>
        /// Nome della Zona
        /// </summary>
        public string Nome
        {
            get
            {
                return _Nome;
            }
            set { _Nome = value; }
        }

        /// <summary>
        /// Altezza minima per il calcolo del carico di neve in m
        /// </summary>
        public double asmin 
        {
            get { return _asmin; }
            set { _asmin = value; }
        }

        /// <summary>
        /// Valore minimo di neve al suolo
        /// </summary>
        public double qskmin
        {
            get { return _qskmin; }
            set { _qskmin = value; }
        }

        /// <summary>
        /// primo coefficiente della formula
        /// </summary>
        public double coeff1
        {
            get { return _coeff1; }
            set { _coeff1 = value; }
        }

        /// <summary>
        /// primo coefficiente della formula
        /// </summary>
        public double coeff2
        {
            get { return _coeff2; }
            set { _coeff2 = value; }
        }

        /// <summary>
        /// Calcola il valore caratteristico del carico al suolo
        /// </summary>
        /// <param name="altezza"></param>
        /// <returns></returns>
        public double ValoreCaratteristico(double altezza)
        {
            if (altezza >= 0)
            {
                if (altezza <= this.asmin)
                {
                    return qskmin;
                }
                else
                {
                    return coeff1 * (1 + Math.Pow((altezza / coeff2), 2));
                }
            }
            else { return 0; }
        }

        #endregion
    }

    /// <summary>
    /// Class per la gestione delle Classi di Topografia
    /// </summary>
    public class ClasseTopografia
    {
        #region proprietà

        #region interne

        private string _Topografia {get; set;}
        private string _Descrizione {get; set;}
        private double _Ce {get; set;}

        #endregion

        #region esterne

        /// <summary>
        /// Proprietà Topogradia alla tabella 3.4.I
        /// </summary>
        public string Topografia
        {
            get {return _Topografia;}

            set
            {
                _Topografia = value;
            }
        }

        /// <summary>
        /// Proprietà per la descrizione della classe di topografia
        /// </summary>
        public string Descrizione
        {
                        get {return _Descrizione;}

            set
            {
                _Descrizione = value;
            }
        }

        /// <summary>
        /// Coeff. di topografia
        /// </summary>
        public double Ce
        {
            get {return _Ce;}
            set
            {
                _Ce=value;
            }
        }

        #endregion

        #endregion
    }

    public class CaricoNeve : CarichiBaseClass
    {
        #region proprietà

        public readonly List<ZoneCaricoNeve> ListaZone = new List<ZoneCaricoNeve>();
        public readonly List<ClasseTopografia> ListaClassiTopografia = new List<ClasseTopografia>();

        ZoneCaricoNeve _ZonaScelta {get; set;}
        ClasseTopografia _ClasseTopografiaScelta {get; set;}
        private double _AltezzaTopografia { get; set; }
        private double _Qsk { get; set; }
        private double _InclinazioneFalda { get; set; }
        private double _CoefficienteM1 { get; set; }
        

        /// <summary>
        /// Zona di neve scelta per determinare i valori di carico
        /// </summary>
        public ZoneCaricoNeve ZonaScelta
        {
            get { return _ZonaScelta; }
            set
            {
                _ZonaScelta = value;
               }
        }

        /// <summary>
        /// Classe di topografia Scelta
        /// </summary>
        public ClasseTopografia ClasseTopografiaScelta
        {
            get { return _ClasseTopografiaScelta; }
            set
            {
                _ClasseTopografiaScelta = value;
            }
        }

        /// <summary>
        /// Altezza Topografica del sito
        /// </summary>
        public double AltezzaTopografia
        {
            get { return _AltezzaTopografia; }
            set
            {
                _AltezzaTopografia = value;
                aggiornaProprietà();
            }
        }

        /// <summary>
        /// Valore Caratteristico al suolo [kN/mq]
        /// </summary>
        public double Qsk
        {
            get
            {
                if (ZonaScelta != null && ClasseTopografiaScelta != null)
                {
                    return ZonaScelta.ValoreCaratteristico(this.AltezzaTopografia);
                }
                else return 0;
            }
        }

        /// <summary>
        /// Inclinazione della falda in gradi sessangesimali    
        /// </summary>
        public double InclinazioneFalda
        {
            get { return _InclinazioneFalda; }
            set
            {
                _InclinazioneFalda = value;
            }

        }

        /// <summary>
        /// Coefficiente m1
        /// </summary>
        public double CoefficienteM1
        {
            get
            {
                if (InclinazioneFalda <= 30)
                {
                    return 0.8;
                }
                else if (InclinazioneFalda > 30 && InclinazioneFalda < 60)
                {
                    return (0.8 * (60 - InclinazioneFalda) / 30);
                }
                else
                {
                    return 0;
                }
            }
        }



        #endregion

        #region costruttore

        public CaricoNeve()
        {
            #region costruzione della lista di Zone

            ListaZone.Add(new ZoneCaricoNeve { Nome = "Zona I - Alpina", asmin = 200, qskmin=1.50, coeff1 = 1.39, coeff2 = 728});
            ListaZone.Add(new ZoneCaricoNeve { Nome = "Zona I - Mediterranea", asmin = 200, qskmin = 1.50, coeff1 = 1.35, coeff2 = 602 });
            ListaZone.Add(new ZoneCaricoNeve { Nome = "Zona II", asmin = 200, qskmin = 1, coeff1 = 0.85, coeff2 = 481 });
            ListaZone.Add(new ZoneCaricoNeve { Nome = "Zona III", asmin = 200, qskmin = 0.6, coeff1 = 0.51, coeff2 = 481 });



            #endregion

            #region costruzione della lista delle classi topografiche

            ListaClassiTopografia.Add(new ClasseTopografia { Topografia = "Battuta dai venti", Descrizione = "Aree pianeggianti non ostruite esposte su tutti i lati, senza costruzioni o alberi più alti.", Ce = 0.9 });
            ListaClassiTopografia.Add(new ClasseTopografia { Topografia = "Normale", Descrizione = "Aree in cui non è presente una significativa rimozione di neve sulla costruzione prodotta dal vento, a causa del terreno, altre costruzioni o alberi.", Ce = 1 });
            ListaClassiTopografia.Add(new ClasseTopografia { Topografia = "Riparata", Descrizione = "Aree in cui la costruzione considerata è sensibilmente più bassa del circostante terreno o circondata da costruzioni o alberi più alti.", Ce = 1.1 });

            #endregion


        }

        #endregion

        public void aggiornaProprietà()
        {


        

        }


    }
}
