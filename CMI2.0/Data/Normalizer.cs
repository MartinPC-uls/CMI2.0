using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CMI.MathUtils;

namespace CMI.Data
{
    public class Normalizer
    {
        private static readonly float MIN = 32;
        private static readonly float MAX = 122;

        public static float[] Normalize(char[] x)
        {
            List<float> valoresNormalizados = new();
            
            foreach (char c in x)
                valoresNormalizados.Add(Normalize(c));
            return valoresNormalizados.ToArray();
        }

        public static float Normalize(char x) => (x - MIN) / (MAX - MIN);

        public static char Denormalize(float output) => (char) Math.Round(output* (MAX - MIN) + MIN, 0);
    }
}
