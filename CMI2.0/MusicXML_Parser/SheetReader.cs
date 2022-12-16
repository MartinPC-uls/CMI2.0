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
        public Score Score;
        public Part Part;
        public List<MusicXml.Domain.MeasureElement> elements = new List<MusicXml.Domain.MeasureElement>();

        public static List<char?> DataSet;

        public string[] notes;
        public Dictionary<string, int> note_map;

        // create char of ASCII characters
        public char[] ascii = Enumerable.Range(0, 128).Select(i => (char)i).ToArray();
        public SheetReader(string file)
        {
            DataSet = new();
            File = file;
            Score = MusicXml.MusicXmlParser.GetScore(File);
            GetNotes();
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
        public void SaveNotes()
        {
            initialize_arrays();
            Console.WriteLine("Initialiazing process...");
            int p = 0;

            NoteType lastNoteType = NoteType.QUARTER;
            foreach (MeasureElement element in elements)
            {
                char? _note = null;
                if (element.Type == MeasureElementType.Note)
                {
                    MusicXml.Domain.Note note = (MusicXml.Domain.Note)element.Element;
                    //_note = GetNote(note);
                    if (note.Staff == 1)
                    {
                        if (note.IsChordTone)
                        {
                            DataSet.RemoveAt(DataSet.Count - 1);
                        }
                        _note = GetNote(note);
                        //lastNoteType = note.Type;
                        DataSet.Add(_note);
                        DataSet.Add(' ');
                    }
                    else
                    {
                        continue;
                    }
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
                p++;
                Console.WriteLine("Processing: " + Math.Round((double)p / elements.Count * 100, 0) + "%");
            }
            System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "dataset.txt").Close();
            // write DataSet into dataset.txt
            using var sw = new StreamWriter("dataset.txt");

            for (int i = 0; i < DataSet.Count; i++)
            {
                Console.WriteLine("Writing: " + Math.Round((double)i / (DataSet.Count - 1) * 100, 0) + "%");
                sw.Write(DataSet[i]);
            }
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

        public string GetSpecificNote(MusicXml.Domain.Note note)
        {
            string[] notes = new string[12] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            int alter = note.Pitch.Alter;
            bool condition = ((GetNoteIndex(note) - 1) + alter) >= 0;
            int value = condition ? ((GetNoteIndex(note) - 1) + alter) : 12 + ((GetNoteIndex(note) - 1) + alter);

            return value <= 11 ? notes[((GetNoteIndex(note) - 1) + alter)] : notes[((GetNoteIndex(note) - 1) + alter) - 12];
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
