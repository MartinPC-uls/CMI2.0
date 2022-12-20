using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXML_Parser.Music
{
    /*
     MusicXML Supported Notes:
        256th   -  Two Hundred Fifty-Sixth
        128th   -  One Hundred Twenty-Eighth
        64th    -  Sixty-Fourth
        32nd    -  Thirty-Second
        16th    -  Sixteenth
        eighth  -  Eighth
        quarter -  Quarter
        half    -  Half
        whole   -  Whole
        breve   -  Breve
        long    -  Long

        Font: https://www.w3.org/2021/06/musicxml40/tutorial/notation-basics/
     */
    public class NoteType
    {
        public string Value { get; private set; }

        public static readonly NoteType _1024TH = new("1024th");
        public static readonly NoteType _512TH = new("512th");
        public static readonly NoteType _256TH = new("256th");
        public static readonly NoteType _128TH = new("128th");
        public static readonly NoteType _64TH = new("64th");
        public static readonly NoteType _32ND = new("32nd");
        public static readonly NoteType _16TH = new("16th");
        public static readonly NoteType EIGHTH = new("eighth");
        public static readonly NoteType QUARTER = new("quarter");
        public static readonly NoteType HALF = new("half");
        public static readonly NoteType WHOLE = new("whole");
        public static readonly NoteType BREVE = new("breve");
        public static readonly NoteType LONG = new("long");
        public static readonly NoteType MAXIMA = new("maxima");

        protected NoteType(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
