using SPICA.Formats.CtrH3D.Animation;

using System;

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

            Frame = 0;
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
                else if (Frame < 0)
                {
                    Frame += BaseAnimation.FramesCount;
                }
            }
        }

        public void SlowDown()
        {
            if (State == AnimState.Playing && Math.Abs(Step) > 0.125f) Step *= 0.5f;
        }

        public void SpeedUp()
        {
            if (State == AnimState.Playing && Math.Abs(Step) < 8) Step *= 2;
        }

        public void Play(float Step)
        {
            this.Step = Step;

            Play();
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
            State = AnimState.Stopped;

            Frame = 0;
        }
    }
}
