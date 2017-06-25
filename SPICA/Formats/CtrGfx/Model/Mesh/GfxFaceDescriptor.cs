using SPICA.PICA.Commands;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

namespace SPICA.Formats.CtrGfx.Model.Mesh
{
    public class GfxFaceDescriptor : ICustomSerialization
    {
        private GfxGLDataType Format;

        private byte _PrimitiveMode;

        public PICAPrimitiveMode PrimitiveMode
        {
            get => (PICAPrimitiveMode)_PrimitiveMode;
            set => _PrimitiveMode = (byte)value;
        }

        [Padding(4)] private byte Visible;

        [Section((uint)GfxSectionId.Image)] private byte[] RawBuffer;

        private uint BufferObj;
        private uint LocationFlag;

        private uint CommandCachePtr;
        private uint CommandCacheLength;

        private uint LocationPtr;
        private uint MemoryArea;

        private uint BoundingVolume;

        [Ignore] public ushort[] Indices;

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer)
        {
            bool IsBuffer16Bits = Format == GfxGLDataType.GL_UNSIGNED_SHORT;

            Indices = new ushort[RawBuffer.Length >> (IsBuffer16Bits ? 1 : 0)];

            for (int i = 0; i < RawBuffer.Length; i += (IsBuffer16Bits ? 2 : 1))
            {
                if (IsBuffer16Bits)
                {
                    Indices[i >> 1] = (ushort)(
                        RawBuffer[i + 0] << 0 |
                        RawBuffer[i + 1] << 8);
                }
                else
                {
                    Indices[i] = RawBuffer[i];
                }
            }
        }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            //TODO

            return false;
        }
    }
}
