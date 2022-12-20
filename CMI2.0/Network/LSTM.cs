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

        private string _parametersFile;

        private ForgetGate _forgetGate;
        private InputNode _inputNode;
        private InputGate _inputGate;
        private OutputGate _outputGate;

        public LSTM(bool initializeParameters, string parametersFile = "")
        {
            _forgetGate = new();
            _inputNode = new();
            _inputGate = new();
            _outputGate = new();

            _parametersFile = parametersFile;

            if (initializeParameters)
            {
                InitializeGateValues();
            }
            else
            {
                LoadParameters(parametersFile);
            }
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
            _inputNode._bias = float.Parse(parameters[8]);
            _inputGate._bias = float.Parse(parameters[9]);
            _forgetGate._bias = float.Parse(parameters[10]);
            _outputGate._bias = float.Parse(parameters[11]);
        }

        private void SetHiddenStateWeights(string[] parameters)
        {
            _inputNode._hiddenStateWeight = float.Parse(parameters[4]);
            _inputGate._hiddenStateWeight = float.Parse(parameters[5]);
            _forgetGate._hiddenStateWeight = float.Parse(parameters[6]);
            _outputGate._hiddenStateWeight = float.Parse(parameters[7]);
        }

        private void SetInputWeights(string[] parameters)
        {
            _inputNode._inputWeight = float.Parse(parameters[0]);
            _inputGate._inputWeight = float.Parse(parameters[1]);
            _forgetGate._inputWeight = float.Parse(parameters[2]);
            _outputGate._inputWeight = float.Parse(parameters[3]);
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

        public void SaveParameters(string path = "")
        {
            if (path == "")
                path = PARAMETERS_FILE_PATH;
            CheckParametersFile(path);
            using var sw = new StreamWriter(path);
            SaveInputWeights(sw);
            SaveHiddenStateWeights(sw);
            SaveBiases(sw);
        }

        private void SaveInputWeights(StreamWriter sw)
        {
            sw.WriteLine(_inputNode._inputWeight);
            sw.WriteLine(_inputGate._inputWeight);
            sw.WriteLine(_forgetGate._inputWeight);
            sw.WriteLine(_outputGate._inputWeight);
        }

        private void SaveHiddenStateWeights(StreamWriter sw)
        {
            sw.WriteLine(_inputNode._hiddenStateWeight);
            sw.WriteLine(_inputGate._hiddenStateWeight);
            sw.WriteLine(_forgetGate._hiddenStateWeight);
            sw.WriteLine(_outputGate._hiddenStateWeight);
        }

        private void SaveBiases(StreamWriter sw)
        {
            sw.WriteLine(_inputNode._bias);
            sw.WriteLine(_inputGate._bias);
            sw.WriteLine(_forgetGate._bias);
            sw.WriteLine(_outputGate._bias);
        }

        private void CheckParametersFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
            File.Create(path).Close();
        }

        public void Predict(float[] inputs, float previousCellState = 0, float previousHiddenState = 0)
        {
            Predicted_Output = new();
            for (int i = 0; i < inputs.Length; i++)
            {
                Cell cell = new(inputs[i], previousCellState, previousHiddenState, _forgetGate, _inputNode, _inputGate, _outputGate);
                cell.Forward();
                previousCellState = cell.cellState;
                previousHiddenState = cell.hiddenState;
                Print(Denormalize(cell.hiddenState));
                Predicted_Output.Add(Denormalize(cell.hiddenState));
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
                if (actualIteration % epochs == 0)
                    Print("\n");
                cells = new();
                FeedForward(inputs, previousCellState, previousHiddenState,epochs,actualIteration);
                previousCellState = auxPreviousCellState;
                previousHiddenState = auxPreviousHiddenState;
                BackPropagationThroughTime(outputs);
                ShowProgress(actualIteration,totalIterations, epochs, outputs);
            }
            SaveParameters(_parametersFile);
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

        private void FeedForward(float[] inputs, float previousCellState, float previousHiddenState, int epochs, int currentIteration)
        {
            for (int j = 0; j < inputs.Length; j++)
            {
                Cell cell = new(inputs[j], previousCellState, previousHiddenState, _forgetGate, _inputNode, _inputGate, _outputGate);
                cell.Forward();
                previousCellState = cell.cellState;
                previousHiddenState = cell.hiddenState;
                cells.Add(cell);
                if (currentIteration % epochs == 0) Print(Denormalize(cell.hiddenState), false);
            }
        }

        private static void ShowTrainingProgress(int totalIterations, float totalLoss, int i)
        {
            Print("TRAINING PROCESS: " + Math.Round(((((float)i + 1) / (float)totalIterations) * 100.0), 0) + "%\tloss: " +
            Decimal.Parse(totalLoss.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.AllowDecimalPoint),
            false);
        }

        public void InitializeGateValues()
        {
            _inputNode._inputWeight = GenerateRandom();
            _inputNode._hiddenStateWeight = GenerateRandom();
            _inputNode._bias = 0;

            _inputGate._inputWeight = GenerateRandom();
            _inputGate._hiddenStateWeight = GenerateRandom();
            _inputGate._bias = 0;

            _forgetGate._inputWeight = GenerateRandom();
            _forgetGate._hiddenStateWeight = GenerateRandom();
            _forgetGate._bias = 0;

            _outputGate._inputWeight = GenerateRandom();
            _outputGate._hiddenStateWeight = GenerateRandom();
            _outputGate._bias = 0;
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
            AdamOptimizer optimizer = new();
            optimizer.Optimize(LEARNING_RATE, cells, _forgetGate, _inputNode, _inputGate, _outputGate);
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
