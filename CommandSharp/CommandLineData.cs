using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSharp
{

    public enum CommandLineDataType
    {
        Output, Error
    }
    public class CommandLineData
    {
        private CommandLineDataType type;

        private string data;

        public CommandLineData(CommandLineDataType type, string data)
        {
            this.type = type;
            this.data = data;
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
    }
}
