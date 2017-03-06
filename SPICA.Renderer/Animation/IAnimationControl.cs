namespace SPICA.Renderer.Animation
{
    interface IAnimationControl
    {
        float Frame       { get; set; }
        float Step        { get; set; }
        bool  IsLooping   { get; set; }
        bool  HasData     { get; }
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
