namespace SPICA.Rendering.Animation
{
    public interface IAnimationControl
    {
        AnimationState State { get; set; }

        float Frame       { get; set; }
        float Step        { get; set; }
        float FramesCount { get; }
        bool  IsLooping   { get; }

        void AdvanceFrame();
        void SlowDown();
        void SpeedUp();
        void Play(float Step = 1);
        void Pause();
        void Stop();
    }
}
