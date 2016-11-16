using SPICA.Math3D;
using SPICA.Serialization;

using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace SPICA.Formats.CtrH3D.Animation
{
    public class H3DAnimQuatTransform : ICustomSerialization
    {
        private uint Flags;

        [NonSerialized] public List<Vector3D> Scales;
        [NonSerialized] public List<Quaternion> Rotations;
        [NonSerialized] public List<Vector3D> Translations;

        public bool HasScale { get { return Scales.Count > 0; } }
        public bool HasRotation { get { return Rotations.Count > 0; } }
        public bool HasTranslation { get { return Translations.Count > 0; } }

        public H3DAnimQuatTransform()
        {
            Scales = new List<Vector3D>();
            Rotations = new List<Quaternion>();
            Translations = new List<Vector3D>();
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            uint[] Addresses = new uint[3];

            Addresses[0] = Deserializer.Reader.ReadUInt32();
            Addresses[1] = Deserializer.Reader.ReadUInt32();
            Addresses[2] = Deserializer.Reader.ReadUInt32();

            for (int Elem = 0; Elem < 3; Elem++)
            {
                if ((Flags & (0x20 >> Elem)) == 0)
                {
                    Deserializer.BaseStream.Seek(Addresses[Elem], SeekOrigin.Begin);

                    uint ElemFlags = 0;
                    uint Address = Addresses[Elem];
                    uint Count = 1;

                    if ((Flags & (4 >> Elem)) == 0)
                    {
                        /* 
                         * gdkchan Note:
                         * Those values have been verified and Start Frame will always be zero on observed BCHs.
                         * This may or may not change on future versions (probably not), so we can safely ignore it.
                         */
                        float StartFrame = Deserializer.Reader.ReadSingle();
                        float EndFrame = Deserializer.Reader.ReadSingle();

                        ElemFlags = Deserializer.Reader.ReadUInt32();
                        Address = Deserializer.Reader.ReadUInt32();
                        Count = Deserializer.Reader.ReadUInt32();
                    }

                    Deserializer.BaseStream.Seek(Address, SeekOrigin.Begin);

                    for (int Index = 0; Index < Count; Index++)
                    {
                        switch (Elem)
                        {
                            case 0: Scales.Add(Deserializer.Deserialize<Vector3D>()); break;
                            case 1: Rotations.Add(Deserializer.Deserialize<Quaternion>()); break;
                            case 2: Translations.Add(Deserializer.Deserialize<Vector3D>()); break;
                        }
                    }
                }
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            throw new NotImplementedException();
        }

        private T GetFrameValue<T>(List<T> Elements, int Frame)
        {
            if (Elements.Count > 0)
            {
                if (Frame < 0)
                    return Elements.First();
                else if (Frame >= Elements.Count)
                    return Elements.Last();
                else
                    return Elements[Frame];
            }
            else
            {
                return default(T);
            }
        }

        public Vector3D GetScaleValue(int Frame)
        {
            return GetFrameValue(Scales, Frame);
        }

        public Quaternion GetRotationValue(int Frame)
        {
            return GetFrameValue(Rotations, Frame);
        }

        public Vector3D GetTranslationValue(int Frame)
        {
            return GetFrameValue(Translations, Frame);
        }
    }
}
