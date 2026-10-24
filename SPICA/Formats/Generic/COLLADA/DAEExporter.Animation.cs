using SPICA.Formats.Common;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;

namespace SPICA.Formats.Generic.COLLADA
{
    internal sealed partial class DAEExporter
    {
        private sealed class Pose
        {
            public Matrix4x4[] Local;
            public Matrix4x4[] World;
        }

        private void ExportMatrixAnimation(H3DAnimation Animation)
        {
            if (!float.IsFinite(Animation.FramesCount) || Animation.FramesCount < 0)
            {
                throw new InvalidDataException("Invalid animation duration.");
            }

            H3DAnimationElement[] Elements = new H3DAnimationElement[BoneNodes.Length];
            foreach (H3DAnimationElement Element in Animation.Elements)
            {
                int Index = Enumerable.Range(0, BoneNodes.Length).FirstOrDefault(I => Model.Skeleton[I].Name == Element.Name, -1);
                if (Index < 0)
                {
                    Warn("Animation track '" + Element.Name + "' does not match a bone of model '" + Model.Name + "'.");
                    continue;
                }

                if (Element.Content is not H3DAnimTransform && Element.Content is not H3DAnimQuatTransform && Element.Content is not H3DAnimMtxTransform)
                {
                    Warn("Animation track '" + Element.Name + "' has an unsupported skeletal primitive: " + Element.PrimitiveType + ".");
                    continue;
                }

                if (Elements[Index] != null)
                {
                    throw new InvalidDataException("Duplicate animation track for bone '" + Element.Name + "'.");
                }

                Elements[Index] = Element;
            }

            Output.extra.Add(Extra("source_animation", new
            {
                Animation.Name,
                Animation.FramesCount,
                Animation.AnimationFlags,
                tracks = Animation.Elements.Select(E => new { E.Name, E.TargetType, E.PrimitiveType, E.Content }).ToArray()
            }));
            SortedSet<float> Frames = new SortedSet<float>
            {
                0,
                Animation.FramesCount
            };
            HashSet<float> Steps = new HashSet<float>();
            for (int I = 1; I < Math.Ceiling((double)Animation.FramesCount * Options.SamplesPerFrame); I++)
            {
                Frames.Add((float)I / Options.SamplesPerFrame);
            }

            void AddStep(float Frame)
            {
                if (Frame <= 0 || Frame > Animation.FramesCount)
                {
                    return;
                }

                Steps.Add(Frame);
                Frames.Add(Frame);
                Frames.Add(MathF.BitDecrement(Frame));
            }

            foreach (H3DAnimationElement Element in Elements.Where(E => E != null))
            {
                if (Element.Content is H3DAnimTransform Transform)
                {
                    foreach (H3DFloatKeyFrameGroup Group in Groups(Transform))
                    {
                        foreach (KeyFrame Key in Group.KeyFrames)
                        {
                            if (Key.Frame < 0 || Key.Frame > Animation.FramesCount)
                            {
                                continue;
                            }

                            Frames.Add(Key.Frame);
                            if (Group.InterpolationType == H3DInterpolationType.Step)
                            {
                                AddStep(Key.Frame);
                            }
                        }

                        if (Group.PreRepeat != H3DLoopType.None || Group.PostRepeat != H3DLoopType.None)
                        {
                            Warn("Curve repeat modes use SPICA's current evaluator within the exported clip; their original metadata is preserved in extras.");
                        }
                    }
                }

                if (Element.Content is H3DAnimMtxTransform Matrix)
                {
                    for (int I = 1; I < Matrix.Frames.Count && I <= Animation.FramesCount; I++)
                    {
                        AddStep(I);
                    }
                }
            }

            SortedDictionary<float, Pose> Poses = new SortedDictionary<float, Pose>();
            Pose Evaluate(float Frame)
            {
                if (!Poses.TryGetValue(Frame, out Pose Pose))
                {
                    Poses.Add(Frame, Pose = EvaluatePose(Elements, Frame));
                }

                return Pose;
            }

            foreach (float Frame in Frames)
            {
                Evaluate(Frame);
            }

            void Refine(float A, float B, int Depth)
            {
                if (Steps.Contains(B) && A == MathF.BitDecrement(B))
                {
                    return;
                }

                float Mid = A + (B - A) * 0.5f;
                if (Mid <= A || Mid >= B)
                {
                    return;
                }

                Pose Lhs = Evaluate(A), Rhs = Evaluate(B), Truth = EvaluatePose(Elements, Mid);
                Matrix4x4[] Interpolated = new Matrix4x4[BoneNodes.Length];
                Matrix4x4[] World = new Matrix4x4[BoneNodes.Length];
                bool[] Done = new bool[BoneNodes.Length];
                for (int I = 0; I < Interpolated.Length; I++)
                {
                    Interpolated[I] = Matrix4x4.Lerp(Lhs.Local[I], Rhs.Local[I], 0.5f);
                }

                Matrix4x4 Resolve(int I)
                {
                    if (Done[I])
                    {
                        return World[I];
                    }

                    int Parent = Model.Skeleton[I].ParentIndex;
                    World[I] = Parent < 0 ? Interpolated[I] : Interpolated[I] * Resolve(Parent);
                    Done[I] = true;
                    return World[I];
                }

                bool Acceptable = true;
                for (int I = 0; I < BoneNodes.Length; I++)
                {
                    Acceptable &= Error(Interpolated[I], Truth.Local[I]) <= Options.MatrixTolerance && Error(Resolve(I), Truth.World[I]) <= Options.MatrixTolerance;
                }

                if (Acceptable)
                {
                    return;
                }

                if (Depth >= Options.MaxSubdivisionDepth)
                {
                    Warn("Animation bake reached the subdivision limit; increase MaxSubdivisionDepth or relax MatrixTolerance for this clip.");
                    return;
                }

                Poses.Add(Mid, Truth);
                Refine(A, Mid, Depth + 1);
                Refine(Mid, B, Depth + 1);
            }

            float[] Initial = Frames.ToArray();
            for (int I = 1; I < Initial.Length; I++)
            {
                Refine(Initial[I - 1], Initial[I], 0);
            }

            float[] Samples = Poses.Keys.ToArray();
            Output.library_animations = new List<DAEAnimation>();
            for (int Bone = 0; Bone < BoneNodes.Length; Bone++)
            {
                DAEAnimation Track = new DAEAnimation
                {
                    id = Prefix + "animation_" + Bone,
                    name = System.Xml.XmlConvert.EncodeLocalName(Animation.Name + "_" + Model.Skeleton[Bone].Name)
                };
                string[] Interpolation = Samples.Select((Frame, I) => I + 1 < Samples.Length && Steps.Contains(Samples[I + 1]) && Frame == MathF.BitDecrement(Samples[I + 1]) ? "STEP" : "LINEAR").ToArray();
                Track.src.Add(new DAESource(Track.id + "_time", 1, Samples.Select(F => ((double)F / Options.FramesPerSecond).ToString("R", CultureInfo.InvariantCulture)).ToArray(), "TIME", "float"));
                Track.src.Add(new DAESource(Track.id + "_pose", 16, Samples.Select(F => MatrixText(Poses[F].Local[Bone])).ToArray(), "TRANSFORM", "float4x4"));
                Track.src.Add(new DAESource(Track.id + "_interpolation", 1, Interpolation, "INTERPOLATION", "Name"));
                Track.sampler.id = Track.id + "_sampler";
                Track.sampler.AddInput("INPUT", "#" + Track.src[0].id);
                Track.sampler.AddInput("OUTPUT", "#" + Track.src[1].id);
                Track.sampler.AddInput("INTERPOLATION", "#" + Track.src[2].id);
                Track.channel.source = "#" + Track.sampler.id;
                Track.channel.target = BoneId(Bone) + "/transform";
                Output.library_animations.Add(Track);
            }

            Output.extra.Add(Extra("animation_bake", new
            {
                Options.FramesPerSecond,
                Options.SamplesPerFrame,
                Options.MatrixTolerance,
                Options.MaxSubdivisionDepth,
                sampleCount = Samples.Length,
                interpolation = "matrix",
                endTime = Animation.FramesCount / Options.FramesPerSecond
            }));
        }

        private Pose EvaluatePose(H3DAnimationElement[] Elements, float Frame)
        {
            int Count = BoneNodes.Length;
            Vector3[] Scales = new Vector3[Count];
            Vector3[] Translations = new Vector3[Count];
            Matrix4x4[] Rotations = new Matrix4x4[Count];
            bool[] MatrixWorld = new bool[Count];
            Pose Pose = new Pose
            {
                Local = new Matrix4x4[Count],
                World = new Matrix4x4[Count]
            };
            for (int I = 0; I < Count; I++)
            {
                H3DBone Bone = Model.Skeleton[I];
                Scales[I] = Bone.Scale;
                Translations[I] = Bone.Translation;
                Vector3 Euler = Bone.Rotation;
                if (Elements[I]?.Content is H3DAnimTransform Transform)
                {
                    Scales[I] = new Vector3(Value(Transform.ScaleX, Frame, Scales[I].X), Value(Transform.ScaleY, Frame, Scales[I].Y), Value(Transform.ScaleZ, Frame, Scales[I].Z));
                    Translations[I] = new Vector3(Value(Transform.TranslationX, Frame, Translations[I].X), Value(Transform.TranslationY, Frame, Translations[I].Y), Value(Transform.TranslationZ, Frame, Translations[I].Z));
                    Euler = new Vector3(Value(Transform.RotationX, Frame, Euler.X), Value(Transform.RotationY, Frame, Euler.Y), Value(Transform.RotationZ, Frame, Euler.Z));
                }

                Rotations[I] = Matrix4x4.CreateRotationX(Euler.X) * Matrix4x4.CreateRotationY(Euler.Y) * Matrix4x4.CreateRotationZ(Euler.Z);
                if (Elements[I]?.Content is H3DAnimQuatTransform Quaternion)
                {
                    int F = (int)Frame;
                    float Weight = Frame - F;
                    if (Quaternion.HasScale)
                    {
                        Scales[I] = Vector3.Lerp(Quaternion.GetScaleValue(F), Quaternion.GetScaleValue(F + 1), Weight);
                    }

                    if (Quaternion.HasTranslation)
                    {
                        Translations[I] = Vector3.Lerp(Quaternion.GetTranslationValue(F), Quaternion.GetTranslationValue(F + 1), Weight);
                    }

                    if (Quaternion.HasRotation)
                    {
                        Quaternion Rotation = System.Numerics.Quaternion.Slerp(Quaternion.GetRotationValue(F), Quaternion.GetRotationValue(F + 1), Weight);
                        if (Rotation.LengthSquared() == 0)
                        {
                            throw new InvalidDataException("Zero-length quaternion in animation.");
                        }

                        Rotations[I] = Matrix4x4.CreateFromQuaternion(System.Numerics.Quaternion.Normalize(Rotation));
                    }
                }

                if (Elements[I]?.Content is H3DAnimMtxTransform Matrix)
                {
                    if (Matrix.Frames.Count == 0)
                    {
                        throw new InvalidDataException("Empty matrix animation track.");
                    }

                    Pose.World[I] = Affine(Matrix.GetTransform((int)Frame));
                    MatrixWorld[I] = true;
                }
            }

            bool[] Done = new bool[Count];
            Matrix4x4 Resolve(int I)
            {
                if (Done[I])
                {
                    return Pose.World[I];
                }

                H3DBone Bone = Model.Skeleton[I];
                int Parent = Bone.ParentIndex;
                Matrix4x4 ParentWorld = Parent < 0 ? Matrix4x4.Identity : Resolve(Parent);
                if (!MatrixWorld[I])
                {
                    bool Compensate = Parent >= 0 && (Bone.Flags & H3DBoneFlags.IsSegmentScaleCompensate) != 0;
                    Pose.Local[I] = Matrix4x4.CreateScale(Scales[I]) * Rotations[I] * Matrix4x4.CreateTranslation(Compensate ? Translations[I] * Scales[Parent] : Translations[I]);
                    if (Compensate)
                    {
                        if (Scales[Parent].X == 0 || Scales[Parent].Y == 0 || Scales[Parent].Z == 0)
                        {
                            throw new InvalidDataException("Segment scale compensation has a zero parent scale.");
                        }

                        Pose.Local[I] *= Matrix4x4.CreateScale(Vector3.One / Scales[Parent]);
                    }

                    Pose.World[I] = Pose.Local[I] * ParentWorld;
                }
                else
                {
                    if (!Matrix4x4.Invert(ParentWorld, out Matrix4x4 InverseParent))
                    {
                        throw new InvalidDataException("Cannot convert world-space matrix animation through a singular parent transform.");
                    }

                    Pose.Local[I] = Pose.World[I] * InverseParent;
                }

                Done[I] = true;
                return Pose.World[I];
            }

            for (int I = 0; I < Count; I++)
            {
                Resolve(I);
            }

            return Pose;
        }

        private static float Value(H3DFloatKeyFrameGroup Group, float Frame, float Fallback)
        {
            return Group.Exists ? Group.GetFrameValue(Frame) : Fallback;
        }

        private static IEnumerable<H3DFloatKeyFrameGroup> Groups(H3DAnimTransform T)
        {
            return new[]
            {
                T.ScaleX,
                T.ScaleY,
                T.ScaleZ,
                T.RotationX,
                T.RotationY,
                T.RotationZ,
                T.TranslationX,
                T.TranslationY,
                T.TranslationZ
            };
        }

        private static float Error(Matrix4x4 A, Matrix4x4 B)
        {
            Matrix4x4 D = A - B;
            return new[]
            {
                D.M11,
                D.M12,
                D.M13,
                D.M14,
                D.M21,
                D.M22,
                D.M23,
                D.M24,
                D.M31,
                D.M32,
                D.M33,
                D.M34,
                D.M41,
                D.M42,
                D.M43,
                D.M44
            }.Max(Math.Abs);
        }
    }
}


