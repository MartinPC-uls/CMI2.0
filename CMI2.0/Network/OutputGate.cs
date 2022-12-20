using static CMI.MathUtils;
using CMI.Network;

namespace CMI2._0.Network
{
    public class OutputGate
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