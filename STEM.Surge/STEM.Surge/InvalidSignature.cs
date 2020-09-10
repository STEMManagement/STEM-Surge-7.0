using System;
using System.Collections.Generic;
using System.Text;

namespace STEM.Surge
{
    public class InvalidSignature : Exception
    {
        public InvalidSignature(string message) : base(message)
        { }
        public InvalidSignature(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
