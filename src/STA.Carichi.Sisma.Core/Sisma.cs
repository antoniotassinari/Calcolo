using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

using System.ComponentModel;
using STA.Carichi.Sisma.Funzioni;
using System.Resources;
using System.Reflection;

namespace STA.Carichi.Sisma
{
    /// <summary>
    /// Enumerazione di terreno
    /// </summary>
    public enum TipiTerreni { A, B, C, D, E }           //enumerazione delle classi di terreno

    public class distanzePunti
    {
        public Point3d puntoRif {get; set;}
        public Point3d puntoEst {get; set;}
        public double distanzaPunti { get; set; }


        public distanzePunti(Point3d punto1In, Point3d punto2In)  //costruttore UNICO
        {
            puntoRif = punto1In;
            puntoEst = punto2In;
            distanzaPunti = funzioni.calcolaDistanza(puntoRif, puntoEst);
        }
        
    }

    public class valoreSpettrale
    {
        public spettro spettroSpec {get; set;}
        public double periodo { get; set; }
        public double risposta { get; set; }
        public double rispostaVert { get; set; }

        
        public valoreSpettrale(spettro spettroIn, double periodoIn) : this(spettroIn, periodoIn, 1) //costruttore unico
        {

                
        }

        public valoreSpettrale(spettro spettroIn, double periodoIn, double fattoreStrutturaIn) 
        {
            spettroSpec = spettroIn;
            periodo = periodoIn;
            risposta = funzioni.calcolaRisposta(periodoIn, spettroIn, fattoreStrutturaIn);
            rispostaVert = funzioni.calcolaRispostaVert(periodoIn, spettroIn);
        }

    }
    
    public class spettro
    {
        
        public Sisma.statiLimite statoLimite { get; set; }
        public double numPvr { get; set; }
        public int numTr { get; set; }
        public double numTcStarMedia { get; set; }
        public double numAgMedia { get; set; }
        public double numF0Media { get; set; }
        public double numcC { get; set; }
        public double numSs { get; set; }
        public double numTb { get; set; }
        public double numTc { get; set; }
        public double numTd { get; set; }
        public double numS { get; set; }
        public double numFv { get; set; }
        public double q { get; set; }
        
        public List<valoreSpettrale> valoriSpettrali {get; set;}    //array di valori spettri

        public spettro(Sisma sismaIn, Sisma.statiLimite statoLimiteIn)  : this(sismaIn, statoLimiteIn, 1)
        {


        }

        public spettro(Sisma sismaIn, Sisma.statiLimite statoLimiteIn, double fattoreStrutturaIn)
        {
            double g = 9.81;
            q = fattoreStrutturaIn;
            statoLimite = statoLimiteIn;
            numPvr = funzioni.calcoloPvr(statoLimiteIn);
            numTr = funzioni.calcoloNumTr(sismaIn.PeriodoRiferimento, statoLimiteIn);
            numAgMedia = funzioni.media("ag", sismaIn, numTr);
            numTcStarMedia = funzioni.media("tc*", sismaIn, numTr);
            numF0Media = funzioni.media("f0", sismaIn, numTr);
            calcoloParametriTerreno(sismaIn.CategoriaSottosuolo.Categoria);
            numTc = numTcStarMedia * numcC;
            numTb = numTc / 3;
            numTd = 4 * numAgMedia / g + 1.6;
            numS = numSs;
            numFv = 1.35 * numF0Media * Math.Pow(numAgMedia / g, 0.5);

            //creazione valori spettrali
            creaValoriSpettrali();
        }

        private void creaValoriSpettrali()
        {
            double periodo = 0;
            int i;
            int k = 0;
            List<valoreSpettrale> valoriProv = new List<valoreSpettrale>();
            for (i = 0; i <= 9; i++)
            {
                periodo = i * numTb / 10;
                valoreSpettrale valoreProv = new valoreSpettrale(this, periodo, q);
                valoriProv.Add(valoreProv);
                k++;
            }
            for (i = 0; i <= 9; i++)
            {
                periodo = numTb + i * (numTc - numTb) / 10;
                valoreSpettrale valoreProv = new valoreSpettrale(this, periodo, q);
                valoriProv.Add(valoreProv);
                k++;
            }
            for (i = 0; i <= 9; i++)
            {
                periodo = numTc + i * (numTd - numTc) / 10;
                valoreSpettrale valoreProv = new valoreSpettrale(this, periodo, q);
                valoriProv.Add(valoreProv);
                k++;
            }
            for (i = 0; i <= 9; i++)
            {
                periodo = numTd + i * 2;
                valoreSpettrale valoreProv = new valoreSpettrale(this, periodo, q);
                valoriProv.Add(valoreProv);
                k++;
            }
            valoriSpettrali = valoriProv;

        }

        private void calcoloParametriTerreno(TipiTerreni terrenoIn)
        {
            double g = 9.81;
            if (terrenoIn == TipiTerreni.A)
            {
                numSs = 1;
                numcC = 1;
            }

            if (terrenoIn == TipiTerreni.B)
            {
                numcC = 1.10 * (Math.Pow(Convert.ToDouble(numTcStarMedia), -0.20));
                numSs = 1.4 - 0.4 * Convert.ToDouble(numF0Media) * Convert.ToDouble(numAgMedia) / g;
                if (numSs > 1.2)
                {
                    numSs = 1.2;
                } if (numSs < 1)
                {
                    numSs = 1;
                }
            }

            if (terrenoIn == TipiTerreni.C)
            {
                numcC = 1.05 * (Math.Pow(Convert.ToDouble(numTcStarMedia), -0.33));
                numSs = 1.7 - 0.6 * Convert.ToDouble(numF0Media) * Convert.ToDouble(numAgMedia) / g;
                if (numSs > 1.5)
                {
                    numSs = 1.5;
                } if (numSs < 1)
                {
                    numSs = 1;
                }

            }

            if (terrenoIn == TipiTerreni.D)
            {
                numcC = 1.25 * (Math.Pow(numTcStarMedia, -0.50));
                numSs = 2.4 - 1.5 * Convert.ToDouble(numF0Media) * Convert.ToDouble(numAgMedia) / g;
                if (numSs > 1.8)
                {
                    numSs = 1.8;
                } if (numSs < 0.9)
                {
                    numSs = 0.9;
                }
            }

            if (terrenoIn == TipiTerreni.E)
            {
                numcC = 1.15 * (Math.Pow(Convert.ToDouble(numTcStarMedia), -0.40));
                numSs = 2 - 1.1 * Convert.ToDouble(numF0Media) * Convert.ToDouble(numAgMedia) / g;
                if (numSs > 1.6)
                {
                    numSs = 1.6;
                } if (numSs < 1)
                {
                    numSs = 1;
                }
            }


        }
    
    }
       
    /// <summary>
    /// Classe per la gestione dei punti
    /// </summary>
    public class Point3d : INotifyPropertyChanged
    {
        private double _X { get; set; }
        private double _Y { get; set; }

        #region pubbliche

        /// <summary>
        /// Coordinata X, ovvero longitudine
        /// </summary>
        public double X
        {
            get { return _X; }
            set
            {
                _X = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("X"));
                }
            }
        }

        
        /// <summary>
        /// Coordinata Y, ovvero latitudine
        /// </summary>
        public double Y
        {
            get { return _Y; }
            set
            {
                _Y = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Y"));
                }
            }
        }
        public double Z { get; set; }
        public string ID { get; set; }

        #endregion

        #region costruttore

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="idIn">Descrittore</param>
        /// <param name="xIn">coordinata X</param>
        /// <param name="yIn">coordinata Y</param>
        /// <param name="zIn">coordinata Z</param>
        public Point3d(string idIn, double xIn, double yIn, double zIn)
        {
            ID = idIn;
            X = xIn;
            Y = yIn;
            Z = zIn;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("ID"));
                PropertyChanged(this, new PropertyChangedEventArgs("X"));
                PropertyChanged(this, new PropertyChangedEventArgs("Y"));
                PropertyChanged(this, new PropertyChangedEventArgs("Z"));
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }

    /// <summary>
    /// Classe per la gestione dei tipi di stati limite del sisma
    /// </summary>
    public class StatoLimiteSismica
    {
        #region prop

        #region private

        private string _Tipo { get; set; }
        private string _Descrizione { get; set; }
        private double _Pvr {get; set;}
        

        #endregion

        #region pubbliche

        /// <summary>
        /// Tipo di stato limite
        /// </summary>
        public string Tipo
        {
            get { return _Tipo; }
            set { _Tipo = value; }
        }

        /// <summary>
        /// Descrizione dello stato limite
        /// </summary>
        public string Descrizione
        {
            get { return _Descrizione; }
            set { _Descrizione = value; }
        }

        public double Pvr
        {
            get {return _Pvr;}
            set {_Pvr=value;}
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Gestione delle caratteristiche della vita nominale
    /// </summary>
    public class ClasseGestioneVitaNominale
    {
        #region prop

        #region private

        private int _ID { get; set; }
        private string _Descrizione { get; set; }
        private int _Vn { get; set; }

        #endregion

        #region pubbliche

        /// <summary>
        /// identificativo
        /// </summary>
        public int ID
        {
            get { return _ID; }
            set
            { _ID = value; }
        }

        /// <summary>
        /// Descrizione
        /// </summary>
        public string Descrizione
        {
            get { return _Descrizione; }
            set
            { _Descrizione = value; }
        }

        /// <summary>
        /// Vita Nominale [anni]
        /// </summary>
        public int Vn
        {
            get { return _Vn; }
            set
            { _Vn = value; }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Gestione delle caratteristiche delle classi d'uso
    /// </summary>
    public class ClasseGestioneClassiUso
    {
        #region prop

        #region interne

        private string _ID { get; set; }
        private string _Descrizione { get; set; }
        private double _Cu { get; set; }

        #endregion

        #region pubb

        /// <summary>
        /// Identificativo
        /// </summary>
        public string ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
            }
        }

        /// <summary>
        /// Descrizione
        /// </summary>
        public string Descrizione
        {
            get { return _Descrizione; }
            set { _Descrizione = value; }
        }

        /// <summary>
        /// Coefficiente d'uso
        /// </summary>
        public double Cu
        {
            get { return _Cu; }
            set { _Cu = value; }
        }
        #endregion
        #endregion
    }

    /// <summary>
    /// Classe per la gestione delle proprietà del sottosuolo
    /// </summary>
    public class ClasseGestioneCategorieSottosuolo
    {
        #region prop
        #region priv
        private TipiTerreni _Categoria { get; set; }
        private string _Descrizione { get; set; }

        #endregion
        
        #region publ

        /// <summary>
        /// Categoria tipo di terreni
        /// </summary>
        public TipiTerreni Categoria
        {
            get { return _Categoria; }
            set { _Categoria = value; }
        }

        /// <summary>
        /// Descrizione del tipo di terreno
        /// </summary>
        public string Descrizione
        {
            get { return _Descrizione; }
            set { _Descrizione = value; }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Classe per la gestione dell'elenco delle vite nominali
    /// </summary>
    public partial class ClasseGestioneElencoViteNominali : List<ClasseGestioneVitaNominale>
    {

    }

    /// <summary>
    /// Classe per la gestione dell'elenco delle classi d'uso
    /// </summary>
    public partial class ClasseGestioneElencoClassiUso : List<ClasseGestioneClassiUso>
    {
    }

    /// <summary>
    /// Classe per la gestione del sisma
    /// </summary>
    public class Sisma : INotifyPropertyChanged
    {

        public static ResourceManager resourceManager;

        #region enumerazioni
        /// <summary>
        /// Eumerazione degli stati limite
        /// </summary>
        public enum statiLimite { 
            /// <summary>
            /// Stato limite di Operatività
            /// </summary>
            SLO,
            /// <summary>
            /// Stato limite di Danno
            /// </summary>
            SLD,
            /// <summary>
            /// Stato limite di salvaguardia della Vita
            /// </summary>
            SLV,
            /// <summary>
            /// Stato limite di collasso
            /// </summary>
            SLC }




        public enum direzione { Orizzontale, Verticale }

        /// <summary>
        /// elenco delle vite nominali
        /// </summary>
        public readonly ClasseGestioneElencoViteNominali ElencoViteNominali = new ClasseGestioneElencoViteNominali
        {
            new ClasseGestioneVitaNominale{ID = 1, Descrizione="Opere provvisorie", Vn=10}, 
            new ClasseGestioneVitaNominale {ID = 2, Descrizione = "Opere ordinarie", Vn = 50},
            new ClasseGestioneVitaNominale {ID = 3, Descrizione = "Oepre strategiche", Vn = 100},
        };

        /// <summary>
        /// elenco delle classi d'uso
        /// </summary>
        public readonly ClasseGestioneElencoClassiUso ElencoClassiUso = new ClasseGestioneElencoClassiUso
        {
            new ClasseGestioneClassiUso{ID = "I", Descrizione="Costruzioni con presenza occasionale di persone", Cu=0.7},
            new ClasseGestioneClassiUso{ID = "II", Descrizione="Costruzioni con normale affollamento", Cu=1},
            new ClasseGestioneClassiUso{ID = "III", Descrizione="Costruzioni con significativi affollamenti", Cu=1.5},
            new ClasseGestioneClassiUso{ID = "IV", Descrizione="Costruzioni con funzioni stratiche o pubblice importanti", Cu=2},
        };

        /// <summary>
        /// 
        /// </summary>
        public readonly List<ClasseGestioneCategorieSottosuolo> ElencoCategorieSottosuolo = new List<ClasseGestioneCategorieSottosuolo>
        {
            new ClasseGestioneCategorieSottosuolo{Categoria = TipiTerreni.A, Descrizione= "Ammassi rocciosi affioranti o terreni molto rigidi caratterizzati da valori di Vs,30 superiori a 800 m/s, eventualmente comprendenti in superficie uno strato di alterazione, con spessore massimo pari a 3 m."},
            new ClasseGestioneCategorieSottosuolo{Categoria = TipiTerreni.B, Descrizione= "Rocce tenere e depositi di terreni a grana grossa molto addensati o terreni a grana fina molto consistenti con spessori superiori a 30 m, caratterizzati da un graduale miglioramento delle proprietà meccaniche con la profondità e da valori di Vs,30 compresi tra 360 m/s e 800 m/s (ovvero NSPT,30 > 50 nei terreni a grana grossa e cu,30 > 250 kPa nei terreni a grana fina)."},
            new ClasseGestioneCategorieSottosuolo{Categoria = TipiTerreni.C, Descrizione= "Depositi di terreni a grana grossa mediamente addensati o terreni a grana fina mediamente consistenti con spessori superiori a 30 m, caratterizzati da un graduale miglioramento delle proprietà meccaniche con la profondità e da valori di Vs,30 compresi tra 180 m/s e 360 m/s (ovvero 15 < NSPT,30 < 50 nei terreni a grana grossa e 70 < cu,30 < 250 kPa nei terreni a grana fina)."},
            new ClasseGestioneCategorieSottosuolo{Categoria = TipiTerreni.D, Descrizione= "Depositi di terreni a grana grossa scarsamente addensati o di terreni a grana fina scarsamente consistenti, con spessori superiori a 30 m, caratterizzati da un graduale miglioramento delle proprietà meccaniche con la profondità e da valori di Vs,30 inferiori a 180 m/s (ovvero NSPT,30 < 15 nei terreni a grana grossa e cu,30 < 70 kPa nei terreni a grana fina)."},
            new ClasseGestioneCategorieSottosuolo{Categoria = TipiTerreni.E, Descrizione= "Terreni dei sottosuoli di tipo C o D per spessore non superiore a 20 m, posti sul substrato di riferimento (con Vs > 800 m/s)."},
        
        };


        #endregion



        #region proprietà
        #region interne
        private Point3d _posizione { get; set; }
        private ClasseGestioneVitaNominale _TipoCostruzione { get; set; }
        private ClasseGestioneClassiUso _ClasseUso { get; set; }
        private ClasseGestioneCategorieSottosuolo _CategoriaSottosuolo { get; set; }
        private distanzePunti[] _elencoDistanze { get; set; }
        private spettro[] _spettri { get; set; }
        private spettro[] _spettriProgetto {get; set;}
        private double _FattoreStruttura { get; set; }

        #endregion


        /// <summary>
        /// Posizione geografica per il calcolo
        /// </summary>
        public Point3d posizione
        {
            get { return _posizione; }
            set
            {
                _posizione = value;
                
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("posizione"));
                }
            }
        }

        /// <summary>
        /// Tipo di costruzione assegnata
        /// </summary>
        public ClasseGestioneVitaNominale TipoCostruzione
        {
            get { return _TipoCostruzione; }
            set { _TipoCostruzione = value;
            AggiornaProprietà();
            }
        }

        /// <summary>
        /// Tipo di classe d'uso Assegnata
        /// </summary>
        public ClasseGestioneClassiUso ClasseUso
        {
            get { return _ClasseUso; }
            set
            {
                _ClasseUso = value;
                AggiornaProprietà();
            }
        }

        /// <summary>
        /// periodo di riferimento [anni]
        /// </summary>
        public double PeriodoRiferimento
        {
            get
            {
                if (ClasseUso.Cu * TipoCostruzione.Vn >= 35)
                {
                    return ClasseUso.Cu * TipoCostruzione.Vn;
                }
                else return 35;
                
            }
        }

        /// <summary>
        /// Categoria di sottosuolo
        /// </summary>
        public ClasseGestioneCategorieSottosuolo CategoriaSottosuolo
        {
            get { return _CategoriaSottosuolo; }
            set
            {
                _CategoriaSottosuolo = value;

                AggiornaProprietà();
            }
        }

        /// <summary>
        /// Elenco dei punti
        /// </summary>
        public distanzePunti[] elencoDistanze
        {
            get { return _elencoDistanze; }
            set
            {
                _elencoDistanze = value;
                if (PropertyChanged != null)
                {

                    PropertyChanged(this, new PropertyChangedEventArgs("elencoDistanze"));
                }
            }
        }

        /// <summary>
        /// Indirizzo per la visualizzazione nella finestra browser
        /// </summary>
        public Uri IndirizzoURLVisualizzazione
        {
            get
            {


                string stringaUrl = "http://www.ingtassinari.it/geo.htm?latitudine=" + posizione.Y.ToString() + "&longitudine=" + posizione.X.ToString();
                try
                {
                    int i = 0;
                    for (i = 0; i < 4; i++)
                    {

                        stringaUrl += "&p" + (i + 1).ToString() + "ID=" + elencoDistanze[i].puntoEst.ID;
                        stringaUrl = stringaUrl + "&p" + (i + 1).ToString() + "Lat=" + elencoDistanze[i].puntoEst.Y.ToString();
                        stringaUrl = stringaUrl + "&p" + (i + 1).ToString() + "Lon=" + elencoDistanze[i].puntoEst.X.ToString();

                    }
                }
                catch { }
                return new Uri(stringaUrl);

            }

            set
            {
                IndirizzoURLVisualizzazione = value;

            }
        }

        public int tipoOpera { get; set; }
        public double numVn { get; set; }
        public int classeUso { get; set; }
        public double numVr { get; set; }
        public TipiTerreni terreno { get; set; }

        /// <summary>
        /// insieme degli spettri selastici associati
        /// </summary>
        public spettro[] spettri
        {
            get { return _spettri; }
            set
            {
                _spettri = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("spettri"));
                }
            }
        }

        public spettro[] spettriProgetto
        {
            get { return _spettriProgetto; }
            set
            {
                _spettriProgetto = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("spettriProgetto"));
                }
            }
        }

        /// <summary>
        /// Fattore di struttura
        /// </summary>
        public double FattoreStruttura
        {
            get { return _FattoreStruttura; }
            set
            {
                _FattoreStruttura = value;
                AggiornaProprietà();
            }
        }

        List<Point3d> PuntiReticolo { get; set; }
        public ArrayList righe { get; set; }
        public string[][] righeConvertite { get; set; }


        char[] delimiterchars = { ';' };


        


        #endregion

        /// <summary>
        /// Costruttore di base
        /// </summary>
        public Sisma()
            : this(12.19944,﻿44.41778)
        {
        }

        public Sisma(double x, double y)
            : this(x, y, 0, 0, TipiTerreni.A)
        {
            
        }

        public Sisma(double coorXIn, double coorYIn, int tipoOperaIn, int classeUsoIn, TipiTerreni terrenoIn)
        {

            TipoCostruzione = ElencoViteNominali[0];
            ClasseUso = ElencoClassiUso[0];
            CategoriaSottosuolo = ElencoCategorieSottosuolo[0];
            FattoreStruttura = 1;

            posizione = new Point3d("PosCalcolo", coorXIn, coorYIn, 0);
            posizione.PropertyChanged += AggiornataProprietàAnnidata;
            PuntiReticolo = new List<Point3d>();

            leggiFile();

            selezionaPuntiReticolo();

            creaSpettri();

            ///questi sono superate...
            tipoOpera = tipoOperaIn;
            numVn = funzioni.calcoloVn(tipoOperaIn);
            classeUso = classeUsoIn;
            numVr = numVn * funzioni.cu(classeUsoIn);
            terreno = terrenoIn;

            //ordinaDistanze();




            



        }

        #region metodi

        public void ordinaDistanze()
        {
            distanzePunti[] distanzeProv = new distanzePunti[righeConvertite.Length - 2];

            int i;
            for (i = 2; i < righeConvertite.Length; i++)
            {

                string[] corrente = righeConvertite[i];

                Point3d puntoConf = new Point3d(corrente[0], double.Parse(corrente[1], System.Globalization.CultureInfo.InvariantCulture), double.Parse(corrente[2], System.Globalization.CultureInfo.InvariantCulture), 0);

                distanzeProv[i - 2] = new distanzePunti(posizione, puntoConf);


            }
            elencoDistanze = distanzeProv;
            confrontoDistanze confronto = new confrontoDistanze();
            Array.Sort(elencoDistanze, confronto);
        }

        /// <summary>
        /// trova i punti del reticolo in base al quadrante di appartenenza
        /// </summary>
        public void selezionaPuntiReticolo()
        {
            distanzePunti[] distanzaProv = new distanzePunti[4];

            List<Point3d> PuntiSX = new List<Point3d>();

            List<Point3d> PuntiDX = new List<Point3d>();

            //punti a sinistra
            foreach (Point3d puntoProv in PuntiReticolo)
            {
                if (puntoProv.X <= this.posizione.X)
                {
                    PuntiSX.Add(puntoProv);
                }


            }

            //punti a destra
            foreach (Point3d puntoProv in PuntiReticolo)
            {
                if (puntoProv.X > this.posizione.X)
                {
                    PuntiDX.Add(puntoProv);
                }


            }

            List<Point3d> Punti_SX_UP = new List<Point3d>();
            List<Point3d> Punti_SX_DOWN = new List<Point3d>();
            List<Point3d> Punti_DX_UP = new List<Point3d>();
            List<Point3d> Punti_DX_DOWN = new List<Point3d>();
            
            //punti in alto a SINISTRA
            foreach (Point3d puntoProv in PuntiSX)
            {
                if (puntoProv.Y >= this.posizione.Y)
                {
                    Punti_SX_UP.Add(puntoProv);
                }
            }

            //punti in basso a SINISTRA
            foreach (Point3d puntoProv in PuntiSX)
            {
                if (puntoProv.Y < this.posizione.Y)
                {
                    Punti_SX_DOWN.Add(puntoProv);
                }
            }

            //punti in alto a DESTRA
            foreach (Point3d puntoProv in PuntiDX)
            {
                if (puntoProv.Y >= this.posizione.Y)
                {
                    Punti_DX_UP.Add(puntoProv);
                }
            }

            //punti in basso a SINISTRA
            foreach (Point3d puntoProv in PuntiDX)
            {
                if (puntoProv.Y < this.posizione.Y)
                {
                    Punti_DX_DOWN.Add(puntoProv);
                }
            }

            if (Punti_SX_UP.Count == 0 || Punti_SX_DOWN.Count == 0 || Punti_DX_UP.Count == 0 || Punti_DX_DOWN.Count == 0)
                throw new ArgumentException("Coordinate esterne al reticolo sismico disponibile.");

            Point3d punto_SX_UP = new Point3d("SX_UP", 0,0,0);
            Point3d punto_SX_DOWN = new Point3d("SX_DOWN", 0,0,0);

            //punto in alto a sinistra
            foreach (Point3d puntoProv in Punti_SX_UP)
            {
                if ((new distanzePunti(this.posizione, punto_SX_UP)).distanzaPunti > (new distanzePunti(this.posizione, puntoProv)).distanzaPunti)
                {
                    punto_SX_UP = puntoProv;
                }
            }

            //punto in basso a sinistra
            foreach (Point3d puntoProv in Punti_SX_DOWN)
            {
                if ((new distanzePunti(this.posizione, punto_SX_DOWN)).distanzaPunti > (new distanzePunti(this.posizione, puntoProv)).distanzaPunti)
                {
                    punto_SX_DOWN = puntoProv;
                }
            }



            Point3d punto_DX_UP = new Point3d("DX_UP", 0, 0, 0);
            Point3d punto_DX_DOWN = new Point3d("DX_DOWN", 0, 0, 0);

            //punto in alto a destra
            foreach (Point3d puntoProv in Punti_DX_UP)
            {
                if ((new distanzePunti(this.posizione, punto_DX_UP)).distanzaPunti > (new distanzePunti(this.posizione, puntoProv)).distanzaPunti)
                {
                    punto_DX_UP = puntoProv;
                }
            }

            //punto in basso a destra
            foreach (Point3d puntoProv in Punti_DX_DOWN)
            {
                if ((new distanzePunti(this.posizione, punto_DX_DOWN)).distanzaPunti > (new distanzePunti(this.posizione, puntoProv)).distanzaPunti)
                {
                    punto_DX_DOWN = puntoProv;
                }
            }

            elencoDistanze = new distanzePunti[4];

            elencoDistanze[0] = new distanzePunti(posizione, punto_SX_UP);
            elencoDistanze[1] = new distanzePunti(posizione, punto_SX_DOWN);
            elencoDistanze[2] = new distanzePunti(posizione, punto_DX_UP);
            elencoDistanze[3] = new distanzePunti(posizione, punto_DX_DOWN);


        }

        /// <summary>
        /// Leggi il file
        /// </summary>
        public void leggiFile()   //metodo, ricostruisce la tabella dal file TabellaParametriSpettrali.csv che fa parte di questo progetto.
        {
            Assembly _assembly;
            Stream _filestream;

            _assembly = Assembly.GetExecutingAssembly();

            _filestream = _assembly.GetManifestResourceStream("STA.Carichi.Sisma.TabellaParametriSpettrali.csv");

            PuntiReticolo.Clear();
            if (righe != null)
            {
                righe.Clear();
            }

            ArrayList righeProv = new ArrayList();

            using (StreamReader sr = new StreamReader(_filestream)) 
            {
                int i = 0;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    ///Legge le righe splittando le righe lette
                    string[] divisa = line.Split(delimiterchars);
                    if (i > 1)
                    {
                        ///Aggiunge il punto alla lista generale dei punti
                        PuntiReticolo.Add(new Point3d(divisa[0], double.Parse(divisa[1], System.Globalization.CultureInfo.InvariantCulture), double.Parse(divisa[2], System.Globalization.CultureInfo.InvariantCulture), 0));
                    }

                    righeProv.Add(divisa);
                    i++;
                }
            }
            righe = righeProv;
            righeConvertite = (string[][])righe.ToArray(typeof(string[]));
        }

        /// <summary>
        /// Metodo per la lettura delle coordinate dall'indirizzo URL del sito su ingtassinari.it
        /// </summary>
        /// <param name="urlIn"></param>
        public void leggiCoorDaUrl(Uri urlIn)
        {
            string lat = this.posizione.Y.ToString();
            string lon = this.posizione.X.ToString();
            char[] dividiPrima = { '?' };
            char[] dividiDopo = { '&' };

            string stringa = urlIn.ToString();
            string stringhe01 = stringa.Split(dividiPrima)[1];
            string[] stringhe02 = stringhe01.Split(dividiDopo);
            foreach (string stringaIn in stringhe02)
            {
                if (stringaIn.Split('=')[0] == "latitudine")
                {
                    lat = stringaIn.Split('=')[1];
                }
                if (stringaIn.Split('=')[0] == "longitudine")
                {
                    lon = stringaIn.Split('=')[1];
                }
            }
            this.posizione.Y = Convert.ToDouble(lat);
            this.posizione.X = Convert.ToDouble(lon);


        }

        #endregion

        class confrontoDistanze : IComparer
        {
            public int Compare(object x, object y)
            {

                distanzePunti a1 = (distanzePunti)x;
                distanzePunti a2 = (distanzePunti)y;
                return a1.distanzaPunti.CompareTo(a2.distanzaPunti);
            }
        }



        public void aggiornaCoordinate(double coorXIn, double coorYIn)
        {
            Point3d puntotemp = new Point3d("PosCalc", coorXIn, coorYIn, 0);
            posizione = puntotemp;
            //ordinaDistanze();

            selezionaPuntiReticolo();

            creaSpettri();

        }

        public void aggiornaOpera(int tipoOperaIn, int classeUsoIn, TipiTerreni terrenoIn)
        {
            tipoOpera = tipoOperaIn;
            numVn = funzioni.calcoloVn(tipoOperaIn);
            classeUso = classeUsoIn;
            numVr = numVn * funzioni.cu(classeUsoIn);
            terreno = terrenoIn;
            creaSpettri();

        }

        /// <summary>
        /// Metodo per la generazione degli spettri
        /// </summary>
        public void creaSpettri()
        {
            spettro[] spettriTemp = new spettro[4];
            int i;
            for (i = 0; i < 4; i++)
            {
                spettro spettroProv = new spettro(this, (statiLimite)i);
                spettriTemp[i] = spettroProv;
            }
            spettri = spettriTemp;

                spettriProgetto = new spettro[1];
                spettro spettroProv1 = new spettro(this, statiLimite.SLV, FattoreStruttura);
                spettriProgetto[0] = spettroProv1;



        }







        #region salvataggi

        public void salvaSuFile(string path)
        {
            System.IO.File.Delete(path);
            StreamWriter sw = new StreamWriter(path);
            sw.WriteLine("Spettri di risposta");
            sw.WriteLine();
            sw.WriteLine("Longitudine: " + posizione.X.ToString());
            sw.WriteLine("Latitudine: " + posizione.Y.ToString());
            sw.WriteLine();
            
            foreach (spettro spettroProv in spettri)
            {
                sw.WriteLine("- "+spettroProv.statoLimite.ToString()+" -");
                foreach (valoreSpettrale valoreProv in spettroProv.valoriSpettrali)
                {
                    sw.WriteLine(valoreProv.periodo.ToString("F5") + "  " + valoreProv.risposta.ToString("F5") + "  " + valoreProv.rispostaVert.ToString("F5"));

                }
                
                sw.WriteLine();
                sw.WriteLine();

            }
            
            sw.Close();
            sw.Dispose();
        }

        public void salvaFileRobot(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);

            }
            StreamWriter sw = new StreamWriter(path);
            
            foreach (spettro spettroProv in spettri)
            {
                for (int i = 0; i < 2; i++)
                {
                    sw.WriteLine("SPECTRUM");
                    string nomeSpettro = spettroProv.statoLimite.ToString();
                    if (i == 0)
                    {
                        nomeSpettro += "_H";
                    }
                    if (i == 1)
                    {
                        nomeSpettro += "_V";
                    }
                    sw.WriteLine(nomeSpettro);
                    sw.WriteLine("0 0 0 1");
                    sw.WriteLine("0 " + spettroProv.valoriSpettrali.Count);
                    foreach (valoreSpettrale valoreProv in spettroProv.valoriSpettrali)
                    {
                        if (i == 0)
                        {
                            sw.WriteLine(valoreProv.periodo.ToString("F5") + "  " + valoreProv.risposta.ToString("F5"));

                        }
                        if (i == 1)
                        {
                            sw.WriteLine(valoreProv.periodo.ToString("F5") + "  " + valoreProv.rispostaVert.ToString("F5"));
                        }
                    }
                    



                }
            }
            sw.Close();
            sw.Dispose();
        }

        public void salvaFilesEtabs(string path)
    {
        foreach (spettro spettroProv in spettri)
        {
            for (int i = 0; i < 2; i++)
            {
                string nomeFile = path+"\\" +spettroProv.statoLimite.ToString();
                string direzione = "";
                if (i == 0)
                {
                    direzione = "_H.txt";
                }
                if (i == 1)
                {
                    direzione = "_V.txt";
                }
                nomeFile += direzione;
                if (System.IO.File.Exists(nomeFile))
                {
                    System.IO.File.Delete(nomeFile);
                }
                StreamWriter sw = new StreamWriter(nomeFile);
                sw.WriteLine("Spettro di risposta" + spettroProv.statoLimite.ToString() + direzione);
                sw.WriteLine("Longitudine: " + posizione.X.ToString());
                sw.WriteLine("Latitudine: " + posizione.Y.ToString());
                foreach (valoreSpettrale valoreProv in spettroProv.valoriSpettrali)
                {
                    if (i == 0)
                    {
                        sw.WriteLine(valoreProv.periodo.ToString("F5") + "  " + valoreProv.risposta.ToString("F5"));

                    }
                    if (i == 1)
                    {
                        sw.WriteLine(valoreProv.periodo.ToString("F5") + "  " + valoreProv.rispostaVert.ToString("F5"));
                    }


                }
                sw.Close();
                sw.Dispose();



            }
        }
    }

        #endregion




            public event PropertyChangedEventHandler PropertyChanged;
            
        public void AggiornaProprietà()
            {
                if (PuntiReticolo != null)
                {
                    selezionaPuntiReticolo();
                    creaSpettri();
                }

                

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FattoreStruttura"));
                    PropertyChanged(this, new PropertyChangedEventArgs("posizione"));
                    PropertyChanged(this, new PropertyChangedEventArgs("IndirizzoURLVisualizzazione"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ElencoViteNominali"));
                    PropertyChanged(this, new PropertyChangedEventArgs("TipoCostruzione"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ClassUso"));
                    PropertyChanged(this, new PropertyChangedEventArgs("PeriodoRiferimento"));
                    PropertyChanged(this, new PropertyChangedEventArgs("CategoriaSottosuolo"));

                }
            }

        /// <summary>
        /// gestione dei cambiamenti delle proprietà interne alle proprietà, come le coordina della posizione
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AggiornataProprietàAnnidata(object sender, PropertyChangedEventArgs e)
        {
            AggiornaProprietà();
        }
    }
    

}
