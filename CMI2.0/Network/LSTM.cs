using static CMI.Utils;
using static CMI.Data.Normalizer;

namespace CMI.Network
{
    public class LSTM
    {
        // W: Weight for input
        // U: Weight for hidden state
        // b: bias
        protected static float Wa, Ua;
        protected static float Wi, Ui;
        protected static float Wf, Uf;
        protected static float Wo, Uo;
        protected static float ba;
        protected static float bi;
        protected static float bf;
        protected static float bo;

        private static float prev_vdWa, prev_vdWi, prev_vdWf, prev_vdWo;
        private static float prev_vdUa, prev_vdUi, prev_vdUf, prev_vdUo;
        private static float prev_vdba, prev_vdbi, prev_vdbf, prev_vdbo;

        private static float prev_sdWa, prev_sdWi, prev_sdWf, prev_sdWo;
        private static float prev_sdUa, prev_sdUi, prev_sdUf, prev_sdUo;
        private static float prev_sdba, prev_sdbi, prev_sdbf, prev_sdbo;

        public List<char> Predicted_Output;

        private static readonly float LEARNING_RATE = 0.001f;

        private static readonly string PARAMETERS_FILE = AppDomain.CurrentDomain.BaseDirectory + "parameters.txt";

        private List<Cell> cells;

        public void LoadParameters(string parameters_file = "")
        {
            if (parameters_file == "")
                parameters_file = PARAMETERS_FILE;

            if (!File.Exists(parameters_file))
                throw new Exception("Parameters file not found.");

            string[] parameters = File.ReadAllLines(parameters_file);
            if (parameters.Length < 12)
                throw new Exception("Could not read the file. Missing parameters.");

            Wa = float.Parse(parameters[0]);
            Wi = float.Parse(parameters[1]);
            Wf = float.Parse(parameters[2]);
            Wo = float.Parse(parameters[3]);

            Ua = float.Parse(parameters[4]);
            Ui = float.Parse(parameters[5]);
            Uf = float.Parse(parameters[6]);
            Uo = float.Parse(parameters[7]);

            ba = float.Parse(parameters[8]);
            bi = float.Parse(parameters[9]);
            bf = float.Parse(parameters[10]);
            bo = float.Parse(parameters[11]);
        }
        public void SaveParameters()
        {
            if (File.Exists(PARAMETERS_FILE))
                File.Delete(PARAMETERS_FILE);

            File.Create(PARAMETERS_FILE).Close();
            using var sw = new StreamWriter(PARAMETERS_FILE);
            sw.WriteLine(Wa);
            sw.WriteLine(Wi);
            sw.WriteLine(Wf);
            sw.WriteLine(Wo);

            sw.WriteLine(Ua);
            sw.WriteLine(Ui);
            sw.WriteLine(Uf);
            sw.WriteLine(Uo);

            sw.WriteLine(ba);
            sw.WriteLine(bi);
            sw.WriteLine(bf);
            sw.WriteLine(bo);
        }

        public void Prediction(float[] inputs, float prev_long = 0, float prev_short = 0)
        {
            Predicted_Output = new();
            for (int i = 0; i < inputs.Length; i++)
            {
                Cell cell = new(inputs[i], prev_long, prev_short);
                cell.Forward();
                prev_long = cell.ct;
                prev_short = cell.ht;
                Print(Denormalize(cell.ht));
                Predicted_Output.Add(Denormalize(cell.ht));
            }
        }

        

        public void Train(List<float[]> inputs, List<float[]> outputs, int number_of_iterations, int epochs = 1, float prev_long = 0, float prev_short = 0)
        {
            float aux_prev_long = prev_long;
            float aux_prev_short = prev_short;
            for (int i = 0; i < inputs.Count; i++)
            {
                for (int j = 1; j <= number_of_iterations; j++)
                {
                    cells = new();
                    for (int k = 0; k < inputs[i].Length; k++)
                    {
                        Cell cell = new(inputs[i][k], prev_long, prev_short);
                        cell.Forward();
                        cells.Add(cell);
                        prev_long = cell.ct;
                        prev_short = cell.ht;
                        if (j % epochs == 0)
                            Print(Denormalize(cell.ht), false);
                    }
                    if (j % epochs == 0)
                        Print("\n");
                    prev_long = aux_prev_long;
                    prev_short = aux_prev_short;

                    BackPropagationThroughTime(outputs[i]);
                }
            }
        }

        public void Train(float[] inputs, float[] outputs, int number_of_iterations, int epochs = 1, float prev_long = 0, float prev_short = 0)
        {
            float aux_prev_long = prev_long;
            float aux_prev_short = prev_short;
            for (int i = 1; i <= number_of_iterations; i++)
            {
                cells = new();
                if (i % epochs == 0)
                    Print("\n");
                for (int j = 0; j < inputs.Length; j++)
                {
                    Cell cell = new(inputs[j], prev_long, prev_short);
                    cell.Forward();
                    if (i % epochs == 0)
                    {
                        Print(Denormalize(cell.ht), false);
                    }
                    prev_long = cell.ct;
                    prev_short = cell.ht;
                    cells.Add(cell);
                }
                prev_long = aux_prev_long;
                prev_short = aux_prev_short;

                BackPropagationThroughTime(outputs);
                if (i % epochs == 0)
                {
                    Print("\n");
                    float total_loss = 0.0f;
                    for (int z = 0; z < cells.Count; z++)
                    {
                        total_loss += (float)Math.Pow(cells[z].ht - outputs[z], 2);
                    }
                    total_loss /= (float)outputs.Length;
                    Print("TRAINING PROCESS: " + Math.Round(((((float)i + 1) / (float)number_of_iterations) * 100.0), 0) + "%\tloss: " +
                        Decimal.Parse(total_loss.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.AllowDecimalPoint),
                        false);
                }
            }
            SaveParameters();
        }

        public LSTM()
        {
            prev_vdWa = 0;
            prev_vdWi = 0;
            prev_vdWf = 0;
            prev_vdWo = 0;

            prev_vdUa = 0;
            prev_vdUi = 0;
            prev_vdUf = 0;
            prev_vdUo = 0;

            prev_vdba = 0;
            prev_vdbi = 0;
            prev_vdbf = 0;
            prev_vdbo = 0;

            prev_sdWa = 0;
            prev_sdWi = 0;
            prev_sdWf = 0;
            prev_sdWo = 0;

            prev_sdUa = 0;
            prev_sdUi = 0;
            prev_sdUf = 0;
            prev_sdUo = 0;

            prev_sdba = 0;
            prev_sdbi = 0;
            prev_sdbf = 0;
            prev_sdbo = 0;
        }

        public void initialize()
        {
            Wa = GenerateRandom();
            Ua = GenerateRandom();
            ba = 0;

            Wi = GenerateRandom();
            Ui = GenerateRandom();
            bi = 0;

            Wf = GenerateRandom();
            Uf = GenerateRandom();
            bf = 0;

            Wo = GenerateRandom();
            Uo = GenerateRandom();
            bo = 0;
        }

        private void BackPropagationThroughTime(float[] original_output)
        {
            float next_dht = 0;
            float next_dct = 0;
            float next_f = 0;
            for (int i = cells.Count - 1; i >= 0; i--)
            {
                cells[i].Backpropagation(original_output[i], next_dht, next_dct, next_f);
                next_dht = cells[i].dht_1;
                next_dct = cells[i].dct;
                next_f = cells[i].f;
            }
            AdamOptimizer(LEARNING_RATE);
        }

        public void AdamOptimizer(float learning_rate, float beta1 = 0.9f, float beta2 = 0.999f, float epsilon = 0.00000001f)
        {
            float dWa = 0, dUa = 0, dba = 0;
            float dWi = 0, dUi = 0, dbi = 0;
            float dWf = 0, dUf = 0, dbf = 0;
            float dWo = 0, dUo = 0, dbo = 0;

            // W and b
            for (int i = 0; i < cells.Count; i++)
            {
                dWa += cells[i].da * cells[i].x;
                dWi += cells[i].di * cells[i].x;
                dWf += cells[i].df * cells[i].x;
                dWo += cells[i].do_ * cells[i].x;

                dba += cells[i].da;
                dbi += cells[i].di;
                dbf += cells[i].df;
                dbo += cells[i].do_;
            }
            // U
            for (int i = 0; i < cells.Count - 1; i++)
            {
                dUa += cells[i + 1].da * cells[i].ht;
                dUi += cells[i + 1].di * cells[i].ht;
                dUf += cells[i + 1].df * cells[i].ht;
                dUo += cells[i + 1].do_ * cells[i].ht;
            }

            /*float min = -1;
            float max = 1;

            dWa = ClipGradient(dWa, min, max, true);
            dWi = ClipGradient(dWi, min, max, true);
            dWf = ClipGradient(dWf, min, max, true);
            dWo = ClipGradient(dWo, min, max, true);

            dUa = ClipGradient(dUa, min, max, true);
            dUi = ClipGradient(dUi, min, max, true);
            dUf = ClipGradient(dUf, min, max, true);
            dUo = ClipGradient(dUo, min, max, true);

            dba = ClipGradient(dba, min, max, true);
            dbi = ClipGradient(dbi, min, max, true);
            dbf = ClipGradient(dbf, min, max, true);
            dbo = ClipGradient(dbo, min, max, true);*/

            var _beta1 = 1 - beta1;

            var vdWa = beta1 * prev_vdWa + _beta1 * dWa;
            var vdWi = beta1 * prev_vdWi + _beta1 * dWi;
            var vdWf = beta1 * prev_vdWf + _beta1 * dWf;
            var vdWo = beta1 * prev_vdWo + _beta1 * dWo;

            var vdUa = beta1 * prev_vdUa + _beta1 * dUa;
            var vdUi = beta1 * prev_vdUi + _beta1 * dUi;
            var vdUf = beta1 * prev_vdUf + _beta1 * dUf;
            var vdUo = beta1 * prev_vdUo + _beta1 * dUo;

            var vdba = beta1 * prev_vdba + _beta1 * dba;
            var vdbi = beta1 * prev_vdbi + _beta1 * dbi;
            var vdbf = beta1 * prev_vdbf + _beta1 * dbf;
            var vdbo = beta1 * prev_vdbo + _beta1 * dbo;

            var _beta2 = 1 - beta2;

            var sdWa = beta2 * prev_sdWa + _beta2 * (float)Math.Pow(dWa, 2);
            var sdWi = beta2 * prev_sdWi + _beta2 * (float)Math.Pow(dWi, 2);
            var sdWf = beta2 * prev_sdWf + _beta2 * (float)Math.Pow(dWf, 2);
            var sdWo = beta2 * prev_sdWo + _beta2 * (float)Math.Pow(dWo, 2);
                                                         
            var sdUa = beta2 * prev_sdUa + _beta2 * (float)Math.Pow(dUa, 2);
            var sdUi = beta2 * prev_sdUi + _beta2 * (float)Math.Pow(dUi, 2);
            var sdUf = beta2 * prev_sdUf + _beta2 * (float)Math.Pow(dUf, 2);
            var sdUo = beta2 * prev_sdUo + _beta2 * (float)Math.Pow(dUo, 2);
                                                         
            var sdba = beta2 * prev_sdba + _beta2 * (float)Math.Pow(dba, 2);
            var sdbi = beta2 * prev_sdbi + _beta2 * (float)Math.Pow(dbi, 2);
            var sdbf = beta2 * prev_sdbf + _beta2 * (float)Math.Pow(dbf, 2);
            var sdbo = beta2 * prev_sdbo + _beta2 * (float)Math.Pow(dbo, 2);

            // Updating weights of input
            Wa -= learning_rate * (vdWa / (float)Math.Sqrt(sdWa + epsilon));
            Wi -= learning_rate * (vdWi / (float)Math.Sqrt(sdWi + epsilon));
            Wf -= learning_rate * (vdWf / (float)Math.Sqrt(sdWf + epsilon));
            Wo -= learning_rate * (vdWo / (float)Math.Sqrt(sdWo + epsilon));
                                          
            // Updating weights of hidden 
            Ua -= learning_rate * (vdUa / (float)Math.Sqrt(sdUa + epsilon));
            Ui -= learning_rate * (vdUi / (float)Math.Sqrt(sdUi + epsilon));
            Uf -= learning_rate * (vdUf / (float)Math.Sqrt(sdUf + epsilon));
            Uo -= learning_rate * (vdUo / (float)Math.Sqrt(sdUo + epsilon));

            // Updating biases            
            ba -= learning_rate * (vdba / (float)Math.Sqrt(sdba + epsilon));
            bi -= learning_rate * (vdbi / (float)Math.Sqrt(sdbi + epsilon));
            bf -= learning_rate * (vdbf / (float)Math.Sqrt(sdbf + epsilon));
            bo -= learning_rate * (vdbo / (float)Math.Sqrt(sdbo + epsilon));

            prev_vdWa = vdWa;
            prev_vdWi = vdWi;
            prev_vdWf = vdWf;
            prev_vdWo = vdWo;

            prev_vdUa = vdUa;
            prev_vdUi = vdUi;
            prev_vdUf = vdUf;
            prev_vdUo = vdUo;

            prev_vdba = vdba;
            prev_vdbi = vdbi;
            prev_vdbf = vdbf;
            prev_vdbo = vdbo;

            prev_sdWa = sdWa;
            prev_sdWi = sdWi;
            prev_sdWf = sdWf;
            prev_sdWo = sdWo;

            prev_sdUa = sdUa;
            prev_sdUi = sdUi;
            prev_sdUf = sdUf;
            prev_sdUo = sdUo;

            prev_sdba = sdba;
            prev_sdbi = sdbi;
            prev_sdbf = sdbf;
            prev_sdbo = sdbo;
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
