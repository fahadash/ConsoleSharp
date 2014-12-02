using System;
using System.Diagnostics;

public class CmdHelperRx
{
 internal static async Task<CommandLineOutput> RunCommand(string command, string parameters, string workingDirectory, string logfile = null)
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
 
                var process = new Process();
                process.StartInfo = info;
                process.EnableRaisingEvents = true;
    
                var all = process.GetEventsObservable();
                var outputs = all.Where(x => x.Type.Equals(CommandLineDataType.Output)).Select(x => x.Data);
                var errors = all.Where(x => x.Type.Equals(CommandLineDataType.Error)).Select(x => x.Data)
                
                using (var outputSubscription = outputs.Subscribe(d => { Debug.WriteLine(d); output.AppendLine(d); }))
                using (var errorSubscription = errors.Subscribe(d => { Debug.WriteLine(d); error.AppendLine(d); }))
                {
                    process.Start();
                    process.WaitForExit();
                }
             
 
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
                Handler.HandleException(e);
                throw;
            }
        }
}

    internal static class ObservableMixins
    {
        public static IObservable<string> ToObservable(this StreamReader reader)
        {
            if (reader == null) throw new ApplicationException("Value cannot be null: reader");
            var x = new { Line = string.Empty, Reader = reader };

            return Observable.Generate(
                x,
                r => !r.Reader.EndOfStream,
                r => new { Line = r.Reader.ReadLine(), Reader = r.Reader },
                r => r.Line);
        }

        public static IObservable<CommandLineData> GetEventsObservable(this Process value)
        {
            if (value == null)
            {
                throw new ApplicationException("value cannot be null");
            }

            var outputs =
                Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
                    x => value.OutputDataReceived += x,
                    x => value.OutputDataReceived -= x)
                    .Select(x => new CommandLineData(CommandLineDataType.Output, x.EventArgs.Data));
            var errors =
                Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
                    x => value.ErrorDataReceived += x,
                    x => value.ErrorDataReceived -= x)
                    .Select(x => new CommandLineData(CommandLineDataType.Error, x.EventArgs.Data));

            return outputs.Merge(errors);

        }
    }
 internal enum CommandLineDataType
    {
        Output, Error
    }
    internal class CommandLineData
    {
        private CommandLineDataType type;

        private string data;

        public CommandLineData (CommandLineDataType type, string data)
        {
            this.type = type;
            this.data = data;
        }

        internal CommandLineDataType Type
        {
            get
            {
                return this.type;
            }
        }

        internal string Data
        {
            get
            {
                return this.data;
            }
        }
    }
