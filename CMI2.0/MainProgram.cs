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
            //InitializeModel(false, false, "_staff1.txt"); // ????
        }

        private void AutoInitialize()
        {
            SheetName = "CMI-generated";
            Random random = new();
            Key = Tonalidad(random.Next(1, 25).ToString());
            Tempo = random.Next(100, 151);
            Autor = "IA CMI";
            //InitializeModel(false, false, "_staff1.txt"); // ???
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
            //TrainModel(true);
            //TrainModel(@"C:\Users\ghanv\OneDrive\Escritorio\DataSet\Codified\0\staff1", "staff1.txt", true);
            //TrainModel(@"C:\Users\ghanv\OneDrive\Escritorio\DataSet\Codified\0\staff2", "staff2.txt", true);
            InitializeModel(false, false, "staff1.txt", "staff2.txt");
        }

        public void TrainModel(string datasetPath, string nombreArchivosParametros, bool inicializarPesos, int epochs = 1)
        {
            string folderPath = datasetPath;
            string[] files = Directory.GetFiles(folderPath, "*.txt");

            LSTM model = new(inicializarPesos, nombreArchivosParametros);
            if (inicializarPesos)
            {
                model.InitializeGateValues();
            }
            else
            {
                model.LoadParameters(nombreArchivosParametros);
            }
            for (int i = 1; i <= epochs; i++)
            {
                foreach (string file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    char[] data = File.ReadAllText(folderPath + "\\" + fileName + ".txt").ToCharArray();
                    float[] normalizedData = Normalize(data);
                    model.Train(normalizedData, normalizedData, 1000000, 100000);
                }
                Console.Clear();
                Print("================= EPOCH " + i + "/" + epochs + " =================");
            }
        }

        public void InitializeModel(bool isRandom, bool inicializarParametros, string nombreArchivoParametros1, string nombreArchivoParametros2)
        {
            //float[] networkInputs = initializeInputs(isRandom);
            char[] staff1Inputs = @"S JO S LO S X T S Q O N O LO L N L J EJ V J M L M V S M L Q P V T M T L J L T Q L J O N T S L S U V JO JQ O O Q S S Q Q O GL I L IL N L L N O O N L EJ GJ EJ EI J BE J BE N N O N L N J EJ EJ V M MS L X X V T S T T S Q O N L LQ LQ K L N N O Q E Q S T S G N L K L N Q O N L N O Q O GL H H K L CG O O S Q O EL EL J X V U LQU L"
                                    .ToCharArray();
            char[] staff2Inputs = @"7 C B 6 4 @ > 2 0 < = 1 2 > < 0 ; DJ EJ 9 8 GJ @ DL 9 EH CH 7 6 EH > BJ 7 C B 6 4 @ = 9 > ; 9 9 2 4 6 7 9 < ; 9 2 2 > < 0 / DJ L EJM 9 8 GJ @ DL 9 E C 7 6 BE G @EH 4 3 4 6 3 4 <C 9E ;E 4 @ > 2 1 = ; / - E C 7 6 B 7 9 2 > < 0 / ;C <C 0 1 =E >E 2 3 ?G @G 8 9 E C 7 6 B > B C 7 5 A @ 4 2 > < 0 / ; 9 E C 7 6 2 7 < > < > 2 7> "
                                    .ToCharArray();

            float[] network1Inputs = Normalize(staff1Inputs);
            float[] network2Inputs = Normalize(staff2Inputs);
            LSTM lstm1 = initializeLSTM(network1Inputs, inicializarParametros, nombreArchivoParametros1);
            LSTM lstm2 = initializeLSTM(network2Inputs, inicializarParametros, nombreArchivoParametros2);
            SheetConfiguration music = getSheetConfiguration();
            XmlNode measure = getMeasure(music);
            WriteSheetMusic(lstm1, music, measure, 1);
            WriteSheetMusic(lstm2, music, measure, 2);
        }

        private void WriteSheetMusic(LSTM lstm, SheetConfiguration music, XmlNode measure, int staff)
        {
            music.Backup(measure);
            char prev_char1 = ' ';
            char next_char1 = lstm.Predicted_Output[1];
            char next_char1_2 = lstm.Predicted_Output[2];
            for (int i = 0; i < lstm.Predicted_Output.Count - 2; i++)
            {
                prev_char1 = lstm.Predicted_Output[i];
                next_char1 = lstm.Predicted_Output[i + 1];
                next_char1_2 = lstm.Predicted_Output[i + 2];
                music.Add(lstm.Predicted_Output[i], staff, measure, prev_char1, next_char1, next_char1_2);
            }
        }

        private LSTM initializeLSTM(float[] networkInputs, bool initializeParameters, string nombreArchivoParametros)
        {
            //lstm.LoadParameters(AppDomain.CurrentDomain.BaseDirectory + "scale1.txt");
            LSTM lstm = new(initializeParameters, nombreArchivoParametros);
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
            string __inputs = @"  GJN   EIN   GJN   EIN   GJN Z ]   EIN [ Z U   GJN S U V   EIN Q   GJN N   EIN N   GJN N   EIN N   GJN Z ]   EIN [ Z U   GJN S U V   EIN Q   EIN U   GJN Z L L L   Q S T   X V S   V T S   V   V V   X Y [   ] T V   X V S   V   V V   GLO [   EIN Z   GJN S Q S   ILQ U V X   EINQ U V X   EJ GJO N HLQT JNQV   GLO [   EJMQ Y   EHM S T Y   HLQ X V T   EHMQ X V T   EJ GJO M";
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
