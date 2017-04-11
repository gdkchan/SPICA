using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Renderer;
using SPICA.Renderer.Animation;

namespace SPICA.WinForms.GUI.Animation
{
    class AllAnimations : IAnimationControl
    {
        public AnimationType Type { get; set; }

        public H3D Scene { get; set; }

        private Model _Model;

        public Model Model
        {
            get
            {
                return _Model;
            }
            set
            {
                Model OldModel = _Model;

                _Model = value;

                if (OldModel != null)
                {
                    _Model.SkeletalAnim.CopyState(OldModel.SkeletalAnim);
                    _Model.MaterialAnim.CopyState(OldModel.MaterialAnim);

                    switch (Type)
                    {

                    }
                }
            }
        }

        public float Frame
        {
            get
            {
                return GetMasterAnimation()?.Frame ?? 0;
            }
            set
            {
                if (_Model != null)
                {
                    _Model.SkeletalAnim.Frame = value;
                    _Model.MaterialAnim.Frame = value;

                    UpdateFrame();
                }
            }
        }

        public float Step
        {
            get
            {
                return GetMasterAnimation()?.Step ?? 1;
            }
            set
            {
                if (_Model != null)
                {
                    _Model.SkeletalAnim.Step = value;
                    _Model.MaterialAnim.Step = value;
                }
            }
        }

        public bool IsLooping
        {
            get
            {
                return GetMasterAnimation()?.IsLooping ?? false;
            }
            set
            {
                if (_Model != null)
                {
                    _Model.SkeletalAnim.IsLooping = value;
                    _Model.MaterialAnim.IsLooping = value;
                }
            }
        }

        public float FramesCount { get { return GetMasterAnimation()?.FramesCount ?? 0; } }

        public void SetAnimation(int Index, AnimationType Type)
        {
            this.Type = Type;

            if (Scene != null && _Model != null)
            {
                H3DAnimation Animation = null;

                switch (Type)
                {
                    case AnimationType.Skeletal:
                        Animation = Scene.SkeletalAnimations[Index];
                        _Model.SkeletalAnim.SetAnimation(Animation);
                        break;

                    case AnimationType.Material:
                        Animation = Scene.MaterialAnimations[Index];
                        _Model.MaterialAnim.SetAnimation(Animation);
                        break;
                }

                if (Type != AnimationType.Skeletal &&
                    (Index = Scene.SkeletalAnimations.FindIndex(Animation.Name)) != -1)
                {
                    _Model.SkeletalAnim.SetAnimation(Scene.SkeletalAnimations[Index]);
                }

                if (Type != AnimationType.Material &&
                    (Index = Scene.MaterialAnimations.FindIndex(Animation.Name)) != -1)
                {
                    _Model.MaterialAnim.SetAnimation(Scene.MaterialAnimations[Index]);
                }
            }
        }

        public void Reset()
        {
            if (_Model != null)
            {
                Stop();

                _Model.SkeletalAnim.SetAnimation(null);
                _Model.MaterialAnim.SetAnimation(null);
            }
        }

        //Needs immediate frame updates
        private void UpdateFrame()
        {
            _Model.UpdateAnimationTransforms();
        }

        private AnimationControl GetMasterAnimation()
        {
            switch (Type)
            {
                case AnimationType.Skeletal: return _Model?.SkeletalAnim;
                case AnimationType.Material: return _Model?.MaterialAnim;
            }

            return null;
        }
        
        public void AdvanceFrame()
        {
            if (_Model != null)
            {
                _Model.SkeletalAnim.AdvanceFrame();
                _Model.MaterialAnim.AdvanceFrame();

                UpdateFrame();
            }
        }

        public void Play(float Step = 1)
        {
            if (_Model != null)
            {
                _Model.SkeletalAnim.Play(Step);
                _Model.MaterialAnim.Play(Step);

                UpdateFrame();
            }
        }

        public void Stop()
        {
            if (_Model != null)
            {
                _Model.SkeletalAnim.Stop();
                _Model.MaterialAnim.Stop();

                UpdateFrame();
            }
        }

        //No need for immediate frame updates
        public void Pause()
        {
            _Model?.SkeletalAnim.Pause();
            _Model?.MaterialAnim.Pause();
        }

        public void SlowDown()
        {
            _Model?.SkeletalAnim.SlowDown();
            _Model?.MaterialAnim.SlowDown();
        }

        public void SpeedUp()
        {
            _Model?.SkeletalAnim.SpeedUp();
            _Model?.MaterialAnim.SpeedUp();
        }
    }
}
