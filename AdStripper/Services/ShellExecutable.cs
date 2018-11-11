using System;
using System.Collections.Generic;
using System.Text;

namespace AdStripper.Services
{
	public interface IShellExecutableFactory
	{
		IShellExecutable Get(string processPath, string arguments, bool redirectStandardOutput = false, bool redirectStandardError = false);
	}

	public class ShellExecutableFactory : IShellExecutableFactory
	{
		public IShellExecutable Get(string processPath, string arguments, bool redirectStandardOutput = false, bool redirectStandardError = false)
		{
			return new ShellExecutable(processPath, arguments)
			{
				RedirectStandardError = redirectStandardError,
				RedirectStandardOutput = redirectStandardOutput
			};
		}
	}

	public interface IShellExecutable
	{
		ShellExecutableResult Execute();

		//string Arguments { get; set; }
		//string ExecutablePath { get; set; }
		//bool RedirectStandardError { get; set; }
		//bool RedirectStandardOutput { get; set; }
	}

	public class ShellExecutable : IShellExecutable
	{
		private StringBuilder _output = null;
		private StringBuilder _errorData = null;

		public ShellExecutable(string executablePath, string arguments)
		{
			ExecutablePath = executablePath;
			Arguments = arguments;
		}

		public string ExecutablePath { get; set; }

		public string Arguments { get; set; }

		public bool RedirectStandardOutput { get; set; }

		public bool RedirectStandardError { get; set; }

		protected int DataLineCount { get; set; }

		protected int ErrorCount { get; set; }

		public ShellExecutableResult Execute()
		{
			if (string.IsNullOrWhiteSpace(ExecutablePath))
				throw new ArgumentException("Must provide path to executable", nameof(ExecutablePath));

			if (!System.IO.File.Exists(ExecutablePath))
				throw new System.IO.FileNotFoundException("Executable not found", System.IO.Path.GetFileName(ExecutablePath));

			ShellExecutableResult result = new ShellExecutableResult();
			StringBuilder output = new StringBuilder();
			StringBuilder errorData = new StringBuilder();

			System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo()
				{
					FileName = ExecutablePath,
					Arguments = Arguments,
					RedirectStandardOutput = RedirectStandardOutput,
					RedirectStandardError = RedirectStandardError,
					CreateNoWindow = true,
					UseShellExecute = false
					//WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
				};

			int? exitCode = null;
			try
			{
				using (var process = System.Diagnostics.Process.Start(startInfo))
				{
					if (RedirectStandardOutput)
					{
						_output = new StringBuilder();
						process.OutputDataReceived += Process_OutputDataReceived;
						process.BeginOutputReadLine();
					}
					if (RedirectStandardError)
					{
						_errorData = new StringBuilder();
						process.ErrorDataReceived += Process_ErrorDataReceived;
						process.BeginErrorReadLine();
					}

					process.WaitForExit();
					exitCode = process.ExitCode;

					//process.CloseMainWindow();
					process.Close();
				}

				result.Success = true;
				result.ErrorData = _errorData?.ToString();
				result.Output = _output?.ToString();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			finally
			{
			}

			return result;
		}

		private void Process_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
		{
			if (e.Data == null)
				return;

			//Console.WriteLine("Dur!");
			//Console.WriteLine(e.Data);
			_errorData.AppendLine(e.Data);
			ErrorCount++;
		}

		private void Process_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
		{
			_output.AppendLine(e.Data);
			DataLineCount++;
		}
	}

	public class ShellExecutableResult
	{
		public bool Success { get; set; }

		public string Output { get; set; }

		public string ErrorData { get; set; }
	}
}
