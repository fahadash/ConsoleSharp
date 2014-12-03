using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace CommandSharp
{
    [DebuggerDisplay("{DisplayMessage}; {Message}")]
    public class CmdException : Exception
    {
        private Exception innerException;
        private string message;

        public override string Message { get { return message; } }

        public string DisplayMessage { get; set; }

        public Exception InnerException { get { return innerException; } }

        public CmdException(string message, string displayMessage)
        {
            this.message = message;
            this.DisplayMessage = displayMessage;
        }

        public CmdException(Exception exception, string displayMessage)
        {
            this.innerException = exception;
            this.DisplayMessage = displayMessage;
            this.message = exception.Message;
        }

        public CmdException(string message) : base(message)
        {
            this.DisplayMessage = this.message = message;
            this.message = message;
        }

        public CmdException(string format, params object[] args)
        {

            this.message  = this.DisplayMessage = string.Format(format, args);
        }

        public CmdException(string format, object args0)
        {
            this.message = this.DisplayMessage = string.Format(format, args0);
        }

        public CmdException(string format, object args0, object arg1)
        {
            this.message = this.DisplayMessage = string.Format(format, args0, arg1);
        }
        public static void ThrowWhen(bool condition, string message)
        {
            if (condition)
            {
                throw new CmdException(message);
            }
        }

        public static void ThrowWhen(bool condition, string message, params object[] args)
        {
            if (condition)
            {
                throw new CmdException(message, args);
            }
        }
    }
}
