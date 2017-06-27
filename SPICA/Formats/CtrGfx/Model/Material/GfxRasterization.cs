using SPICA.PICA;
using SPICA.Serialization;
using SPICA.Serialization.Attributes;

using System.IO;

namespace SPICA.Formats.CtrGfx.Model.Material
{
    public struct GfxRasterization : ICustomSerialization
    {
        public bool IsPolygonOffsetEnabled;

        public GfxFaceCulling FaceCulling;

        public float PolygonOffsetUnit;

        [Inline, FixedLength(2)] private uint[] FaceCullingCommand;

        void ICustomSerialization.Deserialize(BinaryDeserializer Deserializer) { }

        bool ICustomSerialization.Serialize(BinarySerializer Serializer)
        {
            PICACommandWriter Writer = new PICACommandWriter();

            uint CullMode = (uint)FaceCulling.ToPICAFaceCulling() & 3;

            Writer.SetCommand(PICARegister.GPUREG_FACECULLING_CONFIG, CullMode, 1);

            FaceCullingCommand = Writer.GetBuffer();

            return false;
        }

        internal byte[] GetBytes()
        {
            using (MemoryStream MS = new MemoryStream())
            {
                BinaryWriter Writer = new BinaryWriter(MS);

                Writer.Write(IsPolygonOffsetEnabled ? 1u : 0u);
                Writer.Write(FaceCullingCommand[0]);
                Writer.Write(FaceCullingCommand[1]);
                Writer.Write(PolygonOffsetUnit);

                return MS.ToArray();
            }
        }
    }
}
