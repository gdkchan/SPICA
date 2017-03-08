namespace SPICA.Renderer.Animation
{
    interface IAnimationControl
    {
        bool  IsLooping   { get; set; }
        float Frame       { get; set; }
        float Step        { get; set; }
        float FramesCount { get; }

        void AdvanceFrame();
        void SlowDown();
        void SpeedUp();
        void Play(float Step);
        void Play();
        void Pause();
        void Stop();
    }
}
