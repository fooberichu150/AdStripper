using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdStripper.Services
{
	public static class ServiceExtensions
	{
		public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddTransient<IFileProcessorService, FileProcessorService>();
			services.AddTransient<IShellExecutableFactory, ShellExecutableFactory>();

			services.AddTransient<IFFProbeWrapper, FFProbeWrapper>();
			services.AddTransient<IFFMpegWrapper, FFMpegWrapper>();

			services.RegisterConfiguration(configuration);

			return services;
		}

		private static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<Models.Config.ProcessorSettings>(configuration.GetSection("ProcessorSettings"));
			services.Configure<Models.Config.FFMpegSettings>(configuration.GetSection("ProcessorSettings:FFMpegSettings"));
			services.Configure<Models.Config.VideoSettings>(configuration.GetSection("VideoSettings"));

			return services;
		}
	}
}
