namespace SPICA.Rendering.Animation
{
    public interface IAnimationControl
    {
        bool  IsLooping   { get; set; }
        float Frame       { get; set; }
        float Step        { get; set; }
        float FramesCount { get; }

        void AdvanceFrame();
        void SlowDown();
        void SpeedUp();
        void Play(float Step);
        void Pause();
        void Stop();
    }
}
