using System;

namespace SPICA.Rendering.Shaders
{
    class ShaderException : Exception
    {
        public ShaderException() : base() { }

        public ShaderException(string message) : base(message) { }

        public ShaderException(string message, Exception inner) : base(message, inner) { }
    }
}
