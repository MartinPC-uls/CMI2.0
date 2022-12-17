using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMI.Network;
using static CMI.ConsoleUtils;
using static CMI.Data.Normalizer;
using MusicXML_Parser;
using System.Xml;

namespace CMI
{
    public class MainProgram
    {
        public string SheetName { get; set; }
        public string Key { get; set; }
        public int Tempo { get; set; }
        public string Autor { get; set; }
        
        public MainProgram()
        {
            Init();
        }

        private void Init()
        {
            Console.Clear();
            Print("BIENVENIDO A LA IA GENERADORA DE MÚSICA");
            Print("¿Quiere elegir usted los parámetros? (si/no): ", false);
            string opcion = Console.ReadLine();
            if (opcion.ToLower().Equals("si")) UserInitialize();
            else if (opcion.ToLower().Equals("no")) AutoInitialize();
            else { Print("Opción no válida"); EnterKeyToContinue(); Init(); return;}
        }

        private void UserInitialize()
        {
            SetSheetValues();
            Console.Clear();
            ShowSheetValues();
            SetFirstNote();
            InitializeModel(false);
        }

        private void AutoInitialize()
        {
            SheetName = "CMI-generated";
            Random random = new();
            Key = Tonalidad(random.Next(1, 25).ToString());
            Tempo = random.Next(100, 151);
            Autor = "IA CMI";
            InitializeModel(false);
        }

        private void SetSheetValues()
        {
            SetSheetName();
            SetSheetKey();
            SetSheetTempo();
            SetAutorName();
        }

        private void SetFirstNote()
        {
            PrintNoteOptions();
            string? nota = Console.ReadLine();
            if (!CheckNote(nota))
            {
                Print("Nota no válida");
                EnterKeyToContinue();
                SetFirstNote();
                return;
            }
        }

        private bool CheckNote(string? nota)
        {
            List<string> opcionesNota = new(){"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"};
            return opcionesNota.Contains(nota.ToUpper());
        }

        private void PrintNoteOptions()
        {
            Print("Elija una de las siguientes notas musicales con la que desea que empiece la pieza musical:\n");
            Print("  ┌──────┬──────┬─────────────┬──────┬──────┬──────┐\n");
            Print("  │      C#     D#            F#     G#     A#     │\n");
            Print("  └──────┴──────┴─────────────┴──────┴──────┴──────┘\n");
            Print("  ┌──────┬──────┬──────┬──────┬──────┬──────┬──────┐\n");
            Print("  │   C  │   D  │   E  │   F  │   G  │   A  │   B  │\n");
            Print("  └──────┴──────┴──────┴──────┴──────┴──────┴──────┘\n");
            Console.Write(">>: ");
        }

        private void ShowSheetValues()
        {
            Print("###################################################################");
            Print("Nombre de la pieza musical: " + SheetName);
            Print("Tonalidad: " + Key);
            Print("Tempo: " + Tempo + " bpm");
            Print("Tiempo: 4/4");
            Print("Autor: " + Autor);
            Print("###################################################################");
        }

        private void SetAutorName()
        {
            Console.Clear();
            Print("Ingrese nombre del autor: ",false);
            string? autor = Console.ReadLine();
            if(autor == "") 
            {
                Print("Nombre invalido");
                EnterKeyToContinue();
                SetAutorName();
                return;
            }
            Autor = autor;
        }

        private void SetSheetTempo()
        {
            Console.Clear();
            Print("Digite un tempo (1-500): ",false);
            string? tempo = Console.ReadLine();
            if (!CheckTempo(tempo))
            {
                Print("Tempo invalido...");
                EnterKeyToContinue();
                SetSheetTempo();
                return;
            }
            int _tempo = int.Parse(tempo);
            Tempo = _tempo;
        }

        private bool CheckTempo(string? tempo)
        {
            if (!(tempo == ""))
            {
                if (int.TryParse(tempo, out int _tempo))
                {
                    if (_tempo >= 1 && _tempo <= 500)
                        return true;
                }
            }
            return false;
        }

        private void SetSheetKey()
        {
            PrintKeyOptions();
            Print("Escriba el número o el nombre de la tonalidad musical: ",false);
            string? opcionEscogida = Console.ReadLine();
            string tonalidad = Tonalidad(opcionEscogida);
            if (tonalidad == "INVALID")
            {
                Print("La tonalidad ingresada no es válida, intente de nuevo...");
                EnterKeyToContinue();
                SetSheetKey();
                return;
            }
            Key = tonalidad;
        }

        private void PrintKeyOptions()
        {
            Console.Clear();
            Print("Elija una de las siguientes tonalidades musicales");
            Print("1. C Major\t\t13. A minor");
            Print("2. C# Major\t\t14. A# minor");
            Print("3. D Major\t\t15. B minor");
            Print("4. D# Major\t\t16. C minor");
            Print("5. E Major\t\t17. C# minor");
            Print("6. F Major\t\t18. D minor");
            Print("7. F# Major\t\t19. D# minor");
            Print("8. G Major\t\t20. E minor");
            Print("9. G# Major\t\t21. F minor");
            Print("10. A Major\t\t22. F# minor");
            Print("11. A# Major\t\t23. G minor");
            Print("12. B Major\t\t24. G# minor\n");
        }

        private void SetSheetName()
        {
            Console.Clear();
            Console.Write("Escriba un nombre para la composición musical: ");
            string? nombre = Console.ReadLine();
            if(nombre == "")
            {
                Print("Nombre invalido...");
                EnterKeyToContinue();
                SetSheetName();
            }
            Console.Clear();
        }

        public MainProgram(bool train)
        {
            if (!train)
            {
                _ = new MainProgram();
                return;
            }
            TrainModel(false);
            InitializeModel(false);
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
            InitializeModel(false);
        }

        public void TrainModel(bool inicializarPesos, string nombreArchivoParametros = "")
        {
            Print("Entrenando el modelo...");
            LSTM lstm = new();
            if (!inicializarPesos)
            {
                //lstm.LoadParameters(nombreArchivoParametros);
                lstm.LoadParameters();
            }
            else lstm.InitializeGateValues();
            char[] data   = @"  ^ ` ^ ] [   ` ` ^ ] [ [ Y   Y W Y V     V V V V b b       ^ ] ] ] ^ ^     ^ ` ^ ] [   ` ` ^ ] [ [ Y   Y W Y V     V V V V b b".ToCharArray();
            char[] output = @"  ^ ` ^ ] [   ` ` ^ ] [ [ Y   Y W Y V     V V V V b b       ^ ] ] ] ^ ^     ^ ` ^ ] [   ` ` ^ ] [ [ Y   Y W Y V     V V V V b b".ToCharArray();
            
            float[] normalizedInput= Normalize(data);
            float[] normalizedOutput = Normalize(output);
            lstm.Train(normalizedInput, normalizedOutput, 1000000, 100000);
            //lstm.Train(normalized_input, __output, 10000000, 1000000);
        }

        public void InitializeModel(bool isRandom)
        {
            float[] networkInputs = initializeInputs(isRandom);
            LSTM lstm = initializeLSTM(networkInputs);
            SheetConfiguration music = getSheetConfiguration();
            XmlNode measure = getMeasure(music);
            WriteSheetMusic(lstm, music, measure);
        }

        private void WriteSheetMusic(LSTM lstm, SheetConfiguration music, XmlNode measure)
        {
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

        private LSTM initializeLSTM(float[] networkInputs)
        {
            //lstm.LoadParameters(AppDomain.CurrentDomain.BaseDirectory + "scale1.txt");
            LSTM lstm = new();
            lstm.LoadParameters();
            lstm.Predict(networkInputs);
            return lstm;
        }

        private XmlNode getMeasure(SheetConfiguration music)
        {
            XmlDocument score = music.Init(SheetName + ".xml", MusicXML_Parser.Music.Key.CMajor);
            //XmlDocument score = sheet.Load(AppDomain.CurrentDomain.BaseDirectory + "score.xml");
            XmlNode node = score.SelectSingleNode("score-partwise/part[@id='P1']");
            return music.AddMeasure(node);
        }

        private float[] initializeInputs(bool isRandom)
        {
            string __inputs = "_ S T ] S T X S T [ P T Y P T T O P W M P V J M P R V Y";
            char[] inputs = new char[__inputs.Length];
            if (isRandom) inputs = getRandomInput(__inputs.Length);
            else inputs = __inputs.ToCharArray();
            return Normalize(inputs);
        }

        private SheetConfiguration getSheetConfiguration()
        {
            return new SheetConfiguration()
            {
                WorkTitle = SheetName,
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
        }

        private char[] getRandomInput(int maxLenght)
        {
            //inputs = __inputs.Reverse().ToArray();
            char[] inputs = new char[maxLenght];
            Random random = new();
            for (int i = 0; i < maxLenght; i++)
                inputs[i] = (char)random.Next(32, 121);
            return inputs;
        }

        public string Tonalidad(string? tonalidad)
        {
            Dictionary<string,string> opciones = new()
            {
                { "1", "C Major" },
                { "2", "C# Major" },
                { "3", "D Major" },
                { "4", "D# Major" },
                { "5", "E Major" },
                { "6", "F Major" },
                { "7", "F# Major" },
                { "8", "G Major" },
                { "9", "G# Major" },
                { "10", "A Major" },
                { "11", "A# Major" },
                { "12", "B Major" },
                { "13", "A minor" },
                { "14", "A# minor" },
                { "15", "B minor" },
                { "16", "C minor" },
                { "17", "C# minor" },
                { "18", "D minor" },
                { "19", "D# minor" },
                { "20", "E minor" },
                { "21", "F minor" },
                { "22", "F# minor" },
                { "23", "G minor" },
                { "24", "G# minor" }
            };
            return opciones.ContainsKey(tonalidad) ? opciones[tonalidad] : "INVALID";
        }
    }
}
