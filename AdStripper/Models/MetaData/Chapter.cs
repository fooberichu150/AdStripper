using System;
using System.Collections.Generic;
using System.Text;

namespace AdStripper.Models.MetaData
{
	public class Chapter
	{
		public int Id { get; set; }

		[Newtonsoft.Json.JsonProperty("time_base")]
		public string TimeBase { get; set; }

		[Newtonsoft.Json.JsonProperty("start")]
		public long StartMilliseconds { get; set; }

		[Newtonsoft.Json.JsonProperty("end")]
		public long EndMilliseconds { get; set; }

		[Newtonsoft.Json.JsonProperty("start_time")]
		public double StartTime { get; set; }

		[Newtonsoft.Json.JsonProperty("end_time")]
		public double EndTime { get; set; }

		public Dictionary<string, string> Tags { get; set; }
	}
}
