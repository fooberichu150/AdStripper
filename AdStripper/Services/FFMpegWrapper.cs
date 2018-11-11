using System;
using System.Collections.Generic;
using System.Text;
using AdStripper.Models.Config;
using Microsoft.Extensions.Options;

namespace AdStripper.Services
{
	public interface IFFMpegWrapper
	{
		bool JoinVideo(int segmentCount, string fileName = null);
		bool SplitVideo(string inputFilePath, double startTime, double duration, int segmentCount);
	}

	public class FFMpegWrapper : IFFMpegWrapper
	{
		private readonly ProcessorSettings _options;
		private readonly IShellExecutableFactory _shellExecutableFactory;

		public FFMpegWrapper(IOptions<ProcessorSettings> processorSettings,
			IShellExecutableFactory shellExecutableFactory)
		{
			_options = processorSettings.Value;
			_shellExecutableFactory = shellExecutableFactory;
		}

		protected FFMpegSettings FFMpegSettings => _options.FFMpegSettings;

		public bool JoinVideo(int segmentCount, string fileName = null)
		{
			string outputPath = FFMpegSettings.TemporaryOutputLocation;
			string targetPath = _options.ProcessedFileLocation;

			StringBuilder builder = new StringBuilder();
			List<string> segmentList = new List<string>();
			for (int i = 1; i <= segmentCount; i++)
			{
				string tmpPath = System.IO.Path.Combine(outputPath, $"segment{i}.mp4");
				builder.AppendLine($"file '{tmpPath}'");
				//builder.AppendLine($"file 'segment{i}.mp4'");
			}
			string listPath = System.IO.Path.Combine(outputPath, "ffmpeg_mylist.txt");
			targetPath = System.IO.Path.Combine(targetPath, fileName ?? "output.mp4");

			System.IO.File.WriteAllText(listPath, builder.ToString());
			Execute($" -f concat -safe 0 -i {listPath} -c copy \"{targetPath}\"");

			return true;
		}

		public bool SplitVideo(string inputFilePath, double startTime, double duration, int segmentCount)
		{
			string outputPath = FFMpegSettings.TemporaryOutputLocation;
			outputPath = System.IO.Path.Combine(outputPath, $"segment{segmentCount}.mp4");
			Execute($" -i \"{inputFilePath}\" -ss {startTime} -t {duration} -c copy {outputPath}");

			return true;
		}

		private void Execute(string arguments)
		{
			string ffmpegpath = System.IO.Path.Combine(FFMpegSettings.Location, "ffmpeg.exe");

			var result = _shellExecutableFactory
				.Get(ffmpegpath, arguments)
				.Execute();
		}
	}
}
