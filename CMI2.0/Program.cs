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
List<float[]> inputs = new();
List<float[]> outputs = new();
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

void NormalizeData(List<char[]> input_chunks, List<char[]> output_chunks)
{
    foreach (char[] input_chunk in input_chunks)
    {
        inputs.Add(Normalize(input_chunk));
    }
    foreach (char[] output_chunk in output_chunks)
    {
        outputs.Add(Normalize(output_chunk));
    }
}

Split("_ S T ] S T X S T [ P T Y P T T O P W M P V J M P R V Y".ToCharArray(), " S T ] S T X S T [ P T Y P T T O P W M P V J M P R V Y ".ToCharArray(), 5);
NormalizeData(input_chunks, output_chunks);

MainProgram program = new(true);