using OpenTK;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Material.Texture;
using SPICA.Renderer.SPICA_GL;

using System.Linq;

namespace SPICA.Renderer.Animation
{
    public class MaterialAnim : AnimControl
    {
        public Matrix3[][] GetUVTransforms(PatriciaList<H3DMaterial> Materials)
        {
            Matrix3[][] Output = new Matrix3[Materials.Count][];

            for (int Index = 0; Index < Output.Length; Index++)
            {
                Output[Index] = GetUVTransform(
                    Materials[Index].Name,
                    Materials[Index].MaterialParams.TextureCoords);
            }

            return Output;
        }

        public Matrix3[] GetUVTransform(string MaterialName, H3DTextureCoord[] TexCoords)
        {
            Vector2[]
                Scale       = new Vector2[3],
                Translation = new Vector2[3];

            float[] Rotation = new float[3];

            for (int Index = 0; Index < TexCoords.Length; Index++)
            {
                Scale[Index]       = TexCoords[Index].Scale.ToVector2();
                Rotation[Index]    = TexCoords[Index].Rotation;
                Translation[Index] = TexCoords[Index].Translation.ToVector2();
            }

            if (BaseAnimation != null && State != AnimState.Stopped)
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

                Vector2 Offset = new Vector2(0.5f);

                Transform.Row2.Xy = -Translation[Index] - Offset;
                Transform.Row0.X = Scale[Index].X;
                Transform.Row1.Y = Scale[Index].Y;
                Transform *= Matrix3.CreateRotationZ(Rotation[Index]);
                Transform.Row2.Xy += Offset;

                Transforms[Index] = Transform;
            }

            return Transforms;
        }

        private void SetVector2(H3DAnimVector2D Vector, ref Vector2 Target)
        {
            if (Vector.X.HasData) Target.X = Vector.X.GetFrameValue(Frame);
            if (Vector.Y.HasData) Target.Y = Vector.Y.GetFrameValue(Frame);
        }
    }
}
