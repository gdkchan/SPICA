using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model.Material;

using System.Linq;
using System.Numerics;

namespace SPICA.Renderer.Animation
{
    public class MaterialAnimation : AnimationControl
    {
        public OpenTK.Matrix3[][] GetUVTransforms(PatriciaList<H3DMaterial> Materials)
        {
            OpenTK.Matrix3[][] Output = new OpenTK.Matrix3[Materials.Count][];

            for (int Index = 0; Index < Output.Length; Index++)
            {
                Output[Index] = GetUVTransform(
                    Materials[Index].Name,
                    Materials[Index].MaterialParams);
            }

            return Output;
        }

        public OpenTK.Matrix3[] GetUVTransform(string MaterialName, H3DMaterialParams Params)
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
                            case H3DAnimTargetType.MaterialTexCoord0Scale: TC[0].Scale = SetVector2(Vector, TC[0].Scale); break;
                            case H3DAnimTargetType.MaterialTexCoord1Scale: TC[1].Scale = SetVector2(Vector, TC[1].Scale); break;
                            case H3DAnimTargetType.MaterialTexCoord2Scale: TC[2].Scale = SetVector2(Vector, TC[2].Scale); break;

                            case H3DAnimTargetType.MaterialTexCoord0Trans: TC[0].Translation = SetVector2(Vector, TC[0].Translation); break;
                            case H3DAnimTargetType.MaterialTexCoord1Trans: TC[1].Translation = SetVector2(Vector, TC[1].Translation); break;
                            case H3DAnimTargetType.MaterialTexCoord2Trans: TC[2].Translation = SetVector2(Vector, TC[2].Translation); break;
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

            OpenTK.Matrix3[] Transforms = new OpenTK.Matrix3[3];

            for (int Index = 0; Index < 3; Index++)
            {
                Math3D.Matrix3x4 Trans = TC[Index].Transform;

                OpenTK.Matrix3 Transform = OpenTK.Matrix3.Identity;

                Transform.Row0.X = Trans.M11;
                Transform.Row0.Y = Trans.M12;
                Transform.Row1.X = Trans.M21;
                Transform.Row1.Y = Trans.M22;
                Transform.Row2.X = Trans.M41;
                Transform.Row2.Y = Trans.M42;

                Transforms[Index] = Transform;
            }

            return Transforms;
        }

        private Vector2 SetVector2(H3DAnimVector2D Vector, Vector2 Target)
        {
            if (Vector.X.HasData) Target.X = Vector.X.GetFrameValue(Frame);
            if (Vector.Y.HasData) Target.Y = Vector.Y.GetFrameValue(Frame);

            return Target;
        }
    }
}
