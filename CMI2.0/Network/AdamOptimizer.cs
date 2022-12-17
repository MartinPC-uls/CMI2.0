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
        private static float InputNodeWeightMomentum=0, InputGateWeightMomentum=0, ForgetGateWeightMomentum=0, OutputGateWeightMomentum=0;
        private static float vdUa=0, vdUi=0, vdUf=0, vdUo=0;
        private static float vdba=0, vdbi=0, vdbf=0, vdbo=0;

        private static float sdWa=0, sdWi=0, sdWf=0, sdWo=0;
        private static float sdUa=0, sdUi=0, sdUf=0, sdUo=0;
        private static float sdba=0, sdbi=0, sdbf=0, sdbo=0;


        public static void Optimize(float learning_rate,
                                         List<Cell> cells,
                                         float beta1 = 0.9f,
                                         float beta2 = 0.999f,
                                         float epsilon = 1e-8f)
        {
            InitializeGradientValues();
            CalculateGradients(cells);
            CalculateMomentums(beta1);
            CalculateDeviations(beta2);
            UpdateGatesValues(learning_rate, epsilon);     
        }

        private static void UpdateBiases(float learning_rate, float epsilon)
        {
            InputNode._bias -= learning_rate * (vdba / (float)Math.Sqrt(sdba + epsilon));
            InputGate._bias -= learning_rate * (vdbi / (float)Math.Sqrt(sdbi + epsilon));
            ForgetGate._bias -= learning_rate * (vdbf / (float)Math.Sqrt(sdbf + epsilon));
            OutputGate._bias -= learning_rate * (vdbo / (float)Math.Sqrt(sdbo + epsilon));
        }

        private static void UpdateHiddenStateWeights(float learning_rate, float epsilon)
        {
            InputNode._hiddenStateWeight -= learning_rate * (vdUa / (float)Math.Sqrt(sdUa + epsilon));
            InputGate._hiddenStateWeight -= learning_rate * (vdUi / (float)Math.Sqrt(sdUi + epsilon));
            ForgetGate._hiddenStateWeight -= learning_rate * (vdUf / (float)Math.Sqrt(sdUf + epsilon));
            OutputGate._hiddenStateWeight -= learning_rate * (vdUo / (float)Math.Sqrt(sdUo + epsilon));
        }

        private static void UpdateWeights(float learning_rate, float epsilon)
        {
            InputNode._inputWeight -= learning_rate * (InputNodeWeightMomentum / (float)Math.Sqrt(sdWa + epsilon));
            InputGate._inputWeight -= learning_rate * (InputGateWeightMomentum / (float)Math.Sqrt(sdWi + epsilon));
            ForgetGate._inputWeight -= learning_rate * (ForgetGateWeightMomentum / (float)Math.Sqrt(sdWf + epsilon));
            OutputGate._inputWeight -= learning_rate * (OutputGateWeightMomentum / (float)Math.Sqrt(sdWo + epsilon));
        }

        private static void UpdateGatesValues(float learning_rate, float epsilon)
        {
            UpdateWeights(learning_rate, epsilon);
            UpdateHiddenStateWeights(learning_rate, epsilon);
            UpdateBiases(learning_rate, epsilon);
        }

        private static void CalculateDeviations(float beta2)
        {
            var _beta2 = 1 - beta2;
            CalculateWeightDeviation(beta2, _beta2);
            CalculateHiddenStateWeightDeviation(beta2, _beta2);
            CalculateBiasDeviation(beta2, _beta2);
        }

        private static void CalculateMomentums(float beta1)
        {
            var _beta1 = 1 - beta1;
            CalculateWeightsMomentum(beta1, _beta1);
            CalculateHiddenStateWeightMomentum(beta1, _beta1);
            CalculateBiasMomentum(beta1, _beta1);
        }

        private static void CalculateGradients(List<Cell> cells)
        {
            CalculateWeightsAndBiasesGradient(cells);
            CalculateHiddenStateWeightsGradient(cells);
        }

        private static void CalculateBiasDeviation(float beta2, float _beta2)
        {
            sdba = beta2 * sdba + _beta2 * (float)Math.Pow(InputNode.biasGradient, 2);
            sdbi = beta2 * sdbi + _beta2 * (float)Math.Pow(InputGate.biasGradient, 2);
            sdbf = beta2 * sdbf + _beta2 * (float)Math.Pow(ForgetGate.biasGradient, 2);
            sdbo = beta2 * sdbo + _beta2 * (float)Math.Pow(OutputGate.biasGradient, 2);
        }

        private static void CalculateHiddenStateWeightDeviation(float beta2, float _beta2)
        {
            sdUa = beta2 * sdUa + _beta2 * (float)Math.Pow(InputNode.hiddenStateWeightGradient, 2);
            sdUi = beta2 * sdUi + _beta2 * (float)Math.Pow(InputGate.hiddenStateWeightGradient, 2);
            sdUf = beta2 * sdUf + _beta2 * (float)Math.Pow(ForgetGate.hiddenStateWeightGradient, 2);
            sdUo = beta2 * sdUo + _beta2 * (float)Math.Pow(OutputGate.hiddenStateWeightGradient, 2);
        }

        private static void CalculateWeightDeviation(float beta2, float _beta2)
        {
            sdWa = beta2 * sdWa + _beta2 * (float)Math.Pow(InputNode.weightGradient, 2);
            sdWi = beta2 * sdWi + _beta2 * (float)Math.Pow(InputGate.weightGradient, 2);
            sdWf = beta2 * sdWf + _beta2 * (float)Math.Pow(ForgetGate.weightGradient, 2);
            sdWo = beta2 * sdWo + _beta2 * (float)Math.Pow(OutputGate.weightGradient, 2);
        }

        private static void CalculateBiasMomentum(float beta1, float _beta1)
        {
            vdba = beta1 * vdba + _beta1 * InputNode.biasGradient;
            vdbi = beta1 * vdbi + _beta1 * InputGate.biasGradient;
            vdbf = beta1 * vdbf + _beta1 * ForgetGate.biasGradient;
            vdbo = beta1 * vdbo + _beta1 * OutputGate.biasGradient;
        }

        private static void CalculateHiddenStateWeightMomentum(float beta1,float _beta1)
        {
            vdUa = beta1 * vdUa + _beta1 * InputNode.hiddenStateWeightGradient;
            vdUi = beta1 * vdUi + _beta1 * InputGate.hiddenStateWeightGradient;
            vdUf = beta1 * vdUf + _beta1 * ForgetGate.hiddenStateWeightGradient;
            vdUo = beta1 * vdUo + _beta1 * OutputGate.hiddenStateWeightGradient;
        }

        private static void CalculateWeightsMomentum(float beta1,float _beta1)
        {
            InputNodeWeightMomentum = beta1 * InputNodeWeightMomentum + _beta1 * InputNode.weightGradient;
            InputGateWeightMomentum = beta1 * InputGateWeightMomentum + _beta1 * InputGate.weightGradient;
            ForgetGateWeightMomentum = beta1 * ForgetGateWeightMomentum + _beta1 * ForgetGate.weightGradient;
            OutputGateWeightMomentum = beta1 * OutputGateWeightMomentum + _beta1 * OutputGate.weightGradient;
        }

        private static void CalculateHiddenStateWeightsGradient(List<Cell> cells)
        {
            for (int i = 0; i < cells.Count - 1; i++)
            {
                InputNode.hiddenStateWeightGradient += cells[i + 1].inputNodeDerivative * cells[i].hiddenState;
                InputGate.hiddenStateWeightGradient += cells[i + 1].inputGateDerivative * cells[i].hiddenState;
                ForgetGate.hiddenStateWeightGradient += cells[i + 1].forgetGateDerivative * cells[i].hiddenState;
                OutputGate.hiddenStateWeightGradient += cells[i + 1].outputGateDerivative * cells[i].hiddenState;
            }
        }

        private static void CalculateWeightsAndBiasesGradient(List<Cell> cells)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                InputNode.weightGradient += cells[i].inputNodeDerivative * cells[i].input;
                InputGate.weightGradient += cells[i].inputGateDerivative * cells[i].input;
                ForgetGate.weightGradient += cells[i].forgetGateDerivative * cells[i].input;
                OutputGate.weightGradient += cells[i].outputGateDerivative * cells[i].input;

                InputNode.biasGradient += cells[i].inputNodeDerivative;
                InputGate.biasGradient += cells[i].inputGateDerivative;
                ForgetGate.biasGradient += cells[i].forgetGateDerivative;
                OutputGate.biasGradient += cells[i].outputGateDerivative;
            }
        }

        private static void InitializeGradientValues()
        {
            InputNode.weightGradient = 0; InputNode.hiddenStateWeightGradient = 0; InputNode.biasGradient = 0;
            InputGate.weightGradient = 0; InputGate.hiddenStateWeightGradient = 0; InputGate.biasGradient = 0;
            ForgetGate.weightGradient = 0; ForgetGate.hiddenStateWeightGradient = 0; ForgetGate.biasGradient = 0;
            OutputGate.weightGradient = 0; OutputGate.hiddenStateWeightGradient = 0; OutputGate.biasGradient = 0;
        }
    }
}
