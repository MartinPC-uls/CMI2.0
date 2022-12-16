using MusicXML_Parser.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXML_Parser
{
    public class SheetConfiguration
    {

        private readonly static string EXTENSION = ".xml";
        private readonly static string PATH = AppDomain.CurrentDomain.BaseDirectory;

        public string? WorkTitle { get; set; }
        public string? Composer { get; set; }
        public bool Accidental { get; set; }
        public bool Beam { get; set; }
        public bool Print_NewPage { get; set; }
        public bool Print_NewSystem { get; set; }
        public bool Stem { get; set; }
        public string? WordFont { get; set; }
        public int WordFontSize { get; set; }
        public string? LyricFont { get; set; }
        public int LyricFontSize { get; set; }
        public string? CreditWords_Title { get; set; }
        public string? CreditWords_Subtitle { get; set; }
        public string? CreditWords_Composer { get; set; }
        public int StaffLayout { get; set; }
        public int StaffDistance { get; set; }
        public int Divisions { get; set; } // 24 by default
        public int Tempo { get; set; }
        public int TimeBeats { get; set; }
        public int TimeBeatType { get; set; }
        public static XmlDocument? Score { get; set; }
        public static int Measures { get; set; }
        public static string FileName { get; set; }

        public SheetConfiguration()
        {
            initialize_arrays();
        }

        public XmlDocument Init(string fileName, Key? key = null)
        {
            XmlDocument doc = new XmlDocument();

            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            XmlNode docType = doc.CreateDocumentType("score-partwise", "-//Recordare//DTD MusicXML 3.1 Partwise//EN",
                                                      "http://www.musicxml.org/dtds/partwise.dtd", null);
            doc.AppendChild(docType);

            XmlNode score_partwise = doc.CreateElement("score-partwise");
            XmlAttribute version = doc.CreateAttribute("version");
            version.Value = "2.0";
            score_partwise.Attributes.Append(version);

            XmlNode work = doc.CreateElement("work");
            XmlNode work_title = doc.CreateElement("work-title");
            work_title.InnerText = WorkTitle ?? "Untitled";
            work.AppendChild(work_title);
            score_partwise.AppendChild(work);

            XmlNode identification = doc.CreateElement("identification");
            XmlNode creator = doc.CreateElement("creator");
            XmlAttribute type = doc.CreateAttribute("type");
            type.Value = "composer";
            creator.Attributes.Append(type);
            creator.InnerText = Composer ?? "Unknown";
            identification.AppendChild(creator);

            XmlNode encoding = doc.CreateElement("encoding");
            XmlNode software = doc.CreateElement("software");
            software.InnerText = "MusicXML Parser";
            XmlNode encoding_date = doc.CreateElement("encoding-date");
            encoding_date.InnerText = DateTime.Now.ToString("yyyy-MM-dd");
            XmlNode supportsAccidental = doc.CreateElement("supports");
            XmlAttribute accidentalElement = doc.CreateAttribute("element");
            accidentalElement.Value = "accidental";
            XmlAttribute accidentalType = doc.CreateAttribute("type");
            accidentalType.Value = Accidental ? "yes" : "no";
            supportsAccidental.Attributes.Append(accidentalElement);
            supportsAccidental.Attributes.Append(accidentalType);
            XmlNode supportsBeam = doc.CreateElement("supports");
            XmlAttribute beamElement = doc.CreateAttribute("element");
            beamElement.Value = "beam";
            XmlAttribute beamType = doc.CreateAttribute("type");
            beamType.Value = Beam ? "yes" : "no";
            supportsBeam.Attributes.Append(beamElement);
            supportsBeam.Attributes.Append(beamType);
            XmlNode supportsPrintNewPage = doc.CreateElement("supports");
            XmlAttribute printNewPageElement = doc.CreateAttribute("element");
            printNewPageElement.Value = "print";
            XmlAttribute printNewPageAttribute = doc.CreateAttribute("attribute");
            printNewPageAttribute.Value = "new-page";
            XmlAttribute printNewPageType = doc.CreateAttribute("type");
            printNewPageType.Value = Print_NewPage ? "yes" : "no";
            supportsPrintNewPage.Attributes.Append(printNewPageElement);
            supportsPrintNewPage.Attributes.Append(printNewPageAttribute);
            supportsPrintNewPage.Attributes.Append(printNewPageType);
            XmlNode supportsPrintNewSystem = doc.CreateElement("supports");
            XmlAttribute printNewSystemElement = doc.CreateAttribute("element");
            printNewSystemElement.Value = "print";
            XmlAttribute printNewSystemAttribute = doc.CreateAttribute("attribute");
            printNewSystemAttribute.Value = "new-system";
            XmlAttribute printNewSystemType = doc.CreateAttribute("type");
            printNewSystemType.Value = Print_NewSystem ? "yes" : "no";
            supportsPrintNewSystem.Attributes.Append(printNewSystemElement);
            supportsPrintNewSystem.Attributes.Append(printNewSystemAttribute);
            supportsPrintNewSystem.Attributes.Append(printNewSystemType);
            XmlNode supportsStem = doc.CreateElement("supports");
            XmlAttribute stemElement = doc.CreateAttribute("element");
            stemElement.Value = "stem";
            XmlAttribute stemType = doc.CreateAttribute("type");
            stemType.Value = Stem ? "yes" : "no";
            supportsStem.Attributes.Append(stemElement);
            supportsStem.Attributes.Append(stemType);

            encoding.AppendChild(software);
            encoding.AppendChild(encoding_date);
            identification.AppendChild(encoding);
            identification.AppendChild(supportsAccidental);
            identification.AppendChild(supportsBeam);
            identification.AppendChild(supportsPrintNewPage);
            identification.AppendChild(supportsPrintNewSystem);
            identification.AppendChild(supportsStem);

            score_partwise.AppendChild(identification);

            // we can also define the credit words here
            // we can also define the defaults here

            XmlNode part_list = doc.CreateElement("part-list");
            XmlNode score_part = doc.CreateElement("score-part");
            XmlAttribute id = doc.CreateAttribute("id");
            id.Value = "P1";
            score_part.Attributes.Append(id);
            XmlNode part_name = doc.CreateElement("part-name");
            part_name.InnerText = "Piano";
            score_part.AppendChild(part_name);
            part_list.AppendChild(score_part);
            score_partwise.AppendChild(part_list);

            XmlNode part1 = doc.CreateElement("part");
            XmlAttribute _id = doc.CreateAttribute("id");
            _id.Value = "P1";
            part1.Attributes.Append(_id);
            XmlNode measure0 = doc.CreateElement("measure");
            XmlAttribute number = doc.CreateAttribute("number");
            number.Value = "0";
            measure0.Attributes.Append(number);
            XmlAttribute _implicit = doc.CreateAttribute("implicit");
            _implicit.Value = "yes";
            measure0.Attributes.Append(_implicit);

            XmlNode print = doc.CreateElement("print");
            XmlNode staffLayout = doc.CreateElement("staff-layout");
            XmlAttribute staffLayoutNumber = doc.CreateAttribute("number");
            staffLayoutNumber.Value = StaffLayout.ToString();
            XmlAttribute staffLayoutStaffDistance = doc.CreateAttribute("staff-distance");
            staffLayoutStaffDistance.Value = StaffDistance.ToString();
            staffLayout.Attributes.Append(staffLayoutNumber);
            staffLayout.Attributes.Append(staffLayoutStaffDistance);
            print.AppendChild(staffLayout);
            measure0.AppendChild(print);

            XmlNode attributes = doc.CreateElement("attributes");
            XmlNode divisions = doc.CreateElement("divisions");
            divisions.InnerText = Divisions.ToString();
            XmlNode _key = doc.CreateElement("key");
            XmlNode fifths = doc.CreateElement("fifths");
            if (key == null)
            {
                key = Key.CMajor;
            }
            fifths.InnerText = key.ToString();
            XmlNode time = doc.CreateElement("time");
            XmlNode beats = doc.CreateElement("beats");
            beats.InnerText = TimeBeats.ToString();
            XmlNode beat_type = doc.CreateElement("beat-type");
            beat_type.InnerText = TimeBeatType.ToString();
            XmlNode staves = doc.CreateElement("staves");
            staves.InnerText = "2";
            XmlNode clef1 = doc.CreateElement("clef");
            XmlAttribute clef1_number = doc.CreateAttribute("number");
            clef1_number.Value = "1";
            clef1.Attributes.Append(clef1_number);
            XmlNode sign = doc.CreateElement("sign");
            sign.InnerText = "G";
            XmlNode line = doc.CreateElement("line");
            line.InnerText = "2";
            clef1.AppendChild(sign);
            clef1.AppendChild(line);
            XmlNode clef2 = doc.CreateElement("clef");
            XmlAttribute clef2_number = doc.CreateAttribute("number");
            clef2_number.Value = "2";
            clef2.Attributes.Append(clef2_number);
            XmlNode sign2 = doc.CreateElement("sign");
            sign2.InnerText = "F";
            XmlNode line2 = doc.CreateElement("line");
            line2.InnerText = "4";
            clef2.AppendChild(sign2);
            clef2.AppendChild(line2);
            _key.AppendChild(fifths);
            time.AppendChild(beats);
            time.AppendChild(beat_type);
            attributes.AppendChild(divisions);
            attributes.AppendChild(_key);
            attributes.AppendChild(time);
            attributes.AppendChild(staves);
            attributes.AppendChild(clef1);
            attributes.AppendChild(clef2);
            measure0.AppendChild(attributes);

            XmlNode direction = doc.CreateElement("direction");
            XmlAttribute placement = doc.CreateAttribute("placement");
            placement.Value = "above";
            direction.Attributes.Append(placement);
            XmlNode directionType = doc.CreateElement("direction-type");
            XmlNode metronome = doc.CreateElement("metronome");
            XmlAttribute metronomeParentheses = doc.CreateAttribute("parentheses");
            metronomeParentheses.Value = "no";
            XmlNode metronomeBeatUnit = doc.CreateElement("beat-unit");
            metronomeBeatUnit.InnerText = "quarter";
            XmlNode metronomePerMinute = doc.CreateElement("per-minute");
            metronomePerMinute.InnerText = Tempo.ToString();
            metronome.Attributes.Append(metronomeParentheses);
            metronome.AppendChild(metronomeBeatUnit);
            metronome.AppendChild(metronomePerMinute);
            directionType.AppendChild(metronome);
            direction.AppendChild(directionType);
            measure0.AppendChild(direction);

            // note
            XmlNode note = doc.CreateElement("note");
            XmlNode pitch = doc.CreateElement("pitch");
            XmlNode step = doc.CreateElement("step");
            step.InnerText = "C";
            XmlNode octave = doc.CreateElement("octave");
            octave.InnerText = "4";
            pitch.AppendChild(step);
            pitch.AppendChild(octave);
            XmlNode duration = doc.CreateElement("duration");
            duration.InnerText = "96";
            XmlNode noteType = doc.CreateElement("type");
            noteType.InnerText = "whole";
            XmlNode stem = doc.CreateElement("stem");
            stem.InnerText = "down";
            XmlNode voice = doc.CreateElement("voice");
            voice.InnerText = "1";
            XmlNode staff = doc.CreateElement("staff");
            staff.InnerText = "1";
            note.AppendChild(pitch);
            note.AppendChild(duration);
            note.AppendChild(noteType);
            note.AppendChild(stem);
            note.AppendChild(voice);
            note.AppendChild(staff);
            measure0.AppendChild(note);

            part1.AppendChild(measure0);
            score_partwise.AppendChild(part1);

            doc.AppendChild(score_partwise);

            doc.Save(PATH + fileName + EXTENSION);

            FileName = fileName;

            Score = doc;

            return doc;
        }
        public XmlDocument Load(string fileName)
        {
            XmlDocument doc = new();
            doc.Load(fileName);
            Score = doc;
            Measures = Score.SelectNodes("//measure").Count;

            FileName = fileName;

            return doc;
        }
        public XmlNode AddMeasure(XmlNode part)
        {
            XmlNode measure = Score.CreateElement("measure");
            XmlAttribute number = Score.CreateAttribute("number");
            number.Value = Measures.ToString();
            measure.Attributes.Append(number);
            XmlAttribute _implicit = Score.CreateAttribute("implicit");
            _implicit.Value = "yes";
            measure.Attributes.Append(_implicit);

            part.AppendChild(measure);

            Measures += 1;

            Score.Save(FileName);

            return measure;
        }

        public char[] ascii = Enumerable.Range(32, 120).Select(i => (char)i).ToArray();

        public string[] notes;
        public Dictionary<string, int> note_map;
        public Dictionary<int, string> note_map_reverse;

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
            note_map_reverse = new Dictionary<int, string>();
            for (int i = 0; i < notes.Length; i++)
            {
                Console.WriteLine(notes[i] + ", " + (i + 32));
                note_map.Add(notes[i], i + 32);
                note_map_reverse.Add(i + 32, notes[i]);
            }

        }

        public void Add(char ascii, int staff, XmlNode measure, char prev_char, char[] next_chars)
        {
            string note;
            int? alter;
            int octave;
            NoteType noteType;

            int index = (int)ascii;
            /*if (index >= 120)
                index = 119;*/
            char character = this.ascii[index];

            // get string note from ascii note_map
            note = note_map_reverse[index];

            Console.WriteLine("note: " + note);

            if (note[note.Length - 2].Equals('#'))
                alter = 1;
            else
                alter = null;
            octave = int.Parse(note[note.Length - 1].ToString());

            /*if (next_chars[0] == ' ')
                noteType = NoteType.QUARTER;
            else if (next_chars)*/

            /*try
            {
                if (next_chars[0] == ' ')
                    noteType = NoteType.QUARTER;
                else if (next_chars[1] == ' ')
                    noteType = NoteType.HALF;
                else if (next_chars[2] == ' ')
                    noteType = NoteType.WHOLE;
                else
                    noteType = NoteType.QUARTER;
            }
            catch (Exception)
            {
                noteType = NoteType.QUARTER;
            }*/

            noteType = NoteType.QUARTER;

            note = note[0].ToString();

            /*if (octave <= 3)
            {
                Backup(measure, 96);
                staff = 2;
            } // watch this*/
            bool isChord = false;
            if (prev_char != ' ')
            {
                Console.WriteLine("prev_char:" + prev_char);
                isChord = true;
            }
            AddNote(staff, note, alter, octave, noteType, measure, isChord);
        }

        public void AddNote(int staff, string note, int? alter, int octave, NoteType noteType, XmlNode measure, bool isChord)
        {
            if (Score == null)
                throw new Exception("Score is null");

            XmlNode _note = Score.CreateElement("note");
            XmlNode pitch = Score.CreateElement("pitch");
            XmlNode step = Score.CreateElement("step");
            step.InnerText = note;
            XmlNode _octave = Score.CreateElement("octave");
            _octave.InnerText = octave.ToString();
            pitch.AppendChild(step);
            if (alter != null && alter != 0)
            {
                XmlNode _alter = Score.CreateElement("alter");
                _alter.InnerText = alter.ToString();
                pitch.AppendChild(_alter);
            }
            pitch.AppendChild(_octave);

            string _duration = noteType.Value;
            XmlNode duration = Score.CreateElement("duration");
            duration.InnerText = _duration;
            XmlNode _noteType = Score.CreateElement("type");
            _noteType.InnerText = GetNoteType(noteType);

            XmlNode stem = Score.CreateElement("stem");
            stem.InnerText = "down";
            XmlNode voice = Score.CreateElement("voice");
            voice.InnerText = "1";
            XmlNode _staff = Score.CreateElement("staff");
            _staff.InnerText = staff.ToString();
            if (isChord)
            {
                XmlNode chord = Score.CreateElement("chord");
                _note.AppendChild(chord);
            }
            _note.AppendChild(pitch);
            _note.AppendChild(duration);
            _note.AppendChild(_noteType);
            _note.AppendChild(stem);
            _note.AppendChild(voice);
            _note.AppendChild(_staff);
            measure.AppendChild(_note);

            //Score.AppendChild(measure);

            Score.Save(FileName);

        }

        public void Backup(XmlNode measure)
        {
            XmlNode backup = Score.CreateElement("backup");
            XmlNode duration = Score.CreateElement("duration");
            duration.InnerText = "96";
            backup.AppendChild(duration);
            measure.AppendChild(backup);
            Score.Save(FileName);
        }

        public void Backup(XmlNode measure, int duration)
        {
            XmlNode backup = Score.CreateElement("backup");
            XmlNode _duration = Score.CreateElement("duration");
            _duration.InnerText = duration.ToString();
            backup.AppendChild(_duration);
            measure.AppendChild(backup);
            Score.Save(FileName);
        }

        public string GetNoteType(NoteType note)
        {
            switch (note.ToString())
            {
                case "384":
                    return "long";
                case "192":
                    return "breve";
                case "96":
                    return "whole";
                case "48":
                    return "half";
                case "24":
                    return "quarter";
                case "12":
                    return "eighth";
                case "6":
                    return "16th";
                case "3":
                    return "32nd";
                case "1.5":
                    return "64th";
                case "0.75":
                    return "128th";
                case "0.375":
                    return "256th";

                default:
                    return "quarter";
            }
        }

    }
}
