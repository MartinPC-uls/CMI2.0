using CMI.Network;
using static CMI.MathUtils;

namespace CMI2._0.Network
{
    public class InputGate
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
