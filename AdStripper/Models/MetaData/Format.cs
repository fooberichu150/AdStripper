using System;
using System.Collections.Generic;
using System.Text;

namespace AdStripper.Models.MetaData
{
	public class Format
	{
		public string FileName { get; set; }

		[Newtonsoft.Json.JsonProperty("nb_streams")]
		public int StreamCount { get; set; }

		[Newtonsoft.Json.JsonProperty("nb_programs")]
		public int ProgramCount { get; set; }

		[Newtonsoft.Json.JsonProperty("format_name")]
		public string FormatName { get; set; }

		[Newtonsoft.Json.JsonProperty("format_long_name")]
		public string FormatLongName { get; set; }

		[Newtonsoft.Json.JsonProperty("start_time")]
		public double StartTime { get; set; }
		public double Duration { get; set; }
		public long Size { get; set; }

		[Newtonsoft.Json.JsonProperty("bit_rate")]
		public long BitRate { get; set; }

		[Newtonsoft.Json.JsonProperty("probe_score")]
		public int ProbeScore { get; set; }

		public Dictionary<string, string> Tags { get; set; }
	}
}
