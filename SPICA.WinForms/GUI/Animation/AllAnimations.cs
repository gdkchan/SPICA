using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Renderer;
using SPICA.Renderer.Animation;

namespace SPICA.WinForms.GUI.Animation
{
    class AllAnimations : IAnimationControl
    {
        public H3D SceneData;
        public Model Model;

        public AnimationType Type;

        private AnimationControl MasterAnim;

        public float Frame
        {
            get
            {
                return MasterAnim?.Frame ?? 0;
            }
            set
            {
                if (Model != null)
                {
                    Model.SkeletalAnim.Frame = value;
                    Model.MaterialAnim.Frame = value;

                    UpdateFrame();
                }
            }
        }

        public float Step
        {
            get
            {
                return MasterAnim?.Step ?? 1;
            }
            set
            {
                if (Model != null)
                {
                    Model.SkeletalAnim.Step = value;
                    Model.MaterialAnim.Step = value;
                }
            }
        }

        public bool IsLooping
        {
            get
            {
                return MasterAnim?.IsLooping ?? false;
            }
            set
            {
                if (Model != null)
                {
                    Model.SkeletalAnim.IsLooping = value;
                    Model.MaterialAnim.IsLooping = value;
                }
            }
        }

        public float FramesCount { get { return MasterAnim?.FramesCount ?? 0; } }

        public void SetAnimation(int Index, AnimationType Type)
        {
            this.Type = Type;

            if (SceneData != null && Model != null)
            {
                H3DAnimation Animation = null;

                switch (Type)
                {
                    case AnimationType.Skeletal:
                        Animation = SceneData.SkeletalAnimations[Index];
                        MasterAnim = Model.SkeletalAnim;
                        break;

                    case AnimationType.Material:
                        Animation = SceneData.MaterialAnimations[Index];
                        MasterAnim = Model.MaterialAnim;
                        break;
                }

                MasterAnim.SetAnimation(Animation);

                if (Type != AnimationType.Skeletal &&
                    (Index = SceneData.SkeletalAnimations.FindIndex(Animation.Name)) != -1)
                {
                    Model.SkeletalAnim.SetAnimation(SceneData.SkeletalAnimations[Index]);
                }

                if (Type != AnimationType.Material &&
                    (Index = SceneData.MaterialAnimations.FindIndex(Animation.Name)) != -1)
                {
                    Model.MaterialAnim.SetAnimation(SceneData.MaterialAnimations[Index]);
                }
            }
        }

        public void Reset()
        {
            if (Model != null)
            {
                Stop();

                Model.SkeletalAnim.SetAnimation(null);
                Model.MaterialAnim.SetAnimation(null);
            }
        }

        //Needs immediate frame updates
        private void UpdateFrame()
        {
            Model.UpdateAnimationTransforms();
        }
        
        public void AdvanceFrame()
        {
            if (Model != null)
            {
                Model.SkeletalAnim.AdvanceFrame();
                Model.MaterialAnim.AdvanceFrame();

                UpdateFrame();
            }
        }

        public void Play(float Step = 1)
        {
            if (Model != null)
            {
                Model.SkeletalAnim.Play(Step);
                Model.MaterialAnim.Play(Step);

                UpdateFrame();
            }
        }

        public void Stop()
        {
            if (Model != null)
            {
                Model.SkeletalAnim.Stop();
                Model.MaterialAnim.Stop();

                UpdateFrame();
            }
        }

        //No need for immediate frame updates
        public void Pause()
        {
            Model?.SkeletalAnim.Pause();
            Model?.MaterialAnim.Pause();
        }

        public void SlowDown()
        {
            Model?.SkeletalAnim.SlowDown();
            Model?.MaterialAnim.SlowDown();
        }

        public void SpeedUp()
        {
            Model?.SkeletalAnim.SpeedUp();
            Model?.MaterialAnim.SpeedUp();
        }
    }
}
