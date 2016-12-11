using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.GFL2.Model;
using SPICA.Formats.GFL2.Utils;
using SPICA.Math3D;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL2.Motion
{
    public class GFSkeletonMot
    {
        public List<GFMotBoneTransform> Bones;

        public GFSkeletonMot()
        {
            Bones = new List<GFMotBoneTransform>();
        }

        public GFSkeletonMot(BinaryReader Reader, uint FramesCount) : this()
        {
            int BoneNamesCount = Reader.ReadInt32();
            uint BoneNamesLength = Reader.ReadUInt32();

            long Position = Reader.BaseStream.Position;

            string[] BoneNames = GFString.ReadArray(Reader, BoneNamesCount);

            Reader.BaseStream.Seek(Position + BoneNamesLength, SeekOrigin.Begin);

            foreach (string Name in BoneNames)
            {
                Bones.Add(new GFMotBoneTransform(Reader, Name, FramesCount));
            }
        }

        public H3DAnimation ToH3DAnimation(List<GFBone> Skeleton, GFMotion Motion)
        {
            H3DAnimation Output = new H3DAnimation();

            Output.Name        = "GFMotion";
            Output.FramesCount = Motion.FramesCount;

            if (Motion.IsLooping) Output.AnimationFlags = H3DAnimationFlags.IsLooping;

            foreach (GFMotBoneTransform Bone in Bones)
            {
                H3DAnimQuatTransform QuatTransform = new H3DAnimQuatTransform();

                int BoneIndex = Skeleton.FindIndex(x => x.Name == Bone.Name);

                if (BoneIndex == -1) continue;

                for (float Frame = 0; Frame < Motion.FramesCount; Frame++)
                {
                    Vector3D Scale       = Skeleton[BoneIndex].Scale;
                    Vector3D Rotation    = Skeleton[BoneIndex].Rotation;
                    Vector3D Translation = Skeleton[BoneIndex].Translation;

                    GFMotBoneTransform.SetFrameValue(Bone.ScaleX,       Frame, ref Scale.X);
                    GFMotBoneTransform.SetFrameValue(Bone.ScaleY,       Frame, ref Scale.Y);
                    GFMotBoneTransform.SetFrameValue(Bone.ScaleZ,       Frame, ref Scale.Z);

                    GFMotBoneTransform.SetFrameValue(Bone.RotationX,    Frame, ref Rotation.X);
                    GFMotBoneTransform.SetFrameValue(Bone.RotationY,    Frame, ref Rotation.Y);
                    GFMotBoneTransform.SetFrameValue(Bone.RotationZ,    Frame, ref Rotation.Z);

                    GFMotBoneTransform.SetFrameValue(Bone.TranslationX, Frame, ref Translation.X);
                    GFMotBoneTransform.SetFrameValue(Bone.TranslationY, Frame, ref Translation.Y);
                    GFMotBoneTransform.SetFrameValue(Bone.TranslationZ, Frame, ref Translation.Z);

                    /*
                     * gdkchan Note:
                     * When the game uses Axis Angle for rotation,
                     * I believe that the original Euler rotation can be ignored,
                     * because otherwise we would need to either convert Euler to Axis Angle or Axis to Euler,
                     * and both conversions are pretty expensive.
                     */
                    Quaternion QuatRotation;

                    if (Bone.IsAxisAngle)
                        QuatRotation = Quaternion.FromAxisHalvedAngle(Rotation.Normalized(), Rotation.Length);
                    else
                        QuatRotation = Quaternion.FromEuler(Rotation);

                    QuatTransform.Scales.Add(Scale);
                    QuatTransform.Rotations.Add(QuatRotation);
                    QuatTransform.Translations.Add(Translation);
                }

                Output.Elements.Add(new H3DAnimationElement
                {
                    Name = Bone.Name,
                    Content = QuatTransform,
                    TargetType = H3DAnimTargetType.Bone,
                    PrimitiveType = H3DAnimPrimitiveType.QuatTransform
                });
            }

            return Output;
        }
    }
}
