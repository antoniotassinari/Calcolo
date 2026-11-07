using STA.Profilati.Core;
namespace STA.Muratura.Core;
public sealed record FrameResult(SteelProfile Profile, double J, double W, double V, double K, double Dy, double Veff, double FinalV, double FinalK, double DV, double DK, double? Wstar, double AvailableDu);
