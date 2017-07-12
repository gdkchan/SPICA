using OpenTK;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Rendering.SPICA_GL;

using System.Linq;

namespace SPICA.Rendering.Animation
{
    public class MaterialAnimation : AnimationControl
    {
        public Matrix4[][] GetUVTransforms(H3DDict<H3DMaterial> Materials)
        {
            Matrix4[][] Output = new Matrix4[Materials.Count][];

            for (int Index = 0; Index < Output.Length; Index++)
            {
                Output[Index] = GetUVTransform(
                    Materials[Index].Name,
                    Materials[Index].MaterialParams);
            }

            return Output;
        }

        public Matrix4[] GetUVTransform(string MaterialName, H3DMaterialParams Params)
        {
            H3DTextureCoord[] TC = new H3DTextureCoord[3];

            for (int Index = 0; Index < 3; Index++)
            {
                TC[Index] = Params.TextureCoords[Index];
            }

            if (Animation != null && State != AnimationState.Stopped)
            {
                foreach (H3DAnimationElement Elem in Animation.Elements.Where(x => x.Name == MaterialName))
                {
                    if (Elem.PrimitiveType == H3DPrimitiveType.Vector2D)
                    {
                        H3DAnimVector2D Vector = (H3DAnimVector2D)Elem.Content;

                        switch (Elem.TargetType)
                        {
                            case H3DTargetType.MaterialTexCoord0Scale: SetVector2(Vector, ref TC[0].Scale);       break;
                            case H3DTargetType.MaterialTexCoord1Scale: SetVector2(Vector, ref TC[1].Scale);       break;
                            case H3DTargetType.MaterialTexCoord2Scale: SetVector2(Vector, ref TC[2].Scale);       break;

                            case H3DTargetType.MaterialTexCoord0Trans: SetVector2(Vector, ref TC[0].Translation); break;
                            case H3DTargetType.MaterialTexCoord1Trans: SetVector2(Vector, ref TC[1].Translation); break;
                            case H3DTargetType.MaterialTexCoord2Trans: SetVector2(Vector, ref TC[2].Translation); break;
                        }
                    }
                    else if (Elem.PrimitiveType == H3DPrimitiveType.Float)
                    {
                        H3DFloatKeyFrameGroup Float = ((H3DAnimFloat)Elem.Content).Value;

                        if (!Float.Exists) continue;

                        float Value = Float.GetFrameValue(Frame);

                        switch (Elem.TargetType)
                        {
                            case H3DTargetType.MaterialTexCoord0Rot: TC[0].Rotation = Value; break;
                            case H3DTargetType.MaterialTexCoord1Rot: TC[1].Rotation = Value; break;
                            case H3DTargetType.MaterialTexCoord2Rot: TC[2].Rotation = Value; break;
                        }
                    }
                }
            }

            Matrix4[] Transforms = new Matrix4[3];

            for (int Index = 0; Index < 3; Index++)
            {
                Transforms[Index] = TC[Index].GetTransform().ToMatrix4();
            }

            return Transforms;
        }

        private void SetVector2(H3DAnimVector2D Vector, ref System.Numerics.Vector2 Target)
        {
            if (Vector.X.Exists) Target.X = Vector.X.GetFrameValue(Frame);
            if (Vector.Y.Exists) Target.Y = Vector.Y.GetFrameValue(Frame);
        }
    }
}
