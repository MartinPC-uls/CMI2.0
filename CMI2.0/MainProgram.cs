using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMI.Network;
using static CMI.Utils;
using static CMI.Data.Normalizer;
using MusicXML_Parser;
using System.Xml;

namespace CMI
{
    public class MainProgram
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public int Tempo { get; set; }
        public string Autor { get; set; }
        public string NotaPrincipal { get; set; }
        public void print(string text)
        {
            Console.WriteLine(text);
        }
        public MainProgram()
        {
            print("BIENVENIDO A LA IA GENERADORA DE MÚSICA");
            Console.Write("¿Quiere elegir usted los parámetros? (si/no): ");
            string opcion = Console.ReadLine();
            if (opcion.Equals("si"))
            {
                Console.Write("Escriba un nombre para la composición musical: ");
                string? name = Console.ReadLine();
                Name = name;
                Console.Clear();
                print("###################################################################");
                print("\t\t\t\t" + name);
                print("###################################################################\n");
                print("Elija una de las siguientes tonalidades musicales");
                print("1. C Major\t\t13. A minor");
                print("2. C# Major\t\t14. A# minor");
                print("3. D Major\t\t15. B minor");
                print("4. D# Major\t\t16. C minor");
                print("5. E Major\t\t17. C# minor");
                print("6. F Major\t\t18. D minor");
                print("7. F# Major\t\t19. D# minor");
                print("8. G Major\t\t20. E minor");
                print("9. G# Major\t\t21. F minor");
                print("10. A Major\t\t22. F# minor");
                print("11. A# Major\t\t23. G minor");
                print("12. B Major\t\t24. G# minor\n");
                Console.Write("Escriba el número o el nombre de la tonalidad musical: ");
                string? tonalidad = Console.ReadLine();
                string _tonalidad = Tonalidad(tonalidad);
                Key = _tonalidad;
                Console.Write("Elija un tempo: ");
                string? tempo = Console.ReadLine();
                int _tempo = int.Parse(tempo);
                Tempo = _tempo;
                Console.Write("Ingrese nombre del autor: ");
                string? autor = Console.ReadLine();
                Autor = autor;
                Console.Clear();
                print("###################################################################");
                print("\t\t\tNombre de la pieza musical: " + Name);
                print("\t\t\tTonalidad: " + Key);
                print("\t\t\tTempo: " + Tempo + " bpm");
                print("\t\t\tTiempo: 4/4");
                print("\t\t\tAutor: " + Autor);
                print("###################################################################");
                print("Elija una de las siguientes notas musicales con la que desea que empiece la pieza musical:\n");
                print(" C#  D#    F#  G#  A#");
                print("C  D    E F   G  A  B\n\n");
                Console.Write(">>: ");
                string? nota = Console.ReadLine();

                InicializarModelo(false);
            }
            else
            {
                Name = "CMI-generated";
                Random random = new();
                Key = Tonalidad(random.Next(1, 25).ToString());
                Tempo = random.Next(100, 151);
                Autor = "IA CMI";
                if (opcion.Equals(""))
                    InicializarModelo(false);
                else if (opcion.Equals("no"))
                    InicializarModelo(true);
            }


        }
        
        public MainProgram(bool train)
        {
            if (!train)
            {
                _ = new MainProgram();
                return;
            }

            EntrenarModelo(false);
            
        }

        public MainProgram(List<float[]> inputs, List<float[]> outputs)
        {
            EntrenarModelo(inputs, outputs);
        }

        public void EntrenarModelo(List<float[]> inputs, List<float[]> outputs)
        {
            //Print("Entrenando el modelo...");
            //LSTM lstm = new();
            //lstm.initialize();
            //lstm.Train(inputs, outputs, 10000000, 1000000);

            InicializarModelo(false);

            
        }

        public void EntrenarModelo(bool inicializarPesos, string nombreArchivoParametros = "")
        {
            Print("Entrenando el modelo...");
            LSTM lstm = new();
            if (!inicializarPesos)
            {
                //lstm.LoadParameters(nombreArchivoParametros);
                lstm.LoadParameters();
            }
            else
            {
                lstm.initialize();
            }
            char[] data   = @"  ^ ` ^ ] [   ` ` ^ ] [ [ Y   Y W Y V     V V V V b b       ^ ] ] ] ^ ^     ^ ` ^ ] [   ` ` ^ ] [ [ Y   Y W Y V     V V V V b b".ToCharArray();
            char[] output = @"  ^ ` ^ ] [   ` ` ^ ] [ [ Y   Y W Y V     V V V V b b       ^ ] ] ] ^ ^     ^ ` ^ ] [   ` ` ^ ] [ [ Y   Y W Y V     V V V V b b".ToCharArray();
            
            float[] normalized_input= Normalize(data);
            float[] normalized_output = Normalize(output);
            lstm.Train(normalized_input, normalized_output, 1000000, 100000);
            //lstm.Train(normalized_input, __output, 10000000, 1000000);

            InicializarModelo(false);
        }

        public void InicializarModelo(bool isRandom)
        {
            string __inputs = "_ S T ] S T X S T [ P T Y P T T O P W M P V J M P R V Y";
            char[] inputs = new char[__inputs.Length];
            if (isRandom)
            {
                //inputs = __inputs.Reverse().ToArray();
                Random random = new();
                for (int i = 0; i < inputs.Length; i++)
                {
                    inputs[i] = (char)random.Next(32, 121);
                }
            }
            else
            {
                inputs = __inputs.ToCharArray();
            }
            float[] _inputs = Normalize(inputs);
            LSTM lstm = new();

            //lstm.LoadParameters(AppDomain.CurrentDomain.BaseDirectory + "scale1.txt");
            lstm.LoadParameters();

            lstm.Prediction(_inputs);

            SheetConfiguration music = new SheetConfiguration()
            {
                WorkTitle = Name,
                Composer = Autor,
                Accidental = true,
                Beam = true,
                Print_NewPage = true,
                Print_NewSystem = true,
                Stem = true,
                WordFont = "FreeSerif",
                WordFontSize = 10,
                LyricFont = "FreeSerif",
                LyricFontSize = 11,
                CreditWords_Title = "Partitura generada por CMI",
                CreditWords_Subtitle = "",
                CreditWords_Composer = "Autores CMI: Felipe Barrientos, Martín Pizarro, Roberto Verdugo.",
                StaffLayout = 2,
                StaffDistance = 65,
                Divisions = 24,
                Tempo = Tempo,
                TimeBeats = 4,
                TimeBeatType = 4
            };
            XmlDocument score = music.Init(Name + ".xml", MusicXML_Parser.Music.Key.CMajor);
            //XmlDocument score = sheet.Load(AppDomain.CurrentDomain.BaseDirectory + "score.xml");
            XmlNode node = score.SelectSingleNode("score-partwise/part[@id='P1']");
            XmlNode measure = music.AddMeasure(node);

            char prev_char = ' ';
            char[] next_chars = lstm.Predicted_Output.Skip(1).ToArray();
            int j = 0;
            foreach (var predicted in lstm.Predicted_Output)
            {
                //music.Add(predicted, 1, measure);
                if (predicted == ' ') // we're not considering silences
                {
                    prev_char = predicted;
                    j++;
                    continue;
                }
                //Console.WriteLine(predicted);
                music.Add(predicted, 1, measure, prev_char, next_chars);
                prev_char = predicted;
                next_chars = lstm.Predicted_Output.Skip(j + 1).ToArray();
                j++;
            }
        }

        public string Tonalidad(string? tonalidad)
        {
            switch (tonalidad)
            {
                case "1":
                case "C Major":
                    return "C Major";
                case "2":
                case "C# Major":
                    return "C# Major";
                case "3":
                case "D Major":
                    return "D Major";
                case "4":
                case "D# Major":
                    return "D# Major";
                case "5":
                case "E Major":
                    return "E Major";
                case "6":
                case "F Major":
                    return "F Major";
                case "7":
                case "F# Major":
                    return "F# Major";
                case "8":
                case "G Major":
                    return "G Major";
                case "9":
                case "G# Major":
                    return "G# Major";
                case "10":
                case "A Major":
                    return "A Major";
                case "11":
                case "A# Major":
                    return "A# Major";
                case "12":
                case "B Major":
                    return "B Major";
                case "13":
                case "A minor":
                    return "A minor";
                case "14":
                case "A# minor":
                    return "A# minor";
                case "15":
                case "B minor":
                    return "B minor";
                case "16":
                case "C minor":
                    return "C minor";
                case "17":
                case "C# minor":
                    return "C# minor";
                case "18":
                case "D minor":
                    return "D minor";
                case "19":
                case "D# minor":
                    return "D# minor";
                case "20":
                case "E minor":
                    return "E minor";
                case "21":
                case "F minor":
                    return "F minor";
                case "22":
                case "F# minor":
                    return "F# minor";
                case "23":
                case "G minor":
                    return "G minor";
                case "24":
                case "G# minor":
                    return "G# minor";
                default:
                    return "INVALID";
            }
        }
    }
}
