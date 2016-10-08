namespace SPICA.Math3D
{
    struct Vector2D
    {
        public float X;
        public float Y;

        public Vector2D(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public override string ToString()
        {
            return string.Format("X: {0} Y: {1}", X, Y);
        }
    }
}
