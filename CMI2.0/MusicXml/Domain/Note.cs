using System.Xml;

namespace MusicXml.Domain
{
	public class Note
	{
		public Note()
		{
			Type = string.Empty;
			Duration = -1;
			Voice = -1;
			Staff = -1;
			IsChordTone = false;
			Dot = false;
		}

		public string Type { get; set; }

		public int Voice { get; set; }

		public int Duration { get; set; }

		public Lyric Lyric { get; set; }

		public Pitch Pitch { get; set; }

		public int Staff { get; set; }

		public bool IsChordTone { get; set; }

		public bool IsRest { get; set; }

		public string Accidental { get; set; }
		public bool Dot { get; set; }
	}
}
