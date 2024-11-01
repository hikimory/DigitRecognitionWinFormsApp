using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRec.ActivationFunctions
{
    public class TanhActivationFunction : IActivationFunction
    {
        public double CalculateDerivative(double input)
        {
            return 1 - (input * input);
        }

        public double CalculateOutput(double input)
        {
            return Math.Tanh(input);
        }
    }
}
