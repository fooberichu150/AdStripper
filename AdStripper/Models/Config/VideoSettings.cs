using System;
using System.Collections.Generic;
using System.Text;

namespace AdStripper.Models.Config
{
	public class VideoSettings
	{
		/// <summary>
		/// Duration in seconds to skip at beginning of video
		/// </summary>
		public int CutInTime { get; set; }

		/// <summary>
		/// Duration in seconds to skip at end of video
		/// </summary>
		public int CutOutTime { get; set; }
	}
}
