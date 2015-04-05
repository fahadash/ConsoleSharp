using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

using ConsoleSharp;
using System.Collections.Generic;

namespace ConsoleSharp
{
    public class CmdHelperRx
    {

        /// <summary>
        /// Runs a command line or console based executable in the background and allows you to capture the streams of stdout and stderr
        /// </summary>
        /// <param name="command">A string containing the path to the exectuble or command</param>
        /// <param name="parameters">A string containing arguments</param>
        /// <param name="workingDirectory">Working directory</param>
        /// <returns>An observable that allows you to subscribe to stdout or stderr messages or both.</returns>
        public static IObservable<CommandLineData> RunCommand(
            string command,
            string parameters,
            string workingDirectory)
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

                process.StartInfo = info;
                process.EnableRaisingEvents = true;

                var all = process.GetStreamObservable();

                return all;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }

}