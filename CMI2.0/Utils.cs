namespace CMI
{
    public class Utils
    {
        public static void Print(object? obj, bool skipLine = true)
        {
            if (obj is float[,] matrix)
            {
                Console.Write("[");
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    Console.Write("[");
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        Console.Write(matrix[i, j]);
                        if (j != matrix.GetLength(1) - 1)
                            Console.Write("\n");
                    }
                    Console.Write("]");
                    if (i != matrix.GetLength(0) - 1)
                        Console.Write("\n ");
                }
                Console.Write("]");
                return;
            }
            else if (obj is float[] vector)
            {
                Console.Write("[");
                for (int i = 0; i < vector.Length; i++)
                {
                    Console.Write(vector[i]);
                    if (i != vector.Length - 1)
                        Console.Write(" ");
                }
                Console.Write("]");
                Console.WriteLine();
                return;
            }

            if (!skipLine)
            {
                Console.Write(obj.ToString());
                return;
            }
            Console.WriteLine(obj.ToString());
        }
        public static float Softmax(float[] x)
        {
            float e_x = 0;
            float sum = 0;
            for (int i = 0; i < x.Length; i++)
            {
                e_x += (float)Math.Exp(x[i]);
            }
            for (int i = 0; i < x.Length; i++)
            {
                sum += (float)Math.Exp(x[i]) / e_x;
            }
            return sum;
        }
        public static float Softmax(float x)
        {
            float e_x = 0;
            float sum = 0;
            e_x += (float)Math.Exp(x);
            sum += (float)Math.Exp(x) / e_x;
            Print("sum: " + e_x);
            return sum;
        }
        public static float Sigmoid(float x)
        {
            return (float)(1.0f / (1.0f + Math.Exp(-x)));
        }
        public static float Tanh(float x)
        {
            return (float)Math.Tanh(x);
        }
        public static float Tanh2(float x)
        {
            return (float)Math.Pow(Math.Tanh(x), 2);
        }
        public static float GenerateRandom()
        {
            Random random = new();
            return (float)random.NextDouble();
        }

        public static float GenerateXavierRandom()
        {
            // use Xavier initialization
            Random random = new();
            var value = (random.NextDouble() - random.NextDouble()) * Math.Sqrt(2.0 / 2.0);

            Print("GENERATED: " + value);
            return (float)value;
        }

        public static float Round(float value)
        {
            var decPlaces = (int)(((decimal)value % 1) * 100);
            var integralValue = (int)value;

            if (decPlaces >= 75)
            {
                return integralValue + 1;
            }
            else
            {
                return integralValue;
            }
        }

        public static float[] GenerateRandomMatrix(int size)
        {
            Random random = new();
            float[] result = new float[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = (float)(random.NextDouble() - random.NextDouble());
            }
            return result;
        }

        public static float[,] GenerateRandomMatrix(int xSize, int ySize)
        {
            Random random = new();
            float[,] result = new float[xSize, ySize];
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] = (float)(random.NextDouble() - random.NextDouble());
                }
            }
            return result;
        }
    }
}
