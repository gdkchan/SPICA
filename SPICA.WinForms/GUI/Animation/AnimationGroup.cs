using SPICA.Formats.CtrH3D.Animation;
using SPICA.Rendering;
using SPICA.Rendering.Animation;

using System;
using System.Collections.Generic;

namespace SPICA.WinForms.GUI.Animation
{
    class AnimationGroup : IAnimationControl
    {
        public AnimationState State { get; set; }

        public bool  IsLooping   { get; set; }
        public float FramesCount { get; set; }

        private float _Frame;
        private float _Step;

        public float Frame
        {
            get => _Frame;
            set
            {
                _Frame = value;

                UpdateFrame();
            }
        }

        public float Step
        {
            get => _Step;
            set
            {
                _Step = value;

                UpdateStep();
            }
        }

        private Renderer Renderer;

        public AnimationGroup(Renderer Renderer)
        {
            this.Renderer = Renderer;

            _Step = 1;
        }

        private IEnumerable<AnimationControl> EnumerateAnimations()
        {
            foreach (Model Model in Renderer.Models)
            {
                yield return Model.SkeletalAnim;
                yield return Model.MaterialAnim;
                yield return Model.VisibilityAnim;
            }

            yield return Renderer.Camera.Animation;
        }

        public void SetAnimations(IEnumerable<H3DAnimation> Animations, AnimationType Type)
        {
            switch (Type)
            {
                case AnimationType.Skeletal:
                    foreach (Model Model in Renderer.Models) CopyState(Animations, Model.SkeletalAnim);
                    break;

                case AnimationType.Material:
                    foreach (Model Model in Renderer.Models) CopyState(Animations, Model.MaterialAnim);
                    break;

                case AnimationType.Visibility:
                    foreach (Model Model in Renderer.Models) CopyState(Animations, Model.VisibilityAnim);
                    break;

                case AnimationType.Camera:
                    CopyState(Animations, Renderer.Camera.Animation);
                    break;
            }
        }

        private void CopyState(IEnumerable<H3DAnimation> Animations, AnimationControl Target)
        {
            Target.SetAnimations(Animations);

            Target.State = State;
            Target.Step  = _Step;
            Target.Frame = _Frame;
        }

        public void UpdateState()
        {
            float FC = 0;

            bool Loop = true;

            foreach (AnimationControl Ctrl in EnumerateAnimations())
            {
                if (FC < Ctrl.FramesCount)
                    FC = Ctrl.FramesCount;

                Loop &= Ctrl.IsLooping;
            }

            FramesCount = FC;

            Frame %= FC > 0 ? FC : 1;

            IsLooping = Loop;
        }

        private void UpdateStep()
        {
            foreach (AnimationControl Ctrl in EnumerateAnimations())
            {
                Ctrl.Step = _Step;
            }
        }

        private void UpdateFrame()
        {
            foreach (AnimationControl Ctrl in EnumerateAnimations())
            {
                Ctrl.Frame = _Frame;
            }
        }

        public void AdvanceFrame()
        {
            if (FramesCount >= Math.Abs(_Step))
            {
                _Frame += _Step;

                if (_Frame < 0)
                {
                    _Frame += FramesCount;
                }
                else if (_Frame >= FramesCount)
                {
                    _Frame -= FramesCount;
                }
            }

            foreach (AnimationControl Ctrl in EnumerateAnimations())
            {
                Ctrl.Frame = _Frame;
            }
        }

        public void SlowDown()
        {
            if (State == AnimationState.Playing && Math.Abs(_Step) > 0.125f) _Step *= 0.5f;

            UpdateStep();
        }

        public void SpeedUp()
        {
            if (State == AnimationState.Playing && Math.Abs(_Step) < 8) _Step *= 2;

            UpdateStep();
        }

        public void Continue()
        {
            Play(_Step);
        }

        public void Play(float Step = 1)
        {
            _Step = Step;

            State = AnimationState.Playing;

            foreach (AnimationControl Ctrl in EnumerateAnimations())
            {
                Ctrl.Play(Step);
            }
        }

        public void Pause()
        {
            State = AnimationState.Paused;

            foreach (AnimationControl Ctrl in EnumerateAnimations())
            {
                Ctrl.Pause();
            }
        }

        public void Stop()
        {
            State = AnimationState.Stopped;

            _Frame = 0;

            foreach (AnimationControl Ctrl in EnumerateAnimations())
            {
                Ctrl.Stop();
            }
        }
    }
}
