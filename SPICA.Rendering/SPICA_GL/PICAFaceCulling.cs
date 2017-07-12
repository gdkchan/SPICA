using OpenTK.Graphics.OpenGL;

using SPICA.PICA.Commands;

using System;

namespace SPICA.Rendering.SPICA_GL
{
    static class PICAFaceCullingExtensions
    {
        public static CullFaceMode ToCullFaceMode(this PICAFaceCulling Cull)
        {
            switch (Cull)
            {
                case PICAFaceCulling.Never:     return CullFaceMode.FrontAndBack;
                case PICAFaceCulling.FrontFace: return CullFaceMode.Front;
                case PICAFaceCulling.BackFace:  return CullFaceMode.Back;

                default: throw new ArgumentException("Invalid Face culling!");
            }
        }
    }
}
