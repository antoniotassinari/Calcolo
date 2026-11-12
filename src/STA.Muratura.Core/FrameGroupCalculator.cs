namespace STA.Muratura.Core;

public sealed record FrameGroupResult(FrameResult?[] Frames, double K, double Veff, double FinalK, double FinalV);

public static class FrameGroupCalculator
{
    // Each frame is mobilized at the same available masonry displacement.
    // Sum only the frame contributions: masonry must be counted once.
    public static FrameGroupResult Calculate(IEnumerable<FrameInput> frames, StateResult before, StateResult after)
    {
        var results = frames.Select(f => FrameCalculator.Calculate(f ?? throw new ArgumentException("Cerchiatura nulla."), before, after)).ToArray();
        double k = results.Sum(r => r?.K ?? 0), v = results.Sum(r => r?.Veff ?? 0);
        if (!double.IsFinite(k) || !double.IsFinite(v)) throw new ArgumentException("Somma cerchiature fuori intervallo.");
        return new(results, k, v, after.K + k, after.Vt + v);
    }

    public static StateResult IncludeOthers(IEnumerable<FrameInput> others, StateResult before, StateResult after)
    {
        var total = Calculate(others, before, after);
        return after with { K = total.FinalK, Vt = total.FinalV };
    }
}
