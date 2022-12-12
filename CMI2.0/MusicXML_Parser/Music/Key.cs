using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXML_Parser.Music
{
    public class Key
    {
        public int Value { get; private set; }

        public static readonly Key CMajor = new Key(0);
        public static readonly Key Aminor = new Key(0);
        public static readonly Key GMajor = new Key(1);
        public static readonly Key Eminor = new Key(1);
        public static readonly Key DMajor = new Key(2);
        public static readonly Key BMinor = new Key(2);
        public static readonly Key AMajor = new Key(3);
        public static readonly Key F_Minor = new Key(3);
        public static readonly Key GbMinor = new Key(3);
        public static readonly Key EMajor = new Key(4);
        public static readonly Key C_Minor = new Key(4);
        public static readonly Key DbMinor = new Key(4);
        public static readonly Key BMajor = new Key(5);
        public static readonly Key G_Minor = new Key(5);
        public static readonly Key AbMinor = new Key(5);
        public static readonly Key F_Major = new Key(6);
        public static readonly Key GbMajor = new Key(6);
        public static readonly Key D_Minor = new Key(6);
        public static readonly Key EbMinor = new Key(6);
        public static readonly Key FMajor = new Key(-1);
        public static readonly Key DMinor = new Key(-1);
        public static readonly Key A_Major = new Key(-2);
        public static readonly Key BbMajor = new Key(-2);
        public static readonly Key GMinor = new Key(-2);
        public static readonly Key D_Major = new Key(-3);
        public static readonly Key EbMajor = new Key(-3);
        public static readonly Key CMinor = new Key(-3);
        public static readonly Key G_Major = new Key(-4);
        public static readonly Key AbMajor = new Key(-4);
        public static readonly Key FMinor = new Key(-4);
        public static readonly Key C_Major = new Key(-5);
        public static readonly Key DbMajor = new Key(-5);
        public static readonly Key A_Minor = new Key(-5);
        public static readonly Key BbMinor = new Key(-5);

        protected Key(int key)
        {
            Value = key;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
