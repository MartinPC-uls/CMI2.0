namespace MusicXml.Domain
{
	public enum TimeSymbol
	{
		Normal, Common, Cut, SingleNumber
	}

	public class Time
	{
		public Time()
		{
			Beats = 0;
			Mode = string.Empty;
		}

		public int Beats { get; set; }

		// Not really the mode rather which note gets the beat
		public string Mode { get; set; }

		public TimeSymbol Symbol { get; set; }
	}
}
