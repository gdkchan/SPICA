using OpenTK;
using OpenTK.Graphics.ES30;

using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Math3D;
using SPICA.PICA.Commands;

using System;
using System.Collections.Generic;

namespace SPICA.Renderer
{
    public class Mesh
    {
        private int VBOHandle;
        private int VAOHandle;

        private int ShaderHandle;

        private Model Parent;

        private List<H3DSubMesh> SubMeshes;

        private H3DMaterial Material;

        public Mesh(Model Parent, H3DMesh Mesh, int ShaderHandle)
        {
            this.ShaderHandle = ShaderHandle;

            this.Parent = Parent;

            SubMeshes = Mesh.SubMeshes;

            Material = Parent.Materials[Mesh.MaterialIndex];

            IntPtr Length = new IntPtr(Mesh.RawBuffer.Length);

            VBOHandle = GL.GenBuffer();
            VAOHandle = GL.GenVertexArray();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, Length, Mesh.RawBuffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(VAOHandle);

            int Offset = 0;

            for (int Index = 0; Index < Mesh.Attributes.Length; Index++)
            {
                GL.DisableVertexAttribArray(Index);
            }

            for (int Index = 0; Index < Mesh.Attributes.Length; Index++)
            {
                PICAAttribute Attrib = Mesh.Attributes[Index];

                int Size = Attrib.Elements;

                VertexAttribPointerType Type = default(VertexAttribPointerType);

                switch (Attrib.Format)
                {
                    case PICAAttributeFormat.Byte: Type = VertexAttribPointerType.Byte; break;
                    case PICAAttributeFormat.Ubyte: Type = VertexAttribPointerType.UnsignedByte; break;
                    case PICAAttributeFormat.Short: Type = VertexAttribPointerType.Short; Size <<= 1; break;
                    case PICAAttributeFormat.Float: Type = VertexAttribPointerType.Float; Size <<= 2; break;
                }

                int AttribIndex = (int)Attrib.Name;

                if (AttribIndex < 9)
                {
                    GL.EnableVertexAttribArray(AttribIndex);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);
                    GL.VertexAttribPointer(AttribIndex, Attrib.Elements, Type, true, Mesh.VertexStride, Offset);
                }

                Offset += Size;
            }

            GL.BindVertexArray(0);
        }

        public void Render()
        {
            //Upload Material data to Fragment Shader
            for (int Index = 0; Index < 6; Index++)
            {
                PICATexEnvStage Stage = Material.MaterialParams.TexEnvStages[Index];

                int ColorCombLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].ColorCombine");
                int AlphaCombLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].AlphaCombine");

                int ColorScaleLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].ColorScale");
                int AlphaScaleLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].AlphaScale");

                GL.Uniform1(ColorCombLocation, (int)Stage.Combiner.RGBCombiner);
                GL.Uniform1(AlphaCombLocation, (int)Stage.Combiner.AlphaCombiner);

                GL.Uniform1(ColorScaleLocation, (float)(1 << (int)Stage.Scale.RGBScale));
                GL.Uniform1(AlphaScaleLocation, (float)(1 << (int)Stage.Scale.AlphaScale));

                for (int Param = 0; Param < 3; Param++)
                {
                    int ColorArgLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].Args[{Param}].ColorArg");
                    int AlphaArgLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].Args[{Param}].AlphaArg");
                    int ColorOpLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].Args[{Param}].ColorOp");
                    int AlphaOpLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].Args[{Param}].AlphaOp");

                    GL.Uniform1(ColorArgLocation, (int)Stage.Source.RGBSource[Param]);
                    GL.Uniform1(AlphaArgLocation, (int)Stage.Source.AlphaSource[Param]);
                    GL.Uniform1(ColorOpLocation, (int)Stage.Operand.RGBOp[Param]);
                    GL.Uniform1(AlphaOpLocation, (int)Stage.Operand.AlphaOp[Param]);
                }

                Vector4 Color = new Vector4(
                    (float)Stage.Color.R / byte.MaxValue,
                    (float)Stage.Color.G / byte.MaxValue,
                    (float)Stage.Color.B / byte.MaxValue,
                    (float)Stage.Color.A / byte.MaxValue);

                int ColorLocation = GL.GetUniformLocation(ShaderHandle, $"Combiners[{Index}].Color");

                GL.Uniform4(ColorLocation, Color);

                //Only those two are selectable afaik
                int TexUnit2SourceLocation = GL.GetUniformLocation(ShaderHandle, "TexUnit2Source");
                int TexUnit3SourceLocation = GL.GetUniformLocation(ShaderHandle, "TexUnit3Source");

                GL.Uniform1(TexUnit2SourceLocation, Material.TextureCoords[2]);
                GL.Uniform1(TexUnit3SourceLocation, Material.TextureCoords[3]);
            }

            //Setup texture units
            if (Material.EnabledTextures[0])
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, Parent.GetTextureId(Material.Texture0Name));
            }

            if (Material.EnabledTextures[1])
            {
                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, Parent.GetTextureId(Material.Texture1Name));
            }

            if (Material.EnabledTextures[2])
            {
                GL.ActiveTexture(TextureUnit.Texture2);
                GL.BindTexture(TextureTarget.Texture2D, Parent.GetTextureId(Material.Texture2Name));
            }

            //Render all SubMeshes
            GL.BindVertexArray(VAOHandle);

            foreach (H3DSubMesh SubMesh in SubMeshes)
            {
                GL.DrawElements(PrimitiveType.Triangles, SubMesh.Indices.Length, DrawElementsType.UnsignedShort, SubMesh.Indices);
            }

            GL.BindVertexArray(0);
        }
    }
}
