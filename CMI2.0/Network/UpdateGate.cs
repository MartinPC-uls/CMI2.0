using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMI2._0.Network
{
    public class UpdateGate
    {
        public InputNode InputNode;
        public InputGate InputGate;
        public float Forward(float input, float hiddenState)
        {
            return InputNode.Forward(input, hiddenState) * InputGate.Forward(input, hiddenState);
        }

    }
}
