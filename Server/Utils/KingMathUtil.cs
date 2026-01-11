using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Utils;

namespace KingOfTarkov.Utils;

[Injectable(InjectionType.Singleton)]
public class KingMathUtil(MathUtil mathUtil)
{
    //map to range but minOut can be higher than maxOut
    public double MapToRangeInv(double x, double minIn, double maxIn, double minOut, double maxOut)
    {
        bool inv = minOut > maxOut;

        if (inv)
        {
            (minOut, maxOut) = (maxOut, minOut);
        }

        double result = mathUtil.MapToRange(x, minIn, maxIn, minOut, maxOut);

        if (inv)
            result = maxOut - result;
        
        return result;
    }
}