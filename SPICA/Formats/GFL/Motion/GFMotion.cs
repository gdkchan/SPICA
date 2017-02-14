using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.Utils;

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

                for (int j = 0; j < Count; j++)
                {
                    if (FramesCount > byte.MaxValue)
                        KeyFrames[i][j + 1] = Reader.ReadUInt16();
                    else
                        KeyFrames[i][j + 1] = Reader.ReadByte();
                }
            }

            GFUtils.Align(Reader);

            GFMotBoneTransform CurrentBone = null;

            int CurrentKFL = 0;
            int OctalIndex = 0;
            int NameIndex = 0;
            int Elem = 0;
            bool RotOnly = false;

            while (OctalIndex < OctalsCount && NameIndex + 1 < Skeleton.Count)
            {
                CurrentOctal = Octals[OctalIndex++];

                switch (CurrentOctal)
                {
                    case 0: break; //Axis element not used on animation

                    /*
                     * Special code octal where:
                     * 1   = Bone with translation and rotation (6 octals) ahead
                     * 11  = Bone with only rotation (3 octals) ahead
                     * 111 = Bone not used on animation
                     */
                    case 1:
                        if (CurrentBone != null)
                        {
                            CurrentBone.Name = Skeleton[NameIndex++].Name;

                            Bones.Add(CurrentBone);
                        }

                        CurrentBone = new GFMotBoneTransform();

                        Elem = 0;

                        int OneCount = 1;

                        while (OctalIndex < Octals.Length && Octals[OctalIndex] == 1)
                        {
                            OneCount++;
                            OctalIndex++;
                        }

                        NameIndex += OneCount / 3;
                        RotOnly    = OneCount % 3 == 2;

                        break;

                    //Actual Key Frame format
                    case 5:
                    case 6:
                    case 7:
                        List<GFMotKeyFrame> KFs = new List<GFMotKeyFrame>();

                        switch (CurrentOctal)
                        {
                            case 5: KFs.Add(new GFMotKeyFrame(0, Reader.ReadSingle())); break; //Constant

                            case 6: //Linear Key Frames list
                                foreach (int Frame in KeyFrames[CurrentKFL++])
                                {
                                    KFs.Add(new GFMotKeyFrame(Frame, Reader.ReadSingle()));
                                }

                                break;

                            case 7: //Spline(?) Key Frames list
                                foreach (int Frame in KeyFrames[CurrentKFL++])
                                {
                                    KFs.Add(new GFMotKeyFrame(
                                        Frame,
                                        Reader.ReadSingle(),
                                        Reader.ReadSingle()));
                                }

                                break;
                        }

                        switch (Elem)
                        {
                            case 0:
                                if (RotOnly) CurrentBone.RotationX    = KFs;
                                else         CurrentBone.TranslationX = KFs;
                                break;

                            case 1:
                                if (RotOnly) CurrentBone.RotationY    = KFs;
                                else         CurrentBone.TranslationY = KFs;
                                break;

                            case 2:
                                if (RotOnly) CurrentBone.RotationZ    = KFs;
                                else         CurrentBone.TranslationZ = KFs;
                                break;

                            case 3: CurrentBone.RotationX = KFs; break;
                            case 4: CurrentBone.RotationY = KFs; break;
                            case 5: CurrentBone.RotationZ = KFs; break;
                        }

                        break;
                }

                if (CurrentOctal != 1) Elem++;
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

        private H3DFloatKeyFrameGroup GetKeyFrameGroup(List<GFMotKeyFrame> KFs, int CurveIndex)
        {
            H3DFloatKeyFrameGroup Output = new H3DFloatKeyFrameGroup
            {
                StartFrame        = 0,
                EndFrame          = FramesCount,
                CurveIndex        = (ushort)CurveIndex,
                InterpolationType = H3DInterpolationType.Linear
            };

            foreach (GFMotKeyFrame KF in KFs)
            {
                Output.KeyFrames.Add(new H3DFloatKeyFrame(
                    KF.Frame,
                    KF.Value,
                    KF.Slope,
                    KF.Slope));
            }

            return Output;
        }
    }
}
