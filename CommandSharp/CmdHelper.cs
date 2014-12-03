using System;
using System.Diagnostics;
using System.IO;

namespace CommandSharp
{
    using System.Text;
    using System.Threading.Tasks;

    public class CmdHelper
    {
        /// <summary>
        /// Runs a command line or console based executable in the background and allows you to capture the streams of stdout and stderr
        /// </summary>
        /// <param name="command">A string containing the path to the exectuble or command</param>
        /// <param name="parameters">A string containing arguments</param>
        /// <param name="workingDirectory">Working directory</param>
        /// <param name="logfile">Path to a log file where you want the output to be logged</param>
        /// <returns>A task that when finish returns an object that encapsulates output from both the streams and an exit code</returns>

        public static async Task<CommandLineOutput> RunCommand(string command, string parameters, string workingDirectory, string logfile = null)
        {
            var info = new ProcessStartInfo(command, parameters)
            {
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {

                var process = Process.Start(info);

                var output = new StringBuilder();
                var error = new StringBuilder();

                await Task.Run(() =>
                {
                    var errorTask = process.StandardError.ReadLineAsync();

                    var lineTask = process.StandardOutput.ReadLineAsync();

                    while (process.HasExited == false)
                    {
                        if (lineTask.IsCompleted)
                        {
                            output.AppendLine(lineTask.Result);

                            Debug.WriteLine(lineTask.Result);

                            lineTask = process.StandardOutput.ReadLineAsync();
                        }

                        if (errorTask.IsCompleted)
                        {
                            error.AppendLine(errorTask.Result);

                            Debug.WriteLine(errorTask.Result);

                            errorTask = process.StandardError.ReadLineAsync();
                        }

                        errorTask.Wait(TimeSpan.FromMilliseconds(100.0));
                        lineTask.Wait(TimeSpan.FromMilliseconds(100.0));
                    }

                    if (lineTask.IsCompleted)
                    {
                        output.AppendLine(lineTask.Result);

                        Debug.WriteLine(lineTask.Result);
                    }

                    if (errorTask.IsCompleted)
                    {
                        error.AppendLine(errorTask.Result);

                        Debug.WriteLine(errorTask.Result);
                    }
                });

                if (!string.IsNullOrEmpty(logfile))
                {
                    CmdException.ThrowWhen(!Directory.Exists(Path.GetDirectoryName(logfile)),
                        "Could not write to log file {0}. Directory {1} does not exist.",
                         logfile, Path.GetDirectoryName(logfile));

                    using (var logWriter = new StreamWriter(logfile, true))
                    {
                        logWriter.WriteLine();
                        logWriter.WriteLine("Run time: {0}", DateTime.Now);
                        logWriter.WriteLine("Command Issued");
                        logWriter.WriteLine("---------------");
                        logWriter.WriteLine("{0} {1}", command, parameters);
                        logWriter.WriteLine();
                        logWriter.WriteLine("Standard output");
                        logWriter.WriteLine("--------------------");
                        logWriter.WriteLine(output);

                        logWriter.WriteLine();
                        logWriter.WriteLine("Standard error");
                        logWriter.WriteLine("--------------------");

                        logWriter.WriteLine(error);
                        logWriter.WriteLine("-----------THE END----------------");
                    }
                }

                return new CommandLineOutput()
                {
                    ExitCode = process.ExitCode,
                    StandardError = error.ToString(),
                    StandardOutput = output.ToString()
                };
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
    
}