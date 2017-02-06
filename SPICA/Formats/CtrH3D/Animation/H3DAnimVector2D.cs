using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System;
using System.IO;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DAnimVector2D : ICustomSerialization
    {
        private uint Flags;

        [Ignore] public H3DFloatKeyFrameGroup X;
        [Ignore] public H3DFloatKeyFrameGroup Y;

        public H3DAnimVector2D()
        {
            X = new H3DFloatKeyFrameGroup();
            Y = new H3DFloatKeyFrameGroup();
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            long Position = Deserializer.BaseStream.Position;

            uint NotExistMask = 0x100;
            uint ConstantMask = 0x1;

            for (int Axis = 0; Axis < 2; Axis++)
            {
                Deserializer.BaseStream.Seek(Position, SeekOrigin.Begin);

                Position += 4;

                bool Constant = (Flags & ConstantMask) != 0;
                bool Exists = (Flags & NotExistMask) == 0;

                if (Exists)
                {
                    H3DFloatKeyFrameGroup FrameGrp = H3DFloatKeyFrameGroup.ReadGroup(Deserializer, Constant);

                    switch (Axis)
                    {
                        case 0: X = FrameGrp; break;
                        case 1: Y = FrameGrp; break;
                    }
                }

                NotExistMask <<= 1;
                ConstantMask <<= 1;
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            throw new NotImplementedException();
        }
    }
}
