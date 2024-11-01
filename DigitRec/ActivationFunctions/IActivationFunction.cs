using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRec.ActivationFunctions
{
    public interface IActivationFunction
    {
        double CalculateOutput(double input);
        double CalculateDerivative(double input);
    }
}
