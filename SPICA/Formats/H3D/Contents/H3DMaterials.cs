using SPICA.Formats.H3D.Contents.Model;
using SPICA.Formats.H3D.Contents.Model.Material;
using SPICA.Serialization;
using SPICA.Serialization.BinaryAttributes;

using System;

namespace SPICA.Formats.H3D.Contents
{
    class H3DMaterials : ICustomSerializer
    {
        [PointerOf("PointerTable")]
        private uint PointerTableAddress;

        [CountOf("Materials"), CountOf("NameTree", 1)]
        private uint Count;

        [PointerOf("NameTree")]
        private uint NameTreeAddress;

        [TargetSection("DescriptorsSection", 1)]
        public H3DTreeNode[] NameTree;

        [TargetSection("DescriptorsSection", 1), CustomSerialization]
        private uint[] PointerTable;

        [NonSerialized]
        public H3DContents ParentRef;

        public object Serialize(BinarySerializer Serializer, string FName)
        {
            int Count = 0;

            for (int MdlIndex = 0; MdlIndex < ParentRef.Models.Count; MdlIndex++)
            {
                H3DModel Model = ParentRef.Models[MdlIndex];

                foreach (H3DMaterial Mat in Model.Materials)
                {
                    long Position = Serializer.BaseStream.Position + Count++ * 4;

                    Serializer.AddPointer("MaterialParams", Mat, Position, typeof(uint));
                    Serializer.Relocator.AddPointer(Position);
                }
            }

            return new uint[Count];
        }
    }
}
