using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSharp
{

    public enum CommandLineDataType
    {
        Output, Error, Finished
    }
    public class CommandLineData
    {
        private CommandLineDataType type;

        private string data;

        private int exitCode;

        public CommandLineData(CommandLineDataType type, string data)
        {
            this.type = type;
            this.data = data;
        }

        public CommandLineData(CommandLineDataType type, int exitCode)
        {
            this.type = type;
            this.exitCode = exitCode;
        }


        public CommandLineDataType Type
        {
            get
            {
                return this.type;
            }
        }

        public string Data
        {
            get
            {
                return this.data;
            }
        }

        public int ExitCode
        {
            get
            {
                return this.exitCode;
            }
        }
    }
}
