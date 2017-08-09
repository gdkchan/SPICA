using SPICA.Formats.Common;
using SPICA.Formats.CtrH3D.Animation;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL2.Motion
{
    public class GFVisibilityMot
    {
        public readonly List<GFMotBoolean> Visibilities;

        public GFVisibilityMot()
        {
            Visibilities = new List<GFMotBoolean>();
        }

        public GFVisibilityMot(BinaryReader Reader, uint FramesCount) : this()
        {
            int  MeshNamesCount  = Reader.ReadInt32();
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
            H3DAnimation Output = new H3DAnimation()
            {
                Name           = "GFMotion",
                FramesCount    = Motion.FramesCount,
                AnimationType  = H3DAnimationType.Visibility,
                AnimationFlags = Motion.IsLooping ? H3DAnimationFlags.IsLooping : 0
            };

            ushort Index = 0;

            foreach (GFMotBoolean Vis in Visibilities)
            {
                H3DAnimBoolean Anim = new H3DAnimBoolean();

                Anim.StartFrame = 0;
                Anim.EndFrame   = Motion.FramesCount;
                Anim.CurveIndex = Index++;

                foreach (bool Visibility in Vis.Values)
                {
                    Anim.Values.Add(Visibility);
                }

                Output.Elements.Add(new H3DAnimationElement()
                {
                    Name          = Vis.Name,
                    PrimitiveType = H3DPrimitiveType.Boolean,
                    TargetType    = H3DTargetType.MeshNodeVisibility,
                    Content       = Anim
                });
            }

            return Output;
        }
    }
}
