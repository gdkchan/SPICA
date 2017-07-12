using System;

namespace SPICA.Rendering.Shaders
{
    class ShaderCompilationException : Exception
    {
        public ShaderCompilationException() : base() { }

        public ShaderCompilationException(string message) : base(message) { }

        public ShaderCompilationException(string message, Exception inner) : base(message, inner) { }
    }
}
