using SPICA.Formats.Common;
using SPICA.Formats.CtrH3D.Animation;
using System;
using System.Collections.Generic;
using System.IO;

namespace SPICA.Formats.GFL.Motion
{
    class GFMotion
    {
        public ushort FramesCount;

        public List<GFMotBoneTransform> Bones;

        public int Index;

        public GFMotion()
        {
            Bones = new List<GFMotBoneTransform>();
        }

        public GFMotion(BinaryReader Reader, List<GFMotBone> Skeleton, int Index) : this()
        {
            this.Index = Index;

            ushort OctalsCount = Reader.ReadUInt16();

            FramesCount = Reader.ReadUInt16();

            uint[] Octals = new uint[OctalsCount];

            int KeyFramesCount = 0;
            uint CurrentOctal = 0;

            for (int i = 0; i < OctalsCount; i++)
            {
                if ((i & 7) == 0) CurrentOctal = IOUtils.ReadUInt24(Reader);

                Octals[i] = CurrentOctal & 7;

                CurrentOctal >>= 3;

                if (Octals[i] > 5) KeyFramesCount++;
            }

            if (FramesCount > byte.MaxValue && (Reader.BaseStream.Position & 1) != 0) Reader.ReadByte();

            int[][] KeyFrames = new int[KeyFramesCount][];

            for (int i = 0; i < KeyFrames.Length; i++)
            {
                int Count;

                if (FramesCount > byte.MaxValue)
                    Count = Reader.ReadUInt16();
                else
                    Count = Reader.ReadByte();

                KeyFrames[i] = new int[Count + 2];

                KeyFrames[i][Count + 1] = FramesCount;

                for (int j = 1; j <= Count; j++)
                {
                    if (FramesCount > byte.MaxValue)
                        KeyFrames[i][j] = Reader.ReadUInt16();
                    else
                        KeyFrames[i][j] = Reader.ReadByte();
                }
            }

            GFUtils.Align(Reader);

            GFMotBoneTransform CurrentBone = null;

            int CurrentKFL =  0;
            int OctalIndex =  2;
            int ElemIndex  =  0;
            int OldIndex   = -1;

            while (OctalIndex < OctalsCount)
            {
                CurrentOctal = Octals[OctalIndex++];

                if (CurrentOctal != 1)
                {
                    int NameIndex = ElemIndex / 9;

                    if (NameIndex != OldIndex)
                    {
                        CurrentBone = new GFMotBoneTransform { Name = Skeleton[NameIndex].Name };

                        Bones.Add(CurrentBone);

                        OldIndex = NameIndex;
                    }
                }

                if (CurrentOctal != 1)
                {
                    //Actual Key Frame format
                    List<GFMotKeyFrame> KFs = new List<GFMotKeyFrame>();

                    switch (CurrentOctal)
                    {
                        case 0: KFs.Add(new GFMotKeyFrame(0, 0)); break; //Constant Zero (0 deg)
                        case 2: KFs.Add(new GFMotKeyFrame(0, (float)Math.PI *  0.5f)); break; //Constant +Half PI (90 deg)
                        case 3: KFs.Add(new GFMotKeyFrame(0, (float)Math.PI *  1.0f)); break; //Constant +PI (180 deg)
                        case 4: KFs.Add(new GFMotKeyFrame(0, (float)Math.PI * -0.5f)); break; //Constant -Half PI (-90/270 deg)
                        case 5: KFs.Add(new GFMotKeyFrame(0, Reader.ReadSingle())); break; //Constant value (stored as Float)

                        case 6: //Linear Key Frames list
                            foreach (int Frame in KeyFrames[CurrentKFL++])
                            {
                                KFs.Add(new GFMotKeyFrame(Frame, Reader.ReadSingle()));
                            }
                            break;

                        case 7: //Hermite Key Frames list
                            foreach (int Frame in KeyFrames[CurrentKFL++])
                            {
                                KFs.Add(new GFMotKeyFrame(
                                    Frame,
                                    Reader.ReadSingle(),
                                    Reader.ReadSingle()));
                            }
                            break;
                    }

                    switch (ElemIndex % 9)
                    {
                        case 0: CurrentBone.TranslationX = KFs; break;
                        case 1: CurrentBone.TranslationY = KFs; break;
                        case 2: CurrentBone.TranslationZ = KFs; break;

                        case 3: CurrentBone.RotationX    = KFs; break;
                        case 4: CurrentBone.RotationY    = KFs; break;
                        case 5: CurrentBone.RotationZ    = KFs; break;

                        case 6: CurrentBone.ScaleX       = KFs; break;
                        case 7: CurrentBone.ScaleY       = KFs; break;
                        case 8: CurrentBone.ScaleZ       = KFs; break;
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

        public H3DAnimation ToH3DSkeletalAnimation()
        {
            H3DAnimation Output = new H3DAnimation();

            Output.Name        = "GFMotion";
            Output.FramesCount = FramesCount;

            foreach (GFMotBoneTransform Bone in Bones)
            {
                H3DAnimTransform Transform = new H3DAnimTransform();

                Transform.TranslationX = GetKeyFrameGroup(Bone.TranslationX, 0);
                Transform.TranslationY = GetKeyFrameGroup(Bone.TranslationY, 1);
                Transform.TranslationZ = GetKeyFrameGroup(Bone.TranslationZ, 2);

                Transform.RotationX    = GetKeyFrameGroup(Bone.RotationX,    3);
                Transform.RotationY    = GetKeyFrameGroup(Bone.RotationY,    4);
                Transform.RotationZ    = GetKeyFrameGroup(Bone.RotationZ,    5);

                Transform.ScaleX       = GetKeyFrameGroup(Bone.ScaleX,       6);
                Transform.ScaleY       = GetKeyFrameGroup(Bone.ScaleY,       7);
                Transform.ScaleZ       = GetKeyFrameGroup(Bone.ScaleZ,       8);

                Output.Elements.Add(new H3DAnimationElement
                {
                    Name          = Bone.Name,
                    Content       = Transform,
                    TargetType    = H3DAnimTargetType.Bone,
                    PrimitiveType = H3DAnimPrimitiveType.Transform
                });
            }

            return Output;
        }

        //No idea why the Slope needs to be multiplied by this scale
        //This was discovered pretty much by trial and error, and may be wrong too
        private const float SlopeScale = 1 / 30f;

        private H3DFloatKeyFrameGroup GetKeyFrameGroup(List<GFMotKeyFrame> KFs, int CurveIndex)
        {
            H3DFloatKeyFrameGroup Output = new H3DFloatKeyFrameGroup
            {
                StartFrame        = 0,
                EndFrame          = FramesCount,
                CurveIndex        = (ushort)CurveIndex,
                InterpolationType = H3DInterpolationType.Hermite
            };

            foreach (GFMotKeyFrame KF in KFs)
            {
                Output.KeyFrames.Add(new H3DFloatKeyFrame(
                    KF.Frame,
                    KF.Value,
                    KF.Slope * SlopeScale,
                    KF.Slope * SlopeScale));
            }

            return Output;
        }
    }
}
