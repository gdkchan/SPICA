using SPICA.Formats.Common;
using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Math3D;

using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SPICA.Formats.GFL.Motion
{
    public class GF1Motion
    {
        public ushort FramesCount;

        public readonly List<GF1MotBoneTransform> Bones;

        public int Index;

        public GF1Motion()
        {
            Bones = new List<GF1MotBoneTransform>();
        }

        public GF1Motion(BinaryReader Reader, List<GF1MotBone> Skeleton, int Index) : this()
        {
            this.Index = Index;

            ushort OctalsCount = Reader.ReadUInt16();

            FramesCount = Reader.ReadUInt16();

            uint[] Octals = new uint[OctalsCount];

            int  KeyFramesCount = 0;
            uint CurrentOctal   = 0;

            for (int i = 0; i < OctalsCount; i++)
            {
                if ((i & 7) == 0) CurrentOctal = Reader.ReadUInt24();

                Octals[i] = CurrentOctal & 7;

                CurrentOctal >>= 3;

                if (Octals[i] > 5) KeyFramesCount++;
            }

            bool Frame16 = FramesCount > 0xff;

            if (Frame16 && (Reader.BaseStream.Position & 1) != 0) Reader.ReadByte();

            int[][] KeyFrames = new int[KeyFramesCount][];

            for (int i = 0; i < KeyFrames.Length; i++)
            {
                int Count;

                Count = Frame16
                    ? Reader.ReadUInt16()
                    : Reader.ReadByte();

                KeyFrames[i] = new int[Count + 2];

                KeyFrames[i][Count + 1] = FramesCount;

                for (int j = 1; j <= Count; j++)
                {
                    KeyFrames[i][j] = Frame16
                        ? Reader.ReadUInt16()
                        : Reader.ReadByte();
                }
            }

            Reader.Align(4);

            GF1MotBoneTransform CurrentBone = null;

            int CurrentKFL =  0;
            int OctalIndex =  2;
            int ElemIndex  =  0;
            int OldIndex   = -1;

            while (OctalIndex < OctalsCount)
            {
                CurrentOctal = Octals[OctalIndex++];

                if (CurrentOctal != 1)
                {
                    int BoneIndex = ElemIndex / 9;

                    if (BoneIndex != OldIndex)
                    {
                        CurrentBone = new GF1MotBoneTransform()
                        {
                            Name         =  Skeleton[BoneIndex].Name,
                            IsWorldSpace = (Skeleton[BoneIndex].Flags & 8) != 0
                        };

                        Bones.Add(CurrentBone);

                        OldIndex = BoneIndex;
                    }
                }

                if (CurrentOctal != 1)
                {
                    //Actual Key Frame format
                    List<GF1MotKeyFrame> KFs = null;

                    switch (ElemIndex % 9)
                    {
                        case 0: KFs = CurrentBone.TranslationX; break;
                        case 1: KFs = CurrentBone.TranslationY; break;
                        case 2: KFs = CurrentBone.TranslationZ; break;

                        case 3: KFs = CurrentBone.RotationX;    break;
                        case 4: KFs = CurrentBone.RotationY;    break;
                        case 5: KFs = CurrentBone.RotationZ;    break;

                        case 6: KFs = CurrentBone.ScaleX;       break;
                        case 7: KFs = CurrentBone.ScaleY;       break;
                        case 8: KFs = CurrentBone.ScaleZ;       break;
                    }

                    switch (CurrentOctal)
                    {
                        case 0: KFs.Add(new GF1MotKeyFrame(0, 0)); break; //Constant Zero (0 deg)
                        case 2: KFs.Add(new GF1MotKeyFrame(0, (float)Math.PI *  0.5f)); break; //Constant +Half PI (90 deg)
                        case 3: KFs.Add(new GF1MotKeyFrame(0, (float)Math.PI *  1.0f)); break; //Constant +PI (180 deg)
                        case 4: KFs.Add(new GF1MotKeyFrame(0, (float)Math.PI * -0.5f)); break; //Constant -Half PI (-90/270 deg)
                        case 5: KFs.Add(new GF1MotKeyFrame(0, Reader.ReadSingle())); break; //Constant value (stored as Float)

                        case 6: //Linear Key Frames list
                            foreach (int Frame in KeyFrames[CurrentKFL++])
                            {
                                KFs.Add(new GF1MotKeyFrame(Frame, Reader.ReadSingle()));
                            }
                            break;

                        case 7: //Hermite Key Frames list
                            foreach (int Frame in KeyFrames[CurrentKFL++])
                            {
                                KFs.Add(new GF1MotKeyFrame(
                                    Frame,
                                    Reader.ReadSingle(),
                                    Reader.ReadSingle()));
                            }
                            break;
                    }

                    ElemIndex++;
                }
                else
                {
                    //Skip S/R/T Vector
                    ElemIndex += 3;
                }
            }
        }

        public H3DAnimation ToH3DSkeletalAnimation(H3DDict<H3DBone> Skeleton)
        {
            H3DAnimation Output = new H3DAnimation()
            {
                Name          = "GFMotion",
                FramesCount   = FramesCount,
                AnimationType = H3DAnimationType.Skeletal
            };

            foreach (GF1MotBoneTransform Bone in Bones)
            {
                H3DAnimTransform Transform = new H3DAnimTransform();

                SetKeyFrameGroup(Bone.TranslationX, Transform.TranslationX, 0);
                SetKeyFrameGroup(Bone.TranslationY, Transform.TranslationY, 1);
                SetKeyFrameGroup(Bone.TranslationZ, Transform.TranslationZ, 2);

                SetKeyFrameGroup(Bone.RotationX,    Transform.RotationX,    3);
                SetKeyFrameGroup(Bone.RotationY,    Transform.RotationY,    4);
                SetKeyFrameGroup(Bone.RotationZ,    Transform.RotationZ,    5);

                SetKeyFrameGroup(Bone.ScaleX,       Transform.ScaleX,       6);
                SetKeyFrameGroup(Bone.ScaleY,       Transform.ScaleY,       7);
                SetKeyFrameGroup(Bone.ScaleZ,       Transform.ScaleZ,       8);

                Output.Elements.Add(new H3DAnimationElement()
                {
                    Name          = Bone.Name,
                    Content       = Transform,
                    TargetType    = H3DTargetType.Bone,
                    PrimitiveType = H3DPrimitiveType.Transform
                });
            }

            //If we don't have a Skeleton then can't convert anything, just return with
            //what we have.
            if (Skeleton == null) return Output;

            /*
             * Transform World Space bones to Local Space.
             * Not all animations have those.
             * This can be improved, for example, by supporting Scales aswell,
             * and checking for the special case where the World Space bone have no parent.
             * Those cases doesn't actually happen in any of the observed animations,
             * but it's always good pratice to check all fail paths.
             */
            int AnimIdx = 0;

            foreach (GF1MotBoneTransform Bone in Bones)
            {
                if (Bone.IsWorldSpace)
                {
                    if (!Skeleton.Contains(Bone.Name)) break;

                    H3DBone PoseBone = Skeleton[Bone.Name];

                    Vector3[] LocalTrans = new Vector3[FramesCount + 1];
                    Vector3[] ParentRot  = new Vector3[FramesCount + 1];

                    int    BoneIndex   = Skeleton.Find(Bone.Name);
                    int    ParentIndex = Skeleton[BoneIndex].ParentIndex;
                    string ParentName  = Skeleton[ParentIndex].Name;

                    for (int Frame = 0; Frame < FramesCount + 1; Frame++)
                    {
                        Matrix4x4 Transform = Matrix4x4.Identity;

                        int b = BoneIndex;

                        while ((b = Skeleton[b].ParentIndex) != -1)
                        {
                            GetBoneRT(
                                Output,
                                Skeleton,
                                Skeleton[b].Name,
                                Frame,
                                out Vector3 R,
                                out Vector3 T);

                            Transform *= Matrix4x4.CreateRotationX(R.X);
                            Transform *= Matrix4x4.CreateRotationY(R.Y);
                            Transform *= Matrix4x4.CreateRotationZ(R.Z);
                            Transform *= Matrix4x4.CreateTranslation(T);
                        }

                        GetBoneRT(
                            Output,
                            Skeleton,
                            ParentName,
                            Frame,
                            out Vector3 PR,
                            out Vector3 PT);

                        GetBoneRT(
                            Output,
                            Skeleton,
                            Bone.Name,
                            Frame,
                            out Vector3 BR,
                            out Vector3 BT);

                        Matrix4x4.Invert(Transform, out Matrix4x4 ITransform);

                        BT = Vector3.Transform(BT, ITransform);

                        Quaternion Rotation = VectorExtensions.CreateRotationBetweenVectors(PT, BT);

                        LocalTrans[Frame] = Vector3.Transform(BT, Quaternion.Inverse(Rotation));

                        ParentRot[Frame] = Rotation.ToEuler();
                    }

                    H3DAnimationElement B_Anim = Output.Elements[AnimIdx];
                    H3DAnimationElement P_Anim = Output.Elements.Find(x => x.Name == ParentName);

                    if (P_Anim == null)
                    {
                        P_Anim = new H3DAnimationElement()
                        {
                            Name          = ParentName,
                            Content       = new H3DAnimTransform(),
                            TargetType    = H3DTargetType.Bone,
                            PrimitiveType = H3DPrimitiveType.Transform
                        };

                        Output.Elements.Add(P_Anim);
                    }

                    H3DAnimTransform B_AT = (H3DAnimTransform)B_Anim.Content;
                    H3DAnimTransform P_AT = (H3DAnimTransform)P_Anim.Content;

                    AddVectors(LocalTrans, B_AT.TranslationX, B_AT.TranslationY, B_AT.TranslationZ);

                    if (!P_AT.RotationExists)
                    {
                        AddVectors(ParentRot, P_AT.RotationX, P_AT.RotationY, P_AT.RotationZ);
                    }
                }

                AnimIdx++;
            }

            return Output;
        }

        private bool GetBoneRT(
            H3DAnimation     Anim,
            H3DDict<H3DBone> Skeleton,
            string           Name,
            int              Frame,
            out Vector3      Rotation,
            out Vector3      Translation)
        {
            if (!Skeleton.Contains(Name))
            {
                Rotation    = Vector3.Zero;
                Translation = Vector3.Zero;

                return false;
            }

            H3DBone PoseBone = Skeleton[Name];

            Rotation    = PoseBone.Rotation;
            Translation = PoseBone.Translation;

            H3DAnimationElement Bone = Anim.Elements.Find(x => x.Name == Name);

            if (Bone != null)
            {
                H3DAnimTransform Transform = (H3DAnimTransform)Bone.Content;

                if (Transform.RotationX.Exists)    Rotation.X    = Transform.RotationX.GetFrameValue(Frame);
                if (Transform.RotationY.Exists)    Rotation.Y    = Transform.RotationY.GetFrameValue(Frame);
                if (Transform.RotationZ.Exists)    Rotation.Z    = Transform.RotationZ.GetFrameValue(Frame);

                if (Transform.TranslationX.Exists) Translation.X = Transform.TranslationX.GetFrameValue(Frame);
                if (Transform.TranslationY.Exists) Translation.Y = Transform.TranslationY.GetFrameValue(Frame);
                if (Transform.TranslationZ.Exists) Translation.Z = Transform.TranslationZ.GetFrameValue(Frame);
            }

            return true;
        }

        private void AddVectors(
            Vector3[]             Vectors,
            H3DFloatKeyFrameGroup X,
            H3DFloatKeyFrameGroup Y,
            H3DFloatKeyFrameGroup Z)
        {
            X.InterpolationType = Y.InterpolationType = Z.InterpolationType = H3DInterpolationType.Linear;

            X.KeyFrames.Clear();
            Y.KeyFrames.Clear();
            Z.KeyFrames.Clear();

            int Frame = 0;

            foreach (Vector3 Vector in Vectors)
            {
                X.KeyFrames.Add(new KeyFrame(Frame, Vector.X));
                Y.KeyFrames.Add(new KeyFrame(Frame, Vector.Y));
                Z.KeyFrames.Add(new KeyFrame(Frame, Vector.Z));

                Frame++;
            }
        }

        //No idea why the Slope needs to be multiplied by this scale
        //This was discovered pretty much by trial and error, and may be wrong too
        private const float SlopeScale = 1 / 30f;

        private void SetKeyFrameGroup(List<GF1MotKeyFrame> Source, H3DFloatKeyFrameGroup Target, ushort CurveIndex)
        {
            Target.StartFrame        = 0;
            Target.EndFrame          = FramesCount;
            Target.CurveIndex        = CurveIndex;
            Target.InterpolationType = H3DInterpolationType.Hermite;

            foreach (GF1MotKeyFrame KF in Source)
            {
                Target.KeyFrames.Add(new KeyFrame(
                    KF.Frame,
                    KF.Value,
                    KF.Slope * SlopeScale));
            }
        }
    }
}
