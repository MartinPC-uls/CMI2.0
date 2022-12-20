using MusicXML_Parser.Music;
using MusicXml.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXML_Parser
{
    public class SheetReader
    {
        public string File { get; set; }
        public Music.Key? Key { get; set; }
        public int Tempo { get; set; }
        public int TimeSignatureDenominator { get; set; }
        public int TimeSignatureNumerator { get; set; }
        public MusicXml.Domain.Key ScoreKey;
        public char[] _Notes;
        public Score Score;
        public Part Part;
        public List<MusicXml.Domain.MeasureElement> elements = new List<MusicXml.Domain.MeasureElement>();

        public static List<char?> DataSet;
        public static List<char?> DataSet2;

        public string[] notes;
        public Dictionary<string, int> note_map;

        // create char of ASCII characters
        public char[] ascii = Enumerable.Range(0, 171).Select(i => (char)i).ToArray();
        public SheetReader(string file)
        {
            _Notes = new char[7];
            DataSet = new();
            DataSet2 = new();
            File = file;
            Score = MusicXml.MusicXmlParser.GetScore(File);
            ScoreKey = Score.Parts[0].Measures[0].Attributes.Key;
            GetScaleNotes();
            GetNotes();
        }

        public void GetScaleNotes()
        {
            switch (ScoreKey.Fifths)
            {
                case 0:
                    Console.WriteLine("C MAJOR SCALE");
                    _Notes = "CDEFGAB".ToCharArray();
                    break;
                case 1:
                    _Notes = "GABCDE".ToCharArray();
                    break;
                case 2:
                    Console.WriteLine("D MAJOR SCALE");
                    _Notes = "DEGAB".ToCharArray();
                    break;
                case 3:
                    _Notes = "ABDE".ToCharArray();
                    break;
                case 4:
                    _Notes = "EAB".ToCharArray();
                    break;
                case 5:
                    _Notes = "BE".ToCharArray();
                    break;
                case 6:
                    _Notes = "B".ToCharArray();
                    break;
                case -1:
                    _Notes = "FGACDE".ToCharArray();
                    break;
                case -2:
                    _Notes = "CDFGA".ToCharArray();
                    break;
                case -3:
                    _Notes = "FGCD".ToCharArray();
                    break;
                case -4:
                    _Notes = "CFG".ToCharArray();
                    break;
                case -5:
                    _Notes = "FC".ToCharArray();
                    break;
                default:
                    _Notes = "CDEFGAB".ToCharArray();
                    break;
            }
        }

        /*
         * [Note, Alter, Octave, NoteType, IsChord]
         * Example:
         *      [C, 0, 5, QUARTER, false]
         */

        public void GetNotes()
        {
            Part = Score.Parts[0];
            foreach (Measure measure in Part.Measures)
            {
                List<MeasureElement> measureElements = measure.MeasureElements;
                foreach (MeasureElement measureElement in measureElements)
                {
                    elements.Add(measureElement);
                }
            }
        }
        public void PrintNotes()
        {
            initialize_arrays();
            foreach (MeasureElement element in elements)
            {
                if (element.Type == MeasureElementType.Note)
                {
                    MusicXml.Domain.Note note = (MusicXml.Domain.Note)element.Element;
                    PrintSpecificNote(note);
                }
                else if (element.Type == MeasureElementType.Forward)
                {
                    //Forward forward = (Forward)element.Element;
                    //Console.WriteLine("forward: " + forward.Duration);
                }
                else if (element.Type == MeasureElementType.Backup)
                {
                    //Backup backup = (Backup)element.Element;
                    //Console.WriteLine("backup: " + backup.Duration);
                }

            }
        }
        public void SaveNotes(string path = "", string path2 = "")
        {
            if (path.Equals(""))
                path = AppDomain.CurrentDomain.BaseDirectory + "dataset.txt";

            initialize_arrays();
            Console.WriteLine("Initialiazing process...");
            int p = 0;
            List<MusicXml.Domain.Note> notes = new();
            Console.WriteLine("Processing...");
            foreach (MeasureElement element in elements)
            {
                if (element.Type == MeasureElementType.Note)
                {
                    notes.Add((MusicXml.Domain.Note)element.Element);
                }
                p++;
            }
            int k = 0;
            foreach (MusicXml.Domain.Note note in notes)
            {
                MusicXml.Domain.Note _note = note;
                if (k + 1 >= notes.Count)
                {
                    break;
                }
                MusicXml.Domain.Note? next_note = notes[k + 1];
                if (_note.Staff == 1)
                {
                    DataSet.Add(GetNote(_note));
                    if (!next_note.IsChordTone || next_note == null)
                    {
                        DataSet.Add(GetTime(_note));
                        if (_note.Dot)
                        {
                            DataSet.Add((char)133);
                        }
                    }
                }
                else if (_note.Staff == 2)
                {
                    DataSet2.Add(GetNote(_note));
                    if (!next_note.IsChordTone || next_note == null)
                    {
                        DataSet2.Add(GetTime(_note));
                        if (_note.Dot)
                        {
                            DataSet2.Add((char)133);
                        }
                    }
                }
                k++;
            }
            Console.WriteLine("Processing completed");
            System.IO.File.Create(path).Close();
            // write DataSet into dataset_staff1.txt
            List<char> dataset = DataSet
                                        .Where(c => c.HasValue)
                                        .Select(c => c.Value)
                                        .ToList();
            List<char> dataset2 = DataSet2
                                        .Where(c => c.HasValue)
                                        .Select(c => c.Value)
                                        .ToList();

            byte[] data = System.Text.Encoding.UTF8.GetBytes(dataset.ToArray());
            byte[] data2 = System.Text.Encoding.UTF8.GetBytes(dataset2.ToArray());

            using var sw = new StreamWriter(path);
            Console.WriteLine("Writing staff 1...");
            for (int i = 0; i < DataSet.Count; i++)
            {
                sw.BaseStream.Write(data, i, 1);
            }
            Console.WriteLine("Writing completed.");
            Console.WriteLine("Writing staff 2...");
            using var sw2 = new StreamWriter(path2);
            for (int i = 0; i < DataSet2.Count; i++)
            {
                sw2.BaseStream.Write(data2, i, 1);
            }
            Console.WriteLine("Writing completed");
            Console.WriteLine("Disonancia total: " + ((float)Alterations / (float)Notes) * 100f + "%");
            Console.WriteLine("Total notas disonantes: " + Alterations);
            Console.WriteLine("Total notas: " + Notes + "\n");
        }
        public char? GetDot(MusicXml.Domain.Note note)
        {
            if (note.Dot)
                return (char)133;

            return null;
        }
        public char GetTime(MusicXml.Domain.Note note)
        {
            if (note.Type == NoteType._1024TH.Value)
                return (char)161;
            else if (note.Type == NoteType._512TH.Value)
                return (char)162;
            else if (note.Type == NoteType._256TH.Value)
                return (char)163;
            else if (note.Type == NoteType._128TH.Value)
                return (char)164;
            else if (note.Type == NoteType._64TH.Value)
                return (char)165;
            else if (note.Type == NoteType._32ND.Value)
                return (char)166;
            else if (note.Type == NoteType._16TH.Value)
                return (char)167;
            else if (note.Type == NoteType.EIGHTH.Value)
                return (char)168;
            else if (note.Type == NoteType.QUARTER.Value)
                return (char)169;
            else if (note.Type == NoteType.HALF.Value)
                return (char)170;
            else if (note.Type == NoteType.WHOLE.Value)
                return (char)171;

            if (note.Duration == 16)
                return (char)171;
            else if (note.Duration == 8)
                return (char)170;
            else if (note.Duration == 4)
                return (char)169;
            else if (note.Duration == 2)
                return (char)168;
            else if (note.Duration == 1)
                return (char)167;

            return (char)169;
        }
        public char translate(string note, int octave, int duration, bool isChord)
        {
            string _note = note + octave;
            char note_translated = ascii[note_map[_note]];
            return note_translated;
        }

        public void PrintSpecificNote(MusicXml.Domain.Note note)
        {
            if (note.IsRest)
            {
                //Console.WriteLine("[REST, _, " + note.Duration + ", _]");
            }
            else
            {
                //Console.WriteLine("[" + GetSpecificNote(note) + ", " + note.Pitch.Octave + ", " + note.Duration + ", " + note.IsChordTone + "] | " + note.Staff);
                print(translate(GetSpecificNote(note), note.Pitch.Octave, note.Duration, note.IsChordTone), false);
            }
        }
        public char GetNote(MusicXml.Domain.Note note)
        {
            if (!note.IsRest)
                return translate(GetSpecificNote(note), note.Pitch.Octave, note.Duration, note.IsChordTone);

            return ' ';
        }
        public void print(object? s, bool skipLine = true)
        {
            if (skipLine)
            {
                Console.WriteLine(s);
                return;
            }
            Console.Write(s);
        }

        public void initialize_arrays()
        {
            notes = new string[88];
            notes[0] = "A0";
            notes[1] = "A#0";
            notes[2] = "B0";
            string[] base_notes = new string[12];
            base_notes[0] = "C";
            base_notes[1] = "C#";
            base_notes[2] = "D";
            base_notes[3] = "D#";
            base_notes[4] = "E";
            base_notes[5] = "F";
            base_notes[6] = "F#";
            base_notes[7] = "G";
            base_notes[8] = "G#";
            base_notes[9] = "A";
            base_notes[10] = "A#";
            base_notes[11] = "B";

            int octave = 1;
            for (int j = 3, z = 0; j < notes.Length; j++, z++)
            {
                if (base_notes[z].Equals("B"))
                {
                    notes[j] = base_notes[z] + octave;
                    octave += 1;
                    z = -1;
                    continue;
                }
                notes[j] = base_notes[z] + octave;
            }

            note_map = new Dictionary<string, int>();
            for (int i = 0; i < notes.Length; i++)
            {
                note_map.Add(notes[i], i + 33);
            }

        }

        private static int Alterations;
        private static int Notes;

        public string GetSpecificNote(MusicXml.Domain.Note note)
        {
            string[] notes = new string[12] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            int alter = note.Pitch.Alter;
            Notes++;
            if (_Notes.Contains(note.Pitch.Step) && alter != 0)
            {
                Alterations++;
            }
            else if (!_Notes.Contains(note.Pitch.Step) && alter == 0)
            {
                Alterations++;
            }
            bool condition = ((GetNoteIndex(note) - 1) + alter) >= 0;
            int value = condition ? ((GetNoteIndex(note) - 1) + alter) : 12 + ((GetNoteIndex(note) - 1) + alter);

            var noteIndex = ((GetNoteIndex(note) - 1) + alter);
            if (noteIndex == -1)
                noteIndex = 11;
            return value <= 11 ? notes[noteIndex] : notes[((GetNoteIndex(note) - 1) + alter) - 12];
        }
        public int GetNoteIndex(MusicXml.Domain.Note note)
        {
            switch (note.Pitch.Step)
            {
                case 'C':
                    return 1;
                case 'D':
                    return 3;
                case 'E':
                    return 5;
                case 'F':
                    return 6;
                case 'G':
                    return 8;
                case 'A':
                    return 10;
                case 'B':
                    return 12;
                default:
                    return 0;
            }
        }
    }
}
