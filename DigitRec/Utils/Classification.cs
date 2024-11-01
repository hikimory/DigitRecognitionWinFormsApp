using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRec.Utils
{
    public class Classification
    {
        public double[] target { get; private set; }
        public string targetLabel { get; private set; }

        public Classification(string label, double[] val)
        {
            target = val;
            targetLabel = label;
        }
    }
}
