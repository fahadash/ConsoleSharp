using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSharp.WPFApp
{
    using System.IO;
    using System.Reactive;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Input;

    using ReactiveUI;
    using System.Reactive.Linq;

    public class MainViewModel : ReactiveObject
    {
        private string output;

        private string arguments;

        private string command;

        public string Output
        {
            get
            {
                return this.output;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref output, value);
            }
        }

        public string Command
        {
            get
            {
                return this.command;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref command, value);
            }
        }

        public string Arguments
        {
            get
            {
                return this.arguments;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref arguments, value);
            }
        }

        public ICommand RunCommand { get; set; }

        public MainViewModel()
        {
            RunCommand = ReactiveCommand.CreateAsyncTask(
                    _ =>
                    Task.Run(async () =>
                        {
                            var stop = new ManualResetEvent(false);
                            var output = CmdHelperRx.RunCommand(
                                this.Command,
                                this.Arguments,
                               Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

                            output.Subscribe(
                                x => this.Output = this.Output + Environment.NewLine + x.Data,
                                () => stop.Set());

                            await Task.Run(() => stop.WaitOne());

                     
                        }));
        }
    }
}
