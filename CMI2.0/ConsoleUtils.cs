using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMI
{
    public class ConsoleUtils
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
        
        public static void EnterKeyToContinue()
        {
            string aux = Console.ReadLine();
            Console.Clear();
        }
    }
}
