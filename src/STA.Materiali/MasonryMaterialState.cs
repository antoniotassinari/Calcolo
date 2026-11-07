namespace STA.Materiali;

/// <summary>Proprietà della muratura indipendenti dalla geometria del maschio. Resistenze N/cm², moduli MPa, peso daN/m³.</summary>
public sealed record MasonryMaterialState
{
    public string MaterialId {get;set;}="mattoni-calce";
    public string Type {get;set;}="Mattoni pieni e malta di calce";
    public string Lc {get;set;}="LC1";
    public double FmBase {get;set;}=260;
    public double TauBase {get;set;}=5;
    public double E {get;set;}=1500;
    public double G {get;set;}=500;
    public double? Rho {get;set;}=1800;
    public bool Cracked {get;set;}
    public double[] C {get;set;}=[1,1,1,1,1,1,1];
}
