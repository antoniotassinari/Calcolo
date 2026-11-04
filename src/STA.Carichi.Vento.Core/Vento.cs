using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Vento
{
    /// <summary>
    /// Classe per gestione delle proprietà
    /// </summary>
    public class GestioneZonaGeografica
    {
        #region proprietà

        /// <summary>
        /// Zona Geografica
        /// </summary>
        public Vento.Zone Zona {get; set;}

        /// <summary>
        /// Descrizione della zona
        /// </summary>
        public string Descrizione
        { 
            get
            {
                switch (Zona) {
                    case Vento.Zone.Zona1:
                        return "Valle d’Aosta, Piemonte, Lombardia, Trentino Alto Adige, Veneto,Friuli Venezia Giulia (con l’eccezione della provincia di Trieste)";

                    case Vento.Zone.Zona2:
                        return "Emilia Romagna";

                    case Vento.Zone.Zona3:
                        return "Toscana, Marche, Umbria, Lazio, Abruzzo, Molise, Puglia, Campania, Basilicata, Calabria (esclusa la provincia di Reggio Calabria)";

                    case Vento.Zone.Zona4:
                        return "Sicilia e provincia di Reggio Calabria";

                    case Vento.Zone.Zona5:
                        return "Sardegna (zona a oriente della retta congiungente Capo Teulada con l’Isola di Maddalena)";

                    case Vento.Zone.Zona6:
                        return "Sardegna (zona a occidente della retta congiungente Capo Teulada conl’Isola di Maddalena)";

                    case Vento.Zone.Zona7:
                        return "Liguria";

                    case Vento.Zone.Zona8:
                        return "Provincia di Trieste";

                    case Vento.Zone.Zona9:
                        return "Isole (con l’eccezione di Sicilia e Sardegna) e mare aperto";

                    default:
                        throw new NotImplementedException("Necessario individuare la Zona");
                }
            }
        }

        /// <summary>
        /// Velocità di riferimento in m/s
        /// </summary>
        public double Vb0
        {
            get
            {
                switch (Zona) {
                    case Vento.Zone.Zona1:
                        return 25;

                    case Vento.Zone.Zona2:
                        return 25;

                    case Vento.Zone.Zona3:
                        return 27;

                    case Vento.Zone.Zona4:
                        return 28;

                    case Vento.Zone.Zona5:
                        return 28;

                    case Vento.Zone.Zona6:
                        return 28;

                    case Vento.Zone.Zona7:
                        return 28;

                    case Vento.Zone.Zona8:
                        return 30;

                    case Vento.Zone.Zona9:
                        return 31;

                    default:
                        throw new NotImplementedException("Necessario individuare la Zona");
                }
            }
        }

        /// <summary>
        /// Altezza a0 di riferimento
        /// </summary>
        public double a0
        {
            get {
                switch (Zona) {
                    case Vento.Zone.Zona1:
                        return 1000;

                    case Vento.Zone.Zona2:
                        return 750;

                    case Vento.Zone.Zona3:
                        return 500;

                    case Vento.Zone.Zona4:
                        return 500;

                    case Vento.Zone.Zona5:
                        return 750;

                    case Vento.Zone.Zona6:
                        return 500;

                    case Vento.Zone.Zona7:
                        return 1000;

                    case Vento.Zone.Zona8:
                        return 1500;

                    case Vento.Zone.Zona9:
                        return 500;

                    default:
                        throw new NotImplementedException("Necessario individuare la Zona");
            }
        }
        }

        /// <summary>
        /// coefficiente di riferimento ka in 1/s
        /// </summary>
        public double ka
        {
            get
            {
                switch (Zona)
                {
                    case Vento.Zone.Zona1:
                        return 0.01;

                    case Vento.Zone.Zona2:
                        return 0.015;

                    case Vento.Zone.Zona3:
                        return 0.02;

                    case Vento.Zone.Zona4:
                        return 0.02;

                    case Vento.Zone.Zona5:
                        return 0.015;

                    case Vento.Zone.Zona6:
                        return 0.02;

                    case Vento.Zone.Zona7:
                        return 0.015;

                    case Vento.Zone.Zona8:
                        return 0.015;

                    case Vento.Zone.Zona9:
                        return 0.02;

                    default:
                        throw new NotImplementedException("Necessario individuare la Zona");
                }
            }
        }

        #endregion

        #region costruttore

        /// <summary>
        /// unico costruttore pubblico
        /// </summary>
        /// <param name="zonaIn"></param>
        public GestioneZonaGeografica(Vento.Zone zonaIn)
        {
                Zona = zonaIn;

        }

        #endregion
    }

    /// <summary>
    /// Classe per la gestione della rugosità del terreno
    /// </summary>
    public class GestioneRugositàTerreno
    {
        /// <summary>
        /// Rugosità
        /// </summary>
        public Vento.ClassiRugosità ClasseRugosità {get; set;}

        /// <summary>
        /// descrizione della rugosità
        /// </summary>
        public string Descrizione
        {
            get
            {
                switch (ClasseRugosità)
                {
                    case Vento.ClassiRugosità.A:
                        return "Aree urbane in cui almeno il 15% della superficie sia coperto da edifici la cui altezza media superi i 15m";

                    case Vento.ClassiRugosità.B:
                        return "Aree urbane (non di classe A), suburbane, industriali e boschive";

                    case Vento.ClassiRugosità.C:
                        return "Aree con ostacoli diffusi (alberi, case, muri, recinzioni,....); aree con rugosità non riconducibile alle classi A, B, D";

                    case Vento.ClassiRugosità.D:
                        return "Aree prive di ostacoli (aperta campagna, aeroporti, aree agricole, pascoli, zone paludose o sabbiose, superfici innevate o ghiacciate, mare, laghi,....)";

                    default:
                        throw new NotImplementedException("La classe di rugosità deve essere impostata");
                }
            }
        }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="rugoIn">Classe di rugosità in</param>
        public GestioneRugositàTerreno(Vento.ClassiRugosità rugoIn)
        {
            ClasseRugosità = rugoIn;
        }
    }

    /// <summary>
    /// Classe per la gestione delle classe di esposizione
    /// </summary>
    public class GestioneCategoriaEsposizione : INotifyPropertyChanged
    {
        private Vento.CategorieEsposizione _CategoriaEsposizione;

        /// <summary>
        /// Classe di Esposizione
        /// </summary>
        public Vento.CategorieEsposizione CategoriaEsposizione
        {
            get
            {
                return _CategoriaEsposizione;
            }
            set
            {
                _CategoriaEsposizione = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("kr"));
                    PropertyChanged(this, new PropertyChangedEventArgs("Z0"));
                    PropertyChanged(this, new PropertyChangedEventArgs("Zmin"));

                }
            }
        }

        /// <summary>
        /// coefficiente
        /// </summary>
        public double kr
        {
            get
            {
                switch (CategoriaEsposizione)
                {
                    case Vento.CategorieEsposizione.I:
                        return 0.17;

                    case Vento.CategorieEsposizione.II:
                        return 0.19;

                    case Vento.CategorieEsposizione.III:
                        return 0.20;

                    case Vento.CategorieEsposizione.IV:
                        return 0.22;

                    case Vento.CategorieEsposizione.V:
                        return 0.23;

                    default:
                        throw new NotImplementedException("La classe di Esposizione deve essere impostata");
                }
            }

        }

        /// <summary>
        /// altezza base
        /// </summary>
        public double Z0
        {
            get
            {
                switch (CategoriaEsposizione)
                {
                    case Vento.CategorieEsposizione.I:
                        return 0.01;

                    case Vento.CategorieEsposizione.II:
                        return 0.05;

                    case Vento.CategorieEsposizione.III:
                        return 0.10;

                    case Vento.CategorieEsposizione.IV:
                        return 0.30;

                    case Vento.CategorieEsposizione.V:
                        return 0.70;

                    default:
                        throw new NotImplementedException("La classe di Esposizione deve essere impostata");
                }
            }
        }

        /// <summary>
        /// Altezza di base
        /// </summary>
        public double Zmin
        {
            get
            {
                switch (CategoriaEsposizione)
                {
                    case Vento.CategorieEsposizione.I:
                        return 2;

                    case Vento.CategorieEsposizione.II:
                        return 4;

                    case Vento.CategorieEsposizione.III:
                        return 5;

                    case Vento.CategorieEsposizione.IV:
                        return 8;

                    case Vento.CategorieEsposizione.V:
                        return 12;

                    default:
                        throw new NotImplementedException("La classe di Esposizione deve essere impostata");
                }
            }
        }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="catIN"></param>
        public GestioneCategoriaEsposizione(Vento.CategorieEsposizione catIN)
        {
            CategoriaEsposizione = catIN;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Vento : INotifyPropertyChanged
    {

        #region enumerazioni
        /// <summary>
        /// Zone di velocità di riferimento
        /// </summary>
        public enum Zone
        {
            /// <summary>
            /// Valle d’Aosta, Piemonte, Lombardia, Trentino Alto Adige, Veneto,Friuli Venezia Giulia (con l’eccezione della provincia di Trieste)
            /// </summary>
            Zona1, 

            /// <summary>
            /// Emilia Romagna
            /// </summary>
            Zona2,

            /// <summary>
            /// Toscana, Marche, Umbria, Lazio, Abruzzo, Molise, Puglia, Campania, Basilicata, Calabria (esclusa la provincia di Reggio Calabria)
            /// </summary>
            Zona3,

            /// <summary>
            /// Sicilia e provincia di Reggio Calabria
            /// </summary>
            Zona4,

            /// <summary>
            /// Sardegna (zona a oriente della retta congiungente Capo Teulada con l’Isola di Maddalena)
            /// </summary>
            Zona5,

            /// <summary>
            /// Sardegna (zona a occidente della retta congiungente Capo Teulada conl’Isola di Maddalena)
            /// </summary>
            Zona6,

            /// <summary>
            /// Liguria
            /// </summary>
            Zona7,

            /// <summary>
            /// Provincia di Trieste
            /// </summary>
            Zona8,

            /// <summary>
            /// Isole (con l’eccezione di Sicilia e Sardegna) e mare aperto
            /// </summary>
            Zona9
        }

        /// <summary>
        /// Classi Rugosità del terreno
        /// </summary>
        public enum ClassiRugosità
        {
            /// <summary>
            /// Aree urbane in cui almeno il 15% della superficie sia coperto da edifici la cui
            ///altezza media superi i 15m
            /// </summary>
            A,

            /// <summary>
            /// Aree urbane (non di classe A), suburbane, industriali e boschive
            /// </summary>
            B,

            /// <summary>
            /// Aree con ostacoli diffusi (alberi, case, muri, recinzioni,....); aree con rugosità non
            /// riconducibile alle classi A, B, D
            /// </summary>
            C,

            /// <summary>
            /// Aree prive di ostacoli (aperta campagna, aeroporti, aree agricole, pascoli, zone
            /// paludose o sabbiose, superfici innevate o ghiacciate, mare, laghi,....)
            /// </summary>
            D
        }

        /// <summary>
        /// Enumerazione delle possibili localizzazioni
        /// </summary>
        public enum DistanzeDaCosta
        {
            /// <summary>
            /// A mare, entro 2 km da costa
            /// </summary>
            Mare2km,

            /// <summary>
            /// A terra, entro i 10 km dalla costa
            /// </summary>
            TerraMenoDi10km,

            /// <summary>
            /// A terra, a più di 10 km e meno di 30 km dalla costa
            /// </summary>
            TerraMenoDi30km,

            /// <summary>
            /// A terra, a più di 30 km dalla costa, sotto i 500 km
            /// </summary>
            TerraHmenoDi500m,

            /// <summary>
            /// A terra, a più di 500 e meno di 750 m di altezza
            /// </summary>
            TerraHmenoDi750m,

            /// <summary>
            /// A terra, più di 500 m, per zona 6
            /// </summary>
            TerraHPiù500m,

            /// <summary>
            /// A terra, a più di 750 m
            /// </summary>
            TerraHpiù750m,

            /// <summary>
            /// A Mare, per zona 9
            /// </summary>
            Mare,

            /// <summary>
            /// A terra per zona 9
            /// </summary>
            Terra,

            /// <summary>
            /// A mare, entro 1500 m
            /// </summary>
            Mare1500m,

            /// <summary>
            /// Mare, entro 500 m
            /// </summary>
            Mare500m
        }

        /// <summary>
        /// Enumerazione delle categorie
        /// </summary>
        public enum CategorieEsposizione
        {
            I, II, III, IV, V
        }

        #endregion

        #region prop.Interne
        private double _AltezzaGeografica = 0;
        private GestioneRugositàTerreno _RugositàTerreno;
        private DistanzeDaCosta _DistanzaDaCosta;
        private GestioneCategoriaEsposizione _ClasseEsposizione {get; set;}
        private GestioneZonaGeografica _ProprietàZona { get; set; }
        private double _AltezzaStruttura;
        

        #endregion

        #region proprietà Pubbliche

        public event PropertyChangedEventHandler PropertyChanged;

        public double PressioneMassima
        {
            get
            {
                return CeMax * PressioneRiferimento;
            }
        }

        /// <summary>
        /// Altezza della struttura
        /// </summary>
        public double AltezzaStruttura
        {
            get { return _AltezzaStruttura; }
            set
            {
                _AltezzaStruttura = value;
                if (PropertyChanged != null)
                {
                    AggiornaProprietà();
                }
            }
        }

        /// <summary>
        /// Coefficiente massimo
        /// </summary>
        public double CeMax
        {
            get
            {
                try
                {
                    if (AltezzaStruttura > ClasseEsposizione.Zmin)
                    {
                        return CoefficienteEsposizione(AltezzaStruttura);
                    }
                    else
                    {
                        return CoefficienteEsposizione(ClasseEsposizione.Zmin);
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Coeff. esposizione minimo
        /// </summary>
        public double CeMin
        {
            get
            {
                try
                {
                    return CoefficienteEsposizione(ClasseEsposizione.Zmin);
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Istanza per la gestione della classe di esposizione
        /// </summary>
        public GestioneCategoriaEsposizione ClasseEsposizione
        { 
            get
            { 
                return _ClasseEsposizione;
            }
            set
            {
                _ClasseEsposizione = value;
                if (PropertyChanged != null)
                {
                    AggiornaProprietà();
                }
            }
        }


        /// <summary>
        /// Istanza per la gestione delle proprietà della Zona
        /// </summary>
        public GestioneZonaGeografica ProprietàZona
        {
            get { return _ProprietàZona; }
            set
            {
                _ProprietàZona = value;
                if (PropertyChanged != null)
                {
                    AggiornaProprietà();
                }
            }
        }


        /// <summary>
        /// Altezza sul livello del mare
        /// </summary>
        public double AltezzaGeografica
        {
            get { return _AltezzaGeografica; }
            set
            {
                _AltezzaGeografica = value;
                if (PropertyChanged != null)
                {
                    AggiornaProprietà();
                }
            }
        }

        /// <summary>
        /// istanza per la gesione delle proprietà della rugosità
        /// </summary>
        public GestioneRugositàTerreno RugositàTerreno
        {
            get { return _RugositàTerreno; }
            set
            {
                _RugositàTerreno = value;
                AssegnaClasse();
                if (PropertyChanged != null)
                {
                    AggiornaProprietà();
                }
            }
        }

        /// <summary>
        /// Distanza dalla costa assegnata
        /// </summary>
        public DistanzeDaCosta DistanzaDaCosta
        {
            get { return _DistanzaDaCosta; }
            set
            {
                _DistanzaDaCosta = value;
                AssegnaClasse();
                if (PropertyChanged != null)
                {
                    AggiornaProprietà();
                }
            }
        }

        /// <summary>
        /// Velocità di riferimento
        /// </summary>
        public double VelocitàRiferimento
        {
            get
            {
                if (ProprietàZona != null)
                {
                    if (AltezzaGeografica < ProprietàZona.a0)
                    {
                        return ProprietàZona.Vb0;
                    }
                    else
                    {
                        return (ProprietàZona.Vb0 + ProprietàZona.ka * (AltezzaGeografica - ProprietàZona.a0));
                    }
                }
                else return 0;
            }
        }

        /// <summary>
        /// pressione cinetica di riferimento [N/mq]
        /// </summary>
        public double PressioneRiferimento
        {
            get
            {
                return 0.5 * 1.25 * Math.Pow(VelocitàRiferimento, 2);
            }
        }

        #endregion

        #region costruttori

        public Vento(Vento.Zone zonaIn)
        {
            
            AltezzaGeografica = 0;
            ProprietàZona = new GestioneZonaGeografica(zonaIn);
            if (PropertyChanged != null)
            {
                AggiornaProprietà();
            }
        }

        public Vento()
        {
            AltezzaGeografica = 0;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("PosizioniPossibili"));
            }
        }

        /// <summary>
        /// Elenco delle posizioni geografiche possibili
        /// </summary>
        public List<DistanzeDaCosta> PosizioniPossibili
        {
            get
            {
                List<DistanzeDaCosta> provvisoria = new List<DistanzeDaCosta>();
                if (ProprietàZona != null)
                {
                    switch (ProprietàZona.Zona)
                    {
                        case Zone.Zona1:
                            provvisoria.Add(DistanzeDaCosta.Mare2km);
                            provvisoria.Add(DistanzeDaCosta.TerraMenoDi10km);
                            provvisoria.Add(DistanzeDaCosta.TerraMenoDi30km);
                            provvisoria.Add(DistanzeDaCosta.TerraHmenoDi500m);
                            provvisoria.Add(DistanzeDaCosta.TerraHmenoDi750m);
                            provvisoria.Add(DistanzeDaCosta.TerraHpiù750m);
                            break;
                        case Zone.Zona2:
                            provvisoria.Add(DistanzeDaCosta.Mare2km);
                            provvisoria.Add(DistanzeDaCosta.TerraMenoDi10km);
                            provvisoria.Add(DistanzeDaCosta.TerraMenoDi30km);
                            provvisoria.Add(DistanzeDaCosta.TerraHmenoDi500m);
                            provvisoria.Add(DistanzeDaCosta.TerraHmenoDi750m);
                            provvisoria.Add(DistanzeDaCosta.TerraHpiù750m);
                            break;
                        case Zone.Zona3:
                            provvisoria.Add(DistanzeDaCosta.Mare2km);
                            provvisoria.Add(DistanzeDaCosta.TerraMenoDi10km);
                            provvisoria.Add(DistanzeDaCosta.TerraMenoDi30km);
                            provvisoria.Add(DistanzeDaCosta.TerraHmenoDi500m);
                            provvisoria.Add(DistanzeDaCosta.TerraHmenoDi750m);
                            provvisoria.Add(DistanzeDaCosta.TerraHpiù750m);
                            break;
                        case Zone.Zona4:
                            provvisoria.Add(DistanzeDaCosta.Mare2km);
                            provvisoria.Add(DistanzeDaCosta.TerraMenoDi10km);
                            provvisoria.Add(DistanzeDaCosta.TerraMenoDi30km);
                            provvisoria.Add(DistanzeDaCosta.TerraHmenoDi500m);
                            provvisoria.Add(DistanzeDaCosta.TerraHmenoDi750m);
                            provvisoria.Add(DistanzeDaCosta.TerraHpiù750m);
                            break;
                        case Zone.Zona5:
                            provvisoria.Add(DistanzeDaCosta.Mare2km);
                            provvisoria.Add(DistanzeDaCosta.TerraMenoDi10km);
                            provvisoria.Add(DistanzeDaCosta.TerraMenoDi30km);
                            provvisoria.Add(DistanzeDaCosta.TerraHmenoDi500m);
                            provvisoria.Add(DistanzeDaCosta.TerraHmenoDi750m);
                            provvisoria.Add(DistanzeDaCosta.TerraHpiù750m);
                            break;
                        case Zone.Zona6:
                            provvisoria.Add(DistanzeDaCosta.Mare2km);
                            provvisoria.Add(DistanzeDaCosta.TerraMenoDi10km);
                            provvisoria.Add(DistanzeDaCosta.TerraMenoDi30km);
                            provvisoria.Add(DistanzeDaCosta.TerraHPiù500m);
                            break;
                        case Zone.Zona7:
                            provvisoria.Add(DistanzeDaCosta.Mare1500m);
                            provvisoria.Add(DistanzeDaCosta.Mare500m);
                            provvisoria.Add(DistanzeDaCosta.Terra);
                            break;
                        case Zone.Zona8:
                            provvisoria.Add(DistanzeDaCosta.Mare1500m);
                            provvisoria.Add(DistanzeDaCosta.Mare500m);
                            provvisoria.Add(DistanzeDaCosta.Terra);
                            break;
                        case Zone.Zona9:
                            provvisoria.Add(DistanzeDaCosta.Mare);
                            provvisoria.Add(DistanzeDaCosta.Terra);
                            break;
                        default:
                            throw new NotImplementedException("la zona deve essere definita");


                    }
                }
                return provvisoria;
            }
        }

        /// <summary>
        /// elenco delle classi di rugosità possibili
        /// </summary>
        public List<ClassiRugosità> RugositàPossibili
        {
            get
            {
                List<ClassiRugosità> provvisoria = new List<ClassiRugosità>();
                if (ProprietàZona != null)
                {

                    switch (ProprietàZona.Zona)
                    {
                        #region zona 1
                        case Zone.Zona1:
                            switch (DistanzaDaCosta)
                            {
                                case DistanzeDaCosta.Mare2km:
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraMenoDi10km:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraMenoDi30km:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraHmenoDi500m:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraHmenoDi750m:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraHpiù750m:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                default:
                                    throw new NotSupportedException("Mancata compatibilità tra zona e distanze da costa");
                            }
                            break;
                        #endregion

                        #region zona 2
                        case Zone.Zona2:
                            switch (DistanzaDaCosta)
                            {
                                case DistanzeDaCosta.Mare2km:
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraMenoDi10km:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraMenoDi30km:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraHmenoDi500m:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraHmenoDi750m:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraHpiù750m:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                default:
                                    throw new NotSupportedException("Mancata compatibilità tra zona e distanze da costa");
                            }
                            break;

                        #endregion

                        #region zona 3
                        case Zone.Zona3:
                            switch (DistanzaDaCosta)
                            {
                                case DistanzeDaCosta.Mare2km:
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraMenoDi10km:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraMenoDi30km:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraHmenoDi500m:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraHmenoDi750m:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraHpiù750m:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                default:
                                    throw new NotSupportedException("Mancata compatibilità tra zona e distanze da costa");
                            }
                            break;

                        #endregion

                        #region zona 4
                        case Zone.Zona4:
                            switch (DistanzaDaCosta)
                            {
                                case DistanzeDaCosta.Mare2km:
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraMenoDi10km:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraMenoDi30km:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraHmenoDi500m:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraHmenoDi750m:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraHpiù750m:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                default:
                                    throw new NotSupportedException("Mancata compatibilità tra zona e distanze da costa");
                            }
                            break;


                        #endregion

                        #region zona 5
                        case Zone.Zona5:
                            switch (DistanzaDaCosta)
                            {
                                case DistanzeDaCosta.Mare2km:
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraMenoDi10km:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraMenoDi30km:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraHmenoDi500m:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraHmenoDi750m:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraHpiù750m:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                default:
                                    throw new NotSupportedException("Mancata compatibilità tra zona e distanze da costa");
                            }
                            break;

                        #endregion

                        #region zona 6
                        case Zone.Zona6:
                            switch (DistanzaDaCosta)
                            {
                                case DistanzeDaCosta.Mare2km:
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraMenoDi10km:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraMenoDi30km:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraHmenoDi500m:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.TerraHPiù500m:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                default:
                                    throw new NotSupportedException("Mancata compatibilità tra zona e distanze da costa");
                            }
                            break;
                        #endregion

                        #region zona 7
                        case Zone.Zona7:
                            switch (DistanzaDaCosta)
                            {
                                case DistanzeDaCosta.Mare1500m:
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.Mare500m:
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.Terra:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                default:
                                    throw new NotSupportedException("Mancata compatibilità tra zona e distanze da costa");
                            }
                            break;

                        #endregion

                        #region zona 8
                        case Zone.Zona8:
                            switch (DistanzaDaCosta)
                            {
                                case DistanzeDaCosta.Mare1500m:
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.Mare500m:
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.Terra:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                default:
                                    throw new NotSupportedException("Mancata compatibilità tra zona e distanze da costa");
                            }
                            break;

                        #endregion

                        #region zona 9
                        case Zone.Zona9:
                            switch (DistanzaDaCosta)
                            {
                                case DistanzeDaCosta.Mare:
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                case DistanzeDaCosta.Terra:
                                    provvisoria.Add(ClassiRugosità.A);
                                    provvisoria.Add(ClassiRugosità.B);
                                    provvisoria.Add(ClassiRugosità.C);
                                    provvisoria.Add(ClassiRugosità.D);
                                    break;

                                default:
                                    throw new NotSupportedException("Mancata compatibilità tra zona e distanze da costa");
                            }
                            break;

                        #endregion
                        default:
                            throw new NotImplementedException("La Zona deve essere definita");
                    }
                }
                return provvisoria;
            }
        }

        #endregion

        #region metodi

        /// <summary>
        /// metodo statico che restituisce la Categoria di esposizione in funzione delle proprietà in ingresso
        /// </summary>
        /// <param name="zonaIn"></param>
        /// <param name="rugIn"></param>
        /// <param name="distIn"></param>
        /// <returns></returns>
        public static GestioneCategoriaEsposizione CategoriaEsposizione(Zone zonaIn, ClassiRugosità rugIn, DistanzeDaCosta distIn)
        {
            switch (zonaIn)
            {
                #region zona 1

                case Zone.Zona1:
                    switch (rugIn)
                    {
                        case ClassiRugosità.A:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.TerraMenoDi10km:
                                case DistanzeDaCosta.TerraMenoDi30km:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.IV);

                                case DistanzeDaCosta.TerraHmenoDi500m:
                                case DistanzeDaCosta.TerraHmenoDi750m:
                                case DistanzeDaCosta.TerraHpiù750m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.V);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        case ClassiRugosità.B:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.TerraMenoDi10km:
                                case DistanzeDaCosta.TerraMenoDi30km:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.III);

                                case DistanzeDaCosta.TerraHmenoDi500m:
                                case DistanzeDaCosta.TerraHmenoDi750m:
                                case DistanzeDaCosta.TerraHpiù750m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.IV);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        case ClassiRugosità.C:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.TerraMenoDi10km:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.II);

                                case DistanzeDaCosta.TerraMenoDi30km:
                                case DistanzeDaCosta.TerraHmenoDi500m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.III);

                                case DistanzeDaCosta.TerraHmenoDi750m:
                                case DistanzeDaCosta.TerraHpiù750m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.IV);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        case ClassiRugosità.D:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.Mare2km:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.I);

                                case DistanzeDaCosta.TerraMenoDi10km:
                                case DistanzeDaCosta.TerraMenoDi30km:
                                case DistanzeDaCosta.TerraHmenoDi500m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.II);

                                case DistanzeDaCosta.TerraHmenoDi750m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.III);

                                case DistanzeDaCosta.TerraHpiù750m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.IV);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        default:
                            return null;
                            throw new NotSupportedException("Non previsto");
                    }

                #endregion

                #region zone 2,3,4

                case Zone.Zona2:
                case Zone.Zona3:
                case Zone.Zona4:
                    switch (rugIn)
                    {
                        case ClassiRugosità.A:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.TerraMenoDi10km:
                                case DistanzeDaCosta.TerraMenoDi30km:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.IV);

                                case DistanzeDaCosta.TerraHmenoDi500m:
                                case DistanzeDaCosta.TerraHmenoDi750m:
                                case DistanzeDaCosta.TerraHpiù750m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.V);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        case ClassiRugosità.B:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.TerraMenoDi10km:
                                case DistanzeDaCosta.TerraMenoDi30km:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.III);

                                case DistanzeDaCosta.TerraHmenoDi500m:
                                case DistanzeDaCosta.TerraHmenoDi750m:
                                case DistanzeDaCosta.TerraHpiù750m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.IV);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        case ClassiRugosità.C:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.TerraMenoDi10km:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.II);

                                case DistanzeDaCosta.TerraMenoDi30km:
                                case DistanzeDaCosta.TerraHmenoDi500m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.III);

                                case DistanzeDaCosta.TerraHmenoDi750m:
                                case DistanzeDaCosta.TerraHpiù750m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.IV);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        case ClassiRugosità.D:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.Mare2km:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.I);

                                case DistanzeDaCosta.TerraMenoDi10km:
                                case DistanzeDaCosta.TerraMenoDi30km:
                                case DistanzeDaCosta.TerraHmenoDi500m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.II);

                                case DistanzeDaCosta.TerraHmenoDi750m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.III);

                                case DistanzeDaCosta.TerraHpiù750m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.III);
                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        default:
                            return null;
                            throw new NotSupportedException("Non previsto");
                    }

                #endregion

                #region zona 5
                case Zone.Zona5:
                    switch (rugIn)
                    {
                        case ClassiRugosità.A:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.TerraMenoDi10km:
                                case DistanzeDaCosta.TerraMenoDi30km:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.IV);

                                case DistanzeDaCosta.TerraHmenoDi500m:
                                case DistanzeDaCosta.TerraHmenoDi750m:
                                case DistanzeDaCosta.TerraHpiù750m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.V);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        case ClassiRugosità.B:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.TerraMenoDi10km:
                                case DistanzeDaCosta.TerraMenoDi30km:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.III);

                                case DistanzeDaCosta.TerraHmenoDi500m:
                                case DistanzeDaCosta.TerraHmenoDi750m:
                                case DistanzeDaCosta.TerraHpiù750m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.IV);


                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        case ClassiRugosità.C:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.TerraMenoDi10km:
                                case DistanzeDaCosta.TerraMenoDi30km:
                                case DistanzeDaCosta.TerraHmenoDi500m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.III);

                                case DistanzeDaCosta.TerraHmenoDi750m:
                                case DistanzeDaCosta.TerraHpiù750m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.IV);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        case ClassiRugosità.D:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.Mare2km:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.I);

                                case DistanzeDaCosta.TerraMenoDi10km:
                                case DistanzeDaCosta.TerraMenoDi30km:
                                case DistanzeDaCosta.TerraHmenoDi500m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.II);

                                case DistanzeDaCosta.TerraHmenoDi750m:
                                case DistanzeDaCosta.TerraHpiù750m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.III);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        default:
                            return null;
                            throw new NotSupportedException("Non previsto");
                    }

                #endregion

                #region zona 6
                case Zone.Zona6:
                    switch (rugIn)
                    {
                        case ClassiRugosità.A:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.TerraMenoDi10km:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.III);

                                case DistanzeDaCosta.TerraMenoDi30km:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.IV);

                                case DistanzeDaCosta.TerraHmenoDi500m:
                                case DistanzeDaCosta.TerraHPiù500m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.V);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        case ClassiRugosità.B:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.TerraMenoDi10km:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.II);

                                case DistanzeDaCosta.TerraMenoDi30km:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.III);

                                case DistanzeDaCosta.TerraHmenoDi500m:
                                case DistanzeDaCosta.TerraHPiù500m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.IV);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        case ClassiRugosità.C:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.TerraMenoDi10km:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.II);

                                case DistanzeDaCosta.TerraMenoDi30km:
                                case DistanzeDaCosta.TerraHmenoDi500m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.III);

                                case DistanzeDaCosta.TerraHPiù500m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.IV);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        case ClassiRugosità.D:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.Mare2km:
                                case DistanzeDaCosta.TerraMenoDi10km:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.I);

                                case DistanzeDaCosta.TerraMenoDi30km:
                                case DistanzeDaCosta.TerraHmenoDi500m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.II);

                                case DistanzeDaCosta.TerraHPiù500m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.III);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        default:
                            return null;
                            throw new NotSupportedException("Non previsto");
                    }
                #endregion

                #region zone 7

                case Zone.Zona7:
                    switch (rugIn)
                    {
                        case ClassiRugosità.A:
                        case ClassiRugosità.B:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.Terra:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.IV);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        case ClassiRugosità.C:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.Terra:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.III);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        case ClassiRugosità.D:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.Mare1500m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.I);

                                case DistanzeDaCosta.Mare500m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.II);

                                case DistanzeDaCosta.Terra:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.III);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        default:
                            return null;
                            throw new NotSupportedException("Non previsto");
                    }

                #endregion

                #region zona 8

                case Zone.Zona8:
                    switch (rugIn)
                    {
                        case ClassiRugosità.A:
                        case ClassiRugosità.B:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.Terra:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.IV);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        case ClassiRugosità.C:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.Terra:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.III);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        case ClassiRugosità.D:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.Mare1500m:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.I);

                                case DistanzeDaCosta.Mare500m:
                                case DistanzeDaCosta.Terra:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.II);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        default:
                            return null;
                            throw new NotSupportedException("Non previsto");
                    }


                #endregion


                #region zona 9
                case Zone.Zona9:
                    switch (rugIn)
                    {
                        case ClassiRugosità.A:
                        case ClassiRugosità.B:
                        case ClassiRugosità.C:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.Terra:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.I);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        case ClassiRugosità.D:
                            switch (distIn)
                            {
                                case DistanzeDaCosta.Mare:
                                case DistanzeDaCosta.Terra:
                                    return new GestioneCategoriaEsposizione(CategorieEsposizione.II);

                                default:
                                    return null;
                                    throw new NotSupportedException("Non previsto");
                            }

                        default:
                            return null;
                            throw new NotSupportedException("Non previsto");
                    }

                #endregion

                default:
                    return null;
                    throw new NotSupportedException("Non previsto");
            }
        }

        public double CoefficienteEsposizione(double z)
        {
            double prov;

            prov = Math.Pow(ClasseEsposizione.kr, 2);

            prov *= Math.Log(z / ClasseEsposizione.Z0) * (7 + Math.Log(z / ClasseEsposizione.Z0));

            return prov;

        }

        /// <summary>
        /// metodo che assegna la classe di esposizione
        /// </summary>
        private void AssegnaClasse()
        {
            if ((ProprietàZona != null) && (RugositàTerreno != null))
            {
                try
                {

                    ClasseEsposizione = CategoriaEsposizione(ProprietàZona.Zona, RugositàTerreno.ClasseRugosità, DistanzaDaCosta);
                }
                catch
                {
                    ClasseEsposizione = null;
                }
            }
        }

        private void AggiornaProprietà()
        {

            PropertyChanged(this, new PropertyChangedEventArgs("ProprietàZona"));
            PropertyChanged(this, new PropertyChangedEventArgs("RugositàPossibili"));
            PropertyChanged(this, new PropertyChangedEventArgs("PosizioniPossibili"));
            PropertyChanged(this, new PropertyChangedEventArgs("ClasseEsposizione"));
            PropertyChanged(this, new PropertyChangedEventArgs("CeMin"));
            PropertyChanged(this, new PropertyChangedEventArgs("CeMax"));
            PropertyChanged(this, new PropertyChangedEventArgs("AltezzaGeoGrafica"));
            PropertyChanged(this, new PropertyChangedEventArgs("VelocitàRiferimento"));
            PropertyChanged(this, new PropertyChangedEventArgs("PressioneRiferimento"));
            PropertyChanged(this, new PropertyChangedEventArgs("PressioneMassima"));

        }

        

        #endregion

    }
}
