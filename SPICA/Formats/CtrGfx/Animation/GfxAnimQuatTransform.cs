using SPICA.Math3D;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SPICA.Formats.CtrGfx.Animation
{
    public class GfxAnimQuatTransform : ICustomSerialization
    {
        [Ignore] public readonly List<Vector3>    Scales;
        [Ignore] public readonly List<Quaternion> Rotations;
        [Ignore] public readonly List<Vector3>    Translations;

        public bool HasScale       => Scales.Count       > 0;
        public bool HasRotation    => Rotations.Count    > 0;
        public bool HasTranslation => Translations.Count > 0;

        public GfxAnimQuatTransform()
        {
            Scales       = new List<Vector3>();
            Rotations    = new List<Quaternion>();
            Translations = new List<Vector3>();
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            Deserializer.BaseStream.Seek(-0xc, SeekOrigin.Current);

            uint Flags = Deserializer.Reader.ReadUInt32();

            Deserializer.BaseStream.Seek(8, SeekOrigin.Current);

            uint[] Addresses = new uint[3];

            uint ConstantMask = (uint)GfxAnimQuatTransformFlags.IsScaleConstant;
            uint NotExistMask = (uint)GfxAnimQuatTransformFlags.IsScaleInexistent;

            Addresses[1] = Deserializer.ReadPointer();
            Addresses[2] = Deserializer.ReadPointer();
            Addresses[0] = Deserializer.ReadPointer();

            for (int ElemIndex = 0; ElemIndex < 3; ElemIndex++)
            {
                bool Constant = (Flags & ConstantMask) != 0;
                bool Exists   = (Flags & NotExistMask) == 0;

                if (Exists)
                {
                    Deserializer.BaseStream.Seek(Addresses[ElemIndex], SeekOrigin.Begin);

                    float StartFrame  = Deserializer.Reader.ReadSingle();
                    float EndFrame    = Deserializer.Reader.ReadSingle();
                    uint  CurveRelPtr = Deserializer.Reader.ReadUInt32();
                    bool  IsConstant  = Deserializer.Reader.ReadUInt32() != 0;

                    int Count = IsConstant ? 1 : (int)(EndFrame - StartFrame);

                    for (int Index = 0; Index < Count; Index++)
                    {
                        switch (ElemIndex)
                        {
                            case 0:
                                Scales.Add(Deserializer.Reader.ReadVector3());
                                break;

                            case 1:
                                Rotations.Add(Deserializer.Reader.ReadQuaternion());
                                break;

                            case 2:
                                Translations.Add(Deserializer.Reader.ReadVector3());
                                break;
                        }

                        uint SegmentFlags = Deserializer.Reader.ReadUInt32();
                    }
                }

                ConstantMask >>= 1;
                NotExistMask >>= 1;
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            GfxAnimQuatTransformFlags Flags = 0;

            uint ConstantMask = (uint)GfxAnimQuatTransformFlags.IsScaleConstant;
            uint NotExistMask = (uint)GfxAnimQuatTransformFlags.IsScaleInexistent;

            long DescPosition = Serializer.BaseStream.Position;
            long DataPosition = DescPosition + 0xc;

            for (int ElemIndex = 0; ElemIndex < 3; ElemIndex++)
            {
                IList Elem = null;

                switch (ElemIndex)
                {
                    case 0: Elem = Rotations;    break;
                    case 1: Elem = Translations; break;
                    case 2: Elem = Scales;       break;
                }

                if (Elem.Count > 0)
                {
                    Serializer.BaseStream.Seek(DescPosition + ElemIndex * 4, SeekOrigin.Begin);

                    Serializer.WritePointer((uint)DataPosition);

                    Serializer.BaseStream.Seek(DataPosition, SeekOrigin.Begin);

                    Serializer.Writer.Write(0f); //Start Frame
                    Serializer.Writer.Write((float)Elem.Count); //End Frame
                    Serializer.Writer.Write(4u); //Curve Relative Pointer?
                    Serializer.Writer.Write(Elem.Count == 1 ? 1 : 0); //Constant

                    foreach (object Vector in Elem)
                    {
                        if (Vector is Vector3)
                        {
                            Serializer.Writer.Write((Vector3)Vector);
                        }
                        else
                        {
                            Serializer.Writer.Write((Quaternion)Vector);
                        }

                        //TODO: Segment Flags
                        Serializer.Writer.Write(0u);
                    }

                    if (Elem.Count == 1)
                    {
                        Flags |= (GfxAnimQuatTransformFlags)ConstantMask;
                    }

                    DataPosition = Serializer.BaseStream.Position;
                }
                else
                {
                    Flags |= (GfxAnimQuatTransformFlags)NotExistMask;
                }

                ConstantMask >>= 1;
                NotExistMask >>= 1;
            }

            Serializer.BaseStream.Seek(DescPosition - 0xc, SeekOrigin.Begin);

            Serializer.Writer.Write((uint)Flags);

            Serializer.BaseStream.Seek(DataPosition, SeekOrigin.Begin);

            return true;
        }
    }
}
