using System;
using System.Collections.Generic;
using System.Text;

namespace AdStripper.Models.MetaData
{
	public class VideoMetadata
	{
		public Format Format { get; set; }
		public Chapter[] Chapters { get; set; }
		//public Stream[] Streams { get; set; }
	}
}
