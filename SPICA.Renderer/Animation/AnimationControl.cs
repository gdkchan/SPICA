using SPICA.Formats.CtrH3D.Animation;

using System;

namespace SPICA.Renderer.Animation
{
    public class AnimationControl : IAnimationControl
    {
        protected H3DAnimation BaseAnimation;

        protected AnimationState State;

        public float Frame       { get; set; }
        public float Step        { get; set; }
        public bool  IsLooping   { get; set; }
        public bool  HasData     { get { return BaseAnimation != null; } }
        public float FramesCount { get { return BaseAnimation?.FramesCount ?? 0; } }

        public AnimationControl()
        {
            Step = 1;
        }

        public void SetAnimation(H3DAnimation BaseAnimation)
        {
            this.BaseAnimation = BaseAnimation;

            if (BaseAnimation == null)
            {
                Stop();

                return;
            }

            IsLooping = (BaseAnimation.AnimationFlags & H3DAnimationFlags.IsLooping) != 0;

            if (State == AnimationState.Playing)
            {
                if (Step < 0)
                    Frame = BaseAnimation.FramesCount;
                else
                    Frame = 0;
            }
            else
            {
                Stop();
            }
        }

        public void AdvanceFrame()
        {
            if (BaseAnimation != null &&
                BaseAnimation.FramesCount >= Math.Abs(Step) &&
                State == AnimationState.Playing)
            {
                Frame += Step;

                if (Frame < 0)
                {
                    Frame += BaseAnimation.FramesCount;
                }
                else if (Frame >= BaseAnimation.FramesCount)
                {
                    Frame -= BaseAnimation.FramesCount;
                }
            }
        }

        public void SlowDown()
        {
            if (State == AnimationState.Playing && Math.Abs(Step) > 0.125f) Step *= 0.5f;
        }

        public void SpeedUp()
        {
            if (State == AnimationState.Playing && Math.Abs(Step) < 8) Step *= 2;
        }

        public void Play(float Step)
        {
            this.Step = Step;

            Play();
        }

        public void Play()
        {
            State = AnimationState.Playing;
        }

        public void Pause()
        {
            State = AnimationState.Paused;
        }

        public void Stop()
        {
            State = AnimationState.Stopped;

            Frame = 0;
        }
    }
}
