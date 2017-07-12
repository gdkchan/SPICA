using OpenTK.Graphics.OpenGL;

using SPICA.PICA.Commands;

using System;

namespace SPICA.Rendering.SPICA_GL
{
    static class PICATestFuncExtensions
    {
        public static All ToFunction(this PICATestFunc Func)
        {
            switch (Func)
            {
                case PICATestFunc.Never:    return All.Never;
                case PICATestFunc.Always:   return All.Always;
                case PICATestFunc.Equal:    return All.Equal;
                case PICATestFunc.Notequal: return All.Notequal;
                case PICATestFunc.Less:     return All.Less;
                case PICATestFunc.Lequal:   return All.Lequal;
                case PICATestFunc.Greater:  return All.Greater;
                case PICATestFunc.Gequal:   return All.Gequal;

                default: throw new ArgumentException("Invalid function!");
            }
        }

        public static StencilFunction ToStencilFunction(this PICATestFunc Func)
        {
            return (StencilFunction)ToFunction(Func);
        }

        public static DepthFunction ToDepthFunction(this PICATestFunc Func)
        {
            return (DepthFunction)ToFunction(Func);
        }

        public static void SetGL(this PICAStencilTest StencilTest)
        {
            GL.StencilFunc(
                StencilTest.Function.ToStencilFunction(),
                StencilTest.Reference,
                StencilTest.Mask);

            GL.StencilMask(StencilTest.BufferMask);
        }

        public static void SetGL(this PICADepthColorMask DepthColorMask)
        {
            GL.DepthFunc(DepthColorMask.DepthFunc.ToDepthFunction());
            GL.DepthMask(DepthColorMask.DepthWrite);
            GL.ColorMask(
                DepthColorMask.RedWrite,
                DepthColorMask.GreenWrite,
                DepthColorMask.BlueWrite,
                DepthColorMask.AlphaWrite);
        }
    }
}
