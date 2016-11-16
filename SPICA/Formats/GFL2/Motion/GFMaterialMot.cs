using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.GFL2.Utils;

using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL2.Motion
{
    class GFMaterialMot
    {
        public List<GFMotUVTransform> Materials;

        public GFMaterialMot()
        {
            Materials = new List<GFMotUVTransform>();
        }

        public GFMaterialMot(BinaryReader Reader)
        {
            Materials = new List<GFMotUVTransform>();

            int MaterialNamesCount = Reader.ReadInt32();
            uint MaterialNamesLength = Reader.ReadUInt32();
            Reader.ReadUInt32();
            Reader.ReadUInt32();

            long Position = Reader.BaseStream.Position;

            string[] MaterialNames = GFString.ReadArray(Reader, MaterialNamesCount);

            Reader.BaseStream.Seek(Position + MaterialNamesLength, SeekOrigin.Begin);

            foreach (string Name in MaterialNames)
            {
                Materials.Add(new GFMotUVTransform(Reader, Name));
            }
        }

        public H3DAnimation ToH3DAnimation(uint FramesCount)
        {
            H3DAnimation Output = new H3DAnimation();

            Output.Name = "GFMotion";
            Output.FramesCount = FramesCount;

            foreach (GFMotUVTransform Mat in Materials)
            {
                if ((Mat.ScaleX.Count | Mat.ScaleY.Count) > 0)
                {
                    Output.Elements.Add(new H3DAnimationElement
                    {
                        Name          = Mat.Name,
                        Content       = GetAnimVector2D(Mat.ScaleX, Mat.ScaleY, FramesCount),
                        TargetType    = H3DAnimTargetType.MaterialTexCoord0Scale,
                        PrimitiveType = H3DAnimPrimitiveType.Vector2D
                    });
                }

                if (Mat.Rotation.Count > 0)
                {
                    Output.Elements.Add(new H3DAnimationElement
                    {
                        Name          = Mat.Name,
                        Content       = new H3DAnimFloat { Value = GetKeyFrames(Mat.Rotation, FramesCount) },
                        TargetType    = H3DAnimTargetType.MaterialTexCoord0Rot,
                        PrimitiveType = H3DAnimPrimitiveType.Float
                    });
                }

                if ((Mat.TranslationX.Count | Mat.TranslationY.Count) > 0)
                {
                    Output.Elements.Add(new H3DAnimationElement
                    {
                        Name          = Mat.Name,
                        Content       = GetAnimVector2D(Mat.TranslationX, Mat.TranslationY, FramesCount),
                        TargetType    = H3DAnimTargetType.MaterialTexCoord0Trans,
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

            Output.EndFrame = FramesCount;

            foreach (GFMotKeyFrame KeyFrame in KeyFrames)
            {
                Output.KeyFrames.Add(new H3DFloatKeyFrame
                {
                    Frame = KeyFrame.Frame,
                    Value = KeyFrame.Value,
                    InSlope = KeyFrame.Slope,
                    OutSlope = KeyFrame.Slope
                });
            }

            return Output;
        }
    }
}
