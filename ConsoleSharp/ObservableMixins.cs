using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSharp
{
    using System.Diagnostics;
    using System.IO;
    using System.Reactive.Linq;

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

        public static IObservable<CommandLineData> GetStreamObservable(this Process value)
        {
            return
                value.StandardOutput.ToObservable()
                    .Select(x => new CommandLineData(CommandLineDataType.Output, x))
                    .Merge(value.StandardError.ToObservable().Select(x => new CommandLineData(CommandLineDataType.Error, x)))
                    .Concat(Observable.Return(0).Select(_ => new CommandLineData(CommandLineDataType.Finished, value.ExitCode)).Sample(TimeSpan.FromSeconds(1)));
        }
    }
}
