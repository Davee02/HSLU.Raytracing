namespace Common;
public class RenderSettings(int lineSkipStep = 1, int maxRecursionDepth = 3)
{
    public int LineSkipStep { get; } = lineSkipStep;

    public int MaxRecursionDepth { get; } = maxRecursionDepth;
}
