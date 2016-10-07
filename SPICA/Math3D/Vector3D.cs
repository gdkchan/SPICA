namespace SPICA.Math3D
{
    struct Vector3D
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3D(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public override string ToString()
        {
            return string.Format("X: {0} Y: {1} Z: {2}", X, Y, Z);
        }
    }
}
