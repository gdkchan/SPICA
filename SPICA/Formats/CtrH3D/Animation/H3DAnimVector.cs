using SPICA.Serialization;
using SPICA.Serialization.Serializer;
using System.IO;

namespace SPICA.Formats.CtrH3D.Animation
{
    static class H3DAnimVector
    {
        public static void SetVector(BinaryDeserializer Deserializer, H3DFloatKeyFrameGroup[] Vector)
        {
            uint Flags = Deserializer.Reader.ReadUInt32();

            long Position = Deserializer.BaseStream.Position;

            uint ConstantMask = (uint)H3DAnimVectorFlags.IsXConstant;
            uint NotExistMask = (uint)H3DAnimVectorFlags.IsXInexistent;

            for (int Axis = 0; Axis < Vector.Length; Axis++)
            {
                Deserializer.BaseStream.Seek(Position, SeekOrigin.Begin);

                Position += 4;

                bool Constant = (Flags & ConstantMask) != 0;
                bool Exists   = (Flags & NotExistMask) == 0;

                if (Exists)
                {
                    Vector[Axis] = H3DFloatKeyFrameGroup.ReadGroup(Deserializer, Constant);
                }

                ConstantMask <<= 1;
                NotExistMask <<= 1;
            }
        }

        public static void SetVector(BinaryDeserializer Deserializer, ref H3DFloatKeyFrameGroup Vector)
        {
            H3DAnimVectorFlags Flags = (H3DAnimVectorFlags)Deserializer.Reader.ReadUInt32();

            bool Constant = (Flags & H3DAnimVectorFlags.IsXConstant)   != 0;
            bool Exists   = (Flags & H3DAnimVectorFlags.IsXInexistent) == 0;

            if (Exists)
            {
                Vector = H3DFloatKeyFrameGroup.ReadGroup(Deserializer, Constant);
            }
        }

        public static void WriteVector(BinarySerializer Serializer, H3DFloatKeyFrameGroup[] Vector)
        {
            uint ConstantMask = (uint)H3DAnimVectorFlags.IsXConstant;
            uint NotExistMask = (uint)H3DAnimVectorFlags.IsXInexistent;

            long Position = Serializer.BaseStream.Position;

            uint Flags = 0;

            Serializer.Writer.Write(0u);

            for (int ElemIndex = 0; ElemIndex < Vector.Length; ElemIndex++)
            {
                if (Vector[ElemIndex].KeyFrames.Count > 1)
                {
                    Serializer.Sections[(uint)H3DSectionId.Contents].Values.Add(new RefValue()
                    {
                        Value    = Vector[ElemIndex],
                        Position = Serializer.BaseStream.Position
                    });

                    Serializer.Writer.Write(0u);
                }
                else if (Vector[ElemIndex].KeyFrames.Count == 0)
                {
                    Flags |= NotExistMask; Serializer.Writer.Write(0u);
                }
                else
                {
                    Flags |= ConstantMask; Serializer.Writer.Write(Vector[ElemIndex].KeyFrames[0].Value);
                }

                ConstantMask <<= 1;
                NotExistMask <<= 1;
            }

            Serializer.BaseStream.Seek(Position, SeekOrigin.Begin);

            Serializer.Writer.Write(Flags);

            Serializer.BaseStream.Seek(Position + 4 + Vector.Length * 4, SeekOrigin.Begin);
        }

        public static void WriteVector(BinarySerializer Serializer, H3DFloatKeyFrameGroup Vector)
        {
            WriteVector(Serializer, new H3DFloatKeyFrameGroup[] { Vector });
        }
    }
}
