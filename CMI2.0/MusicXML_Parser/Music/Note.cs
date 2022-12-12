using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXML_Parser.Music
{
    public class Note
    {
        public string Pitch { get; private set; }
        public int? Alter { get; private set; }
        public int Octave { get; private set; }
        public NoteType NoteType { get; private set; }
        public bool IsChord { get; private set; }

        public Note()
        {
            Pitch = "";
            Alter = 0;
            Octave = 0;
            NoteType = NoteType.QUARTER;
            IsChord = false;
        }

        public Note(string pitch, int? alter, int octave, NoteType noteType, bool isChord)
        {
            Pitch = pitch;
            Alter = alter;
            Octave = octave;
            NoteType = noteType;
            IsChord = isChord;
        }

        public override string ToString()
        {
            return "[" + Pitch + ", " + Alter + ", " + Octave + ", " + NoteType + ", " + IsChord + "]";
        }
    }
}
