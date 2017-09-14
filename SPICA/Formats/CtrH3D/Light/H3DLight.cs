using SPICA.Formats.Common;
using SPICA.PICA.Commands;
using SPICA.Serialization.Attributes;

using System.Numerics;

namespace SPICA.Formats.CtrH3D.Light
{
    public class H3DLight : INamed
    {
        private string _Name;

        public string Name
        {
            get => _Name;
            set => _Name = value ?? throw Exceptions.GetNullException("Name");
        }

        public Vector3 TransformScale;
        public Vector3 TransformRotation;
        public Vector3 TransformTranslation;

        public H3DLightType Type;

        private byte Enabled;

        public bool IsEnabled
        {
            get => Enabled != 0;
            set => Enabled = (byte)(value ? 1 : 0);
        }

        public H3DLightFlags Flags;

        private byte LUTConfig;

        public PICALUTInput LUTInput
        {
            get => (PICALUTInput)BitUtils.GetBits(LUTConfig, 0, 4);
            set => LUTConfig = (byte)BitUtils.SetBits(LUTConfig, (int)value, 0, 4);
        }

        public PICALUTScale LUTScale
        {
            get => (PICALUTScale)BitUtils.GetBits(LUTConfig, 4, 4);
            set => LUTConfig = (byte)BitUtils.SetBits(LUTConfig, (int)value, 4, 4);
        }

        public H3DMetaData MetaData;

        [Inline]
        [TypeChoiceName("Type")]
        [TypeChoice((uint)H3DLightType.Hemisphere,    typeof(H3DHemisphereLight))]
        [TypeChoice((uint)H3DLightType.Ambient,       typeof(H3DAmbientLight))]
        [TypeChoice((uint)H3DLightType.Vertex,        typeof(H3DVertexLight))]
        [TypeChoice((uint)H3DLightType.VertexDir,     typeof(H3DVertexLight))]
        [TypeChoice((uint)H3DLightType.VertexPoint,   typeof(H3DVertexLight))]
        [TypeChoice((uint)H3DLightType.VertexSpot,    typeof(H3DVertexLight))]
        [TypeChoice((uint)H3DLightType.Fragment,      typeof(H3DFragmentLight))]
        [TypeChoice((uint)H3DLightType.FragmentDir,   typeof(H3DFragmentLight))]
        [TypeChoice((uint)H3DLightType.FragmentPoint, typeof(H3DFragmentLight))]
        [TypeChoice((uint)H3DLightType.FragmentSpot,  typeof(H3DFragmentLight))]
        public object Content;
    }
}
