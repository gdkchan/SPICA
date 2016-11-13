using OpenTK;

using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Renderer.SPICA_GL;

namespace SPICA.Renderer
{
    public struct Animation
    {
        public float Frame;
        public float Step;

        private H3DAnimation BaseAnimation;

        public bool HasData { get { return BaseAnimation != null; } }

        public Animation(H3DAnimation BaseAnimation)
        {
            Frame = 0;
            Step = 1;

            this.BaseAnimation = BaseAnimation;
        }

        public void AdvanceFrame()
        {
            if (BaseAnimation != null)
            {
                Frame += Step;

                if (Frame >= BaseAnimation.FramesCount)
                {
                    Frame -= BaseAnimation.FramesCount;
                }
            }
        }

        public Matrix4[] GetSkeletonTransform(PatriciaList<H3DBone> Skeleton)
        {
            var Matrices = BaseAnimation.GetSkeletonTransform(Skeleton, Frame);

            Matrix4[] Output = new Matrix4[Skeleton.Count];

            for (int Index = 0; Index < Output.Length; Index++)
            {
                Output[Index] = GLConverter.ToMatrix4(Matrices[Index]);
            }

            return Output;
        }
    }
}
