using System;
using System.Collections.Generic;
using System.Text;

namespace FilemakerSharp.Core
{
    public class FilemakerException : Exception
    {

        public FilemakerException(string message)
            : base(message) { }
    }
}
