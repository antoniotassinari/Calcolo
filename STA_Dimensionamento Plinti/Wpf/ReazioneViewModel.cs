using System;
using System.Collections.Generic;
using System.Globalization;

namespace STA_Dimensionamento_Plinti.Wpf
{
    public sealed class ReazioneViewModel : ObservableObject
    {
        private string nome;
        private string fx = "0", fy = "0", fz = "0", mx = "0", my = "0", mz = "0";
        public string Nome { get => nome; set => Set(ref nome, value ?? ""); }
        public string FX { get => fx; set => Set(ref fx, value ?? ""); }
        public string FY { get => fy; set => Set(ref fy, value ?? ""); }
        public string FZ { get => fz; set => Set(ref fz, value ?? ""); }
        public string MX { get => mx; set => Set(ref mx, value ?? ""); }
        public string MY { get => my; set => Set(ref my, value ?? ""); }
        public string MZ { get => mz; set => Set(ref mz, value ?? ""); }
        public ReazioneViewModel(string name) { nome = name ?? throw new ArgumentNullException(nameof(name)); }
        public ReazioneViewModel(Reazione model) : this(model?.Combinazione ?? throw new ArgumentNullException(nameof(model)))
        {
            FX = Format(model.FX); FY = Format(model.FY); FZ = Format(model.FZ);
            MX = Format(model.MX); MY = Format(model.MY); MZ = Format(model.MZ);
        }
        private static string Format(double value) => value.ToString("R", CultureInfo.CurrentCulture);
        private double Read(string value, string label)
        {
            if (!double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out double number) || !double.IsFinite(number))
                throw new InputException(Nome + " — " + label + ": inserire un numero finito.");
            return number;
        }
        public Reazione Build()
        {
            if (string.IsNullOrWhiteSpace(Nome)) throw new InputException("Indicare il nome della combinazione.");
            return new Reazione(Nome.Trim(), Read(FX, "FX"), Read(FY, "FY"), Read(FZ, "FZ"),
                Read(MX, "MX"), Read(MY, "MY"), Read(MZ, "MZ"));
        }
    }

    public sealed class PlintoItem
    {
        public IPlinto Model { get; }
        public string Descrizione => Model.CaratteristichePlinto();
        public PlintoItem(IPlinto model) { Model = model ?? throw new ArgumentNullException(nameof(model)); }
        public override string ToString() => Descrizione;
    }

    public sealed class NodoItem : ObservableObject
    {
        private bool selezionato;
        private PlintoItem assegnato;
        public Nodo Model { get; }
        public string Nome => Model.Nome;
        public IReadOnlyList<Reazione> Reazioni => Model.Reazioni;
        public bool Selezionato { get => selezionato; set => Set(ref selezionato, value); }
        public PlintoItem Assegnato
        {
            get => assegnato;
            set { if (Set(ref assegnato, value)) Raise(nameof(Assegnazione)); }
        }
        public string Assegnazione => Assegnato?.Descrizione ?? "Automatico: tutti i plinti";
        public NodoItem(Nodo model) { Model = model ?? throw new ArgumentNullException(nameof(model)); }
    }
}
