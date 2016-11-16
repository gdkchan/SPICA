using SPICA.Formats.CtrH3D.Animation;

namespace SPICA.Renderer.Animation
{
    public class AnimControl
    {
        public float Frame;
        public float Step;

        protected H3DAnimation BaseAnimation;

        protected AnimState State;

        public bool HasData { get { return BaseAnimation != null; } }

        public AnimControl()
        {
            Step = 1;
        }

        public void SetAnimation(H3DAnimation BaseAnimation)
        {
            this.BaseAnimation = BaseAnimation;
        }

        public void AdvanceFrame()
        {
            if (BaseAnimation != null && State == AnimState.Playing)
            {
                Frame += Step;

                if (Frame >= BaseAnimation.FramesCount)
                {
                    Frame -= BaseAnimation.FramesCount;
                }
            }
        }

        public void Play()
        {
            State = AnimState.Playing;
        }

        public void Pause()
        {
            State = AnimState.Paused;
        }

        public void Stop()
        {
            Pause();
            Frame = 0;
        }
    }
}
