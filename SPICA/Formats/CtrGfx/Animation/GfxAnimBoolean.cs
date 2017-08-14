using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.CtrGfx.Animation
{
    public class GfxAnimBoolean : ICustomSerialization
    {
        [Ignore] public float StartFrame;
        [Ignore] public float EndFrame;

        [Ignore] public GfxLoopType PreRepeat;
        [Ignore] public GfxLoopType PostRepeat;

        [Ignore] public readonly List<bool> Values;

        public GfxAnimBoolean()
        {
            Values = new List<bool>();
        }

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            Deserializer.BaseStream.Seek(Deserializer.ReadPointer(), SeekOrigin.Begin);

            StartFrame = Deserializer.Reader.ReadSingle();
            EndFrame   = Deserializer.Reader.ReadSingle();

            PreRepeat  = (GfxLoopType)Deserializer.Reader.ReadByte();
            PostRepeat = (GfxLoopType)Deserializer.Reader.ReadByte();

            ushort Padding = Deserializer.Reader.ReadUInt16();

            Deserializer.BaseStream.Seek(Deserializer.ReadPointer(), SeekOrigin.Begin);

            BitReader BR = new BitReader(Deserializer.Reader);

            for (int i = 0; i < EndFrame - StartFrame; i++)
            {
                Values.Add(BR.ReadBit());
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            Serializer.Writer.Write(4u);

            Serializer.Writer.Write(StartFrame);
            Serializer.Writer.Write(EndFrame);

            Serializer.Writer.Write((byte)PreRepeat);
            Serializer.Writer.Write((byte)PostRepeat);

            Serializer.Writer.Write((ushort)0);

            Serializer.Writer.Write(4u);

            BitWriter BW = new BitWriter(Serializer.Writer);

            foreach (bool Value in Values)
            {
                BW.WriteBit(Value);
            }

            BW.Flush();

            return true;
        }
    }
}
