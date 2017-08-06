using OpenTK;
using OpenTK.Graphics;
using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Rendering.SPICA_GL;

using System.Linq;

namespace SPICA.Rendering.Animation
{
    public class MaterialAnimation : AnimationControl
    {
        public MaterialState[] GetMaterialStates(H3DDict<H3DMaterial> Materials)
        {
            MaterialState[] Output = new MaterialState[Materials.Count];

            for (int Index = 0; Index < Output.Length; Index++)
            {
                Output[Index] = GetMaterialState(
                    Materials[Index].MaterialParams,
                    Materials[Index].Name);
            }

            return Output;
        }

        public MaterialState GetMaterialState(H3DMaterialParams Params, string MaterialName)
        {
            MaterialState Output = new MaterialState()
            {
                Emission  = Params.EmissionColor.ToColor4(),
                Ambient   = Params.AmbientColor.ToColor4(),
                Diffuse   = Params.DiffuseColor.ToColor4(),
                Specular0 = Params.Specular0Color.ToColor4(),
                Specular1 = Params.Specular1Color.ToColor4(),
                Constant0 = Params.Constant0Color.ToColor4(),
                Constant1 = Params.Constant1Color.ToColor4(),
                Constant2 = Params.Constant2Color.ToColor4(),
                Constant3 = Params.Constant3Color.ToColor4(),
                Constant4 = Params.Constant4Color.ToColor4(),
                Constant5 = Params.Constant5Color.ToColor4()
            };

            H3DTextureCoord[] TC = new H3DTextureCoord[3];

            for (int Index = 0; Index < 3; Index++)
            {
                TC[Index] = Params.TextureCoords[Index];
            }

            if (Animation != null && State != AnimationState.Stopped)
            {
                foreach (H3DAnimationElement Elem in Animation.Elements.Where(x => x.Name == MaterialName))
                {
                    if (Elem.PrimitiveType == H3DPrimitiveType.RGBA)
                    {
                        H3DAnimRGBA RGBA = (H3DAnimRGBA)Elem.Content;

                        switch (Elem.TargetType)
                        {
                            case H3DTargetType.MaterialEmission:  SetRGBA(RGBA, ref Output.Emission);  break;
                            case H3DTargetType.MaterialAmbient:   SetRGBA(RGBA, ref Output.Ambient);   break;
                            case H3DTargetType.MaterialDiffuse:   SetRGBA(RGBA, ref Output.Diffuse);   break;
                            case H3DTargetType.MaterialSpecular0: SetRGBA(RGBA, ref Output.Specular0); break;
                            case H3DTargetType.MaterialSpecular1: SetRGBA(RGBA, ref Output.Specular1); break;
                            case H3DTargetType.MaterialConstant0: SetRGBA(RGBA, ref Output.Constant0); break;
                            case H3DTargetType.MaterialConstant1: SetRGBA(RGBA, ref Output.Constant1); break;
                            case H3DTargetType.MaterialConstant2: SetRGBA(RGBA, ref Output.Constant2); break;
                            case H3DTargetType.MaterialConstant3: SetRGBA(RGBA, ref Output.Constant3); break;
                            case H3DTargetType.MaterialConstant4: SetRGBA(RGBA, ref Output.Constant4); break;
                            case H3DTargetType.MaterialConstant5: SetRGBA(RGBA, ref Output.Constant5); break;
                        }
                    }
                    else if (Elem.PrimitiveType == H3DPrimitiveType.Vector2D)
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

            for (int Index = 0; Index < 3; Index++)
            {
                Output.Transforms[Index] = TC[Index].GetTransform().ToMatrix4();
            }

            return Output;
        }

        private void SetRGBA(H3DAnimRGBA RGBA, ref Color4 Target)
        {
            if (RGBA.R.Exists) Target.R = RGBA.R.GetFrameValue(Frame);
            if (RGBA.G.Exists) Target.G = RGBA.G.GetFrameValue(Frame);
            if (RGBA.B.Exists) Target.B = RGBA.B.GetFrameValue(Frame);
            if (RGBA.A.Exists) Target.A = RGBA.A.GetFrameValue(Frame);
        }

        private void SetVector2(H3DAnimVector2D Vector, ref System.Numerics.Vector2 Target)
        {
            if (Vector.X.Exists) Target.X = Vector.X.GetFrameValue(Frame);
            if (Vector.Y.Exists) Target.Y = Vector.Y.GetFrameValue(Frame);
        }
    }
}
