using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.Utils;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL2.Motion
{
    public class GFMaterialMot
    {
        public List<GFMotUVTransform> Materials;

        public GFMaterialMot()
        {
            Materials = new List<GFMotUVTransform>();
        }

        public GFMaterialMot(BinaryReader Reader, uint FramesCount) : this()
        {
            int MaterialNamesCount = Reader.ReadInt32();
            uint MaterialNamesLength = Reader.ReadUInt32();

            uint[] Units = new uint[MaterialNamesCount];
            
            for (int Index = 0; Index < Units.Length; Index++)
            {
                Units[Index] = Reader.ReadUInt32();
            }

            long Position = Reader.BaseStream.Position;

            string[] MaterialNames = Reader.ReadStringArray(MaterialNamesCount);

            Reader.BaseStream.Seek(Position + MaterialNamesLength, SeekOrigin.Begin);

            for (int Index = 0; Index < MaterialNames.Length; Index++)
            {
                for (int Unit = 0; Unit < Units[Index]; Unit++)
                {
                    Materials.Add(new GFMotUVTransform(Reader, MaterialNames[Index], FramesCount));
                }
            }
        }

        public H3DAnimation ToH3DAnimation(GFMotion Motion)
        {
            H3DAnimation Output = new H3DAnimation();

            Output.Name        = "GFMotion";
            Output.FramesCount = Motion.FramesCount;

            if (Motion.IsLooping) Output.AnimationFlags = H3DAnimationFlags.IsLooping;

            foreach (GFMotUVTransform Mat in Materials)
            {
                ushort Unit = (ushort)(Mat.UnitIndex * 3);

                if ((Mat.ScaleX.Count | Mat.ScaleY.Count) > 0)
                {
                    Output.Elements.Add(new H3DAnimationElement
                    {
                        Name          = Mat.Name,
                        Content       = GetAnimVector2D(Mat.ScaleX, Mat.ScaleY, Motion.FramesCount),
                        TargetType    = H3DAnimTargetType.MaterialTexCoord0Scale + Unit,
                        PrimitiveType = H3DAnimPrimitiveType.Vector2D
                    });
                }

                if (Mat.Rotation.Count > 0)
                {
                    Output.Elements.Add(new H3DAnimationElement
                    {
                        Name          = Mat.Name,
                        Content       = new H3DAnimFloat { Value = GetKeyFrames(Mat.Rotation, Motion.FramesCount) },
                        TargetType    = H3DAnimTargetType.MaterialTexCoord0Rot + Unit,
                        PrimitiveType = H3DAnimPrimitiveType.Float
                    });
                }

                if ((Mat.TranslationX.Count | Mat.TranslationY.Count) > 0)
                {
                    Output.Elements.Add(new H3DAnimationElement
                    {
                        Name          = Mat.Name,
                        Content       = GetAnimVector2D(Mat.TranslationX, Mat.TranslationY, Motion.FramesCount),
                        TargetType    = H3DAnimTargetType.MaterialTexCoord0Trans + Unit,
                        PrimitiveType = H3DAnimPrimitiveType.Vector2D
                    });
                }
            }

            return Output;
        }

        private H3DAnimVector2D GetAnimVector2D(List<GFMotKeyFrame> X, List<GFMotKeyFrame> Y, uint FramesCount)
        {
            return new H3DAnimVector2D
            {
                X = GetKeyFrames(X, FramesCount),
                Y = GetKeyFrames(Y, FramesCount)
            };
        }

        private H3DFloatKeyFrameGroup GetKeyFrames(List<GFMotKeyFrame> KeyFrames, uint FramesCount)
        {
            H3DFloatKeyFrameGroup Output = new H3DFloatKeyFrameGroup();

            Output.InterpolationType = H3DInterpolationType.Hermite;

            Output.EndFrame = FramesCount;

            foreach (GFMotKeyFrame KeyFrame in KeyFrames)
            {
                Output.KeyFrames.Add(new H3DFloatKeyFrame
                {
                    Frame    = KeyFrame.Frame,
                    Value    = KeyFrame.Value,
                    InSlope  = KeyFrame.Slope,
                    OutSlope = KeyFrame.Slope
                });
            }

            return Output;
        }
    }
}
