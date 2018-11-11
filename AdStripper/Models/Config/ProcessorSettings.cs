using System;
using System.Collections.Generic;
using System.Text;

namespace AdStripper.Models.Config
{
	public class ProcessorSettings
	{
		public FFMpegSettings FFMpegSettings { get; set; }

		public bool DryRun { get; set; }
		public string SourceFileLocation { get; set; }
		public string CompletedFileLocation { get; set; }
		public string ProcessedFileLocation { get; set; }
	}
}
