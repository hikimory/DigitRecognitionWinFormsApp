using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DigitRec.Utils
{
    public class CryptoRandom
    {
        private Random random;
        public double RandomValue { get; set; }

        public CryptoRandom()
        {
            using (RNGCryptoServiceProvider p = new RNGCryptoServiceProvider())
            {
                random = new Random(p.GetHashCode());
            }
        }

        public double NextDouble()
        {
            return random.NextDouble();
        }

    }
}
