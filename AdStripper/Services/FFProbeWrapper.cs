using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdStripper.Models.Config;
using AdStripper.Models.MetaData;
using Microsoft.Extensions.Options;

namespace AdStripper.Services
{
	public interface IFFProbeWrapper
	{
		VideoMetadata GetMetaData(string filePath);
	}

	public class FFProbeWrapper : IFFProbeWrapper
	{
		private readonly FFMpegSettings _options;
		private readonly VideoSettings _videoSettings;
		private readonly IShellExecutableFactory _shellExecutableFactory;

		public FFProbeWrapper(IOptions<FFMpegSettings> ffmpegSettings,
			IOptions<VideoSettings> videoSettings,
			IShellExecutableFactory shellExecutableFactory)
		{
			_options = ffmpegSettings.Value;
			_videoSettings = videoSettings.Value;
			_shellExecutableFactory = shellExecutableFactory;
		}

		protected int CutInTime => _videoSettings.CutInTime;
		protected int CutOutTime => _videoSettings.CutOutTime;

		private void CleanTemporaryFolder()
		{
			try
			{
				foreach (string file in System.IO.Directory.GetFiles(_options.TemporaryOutputLocation))
					System.IO.File.Delete(file);
			}
			catch
			{
			}
		}

		public VideoMetadata GetMetaData(string filePath)
		{
			CleanTemporaryFolder();

			string ffprobePath = System.IO.Path.Combine(_options.Location, "ffprobe.exe");
			string args = $"-v quiet -print_format json -show_format -show_streams -show_chapters -i \"{filePath}\"";

			var results = _shellExecutableFactory
				.Get(ffprobePath, args, true, true)
				.Execute();

			string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
			string outputPath = System.IO.Path.Combine(_options.TemporaryOutputLocation, "output.json");
			//string outputPath = System.IO.Path.Combine(_options.TemporaryOutputLocation, $"{fileName}.json");
			System.IO.File.WriteAllText(outputPath, results.Output);

			var metadata = Newtonsoft.Json.JsonConvert.DeserializeObject<VideoMetadata>(results.Output);

			// ensure at least 1 chapter
			if (metadata.Chapters.Length == 0)
			{
				Dictionary<string, string> tags = new Dictionary<string, string>();
				tags["title"] = "video";

				metadata.Chapters = new Chapter[]
				{
						new Chapter()
						{
							EndTime = metadata.Format.Duration,
							StartTime = metadata.Format.StartTime,
							Tags = tags
						}
				};
			}

			// check to cut-in and cut-out if necessary
			if (metadata.Chapters.Length >= 0)
			{
				if (CutInTime > 0 && string.Compare(metadata.Chapters.First().Tags["title"], "video", true) == 0)
				{
					var chapter = metadata.Chapters.First();
					chapter.StartTime += CutInTime;
				}
				if (CutOutTime > 0 && string.Compare(metadata.Chapters.Last().Tags["title"], "video", true) == 0)
				{
					var chapter = metadata.Chapters.Last();
					chapter.EndTime -= CutOutTime;
				}
			}

			return metadata;
		}
	}
}
