using CMI.Network;
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
        public float _inputWeight;
        public float _hiddenStateWeight;
        public float _bias;
        public float weightGradient;
        public float hiddenStateWeightGradient;
        public float biasGradient;

        public float Forward(float input, float hiddenState)
        {
            return Sigmoid(input * _inputWeight + hiddenState * _hiddenStateWeight + _bias);
        }

    }
}
