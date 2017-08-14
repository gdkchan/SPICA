using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.PICA.Commands;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace SPICA.Formats.ModelBinary
{
    public class MBn
    {
        public ushort Type;

        public readonly List<MBnIndicesDesc>  IndicesDesc;
        public readonly List<MBnVerticesDesc> VerticesDesc;

        public H3D BaseScene;

        public MBn()
        {
            IndicesDesc  = new List<MBnIndicesDesc>();
            VerticesDesc = new List<MBnVerticesDesc>();
        }

        public MBn(BinaryReader Reader, H3D BaseScene) : this()
        {
            this.BaseScene = BaseScene;

            Type = (ushort)Reader.ReadUInt32();

            uint MeshFlags   = Reader.ReadUInt32();
            uint VertexFlags = Reader.ReadUInt32();
            int  MeshesCount = Reader.ReadInt32();

            bool HasSingleVerticesDesc = (VertexFlags & 1) != 0;
            bool HasBuiltInDataBuffer = Type == 4;

            if (HasSingleVerticesDesc)
            {
                /*
                 * This is used when all meshes inside the file uses the same vertex format.
                 * This save some file space by only storing this information once.
                 * In this case the file will have only one big vertex buffer at the beggining,
                 * and all meshes will use that buffer.
                 */
                VerticesDesc.Add(new MBnVerticesDesc(Reader, MeshesCount, HasBuiltInDataBuffer));
            }

            for (int i = 0; i < MeshesCount; i++)
            {
                int SubMeshesCount = Reader.ReadInt32();

                for (int j = 0; j < SubMeshesCount; j++)
                {
                    IndicesDesc.Add(new MBnIndicesDesc(Reader, HasBuiltInDataBuffer));
                }

                if (!HasSingleVerticesDesc)
                {
                    VerticesDesc.Add(new MBnVerticesDesc(Reader, SubMeshesCount, HasBuiltInDataBuffer));
                }
            }

            if (HasSingleVerticesDesc)
            {
                //This is used when the model only have one vertex buffer at the beggining.
                for (int i = 0; i < IndicesDesc.Count; i++)
                {
                    if (i == 0 && !HasBuiltInDataBuffer)
                        VerticesDesc[0].ReadBuffer(Reader, true);
                    else if (i > 0)
                        VerticesDesc.Add(VerticesDesc[0]);

                    if (!HasBuiltInDataBuffer)
                    {
                        IndicesDesc[i].ReadBuffer(Reader, true);
                    }
                }
            }
            else if (!HasBuiltInDataBuffer)
            {
                //This is used when the file have various vertex/index buffer after the descriptors.
                int IndicesIndex = 0;

                for (int i = 0; i < MeshesCount; i++)
                {
                    VerticesDesc[i].ReadBuffer(Reader, true);

                    for (int j = 0; j < VerticesDesc[i].SubMeshesCount; j++)
                    {
                        IndicesDesc[IndicesIndex++].ReadBuffer(Reader, true);
                    }
                }
            }
        }

        public H3D ToH3D()
        {
            H3D Output = BaseScene;

            H3DModel Model = Output.Models[0];

            int IndicesIndex = 0, i = 0;

            foreach (H3DMesh Mesh in Model.Meshes.OrderBy(x => (int)x.MetaData["ShapeId"].Values[0]))
            {
                Mesh.PositionOffset = Vector4.Zero;

                Mesh.Attributes.Clear();
                Mesh.Attributes.AddRange(VerticesDesc[i].Attributes);

                Mesh.RawBuffer    = VerticesDesc[i].RawBuffer;
                Mesh.VertexStride = VerticesDesc[i].VertexStride;

                for (int j = 0; j < Mesh.SubMeshes.Count; j++)
                {
                    H3DSubMesh SM = Mesh.SubMeshes[j];

                    SM.Indices     = IndicesDesc[IndicesIndex].Indices;
                    SM.BoneIndices = IndicesDesc[IndicesIndex].BoneIndices;

                    IndicesIndex++;
                }

                i++;
            }

            return Output;
        }
    }
}
