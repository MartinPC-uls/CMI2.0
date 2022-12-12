using CMI.Network;
using static CMI.Data.Normalizer;
using static CMI.Utils;
using static CMI.Matrix;
using CMI.Data;
using MusicXML_Parser;
using System.Xml;
using MusicXML_Parser.Music;
using CMI;

List<char[]> input_chunks = new();
List<char[]> output_chunks = new();
void Split(char[] input, char[] output, int chunkSize = 0)
{
    if (chunkSize == 0)
        chunkSize = input.Length;
    if (chunkSize > input.Length)
        throw new ArgumentException("Chunk size is greater than input length.");
    for (int i = 0; i < input.Length; i++)
    {
        char[] input_chunk = new char[chunkSize];
        char[] output_chunk = new char[chunkSize];
        if (i + chunkSize > input.Length)
            break;
        for (int j = 0; j < chunkSize; j++)
        {
            input_chunk[j] = input[i + j];
            output_chunk[j] = output[i + j];
        }
        input_chunks.Add(input_chunk);
        output_chunks.Add(output_chunk);
    }
}

Split(" LQS QULQXPSLPXEDNQIN QVXBVZQV]SXQUUJISVNSUVUSQGJGNEINNPQSEJEEPSSUVX@GLLVUL9@EELNJLNLP@EG>E@EI@ELDG@DL98BE=BDEEJL6EJNNQ".ToCharArray(), "LQS QULQXPSLPXEDNQIN QVXBVZQV]SXQUUJISVNSUVUSQGJGNEINNPQSEJEEPSSUVX@GLLVUL9@EELNJLNLP@EG>E@EI@ELDG@DL98BE=BDEEJL6EJNNQG".ToCharArray(), 10);

/*foreach (var input in input_chunks)
{
    foreach (var i in input)
    {
        Print(i, false);
    }
    Print("\n");
}*/

//LSTM lstm = new(3);
//LSTM lstm = new();
//lstm.LoadParameters();
//lstm.InitializeWeights();
//lstm.initialize();

MainProgram program = new();