using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CMI.Utils;

namespace CMI.Data
{
    public class Normalizer
    {
        private List<char[]> Dataset;
        public static List<int[]>? NormalizedDataset;
        public static List<char[]>? DenormalizedDataset;

        private static readonly float MIN = 32;
        private static readonly float MAX = 122;

        private static readonly float MODIFIER = 1000.0f;

        public Normalizer(List<char[]> Dataset)
        {
            this.Dataset = Dataset;
            NormalizedDataset = new();
        }

        public static float[] Normalize(char[] x)
        {
            float limIzquierdo = 32;
            float limDerecho, centro = 122;
            List<float> valoresNormalizados = new List<float>();
            foreach (char c in x)
            {
                //print((c - limIzquierdo) / (centro - limIzquierdo));
                valoresNormalizados.Add((c - limIzquierdo) / (centro - limIzquierdo));
            }
            return valoresNormalizados.ToArray();
        }

        public static float Normalize(char x)
        {
            float limIzquierdo = 32;
            float limDerecho, centro = 122;
            return (x - limIzquierdo) / (centro - limIzquierdo);
        }

        public static char Denormalize(float output)
        {
            float valor = output * (122 - 32) + 32;
            return (char)Math.Round(valor, 0);
        }
    }
}
