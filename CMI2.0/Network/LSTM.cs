using static CMI.MathUtils;
using static CMI.ConsoleUtils;
using static CMI.Data.Normalizer;
using CMI2._0.Network;

namespace CMI.Network
{
    public class LSTM
    {
        public List<char> Predicted_Output;
        private static readonly float LEARNING_RATE = 0.001f;
        private static readonly string PARAMETERS_FILE_PATH = AppDomain.CurrentDomain.BaseDirectory + "parameters.txt";
        private List<Cell> cells;

        public LSTM()
        {

        }

        public void LoadParameters(string filePath = "")
        {
            string[] parameters = GetParameters(filePath);
            SetParameters(parameters);
        }

        private void SetParameters(string[] parameters)
        {
            SetInputWeights(parameters);
            SetHiddenStateWeights(parameters);
            SetBiases(parameters);
        }

        private void SetBiases(string[] parameters)
        {
            InputNode._bias = float.Parse(parameters[8]);
            InputGate._bias = float.Parse(parameters[9]);
            ForgetGate._bias = float.Parse(parameters[10]);
            OutputGate._bias = float.Parse(parameters[11]);
        }

        private void SetHiddenStateWeights(string[] parameters)
        {
            InputNode._hiddenStateWeight = float.Parse(parameters[4]);
            InputGate._hiddenStateWeight = float.Parse(parameters[5]);
            ForgetGate._hiddenStateWeight = float.Parse(parameters[6]);
            OutputGate._hiddenStateWeight = float.Parse(parameters[7]);
        }

        private void SetInputWeights(string[] parameters)
        {
            InputNode._inputWeight = float.Parse(parameters[0]);
            InputGate._inputWeight = float.Parse(parameters[1]);
            ForgetGate._inputWeight = float.Parse(parameters[2]);
            OutputGate._inputWeight = float.Parse(parameters[3]);
        }

        private string[] GetParameters(string filePath)
        {
            if (filePath == "")
                filePath = PARAMETERS_FILE_PATH;

            if (!File.Exists(filePath))
                throw new Exception("Parameters file not found.");

            string[] parameters = File.ReadAllLines(filePath);
            if (parameters.Length < 12)
                throw new Exception("Could not read the file. Missing parameters.");
            return parameters;
        }

        public void SaveParameters()
        {
            CheckParametersFile();
            using var sw = new StreamWriter(PARAMETERS_FILE_PATH);
            SaveInputWeights(sw);
            SaveHiddenStateWeights(sw);
            SaveBiases(sw);
        }

        private void SaveInputWeights(StreamWriter sw)
        {
            sw.WriteLine(InputNode._inputWeight);
            sw.WriteLine(InputGate._inputWeight);
            sw.WriteLine(ForgetGate._inputWeight);
            sw.WriteLine(OutputGate._inputWeight);
        }

        private void SaveHiddenStateWeights(StreamWriter sw)
        {
            sw.WriteLine(InputNode._hiddenStateWeight);
            sw.WriteLine(InputGate._hiddenStateWeight);
            sw.WriteLine(ForgetGate._hiddenStateWeight);
            sw.WriteLine(OutputGate._hiddenStateWeight);
        }

        private void SaveBiases(StreamWriter sw)
        {
            sw.WriteLine(InputNode._bias);
            sw.WriteLine(InputGate._bias);
            sw.WriteLine(ForgetGate._bias);
            sw.WriteLine(OutputGate._bias);
        }

        private void CheckParametersFile()
        {
            if (File.Exists(PARAMETERS_FILE_PATH))
                File.Delete(PARAMETERS_FILE_PATH);
            File.Create(PARAMETERS_FILE_PATH).Close();
        }

        public void Predict(float[] inputs, float previousCellState = 0, float previousHiddenState = 0)
        {
            Predicted_Output = new();
            for (int i = 0; i < inputs.Length; i++)
            {
                Cell cell = new(inputs[i], previousCellState, previousHiddenState);
                cell.Forward();
                previousCellState = cell.cellState;
                previousHiddenState = cell.hiddenState;
                Print(Denormalize(cell.hiddenState));
                Predicted_Output.Add(Denormalize(cell.hiddenState));
            }
        }

        public void Train(List<float[]> inputs,
                          List<float[]> outputs,
                          int totalIterations,
                          int epochs = 1,
                          float previousCellState = 0,
                          float previousHiddenState = 0)
        {
            float aux_prev_long = previousCellState;
            float aux_prev_short = previousHiddenState;
            for (int i = 0; i < inputs.Count; i++)
            {
                for (int j = 1; j <= totalIterations; j++)
                {
                    cells = new();
                    for (int k = 0; k < inputs[i].Length; k++)
                    {
                        Cell cell = new(inputs[i][k], previousCellState, previousHiddenState);
                        cell.Forward();
                        cells.Add(cell);
                        previousCellState = cell.cellState;
                        previousHiddenState = cell.hiddenState;
                        if (j % epochs == 0)
                            Print(Denormalize(cell.hiddenState), false);
                    }
                    if (j % epochs == 0)
                        Print("\n");
                    previousCellState = aux_prev_long;
                    previousHiddenState = aux_prev_short;
                    BackPropagationThroughTime(outputs[i]);
                }
            }
        }

        public void Train(float[] inputs,
                          float[] outputs,
                          int totalIterations,
                          int epochs = 1,
                          float previousCellState = 0,
                          float previousHiddenState = 0)
        {
            float auxPreviousCellState = previousCellState;
            float auxPreviousHiddenState = previousHiddenState;
            for (int actualIteration = 1; actualIteration <= totalIterations; actualIteration++)
            {
                if (actualIteration % epochs == 0) Print("\n");
                cells = new();
                FeedForward(inputs, previousCellState, previousHiddenState,epochs,actualIteration);
                previousCellState = auxPreviousCellState;
                previousHiddenState = auxPreviousHiddenState;
                BackPropagationThroughTime(outputs);
                ShowProgress(actualIteration,totalIterations, epochs, outputs);
            }
            SaveParameters();
        }

        private void ShowProgress(int actualIteration,int totalIterations, int epochs, float[] outputs)
        {
            if (actualIteration % epochs == 0)
            {
                Print("\n");
                float totalLoss = 0.0f;
                for (int z = 0; z < cells.Count; z++)
                {
                    totalLoss += (float)Math.Pow(cells[z].hiddenState - outputs[z], 2);
                }
                totalLoss /= (float)outputs.Length;
                ShowTrainingProgress(totalIterations, totalLoss, actualIteration);
            }
        }

        private void FeedForward(float[] inputs, float previousCellState, float previousHiddenState, int epochs, int actualIteration)
        {
            for (int j = 0; j < inputs.Length; j++)
            {
                Cell cell = new(inputs[j], previousCellState, previousHiddenState);
                cell.Forward();
                previousCellState = cell.cellState;
                previousHiddenState = cell.hiddenState;
                cells.Add(cell);
                if (actualIteration % epochs == 0) Print(Denormalize(cell.hiddenState), false);
            }
        }

        private void ShowTrainingProgress(int totalIterations, float totalLoss, int i)
        {
            Print("TRAINING PROCESS: " + Math.Round(((((float)i + 1) / (float)totalIterations) * 100.0), 0) + "%\tloss: " +
            Decimal.Parse(totalLoss.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.AllowDecimalPoint),
            false);
        }

        public void InitializeGateValues()
        {
            InputNode._inputWeight = GenerateRandom();
            InputNode._hiddenStateWeight = GenerateRandom();
            InputNode._bias = 0;

            InputGate._inputWeight = GenerateRandom();
            InputGate._hiddenStateWeight = GenerateRandom();
            InputGate._bias = 0;

            ForgetGate._inputWeight = GenerateRandom();
            ForgetGate._hiddenStateWeight = GenerateRandom();
            ForgetGate._bias = 0;

            OutputGate._inputWeight = GenerateRandom();
            OutputGate._hiddenStateWeight = GenerateRandom();
            OutputGate._bias = 0;
        }

        private void BackPropagationThroughTime(float[] originalOutputs)
        {
            float nextHiddenStateDerivative = 0;
            float nextCurrentStateDerivative = 0;
            float nextForgetGateValue = 0;
            for (int i = cells.Count - 1; i >= 0; i--)
            {
                cells[i].Backpropagation(originalOutputs[i],
                                         nextHiddenStateDerivative,
                                         nextCurrentStateDerivative,
                                         nextForgetGateValue);
                nextHiddenStateDerivative = cells[i].previousHiddenStateDerivative;
                nextCurrentStateDerivative = cells[i].cellStateDerivative;
                nextForgetGateValue = cells[i].forgetGateValue;
            }
            UpdateWeights();
        }

        private void UpdateWeights()
        {
            AdamOptimizer.Optimize(LEARNING_RATE, cells);
        }

        private float ClipGradient(float gradient, float min, float max, bool normalization)
        {
            if (gradient > max)
            {
                gradient = max;
            }
            else if (gradient < min)
            {
                gradient = min;
            }

            if (normalization)
            {
                gradient /= max;
            }

            return gradient;
        }
    }
}
