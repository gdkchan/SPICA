using SPICA.Formats.Common;
using SPICA.Formats.CtrH3D.Animation;

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
            int  MaterialNamesCount  = Reader.ReadInt32();
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

        public H3DMaterialAnim ToH3DAnimation(GFMotion Motion)
        {
            H3DMaterialAnim Output = new H3DMaterialAnim()
            {
                Name           = "GFMotion",
                FramesCount    = Motion.FramesCount,
                AnimationType  = H3DAnimationType.Material,
                AnimationFlags = Motion.IsLooping ? H3DAnimationFlags.IsLooping : 0
            };

            foreach (GFMotUVTransform Mat in Materials)
            {
                ushort Unit = (ushort)(Mat.UnitIndex * 3);

                if ((Mat.ScaleX.Count | Mat.ScaleY.Count) > 0)
                {
                    Output.Elements.Add(new H3DAnimationElement()
                    {
                        Name          = Mat.Name,
                        Content       = GetAnimVector2D(Mat.ScaleX, Mat.ScaleY, Motion.FramesCount),
                        TargetType    = H3DTargetType.MaterialTexCoord0Scale + Unit,
                        PrimitiveType = H3DPrimitiveType.Vector2D
                    });
                }

                if (Mat.Rotation.Count > 0)
                {
                    Output.Elements.Add(new H3DAnimationElement()
                    {
                        Name          = Mat.Name,
                        Content       = GetAnimFloat(Mat.Rotation, Motion.FramesCount),
                        TargetType    = H3DTargetType.MaterialTexCoord0Rot + Unit,
                        PrimitiveType = H3DPrimitiveType.Float
                    });
                }

                if ((Mat.TranslationX.Count | Mat.TranslationY.Count) > 0)
                {
                    Output.Elements.Add(new H3DAnimationElement()
                    {
                        Name          = Mat.Name,
                        Content       = GetAnimVector2D(Mat.TranslationX, Mat.TranslationY, Motion.FramesCount),
                        TargetType    = H3DTargetType.MaterialTexCoord0Trans + Unit,
                        PrimitiveType = H3DPrimitiveType.Vector2D
                    });
                }
            }

            return Output;
        }

        private H3DAnimFloat GetAnimFloat(List<GFMotKeyFrame> Value, uint FramesCount)
        {
            H3DAnimFloat Output = new H3DAnimFloat();

            SetKeyFrames(Value, Output.Value, FramesCount);

            return Output;
        }

        private H3DAnimVector2D GetAnimVector2D(List<GFMotKeyFrame> X, List<GFMotKeyFrame> Y, uint FramesCount)
        {
            H3DAnimVector2D Output = new H3DAnimVector2D();

            SetKeyFrames(X, Output.X, FramesCount);
            SetKeyFrames(Y, Output.Y, FramesCount);

            return Output;
        }

        private void SetKeyFrames(List<GFMotKeyFrame> Source, H3DFloatKeyFrameGroup Target, uint FramesCount)
        {
            Target.InterpolationType = H3DInterpolationType.Hermite;

            Target.EndFrame = FramesCount;

            foreach (GFMotKeyFrame KF in Source)
            {
                Target.KeyFrames.Add(new KeyFrame(
                    KF.Frame,
                    KF.Value,
                    KF.Slope));
            }
        }
    }
}
