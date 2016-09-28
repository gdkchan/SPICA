using OpenTK.Graphics.OpenGL;

using SPICA.PICA;

using System;

namespace SPICA.WinForms.Rendering
{
    class Mesh
    {
        private int VBOHandle;
        private int VAOHandle;

        private ushort[][] Indices;

        public Mesh(PICAAttribute[] Attributes, byte[] Buffer, ushort[][] Indices, int Stride, int Shader)
        {
            VBOHandle = GL.GenBuffer();
            VAOHandle = GL.GenVertexArray();

            this.Indices = Indices;

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(Buffer.Length), Buffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(VAOHandle);

            int Offset = 0;

            for (int Index = 0; Index < Attributes.Length; Index++)
            {
                PICAAttribute Attrib = Attributes[Index];

                int Size = Attrib.Elements;
                VertexAttribPointerType Type = default(VertexAttribPointerType);

                switch (Attrib.Format)
                {
                    case PICAAttributeFormat.SignedByte: Type = VertexAttribPointerType.Byte; break;
                    case PICAAttributeFormat.UnsignedByte: Type = VertexAttribPointerType.UnsignedByte; break;
                    case PICAAttributeFormat.SignedShort: Type = VertexAttribPointerType.Short; Size <<= 1; break;
                    case PICAAttributeFormat.Single: Type = VertexAttribPointerType.Float; Size <<= 2; break;
                }

                GL.EnableVertexAttribArray(Index);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);
                GL.VertexAttribPointer(Index, Attrib.Elements, Type, true, Stride, Offset);

                switch (Attrib.Name)
                {
                    case PICAAttributeName.Position: GL.BindAttribLocation(Shader, Index, "in_position"); break;
                    case PICAAttributeName.Normal: GL.BindAttribLocation(Shader, Index, "in_normal"); break;
                }

                Offset += Size;
            }

            GL.BindVertexArray(0);
        }

        public void Render()
        {
            GL.BindVertexArray(VAOHandle);

            for (int Face = 0; Face < Indices.Length; Face++)
            {
                GL.DrawElements(PrimitiveType.Triangles, Indices[Face].Length, DrawElementsType.UnsignedShort, Indices[Face]);
            }

            GL.BindVertexArray(0);
        }
    }
}
