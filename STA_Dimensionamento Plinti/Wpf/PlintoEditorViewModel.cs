using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using STA_Dimensionamento_Plinti.pali;

namespace STA_Dimensionamento_Plinti.Wpf
{
    public sealed class PlintoEditorViewModel : ObservableObject, IStartupCheck
    {
        public const string Superficiale = "Superficiale";
        public const string Rettangolare = "Su pali rettangolare";
        public const string Triangolare = "Su pali triangolare";
        private readonly Dictionary<string, NumberField> fields;
        private string tipo = Superficiale;
        private bool consideraTerreno;
        public IReadOnlyList<string> Tipi { get; } = new[] { Superficiale, Rettangolare, Triangolare };
        public ObservableCollection<NumberField> Fields { get; } = new ObservableCollection<NumberField>();
        public event EventHandler Changed;
        public string Tipo
        {
            get => tipo;
            set
            {
                if (!Tipi.Contains(value)) throw new ArgumentOutOfRangeException(nameof(value));
                if (!Set(ref tipo, value)) return;
                RefreshFields();
                Raise(nameof(IsSuperficiale));
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
        public bool IsSuperficiale => Tipo == Superficiale;
        public bool ConsideraTerreno
        {
            get => consideraTerreno;
            set
            {
                if (!Set(ref consideraTerreno, value)) return;
                RefreshFields();
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
        public PlintoEditorViewModel()
        {
            fields = new[]
            {
                new NumberField("A", "A [cm]"), new NumberField("B", "B [cm]"),
                new NumberField("H", "Spessore [cm]"), new NumberField("Q", "Quota di imposta [cm]"),
                new NumberField("Limite", "Pressione limite [daN/cm²]"),
                new NumberField("EccX", "Eccentricità X [cm]", "0"),
                new NumberField("EccY", "Eccentricità Y [cm]", "0"),
                new NumberField("Gamma", "Peso specifico terreno [daN/m³]"),
                new NumberField("Diametro", "Diametro pali [cm]"),
                new NumberField("Portata", "Portata limite palo [kN]"),
                new NumberField("Ordini", "Ordini pali"), new NumberField("Colonne", "Colonne pali")
            }.ToDictionary(f => f.Key);
            foreach (NumberField field in fields.Values) field.PropertyChanged += FieldChanged;
            RefreshFields();
            VerifyInitialization();
        }
        private void FieldChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NumberField.Text)) Changed?.Invoke(this, EventArgs.Empty);
        }
        private void RefreshFields()
        {
            Fields.Clear();
            string[] keys = IsSuperficiale
                ? new[] { "A", "B", "H", "Q", "Limite", "EccX", "EccY" }
                : Tipo == Rettangolare
                    ? new[] { "Diametro", "Portata", "Ordini", "Colonne", "H" }
                    : new[] { "Diametro", "Portata", "H" };
            foreach (string key in keys) Fields.Add(fields[key]);
            if (IsSuperficiale && ConsideraTerreno) Fields.Add(fields["Gamma"]);
        }
        public IPlinto Build()
        {
            VerifyInitialization();
            double h = fields["H"].Read(positive: true);
            if (IsSuperficiale)
            {
                var plinto = new Plinto(TipoPlinto.Superficiale,
                    fields["A"].Read(positive: true), fields["B"].Read(positive: true), h,
                    fields["Q"].Read(), fields["Limite"].Read(positive: true));
                plinto.assegnaPosizione(fields["EccX"].Read(), fields["EccY"].Read());
                if (ConsideraTerreno) plinto.assegnaTerreno(fields["Gamma"].Read(positive: true));
                else plinto.nonConsiderareTerreno();
                CalculationService.ValidatePlinto(plinto);
                return plinto;
            }
            var palo = new Palo(fields["Diametro"].Read(positive: true), fields["Portata"].Read(positive: true));
            var suPali = new PlintoSuPali();
            if (Tipo == Rettangolare)
                suPali.generaPlintoRettangolare(palo, fields["Ordini"].ReadCount(), fields["Colonne"].ReadCount(), h);
            else
                suPali.generaPlintoTriangolare(palo, h);
            CalculationService.ValidatePlinto(suPali);
            return suPali;
        }
        public void Load(IPlinto model)
        {
            ArgumentNullException.ThrowIfNull(model);
            if (model is Plinto p)
            {
                Tipo = Superficiale;
                fields["A"].Load(p.A); fields["B"].Load(p.B); fields["H"].Load(p.H);
                fields["Q"].Load(p.Imposta); fields["Limite"].Load(p.ValoreLimite);
                fields["EccX"].Load(p.EccentricitaPilastro.X); fields["EccY"].Load(p.EccentricitaPilastro.Y);
                fields["Gamma"].Load(p.PesoSpecTerreno);
                ConsideraTerreno = p.ConsideraTerrenoSopra;
            }
            else if (model is PlintoSuPali pp)
            {
                CalculationService.ValidatePlinto(pp);
                Tipo = pp.TipoSuPali == TipoligiePlintiPali.Triangolare ? Triangolare : Rettangolare;
                fields["Diametro"].Load(pp.Pali[0].Diametro); fields["Portata"].Load(pp.ValoreLimite);
                fields["Ordini"].Load(pp.Ordini); fields["Colonne"].Load(pp.Colonne); fields["H"].Load(pp.H);
            }
            else throw new InputException("Tipo di plinto non supportato.");
        }
        public void VerifyInitialization()
        {
            if (fields == null || fields.Count != 12 || fields.Values.Any(f => f == null) || Fields.Count == 0)
                throw new InvalidOperationException("Il modello di inserimento del plinto non è inizializzato.");
        }
    }
}
