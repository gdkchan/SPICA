using OpenTK;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Material.Texture;
using SPICA.Math3D;

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
            H3DTextureCoord[] TC = new H3DTextureCoord[3];

            for (int Index = 0; Index < 3; Index++)
            {
                TC[Index] = Params.TextureCoords[Index];
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
                            case H3DAnimTargetType.MaterialTexCoord0Scale: SetVector2(Vector, ref TC[0].Scale); break;
                            case H3DAnimTargetType.MaterialTexCoord1Scale: SetVector2(Vector, ref TC[1].Scale); break;
                            case H3DAnimTargetType.MaterialTexCoord2Scale: SetVector2(Vector, ref TC[2].Scale); break;

                            case H3DAnimTargetType.MaterialTexCoord0Trans: SetVector2(Vector, ref TC[0].Translation); break;
                            case H3DAnimTargetType.MaterialTexCoord1Trans: SetVector2(Vector, ref TC[1].Translation); break;
                            case H3DAnimTargetType.MaterialTexCoord2Trans: SetVector2(Vector, ref TC[2].Translation); break;
                        }
                    }
                    else if (Elem.PrimitiveType == H3DAnimPrimitiveType.Float)
                    {
                        H3DFloatKeyFrameGroup Float = ((H3DAnimFloat)Elem.Content).Value;

                        if (!Float.HasData) continue;

                        float Value = Float.GetFrameValue(Frame);

                        switch (Elem.TargetType)
                        {
                            case H3DAnimTargetType.MaterialTexCoord0Rot: TC[0].Rotation = Value; break;
                            case H3DAnimTargetType.MaterialTexCoord1Rot: TC[1].Rotation = Value; break;
                            case H3DAnimTargetType.MaterialTexCoord2Rot: TC[2].Rotation = Value; break;
                        }
                    }
                }
            }

            Matrix3[] Transforms = new Matrix3[3];

            for (int Index = 0; Index < 3; Index++)
            {
                Math3D.Matrix3x4 Trans = TC[Index].Transform;

                Matrix3 Transform = Matrix3.Identity;

                Transform.Row0.X = Trans.M11;
                Transform.Row0.Y = Trans.M21;
                Transform.Row1.X = Trans.M12;
                Transform.Row1.Y = Trans.M22;
                Transform.Row2.X = Trans.M14;
                Transform.Row2.Y = Trans.M24;

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
