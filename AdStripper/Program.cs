using System;
using System.IO;
using AdStripper.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdStripper
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			try
			{
				string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
				string launch = Environment.GetEnvironmentVariable("LAUNCH_PROFILE");

				if (string.IsNullOrWhiteSpace(env))
				{
					env = "Development";
				}

				var builder = new ConfigurationBuilder()
								.SetBasePath(Directory.GetCurrentDirectory())
								.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
								.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
								.AddEnvironmentVariables();

				IConfigurationRoot configuration = builder.Build();
				var services = new ServiceCollection();
				services.RegisterServices(configuration);
				//services.AddTransient<IFileProcessorService, FileProcessorService>();

				var provider = services.BuildServiceProvider();

				var processingService = provider.GetService<IFileProcessorService>();
				processingService.Run();
			}
			finally
			{
				Console.WriteLine("\nPress any key to exit...");
				Console.ReadKey();
			}
		}
	}
}