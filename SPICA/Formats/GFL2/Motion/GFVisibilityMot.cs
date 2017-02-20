using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.Utils;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL2.Motion
{
    public class GFVisibilityMot
    {
        public List<GFMotBoolean> Visibilities;

        public GFVisibilityMot()
        {
            Visibilities = new List<GFMotBoolean>();
        }

        public GFVisibilityMot(BinaryReader Reader, uint FramesCount) : this()
        {
            int MeshNamesCount = Reader.ReadInt32();
            uint MeshNamesLength = Reader.ReadUInt32();

            long Position = Reader.BaseStream.Position;

            string[] MeshNames = Reader.ReadStringArray(MeshNamesCount);

            Reader.BaseStream.Seek(Position + MeshNamesLength, SeekOrigin.Begin);

            foreach (string Name in MeshNames)
            {
                Visibilities.Add(new GFMotBoolean(Reader, Name, (int)(FramesCount + 1)));
            }
        }

        public H3DAnimation ToH3DAnimation(GFMotion Motion)
        {
            H3DAnimation Output = new H3DAnimation();

            Output.Name        = "GFMotion";
            Output.FramesCount = Motion.FramesCount;

            if (Motion.IsLooping) Output.AnimationFlags = H3DAnimationFlags.IsLooping;

            ushort Index = 0;

            foreach (GFMotBoolean Vis in Visibilities)
            {
                H3DAnimationElement Elem = new H3DAnimationElement
                {
                    Name = Vis.Name,

                    PrimitiveType = H3DAnimPrimitiveType.Boolean,
                    TargetType    = H3DAnimTargetType.MeshNodeVisibility,

                    Content = new H3DAnimBoolean
                    {
                        StartFrame = 0,
                        EndFrame   = Motion.FramesCount,
                        CurveIndex = Index++,
                        Values     = Vis.Values
                    }
                };

                Output.Elements.Add(Elem);
            }

            return Output;
        }
    }
}
