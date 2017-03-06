using OpenTK;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Material.Texture;
using SPICA.Math3D;

using System;
using System.Linq;

namespace SPICA.Renderer.Animation
{
    public class MaterialAnimation : AnimationControl
    {
        public Matrix3[][] GetUVTransforms(PatriciaList<H3DMaterial> Materials)
        {
            Matrix3[][] Output = new Matrix3[Materials.Count][];

            for (int Index = 0; Index < Output.Length; Index++)
            {
                Output[Index] = GetUVTransform(
                    Materials[Index].Name,
                    Materials[Index].MaterialParams);
            }

            return Output;
        }

        public Matrix3[] GetUVTransform(string MaterialName, H3DMaterialParams Params)
        {
            Vector2D[]
                Scale       = new Vector2D[3],
                Translation = new Vector2D[3];

            float[] Rotation = new float[3];

            for (int Index = 0; Index < 3; Index++)
            {
                Scale[Index]       = Params.TextureCoords[Index].Scale;
                Rotation[Index]    = Params.TextureCoords[Index].Rotation;
                Translation[Index] = Params.TextureCoords[Index].Translation;
            }

            if (BaseAnimation != null && State != AnimationState.Stopped)
            {
                foreach (H3DAnimationElement Elem in BaseAnimation.Elements.Where(x => x.Name == MaterialName))
                {
                    if (Elem.PrimitiveType == H3DAnimPrimitiveType.Vector2D)
                    {
                        H3DAnimVector2D Vector = (H3DAnimVector2D)Elem.Content;

                        switch (Elem.TargetType)
                        {
                            case H3DAnimTargetType.MaterialTexCoord0Scale: SetVector2(Vector, ref Scale[0]); break;
                            case H3DAnimTargetType.MaterialTexCoord1Scale: SetVector2(Vector, ref Scale[1]); break;
                            case H3DAnimTargetType.MaterialTexCoord2Scale: SetVector2(Vector, ref Scale[2]); break;

                            case H3DAnimTargetType.MaterialTexCoord0Trans: SetVector2(Vector, ref Translation[0]); break;
                            case H3DAnimTargetType.MaterialTexCoord1Trans: SetVector2(Vector, ref Translation[1]); break;
                            case H3DAnimTargetType.MaterialTexCoord2Trans: SetVector2(Vector, ref Translation[2]); break;
                        }
                    }
                    else if (Elem.PrimitiveType == H3DAnimPrimitiveType.Float)
                    {
                        H3DFloatKeyFrameGroup Float = ((H3DAnimFloat)Elem.Content).Value;

                        if (!Float.HasData) continue;

                        float Value = Float.GetFrameValue(Frame);

                        switch (Elem.TargetType)
                        {
                            case H3DAnimTargetType.MaterialTexCoord0Rot: Rotation[0] = Value; break;
                            case H3DAnimTargetType.MaterialTexCoord1Rot: Rotation[1] = Value; break;
                            case H3DAnimTargetType.MaterialTexCoord2Rot: Rotation[2] = Value; break;
                        }
                    }
                }
            }

            Matrix3[] Transforms = new Matrix3[3];

            for (int Index = 0; Index < 3; Index++)
            {
                Matrix3 Transform = Matrix3.Identity;

                float SX = Scale[Index].X;
                float SY = Scale[Index].Y;

                float TX = Translation[Index].X;
                float TY = Translation[Index].Y;

                float CA = (float)Math.Cos(Rotation[Index]);
                float SA = (float)Math.Sin(Rotation[Index]);

                Transform.Row0.X = SX *  CA;
                Transform.Row0.Y = SY *  SA;
                Transform.Row1.X = SX * -SA;
                Transform.Row1.Y = SY *  CA;

                switch (Params.TextureCoords[Index].TransformType)
                {
                    case H3DTextureTransformType.DccMaya:
                        Transform.Row2.X = SX * ((0.5f *  SA - 0.5f * CA) + 0.5f - TX);
                        Transform.Row2.Y = SY * ((0.5f * -SA - 0.5f * CA) + 0.5f - TY);
                        break;

                    case H3DTextureTransformType.DccSoftImage:
                        Transform.Row2.X = SX * (-CA * TX - SA * TY);
                        Transform.Row2.Y = SY * ( SA * TX - CA * TY);
                        break;

                    case H3DTextureTransformType.Dcc3dsMax:
                        Transform.Row2.X =
                            SX * CA * (-TX - 0.5f) -
                            SX * SA * ( TY - 0.5f) + 0.5f;
                        Transform.Row2.Y =
                            SY * SA * (-TX - 0.5f) +
                            SY * CA * ( TY - 0.5f) + 0.5f;
                        break;
                }

                Transforms[Index] = Transform;
            }

            return Transforms;
        }

        private void SetVector2(H3DAnimVector2D Vector, ref Vector2D Target)
        {
            if (Vector.X.HasData) Target.X = Vector.X.GetFrameValue(Frame);
            if (Vector.Y.HasData) Target.Y = Vector.Y.GetFrameValue(Frame);
        }
    }
}
