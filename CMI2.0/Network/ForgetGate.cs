using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CMI.MathUtils;
namespace CMI2._0.Network
{

    public class ForgetGate
    {
        public static float _inputWeight;
        public static float _hiddenStateWeight;
        public static float _bias;
        public static float weightGradient;
        public static float hiddenStateWeightGradient;
        public static float biasGradient;

        public static float Forward(float input, float hiddenState)
        {
            return Sigmoid(input * _inputWeight + hiddenState * _hiddenStateWeight + _bias);
        }

    }
}
