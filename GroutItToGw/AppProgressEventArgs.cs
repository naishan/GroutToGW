using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroutItToGw
{
    public class AppProgressEventArgs :EventArgs
    {
        public string progressMessage;

        public AppProgressEventArgs(string progressMessage)
        {
            this.progressMessage = progressMessage;
        }
    }
}
