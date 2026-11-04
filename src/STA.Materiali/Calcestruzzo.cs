using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Calcestruzzo
{
    /// <summary>
    /// Enumerazione delle possibili classi di calcestruzzo
    /// </summary>
    public enum ClassiNormale
    {
        C8_10,
        C12_15,
        C16_20,
        C20_25,
        C25_30,
        C28_35,
        C32_40,
        C35_45,
        C40_50,
        C45_55,
        C50_60,
        C55_67,
        C60_75,
        C70_85,
        C80_95,
        C90_105,
        C100_115
    }



    /// <summary>
    /// Classe per la gestione delle proprietà del calcestruzzo
    /// </summary>
    public class Calcestruzzo
    {
        #region prop. interne

        ClassiNormale _Classe { get; set; }

        double _Rck {get; set;}
        double _Ecm { get; set; }
        

        #endregion


        #region pro. esterne

        /// <summary>
        /// Classe del Calcestruzzo secondo la UNI EN 206-1:2006
        /// </summary>
        public ClassiNormale Classe { get { return _Classe; } set { _Classe = value; } }

        /// <summary>
        /// Resistenza caratteristica cilindrica [N/mmq]
        /// </summary>
        public double fck { get { return 0.83 * Rck; } }

        /// <summary>
        /// Resistenza caratteristica cubica [N/mmq]
        /// </summary>
        public double Rck { get { return _Rck; } }

        /// <summary>
        /// Resistenza di calcolo a compressione del cls [N/mmq]
        /// </summary>
        public double fcd { get { return 0.85 * fck / 1.5; } }

        /// <summary>
        /// Valore caratteristico medio di resistenza cilindrica [N/mmq]
        /// </summary>
        public double fcm { get { return (fck + 8); } }

        /// <summary>
        /// Resistenza media a trazione semplice [N/mmq], [NTC2008 - 11.2.10.2]
        /// </summary>
        public double fctm
        {
            get
            {
                if (Classe > ClassiNormale.C50_60)
                {
                    return (2.12 * Math.Log(1 + fcm / 10));
                }
                else
                {
                    return (0.3 * Math.Pow(fck, 0.66666666));
                }
            }
            
        }

        /// <summary>
        /// Resistenza media a flessione [N/mmq], [NTC2008 - 11.2.10.2]
        /// </summary>
        public double fcfm
        {
            get { return 1.2 * fctm; }
            
        }

        /// <summary>
        /// Modulo elastico istantaneo [N/mmq], [NTC2008 - 11.2.10.3]
        /// </summary>
        public double Ecm
        {
            get { _Ecm =  (22000 * Math.Pow(fcm / 10, 0.3));
            return _Ecm;
            }
            set { _Ecm = value; }
        }


        #endregion

        #region Costruttori

        /// <summary>
        /// Costruttore a partire dalla classe del calcestruzzo
        /// </summary>
        /// <param name="classeIn">Classe in ingresso</param>
        public Calcestruzzo(ClassiNormale classeIn)
        {
            //Assegna la classe
            _Classe = classeIn;

            //Calcola e assgna i restanti parametri;
            calcolaCaratteristiche();

            
        }

        /// <summary>
        /// Costruttore di base
        /// </summary>
        public Calcestruzzo()
        {
        }

        #endregion

        #region metodi privati

        /// <summary>
        /// Metodo per calcolare le proprietà
        /// </summary>
        private void calcolaCaratteristiche()
        {
            //assegna le caratteristiche in funzione della classe del cls
            switch (Classe)
            {
                case ClassiNormale.C8_10:
                    _Rck = 10;
                    break;

                case ClassiNormale.C12_15:
                    _Rck = 15;
                    break;

                case ClassiNormale.C16_20:
                    _Rck = 20;
                    break;

                case ClassiNormale.C20_25:
                    _Rck = 25;
                    break;

                case ClassiNormale.C25_30:
                    _Rck = 30;
                    break;

                case ClassiNormale.C28_35:
                    _Rck = 35;
                    break;

                case ClassiNormale.C32_40:
                    _Rck = 40;
                    break;

                case ClassiNormale.C35_45:
                    _Rck = 45;
                    break;

                case ClassiNormale.C40_50:
                    _Rck = 50;
                    break;

                case ClassiNormale.C45_55:
                    _Rck = 55;
                    break;

                case ClassiNormale.C50_60:
                    _Rck = 60;
                    break;

                case ClassiNormale.C55_67:
                    _Rck = 67;
                    break;

                case ClassiNormale.C60_75:
                    _Rck = 75;
                    break;

                case ClassiNormale.C70_85:
                    _Rck = 85;
                    break;

                case ClassiNormale.C80_95:
                    _Rck = 85;
                    break;

                case ClassiNormale.C90_105:
                    _Rck = 85;
                    break;

                case ClassiNormale.C100_115:
                    _Rck = 85;
                    break;

                default:
                    throw new NotImplementedException();
            }

            

        }

        #endregion
    }

}
