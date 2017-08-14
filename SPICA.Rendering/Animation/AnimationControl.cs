using SPICA.Formats.Common;
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

        protected List<int> Indices;

        protected List<H3DAnimationElement> Elements;

        public AnimationControl()
        {
            Step = 1;

            Indices = new List<int>();

            Elements = new List<H3DAnimationElement>();
        }

        public virtual void SetAnimations(IEnumerable<H3DAnimation> Animations) { }

        protected void SetAnimations(IEnumerable<H3DAnimation> Animations, INameIndexed Dictionary)
        {
            Indices.Clear();
            Elements.Clear();

            float FC = 0;

            HashSet<string> UsedNames = new HashSet<string>();

            foreach (H3DAnimation Anim in Animations)
            {
                if (FC < Anim.FramesCount)
                    FC = Anim.FramesCount;

                foreach (H3DAnimationElement Elem in Anim.Elements)
                {
                    if (UsedNames.Contains(Elem.Name)) continue;

                    UsedNames.Add(Elem.Name);

                    int Index = Dictionary.Find(Elem.Name);

                    if (Index != -1)
                    {
                        Indices.Add(Index);
                        Elements.Add(Elem);
                    }
                }
            }

            FramesCount = FC;
        }

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
