using System;

namespace SPICA.Formats.Common
{
    class InvalidMagicException : Exception
    {
        public InvalidMagicException() : base() { }

        public InvalidMagicException(string message) : base(message) { }

        public InvalidMagicException(string message, Exception inner) : base(message, inner) { }
    }
}
