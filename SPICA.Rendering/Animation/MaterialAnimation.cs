using OpenTK.Graphics;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Rendering.SPICA_GL;

using System.Collections.Generic;

namespace SPICA.Rendering.Animation
{
    public class MaterialAnimation : AnimationControl
    {
        private H3DDict<H3DMaterial> Materials;

        private MaterialState[] States;

        public MaterialAnimation(H3DDict<H3DMaterial> Materials)
        {
            this.Materials = Materials;

            States = new MaterialState[Materials.Count];

            for (int i = 0; i < States.Length; i++)
            {
                States[i] = new MaterialState();
            }
        }

        public override void SetAnimations(IEnumerable<H3DAnimation> Animations)
        {
            ResetStates();

            SetAnimations(Animations, Materials);
        }

        private void ResetStates()
        {
            for (int i = 0; i < Materials.Count; i++)
            {
                MaterialState State = States[i];

                H3DMaterialParams Params = Materials[i].MaterialParams;

                State.Transforms[0] = Params.TextureCoords[0].GetTransform().ToMatrix4();
                State.Transforms[1] = Params.TextureCoords[1].GetTransform().ToMatrix4();
                State.Transforms[2] = Params.TextureCoords[2].GetTransform().ToMatrix4();

                State.Emission  = Params.EmissionColor .ToColor4();
                State.Ambient   = Params.AmbientColor  .ToColor4();
                State.Diffuse   = Params.DiffuseColor  .ToColor4();
                State.Specular0 = Params.Specular0Color.ToColor4();
                State.Specular1 = Params.Specular1Color.ToColor4();
                State.Constant0 = Params.Constant0Color.ToColor4();
                State.Constant1 = Params.Constant1Color.ToColor4();
                State.Constant2 = Params.Constant2Color.ToColor4();
                State.Constant3 = Params.Constant3Color.ToColor4();
                State.Constant4 = Params.Constant4Color.ToColor4();
                State.Constant5 = Params.Constant5Color.ToColor4();

                State.Texture0Name = Materials[i].Texture0Name;
                State.Texture1Name = Materials[i].Texture1Name;
                State.Texture2Name = Materials[i].Texture2Name;
            }
        }

        public MaterialState[] GetMaterialStates()
        {
            if (State == AnimationState.Stopped)
            {
                ResetStates();
            }

            if (State != AnimationState.Playing || Elements.Count == 0)
            {
                return States;
            }

            for (int i = 0; i < Elements.Count; i++)
            {
                int Index = Indices[i];

                MaterialState State = States[Index];

                H3DMaterial Material = Materials[Index];

                H3DMaterialParams Params = Material.MaterialParams;

                H3DAnimationElement Elem = Elements[i];

                H3DTextureCoord[] TC = new H3DTextureCoord[]
                {
                    Params.TextureCoords[0],
                    Params.TextureCoords[1],
                    Params.TextureCoords[2]
                };

                if (Elem.PrimitiveType == H3DPrimitiveType.RGBA)
                {
                    H3DAnimRGBA RGBA = (H3DAnimRGBA)Elem.Content;

                    switch (Elem.TargetType)
                    {
                        case H3DTargetType.MaterialEmission:  SetRGBA(RGBA, ref State.Emission);  break;
                        case H3DTargetType.MaterialAmbient:   SetRGBA(RGBA, ref State.Ambient);   break;
                        case H3DTargetType.MaterialDiffuse:   SetRGBA(RGBA, ref State.Diffuse);   break;
                        case H3DTargetType.MaterialSpecular0: SetRGBA(RGBA, ref State.Specular0); break;
                        case H3DTargetType.MaterialSpecular1: SetRGBA(RGBA, ref State.Specular1); break;
                        case H3DTargetType.MaterialConstant0: SetRGBA(RGBA, ref State.Constant0); break;
                        case H3DTargetType.MaterialConstant1: SetRGBA(RGBA, ref State.Constant1); break;
                        case H3DTargetType.MaterialConstant2: SetRGBA(RGBA, ref State.Constant2); break;
                        case H3DTargetType.MaterialConstant3: SetRGBA(RGBA, ref State.Constant3); break;
                        case H3DTargetType.MaterialConstant4: SetRGBA(RGBA, ref State.Constant4); break;
                        case H3DTargetType.MaterialConstant5: SetRGBA(RGBA, ref State.Constant5); break;
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
                else if (Elem.PrimitiveType == H3DPrimitiveType.Texture)
                {
                    H3DFloatKeyFrameGroup Int = ((H3DAnimFloat)Elem.Content).Value;

                    if (!Int.Exists) continue;

                    int Value = (int)Int.GetFrameValue(Frame);

                    string Name = TextureNames[Value];

                    switch (Elem.TargetType)
                    {
                        case H3DTargetType.MaterialMapper0Texture: State.Texture0Name = Name; break;
                        case H3DTargetType.MaterialMapper1Texture: State.Texture1Name = Name; break;
                        case H3DTargetType.MaterialMapper2Texture: State.Texture2Name = Name; break;
                    }
                }

                State.Transforms[0] = TC[0].GetTransform().ToMatrix4();
                State.Transforms[1] = TC[1].GetTransform().ToMatrix4();
                State.Transforms[2] = TC[2].GetTransform().ToMatrix4();
            }

            return States;
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
