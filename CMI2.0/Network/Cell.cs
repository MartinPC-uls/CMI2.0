using CMI2._0.Network;
using static CMI.MathUtils;

namespace CMI.Network
{
    public class Cell
    {
        #region Valores actuales
        public float input { get; set; }
        public float cellState { get; set; }
        public float hiddenState { get; set; }

        #endregion Valores actuales

        #region Valores celda anterior
        public float previousHiddenState { get; set; }
        public float previousCellState { get; set; }

        #endregion Valores celda anterior

        #region Valores compuertas
        public float inputNodeValue { get; set; }
        public float inputGateValue { get; set; }
        public float forgetGateValue { get; set; }
        public float outputGateValue { get; set; }
        #endregion Valores compuertas

        #region Derivadas
        public float hiddenStateDerivative { get; set; }
        public float previousHiddenStateDerivative { get; set; }
        public float cellStateDerivative { get; set; }
        public float inputGateDerivative { get; set; }
        public float inputNodeDerivative { get; set; }
        public float forgetGateDerivative { get; set; }
        public float outputGateDerivative { get; set; }
        public float lossDerivative { get; set; }
        //public float dx { get; set; }
        #endregion Derivadas

        public Cell(float input, float previousCellState, float previousHiddenState) : base()
        {
            this.input = input;
            this.previousCellState = previousCellState;
            this.previousHiddenState = previousHiddenState;
        }

        public void Forward()
        {
            CalculateForgetGate();
            ForgetCellState();
            CalculateUpdateGate();
            UpdateCellState();
            CalculateOutputGate();
            CalculateHiddenState();
        }

        private void CalculateHiddenState() => hiddenState = Tanh(cellState) * outputGateValue;

        private void UpdateCellState() => cellState += inputNodeValue * inputGateValue;

        private void ForgetCellState() => cellState = forgetGateValue * previousCellState;

        private void CalculateForgetGate() => forgetGateValue = ForgetGate.Forward(input, previousHiddenState);

        private void CalculateOutputGate() => outputGateValue = OutputGate.Forward(input, previousHiddenState);

        private void CalculateUpdateGate()
        {
            inputNodeValue = InputNode.Forward(input, previousHiddenState);
            inputGateValue = InputGate.Forward(input, previousHiddenState);
        }

        public void Backpropagation(float target,
                                    float nextHiddenStateDerivativeValue,
                                    float nextCellStateDerivativeValue,
                                    float nextForgetGateValue)
        {
            lossDerivative = hiddenState - target;
            hiddenStateDerivative = lossDerivative + nextHiddenStateDerivativeValue;
            cellStateDerivative = hiddenStateDerivative * outputGateValue * (1 - Tanh2(cellState)) + nextCellStateDerivativeValue * nextForgetGateValue;
            CalculateGatesDerivatives();
            CalculateHiddenStateDerivative();
        }

        private void CalculateGatesDerivatives()
        {
            inputNodeDerivative = cellStateDerivative * inputGateValue * (1 - (float)Math.Pow(inputNodeValue, 2));
            inputGateDerivative = cellStateDerivative * inputNodeValue * inputGateValue * (1 - inputGateValue);
            forgetGateDerivative = cellStateDerivative * previousCellState * forgetGateValue * (1 - forgetGateValue);
            outputGateDerivative = hiddenStateDerivative * Tanh(cellState) * outputGateValue * (1 - outputGateValue);
        }

        private void CalculateHiddenStateDerivative()
        {
            //dx = Wa * da + Wi * di + Wf * df + Wo * do_; // it's never used
            float ForgetGateFactor = ForgetGate._hiddenStateWeight * forgetGateDerivative;
            float InputNodeFactor = InputNode._hiddenStateWeight * inputNodeDerivative;
            float InputGateFactor = InputGate._hiddenStateWeight * inputGateDerivative;
            float OutputFactor = OutputGate._hiddenStateWeight * outputGateDerivative;
            previousHiddenStateDerivative = InputNodeFactor + InputGateFactor + ForgetGateFactor + OutputFactor;
        }
    }
}
