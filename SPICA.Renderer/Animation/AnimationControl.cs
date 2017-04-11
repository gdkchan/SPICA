using SPICA.Formats.CtrH3D.Animation;

using System;

namespace SPICA.Renderer.Animation
{
    public class AnimationControl : IAnimationControl
    {
        protected H3DAnimation BaseAnimation;

        protected AnimationState State;

        private float _Frame;

        public float Frame
        {
            get
            {
                return _Frame;
            }
            set
            {
                if (value > FramesCount)
                    _Frame = value % FramesCount;
                else
                    _Frame = value;
            }
        }

        public bool  IsLooping   { get; set; }
        public float Step        { get; set; }
        public bool  HasData     { get { return BaseAnimation != null; } }
        public float FramesCount { get { return BaseAnimation?.FramesCount ?? 0; } }

        public AnimationControl()
        {
            Step = 1;
        }

        public void CopyState(AnimationControl Control)
        {
            BaseAnimation = Control.BaseAnimation;
            State         = Control.State;
            _Frame        = Control.Frame;
            IsLooping     = Control.IsLooping;
            Step          = Control.Step;
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
                    _Frame = BaseAnimation.FramesCount;
                else
                    _Frame = 0;
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
                _Frame += Step;

                if (_Frame < 0)
                {
                    _Frame += BaseAnimation.FramesCount;
                }
                else if (_Frame >= BaseAnimation.FramesCount)
                {
                    _Frame -= BaseAnimation.FramesCount;
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

        public void Play(float Step = 1)
        {
            this.Step = Step;

            State = AnimationState.Playing;
        }

        public void Pause()
        {
            State = AnimationState.Paused;
        }

        public void Stop()
        {
            State = AnimationState.Stopped;

            _Frame = 0;
        }
    }
}
