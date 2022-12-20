using CMI.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMI2._0.Network
{
    public class AdamOptimizer
    {
        private float InputNodeWeightMomentum=0, InputGateWeightMomentum=0, ForgetGateWeightMomentum=0, OutputGateWeightMomentum=0;
        private float vdUa = 0, vdUi = 0, vdUf = 0, vdUo = 0;
        private float vdba = 0, vdbi = 0, vdbf = 0, vdbo = 0;

        private float sdWa = 0, sdWi = 0, sdWf = 0, sdWo = 0;
        private float sdUa = 0, sdUi = 0, sdUf = 0, sdUo = 0;
        private float sdba = 0, sdbi = 0, sdbf = 0, sdbo = 0;

        private ForgetGate _forgetGate;
        private InputNode _inputNode;
        private InputGate _inputGate;
        private OutputGate _outputGate;

        public void Optimize(float learningRate,
                             List<Cell> cells,
                             ForgetGate forgetGate,
                             InputNode inputNode,
                             InputGate inputGate,
                             OutputGate outputGate,
                             float beta1 = 0.9f,
                             float beta2 = 0.999f,
                             float epsilon = 1e-8f)
        {

            _forgetGate = forgetGate;
            _inputNode = inputNode;
            _inputGate = inputGate;
            _outputGate = outputGate;

            InitializeGradientValues();
            CalculateGradients(cells);
            CalculateMomentums(beta1);
            CalculateDeviations(beta2);
            UpdateGatesValues(learningRate, epsilon);     
        }

        private void UpdateBiases(float learning_rate, float epsilon)
        {
            _inputNode._bias -= learning_rate * (vdba / (float)Math.Sqrt(sdba + epsilon));
            _inputGate._bias -= learning_rate * (vdbi / (float)Math.Sqrt(sdbi + epsilon));
            _forgetGate._bias -= learning_rate * (vdbf / (float)Math.Sqrt(sdbf + epsilon));
            _outputGate._bias -= learning_rate * (vdbo / (float)Math.Sqrt(sdbo + epsilon));
        }

        private void UpdateHiddenStateWeights(float learning_rate, float epsilon)
        {
            _inputNode._hiddenStateWeight -= learning_rate * (vdUa / (float)Math.Sqrt(sdUa + epsilon));
            _inputGate._hiddenStateWeight -= learning_rate * (vdUi / (float)Math.Sqrt(sdUi + epsilon));
            _forgetGate._hiddenStateWeight -= learning_rate * (vdUf / (float)Math.Sqrt(sdUf + epsilon));
            _outputGate._hiddenStateWeight -= learning_rate * (vdUo / (float)Math.Sqrt(sdUo + epsilon));
        }

        private void UpdateWeights(float learning_rate, float epsilon)
        {
            _inputNode._inputWeight -= learning_rate * (InputNodeWeightMomentum / (float)Math.Sqrt(sdWa + epsilon));
            _inputGate._inputWeight -= learning_rate * (InputGateWeightMomentum / (float)Math.Sqrt(sdWi + epsilon));
            _forgetGate._inputWeight -= learning_rate * (ForgetGateWeightMomentum / (float)Math.Sqrt(sdWf + epsilon));
            _outputGate._inputWeight -= learning_rate * (OutputGateWeightMomentum / (float)Math.Sqrt(sdWo + epsilon));
        }

        private void UpdateGatesValues(float learning_rate, float epsilon)
        {
            UpdateWeights(learning_rate, epsilon);
            UpdateHiddenStateWeights(learning_rate, epsilon);
            UpdateBiases(learning_rate, epsilon);
        }

        private void CalculateDeviations(float beta2)
        {
            var _beta2 = 1 - beta2;
            CalculateWeightDeviation(beta2, _beta2);
            CalculateHiddenStateWeightDeviation(beta2, _beta2);
            CalculateBiasDeviation(beta2, _beta2);
        }

        private void CalculateMomentums(float beta1)
        {
            var _beta1 = 1 - beta1;
            CalculateWeightsMomentum(beta1, _beta1);
            CalculateHiddenStateWeightMomentum(beta1, _beta1);
            CalculateBiasMomentum(beta1, _beta1);
        }

        private void CalculateGradients(List<Cell> cells)
        {
            CalculateWeightsAndBiasesGradient(cells);
            CalculateHiddenStateWeightsGradient(cells);
        }

        private  void CalculateBiasDeviation(float beta2, float _beta2)
        {
            sdba = beta2 * sdba + _beta2 * (float)Math.Pow(_inputNode.biasGradient, 2);
            sdbi = beta2 * sdbi + _beta2 * (float)Math.Pow(_inputGate.biasGradient, 2);
            sdbf = beta2 * sdbf + _beta2 * (float)Math.Pow(_forgetGate.biasGradient, 2);
            sdbo = beta2 * sdbo + _beta2 * (float)Math.Pow(_outputGate.biasGradient, 2);
        }

        private  void CalculateHiddenStateWeightDeviation(float beta2, float _beta2)
        {
            sdUa = beta2 * sdUa + _beta2 * (float)Math.Pow(_inputNode.hiddenStateWeightGradient, 2);
            sdUi = beta2 * sdUi + _beta2 * (float)Math.Pow(_inputGate.hiddenStateWeightGradient, 2);
            sdUf = beta2 * sdUf + _beta2 * (float)Math.Pow(_forgetGate.hiddenStateWeightGradient, 2);
            sdUo = beta2 * sdUo + _beta2 * (float)Math.Pow(_outputGate.hiddenStateWeightGradient, 2);
        }

        private  void CalculateWeightDeviation(float beta2, float _beta2)
        {
            sdWa = beta2 * sdWa + _beta2 * (float)Math.Pow(_inputNode.weightGradient, 2);
            sdWi = beta2 * sdWi + _beta2 * (float)Math.Pow(_inputGate.weightGradient, 2);
            sdWf = beta2 * sdWf + _beta2 * (float)Math.Pow(_forgetGate.weightGradient, 2);
            sdWo = beta2 * sdWo + _beta2 * (float)Math.Pow(_outputGate.weightGradient, 2);
        }

        private  void CalculateBiasMomentum(float beta1, float _beta1)
        {
            vdba = beta1 * vdba + _beta1 * _inputNode.biasGradient;
            vdbi = beta1 * vdbi + _beta1 * _inputGate.biasGradient;
            vdbf = beta1 * vdbf + _beta1 * _forgetGate.biasGradient;
            vdbo = beta1 * vdbo + _beta1 * _outputGate.biasGradient;
        }

        private  void CalculateHiddenStateWeightMomentum(float beta1,float _beta1)
        {
            vdUa = beta1 * vdUa + _beta1 * _inputNode.hiddenStateWeightGradient;
            vdUi = beta1 * vdUi + _beta1 * _inputGate.hiddenStateWeightGradient;
            vdUf = beta1 * vdUf + _beta1 * _forgetGate.hiddenStateWeightGradient;
            vdUo = beta1 * vdUo + _beta1 * _outputGate.hiddenStateWeightGradient;
        }

        private  void CalculateWeightsMomentum(float beta1,float _beta1)
        {
            InputNodeWeightMomentum = beta1 * InputNodeWeightMomentum + _beta1 * _inputNode.weightGradient;
            InputGateWeightMomentum = beta1 * InputGateWeightMomentum + _beta1 * _inputGate.weightGradient;
            ForgetGateWeightMomentum = beta1 * ForgetGateWeightMomentum + _beta1 * _forgetGate.weightGradient;
            OutputGateWeightMomentum = beta1 * OutputGateWeightMomentum + _beta1 * _outputGate.weightGradient;
        }

        private  void CalculateHiddenStateWeightsGradient(List<Cell> cells)
        {
            for (int i = 0; i < cells.Count - 1; i++)
            {
                _inputNode.hiddenStateWeightGradient += cells[i + 1].inputNodeDerivative * cells[i].hiddenState;
                _inputGate.hiddenStateWeightGradient += cells[i + 1].inputGateDerivative * cells[i].hiddenState;
                _forgetGate.hiddenStateWeightGradient += cells[i + 1].forgetGateDerivative * cells[i].hiddenState;
                _outputGate.hiddenStateWeightGradient += cells[i + 1].outputGateDerivative * cells[i].hiddenState;
            }
        }

        private  void CalculateWeightsAndBiasesGradient(List<Cell> cells)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                _inputNode.weightGradient += cells[i].inputNodeDerivative * cells[i].input;
                _inputGate.weightGradient += cells[i].inputGateDerivative * cells[i].input;
                _forgetGate.weightGradient += cells[i].forgetGateDerivative * cells[i].input;
                _outputGate.weightGradient += cells[i].outputGateDerivative * cells[i].input;

                _inputNode.biasGradient += cells[i].inputNodeDerivative;
                _inputGate.biasGradient += cells[i].inputGateDerivative;
                _forgetGate.biasGradient += cells[i].forgetGateDerivative;
                _outputGate.biasGradient += cells[i].outputGateDerivative;
            }
        }

        private  void InitializeGradientValues()
        {
            _inputNode.weightGradient = 0; _inputNode.hiddenStateWeightGradient = 0; _inputNode.biasGradient = 0;
            _inputGate.weightGradient = 0; _inputGate.hiddenStateWeightGradient = 0; _inputGate.biasGradient = 0;
            _forgetGate.weightGradient = 0; _forgetGate.hiddenStateWeightGradient = 0; _forgetGate.biasGradient = 0;
            _outputGate.weightGradient = 0; _outputGate.hiddenStateWeightGradient = 0; _outputGate.biasGradient = 0;
        }
    }
}
