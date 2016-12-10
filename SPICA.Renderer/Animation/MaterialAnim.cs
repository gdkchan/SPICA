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
        public UVTransform[][] GetUVTransforms(PatriciaList<H3DMaterial> Materials)
        {
            UVTransform[][] Output = new UVTransform[Materials.Count][];

            for (int Index = 0; Index < Output.Length; Index++)
            {
                Output[Index] = GetUVTransform(
                    Materials[Index].Name,
                    Materials[Index].MaterialParams.TextureCoords);
            }

            return Output;
        }

        public UVTransform[] GetUVTransform(string MaterialName, H3DTextureCoord[] TexCoords)
        {
            Vector2[] Scale = new Vector2[3];
            float[]   Rot   = new float[3];
            Vector2[] Trans = new Vector2[3];

            for (int Index = 0; Index < TexCoords.Length; Index++)
            {
                Scale[Index] = TexCoords[Index].Scale.ToVector2();
                Rot[Index]   = TexCoords[Index].Rotation;
                Trans[Index] = TexCoords[Index].Translation.ToVector2();
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

                            case H3DAnimTargetType.MaterialTexCoord0Trans: SetVector2(Vector, ref Trans[0]); break;
                            case H3DAnimTargetType.MaterialTexCoord1Trans: SetVector2(Vector, ref Trans[1]); break;
                            case H3DAnimTargetType.MaterialTexCoord2Trans: SetVector2(Vector, ref Trans[2]); break;
                        }
                    }
                    else if (Elem.PrimitiveType == H3DAnimPrimitiveType.Float)
                    {
                        H3DFloatKeyFrameGroup Float = ((H3DAnimFloat)Elem.Content).Value;

                        if (!Float.HasData) continue;

                        float Value = Float.GetFrameValue(Frame);

                        switch (Elem.TargetType)
                        {
                            case H3DAnimTargetType.MaterialTexCoord0Rot: Rot[0] = Value; break;
                            case H3DAnimTargetType.MaterialTexCoord1Rot: Rot[1] = Value; break;
                            case H3DAnimTargetType.MaterialTexCoord2Rot: Rot[2] = Value; break;
                        }
                    }
                }
            }

            UVTransform[] Transforms = new UVTransform[3];

            for (int Index = 0; Index < 3; Index++)
            {
                Transforms[Index].Scale       = Scale[Index];
                Transforms[Index].Transform   = Matrix2.CreateRotation(Rot[Index]);
                Transforms[Index].Translation = Trans[Index];
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
