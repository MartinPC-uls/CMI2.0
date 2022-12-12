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

        public static readonly NoteType LONG = new NoteType("384");
        public static readonly NoteType BREVE = new NoteType("192");
        public static readonly NoteType WHOLE = new NoteType("96");
        public static readonly NoteType HALF = new NoteType("48");
        public static readonly NoteType QUARTER = new NoteType("24");
        public static readonly NoteType EIGHTH = new NoteType("12");
        public static readonly NoteType SIXTEENTH = new NoteType("6");
        public static readonly NoteType THIRTY_SECOND = new NoteType("3");
        public static readonly NoteType SIXTY_FOURTH = new NoteType("1.5");
        public static readonly NoteType ONE_HUNDRED_TWENTY_EIGHTH = new NoteType("0.75");
        public static readonly NoteType TWO_HUNDRED_FIFTY_SIXTH = new NoteType("0.375");

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
