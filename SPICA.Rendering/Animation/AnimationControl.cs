using SPICA.Formats.CtrH3D.Animation;

using System;
using System.Collections.Generic;

namespace SPICA.Rendering.Animation
{
    public class AnimationControl : IAnimationControl
    {
        public AnimationState State { get; set; }

        private float _Frame;

        public float Frame
        {
            get
            {
                return _Frame;
            }
            set
            {
                _Frame = value > FramesCount
                    ? value % FramesCount
                    : value;
            }
        }

        public float Step        { get; set; }
        public float FramesCount { get; protected set; }
        public bool  IsLooping   { get; protected set; }

        public AnimationControl()
        {
            Step = 1;
        }

        public virtual void SetAnimations(IEnumerable<H3DAnimation> Animations) { }

        public void AdvanceFrame()
        {
            if (FramesCount >= Math.Abs(Step) && State == AnimationState.Playing)
            {
                _Frame += Step;

                if (_Frame < 0)
                {
                    _Frame += FramesCount;
                }
                else if (_Frame >= FramesCount)
                {
                    _Frame -= FramesCount;
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
