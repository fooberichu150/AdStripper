using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdStripper.Models.Config;
using Microsoft.Extensions.Options;

namespace AdStripper.Services
{
	public interface IFileProcessorService
	{
		bool Run();
	}

	public class FileProcessorService : IFileProcessorService
	{
		private readonly ProcessorSettings _processorSettings;
		private readonly IFFProbeWrapper _probeWrapper;
		private readonly IFFMpegWrapper _ffMpegWrapper;

		public FileProcessorService(IOptions<ProcessorSettings> processorSettings,
			IFFProbeWrapper probeWrapper, 
			IFFMpegWrapper ffMpegWrapper)
		{
			_processorSettings = processorSettings.Value;
			_probeWrapper = probeWrapper;
			_ffMpegWrapper = ffMpegWrapper;
		}

		protected FFMpegSettings FFMpegSettings => _processorSettings.FFMpegSettings;

		public bool Run()
		{
			if (!System.IO.Directory.Exists(_processorSettings.SourceFileLocation))
				throw new System.IO.DirectoryNotFoundException("Source folder not found");

			foreach (string file in System.IO.Directory.GetFiles(_processorSettings.SourceFileLocation, "*.mp4"))
			{
				Console.WriteLine(file);

				if (ProcessVideo(file))
				{
					System.IO.File.Copy(file, System.IO.Path.Combine(_processorSettings.CompletedFileLocation, System.IO.Path.GetFileName(file)), true);
					System.IO.File.Delete(file);
				}
			}

			return true;
		}

		private bool ProcessVideo(string filePath)
		{
			var metaData = _probeWrapper.GetMetaData(filePath);

			if (_processorSettings.DryRun)
				return false;

			var videoChapters = metaData.Chapters.Where(c => string.Compare(c.Tags["title"], "video", true) == 0);
			int segmentCount = 0;
			foreach (var chapter in videoChapters)
			{
				double duration = chapter.EndTime - chapter.StartTime;
				if (duration <= 1)
					continue;

				_ffMpegWrapper.SplitVideo(filePath, chapter.StartTime, duration, ++segmentCount);
			}

			string inputFileName = System.IO.Path.GetFileName(filePath);
			if (segmentCount > 1)
				_ffMpegWrapper.JoinVideo(segmentCount, inputFileName);
			else
			{
				// simply copy single segment to output location...
				System.IO.File.Copy(System.IO.Path.Combine(FFMpegSettings.TemporaryOutputLocation, "segment1.mp4"),
					System.IO.Path.Combine(_processorSettings.ProcessedFileLocation, inputFileName));
			}

			return true;
		}
	}
}
