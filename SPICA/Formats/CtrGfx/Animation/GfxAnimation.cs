using SPICA.Formats.Common;
using SPICA.Formats.CtrH3D.Animation;

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SPICA.Formats.CtrGfx.Animation
{
    public class GfxAnimation : INamed
    {
        private GfxRevHeader Header;

        private string _Name;

        public string Name
        {
            get => _Name;
            set => _Name = value ?? throw Exceptions.GetNullException("Name");
        }

        public string TargetAnimGroupName;

        public GfxLoopMode LoopMode;

        public float FramesCount;

        public GfxDict<GfxAnimationElement> Elements;

        public GfxMetaData MetaData;

        private const string MatCoordScaleREx = @"Materials\[""(.+)""\]\.TextureCoordinators\[(\d)\]\.Scale";
        private const string MatCoordRotREx   = @"Materials\[""(.+)""\]\.TextureCoordinators\[(\d)\]\.Rotate";
        private const string MatCoordTransREx = @"Materials\[""(.+)""\]\.TextureCoordinators\[(\d)\]\.Translate";
        private const string MatColorREx      = @"Materials\[""(.+)""\]\.MaterialColor\.(\w+)";
        private const string MatMapperBCREx   = @"Materials\[""(.+)""\]\.TextureMappers\[(\d)\]\.Sampler\.BorderColor";
        private const string MeshNodeVisREx   = @"MeshNodeVisibilities\[""(.+)""\]\.IsVisible";

        private const string ViewUpdaterTarget = "ViewUpdater.TargetPosition";
        private const string ViewUpdaterUpVec  = "ViewUpdater.UpwardVector";
        private const string ViewUpdaterRotate = "ViewUpdater.ViewRotate";
        private const string ViewUpdaterTwist  = "ViewUpdater.Twist";

        private const string ProjectionUpdaterNear = "ProjectionUpdater.Near";
        private const string ProjectionUpdaterFar  = "ProjectionUpdater.Far";
        private const string ProjectionUpdaterFOVY = "ProjectionUpdater.Fovy";

        public GfxAnimation()
        {
            Elements = new GfxDict<GfxAnimationElement>();
        }

        public H3DAnimation ToH3DAnimation()
        {
            H3DAnimation Output = new H3DAnimation()
            {
                Name           = _Name,
                FramesCount    = FramesCount,
                AnimationFlags = (H3DAnimationFlags)LoopMode
            };

            switch (TargetAnimGroupName)
            {
                case "SkeletalAnimation":   Output.AnimationType = H3DAnimationType.Skeletal;   break;
                case "MaterialAnimation":   Output.AnimationType = H3DAnimationType.Material;   break;
                case "VisibilityAnimation": Output.AnimationType = H3DAnimationType.Visibility; break;
                case "LightAnimation":      Output.AnimationType = H3DAnimationType.Light;      break;
                case "CameraAnimation":     Output.AnimationType = H3DAnimationType.Camera;     break;
                case "FogAnimation":        Output.AnimationType = H3DAnimationType.Fog;        break;
            }

            foreach (GfxAnimationElement Elem in Elements)
            {
                switch (Elem.PrimitiveType)
            	{
                    case GfxPrimitiveType.Float:
                        {
                            H3DAnimFloat Float = new H3DAnimFloat();

                            CopyKeyFrames(((GfxAnimFloat)Elem.Content).Value, Float.Value);

                            H3DTargetType TargetType = 0;

                            string Name = Elem.Name;

                            if (Name == ProjectionUpdaterNear)
                            {
                                TargetType = H3DTargetType.CameraZNear;
                            }
                            else if (Name == ProjectionUpdaterFar)
                            {
                                TargetType = H3DTargetType.CameraZFar;
                            }
                            else if (Name == ViewUpdaterTwist)
                            {
                                TargetType = H3DTargetType.CameraTwist;
                            }
                            else
                            {
                                Match Path = Regex.Match(Elem.Name, MatCoordRotREx);

                                if (Path.Success && int.TryParse(Path.Groups[2].Value, out int CoordIdx))
                                {
                                    Name = Path.Groups[1].Value;

                                    switch (CoordIdx)
                                    {
                                        case 0: TargetType = H3DTargetType.MaterialTexCoord0Rot; break;
                                        case 1: TargetType = H3DTargetType.MaterialTexCoord1Rot; break;
                                        case 2: TargetType = H3DTargetType.MaterialTexCoord2Rot; break;
                                    }                                    
                                }
                            }

                            if (TargetType != 0)
                            {
                                Output.Elements.Add(new H3DAnimationElement()
                                {
                                    Name          = Name,
                                    Content       = Float,
                                    PrimitiveType = H3DPrimitiveType.Float,
                                    TargetType    = TargetType
                                });
                            }
                        }
                        break;

                    case GfxPrimitiveType.Boolean:
                        {
                            H3DAnimBoolean Bool = new H3DAnimBoolean();

                            GfxAnimBoolean Source = (GfxAnimBoolean)Elem.Content;

                            Bool.StartFrame = Source.StartFrame;
                            Bool.EndFrame   = Source.EndFrame;

                            Bool.PreRepeat  = (H3DLoopType)Source.PreRepeat;
                            Bool.PostRepeat = (H3DLoopType)Source.PostRepeat;

                            CopyList(Source.Values, Bool.Values);

                            H3DTargetType TargetType = 0;

                            Match Path = Regex.Match(Elem.Name, MeshNodeVisREx);

                            if (Path.Success)
                            {
                                TargetType = H3DTargetType.MeshNodeVisibility;
                            }

                            if (Path.Success && TargetType != 0)
                            {
                                string Name = Path.Groups[1].Value;

                                Output.Elements.Add(new H3DAnimationElement()
                                {
                                    Name          = Name,
                                    Content       = Bool,
                                    PrimitiveType = H3DPrimitiveType.Boolean,
                                    TargetType    = TargetType
                                });
                            }
                        }
                        break;

                    case GfxPrimitiveType.Vector2D:
                        {
                            H3DAnimVector2D Vector = new H3DAnimVector2D();

                            CopyKeyFrames(((GfxAnimVector2D)Elem.Content).X, Vector.X);
                            CopyKeyFrames(((GfxAnimVector2D)Elem.Content).Y, Vector.Y);

                            Match Path = Regex.Match(Elem.Name, MatCoordScaleREx);

                            bool IsTranslation = !Path.Success;

                            if (IsTranslation)
                            {
                                Path = Regex.Match(Elem.Name, MatCoordTransREx);
                            }

                            if (Path.Success && int.TryParse(Path.Groups[2].Value, out int CoordIdx))
                            {
                                H3DTargetType TargetType = 0;

                                switch (CoordIdx)
                                {
                                    case 0: TargetType = H3DTargetType.MaterialTexCoord0Scale; break;
                                    case 1: TargetType = H3DTargetType.MaterialTexCoord1Scale; break;
                                    case 2: TargetType = H3DTargetType.MaterialTexCoord2Scale; break;
                                }

                                if (TargetType != 0)
                                {
                                    string Name = Path.Groups[1].Value;

                                    if (IsTranslation)
                                    {
                                        TargetType += 2;
                                    }

                                    Output.Elements.Add(new H3DAnimationElement()
                                    {
                                        Name          = Name,
                                        Content       = Vector,
                                        PrimitiveType = H3DPrimitiveType.Vector2D,
                                        TargetType    = TargetType
                                    });
                                }
                            }
                        }
                        break;

                    case GfxPrimitiveType.Vector3D:
                        {
                            H3DAnimVector3D Vector = new H3DAnimVector3D();

                            CopyKeyFrames(((GfxAnimVector3D)Elem.Content).X, Vector.X);
                            CopyKeyFrames(((GfxAnimVector3D)Elem.Content).Y, Vector.Y);
                            CopyKeyFrames(((GfxAnimVector3D)Elem.Content).Z, Vector.Z);

                            H3DTargetType TargetType = 0;

                            switch (Elem.Name)
                            {
                                case ViewUpdaterTarget: TargetType = H3DTargetType.CameraTargetPos;    break;
                                case ViewUpdaterUpVec:  TargetType = H3DTargetType.CameraUpVector;     break;
                                case ViewUpdaterRotate: TargetType = H3DTargetType.CameraViewRotation; break;
                            }

                            if (TargetType != 0)
                            {
                                Output.Elements.Add(new H3DAnimationElement()
                                {
                                    Name          = Elem.Name,
                                    Content       = Vector,
                                    PrimitiveType = H3DPrimitiveType.Vector3D,
                                    TargetType    = TargetType
                                });
                            }
                        }
                        break;

                    case GfxPrimitiveType.Transform:
                        {
                            H3DAnimTransform Transform = new H3DAnimTransform();

                            CopyKeyFrames(((GfxAnimTransform)Elem.Content).ScaleX,       Transform.ScaleX);
                            CopyKeyFrames(((GfxAnimTransform)Elem.Content).ScaleY,       Transform.ScaleY);
                            CopyKeyFrames(((GfxAnimTransform)Elem.Content).ScaleZ,       Transform.ScaleZ);

                            CopyKeyFrames(((GfxAnimTransform)Elem.Content).RotationX,    Transform.RotationX);
                            CopyKeyFrames(((GfxAnimTransform)Elem.Content).RotationY,    Transform.RotationY);
                            CopyKeyFrames(((GfxAnimTransform)Elem.Content).RotationZ,    Transform.RotationZ);

                            CopyKeyFrames(((GfxAnimTransform)Elem.Content).TranslationX, Transform.TranslationX);
                            CopyKeyFrames(((GfxAnimTransform)Elem.Content).TranslationY, Transform.TranslationY);
                            CopyKeyFrames(((GfxAnimTransform)Elem.Content).TranslationZ, Transform.TranslationZ);

                            H3DTargetType TargetType = 0;

                            switch (Output.AnimationType)
                            {
                                case H3DAnimationType.Skeletal: TargetType = H3DTargetType.Bone;            break;
                                case H3DAnimationType.Camera:   TargetType = H3DTargetType.CameraTransform; break;
                                case H3DAnimationType.Light:    TargetType = H3DTargetType.LightTransform;  break;
                            }

                            Output.Elements.Add(new H3DAnimationElement()
                            {
                                Name          = Elem.Name,
                                Content       = Transform,
                                PrimitiveType = H3DPrimitiveType.Transform,
                                TargetType    = TargetType
                            });
                        }
                        break;

                    case GfxPrimitiveType.RGBA:
                        {
                            H3DAnimRGBA RGBA = new H3DAnimRGBA();

                            CopyKeyFrames(((GfxAnimRGBA)Elem.Content).R, RGBA.R);
                            CopyKeyFrames(((GfxAnimRGBA)Elem.Content).G, RGBA.G);
                            CopyKeyFrames(((GfxAnimRGBA)Elem.Content).B, RGBA.B);
                            CopyKeyFrames(((GfxAnimRGBA)Elem.Content).A, RGBA.A);

                            Match Path = Regex.Match(Elem.Name, MatColorREx);

                            H3DTargetType TargetType = 0;

                            if (Path.Success)
                            {
                                switch (Path.Groups[2].Value)
                                {
                                    case "Emission":  TargetType = H3DTargetType.MaterialEmission;  break;
                                    case "Ambient":   TargetType = H3DTargetType.MaterialAmbient;   break;
                                    case "Diffuse":   TargetType = H3DTargetType.MaterialDiffuse;   break;
                                    case "Specular0": TargetType = H3DTargetType.MaterialSpecular0; break;
                                    case "Specular1": TargetType = H3DTargetType.MaterialSpecular1; break;
                                    case "Constant0": TargetType = H3DTargetType.MaterialConstant0; break;
                                    case "Constant1": TargetType = H3DTargetType.MaterialConstant1; break;
                                    case "Constant2": TargetType = H3DTargetType.MaterialConstant2; break;
                                    case "Constant3": TargetType = H3DTargetType.MaterialConstant3; break;
                                    case "Constant4": TargetType = H3DTargetType.MaterialConstant4; break;
                                    case "Constant5": TargetType = H3DTargetType.MaterialConstant5; break;
                                }
                            }
                            else
                            {
                                Path = Regex.Match(Elem.Name, MatMapperBCREx);

                                if (Path.Success && int.TryParse(Path.Groups[2].Value, out int MapperIdx))
                                {
                                    switch (MapperIdx)
                                    {
                                        case 0: TargetType = H3DTargetType.MaterialMapper0BorderCol; break;
                                        case 1: TargetType = H3DTargetType.MaterialMapper1BorderCol; break;
                                        case 2: TargetType = H3DTargetType.MaterialMapper2BorderCol; break;
                                    }
                                }
                            }

                            if (Path.Success && TargetType != 0)
                            {
                                string Name = Path.Groups[1].Value;

                                Output.Elements.Add(new H3DAnimationElement()
                                {
                                    Name          = Name,
                                    Content       = RGBA,
                                    PrimitiveType = H3DPrimitiveType.RGBA,
                                    TargetType    = TargetType
                                });
                            }
                        }
                        break;

                    case GfxPrimitiveType.QuatTransform:
                        {
                            H3DAnimQuatTransform QuatTransform = new H3DAnimQuatTransform();

                            CopyList(((GfxAnimQuatTransform)Elem.Content).Scales,       QuatTransform.Scales);
                            CopyList(((GfxAnimQuatTransform)Elem.Content).Rotations,    QuatTransform.Rotations);
                            CopyList(((GfxAnimQuatTransform)Elem.Content).Translations, QuatTransform.Translations);

                            Output.Elements.Add(new H3DAnimationElement()
                            {
                                Name          = Elem.Name,
                                Content       = QuatTransform,
                                PrimitiveType = H3DPrimitiveType.QuatTransform,
                                TargetType    = H3DTargetType.Bone
                            });
                        }
                        break;

                    case GfxPrimitiveType.MtxTransform:
                        {
                            H3DAnimMtxTransform MtxTransform = new H3DAnimMtxTransform();

                            GfxAnimMtxTransform Source = (GfxAnimMtxTransform)Elem.Content;

                            MtxTransform.StartFrame = Source.StartFrame;
                            MtxTransform.EndFrame   = Source.EndFrame;

                            MtxTransform.PreRepeat  = (H3DLoopType)Source.PreRepeat;
                            MtxTransform.PostRepeat = (H3DLoopType)Source.PostRepeat;

                            CopyList(Source.Frames, MtxTransform.Frames);

                            Output.Elements.Add(new H3DAnimationElement()
                            {
                                Name          = Elem.Name,
                                Content       = MtxTransform,
                                PrimitiveType = H3DPrimitiveType.MtxTransform,
                                TargetType    = H3DTargetType.Bone
                            });
                        }
                        break;
                }
            }

            return Output;
        }

        private void CopyKeyFrames(GfxFloatKeyFrameGroup Source, H3DFloatKeyFrameGroup Target)
        {
            Target.StartFrame = Source.StartFrame;
            Target.EndFrame   = Source.EndFrame;

            Target.PreRepeat  = (H3DLoopType)Source.PreRepeat;
            Target.PostRepeat = (H3DLoopType)Source.PostRepeat;

            Target.Quantization = Source.Quantization;

            if (Source.Quantization == KeyFrameQuantization.StepLinear32 ||
                Source.Quantization == KeyFrameQuantization.StepLinear64)
            {
                Target.InterpolationType = Source.IsLinear
                    ? H3DInterpolationType.Linear
                    : H3DInterpolationType.Step;
            }
            else
            {
                Target.InterpolationType = H3DInterpolationType.Hermite;
            }

            foreach (KeyFrame KF in Source.KeyFrames)
            {
                Target.KeyFrames.Add(KF);
            }
        }

        private void CopyList<T>(List<T> Source, List<T> Target)
        {
            foreach (T Item in Source)
            {
                Target.Add(Item);
            }
        }
    }
}
