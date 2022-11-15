namespace Isoline_Grapher
{
    public class Vertex
    {
        public float X;
        public float Y;
        public float Z;
        public int Index; // only for triangulation or going over the set.

        /// <summary>
        /// default constructor sets all to 0
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vertex()
        {
            X = 0; Y = 0; Z = 0;
        }

        public Vertex(float x, float y, float z)
        {
            X = x; Y = y; Z = z;
        }

        /// <summary>
        /// squared distance between this point and t. not counting heights.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public float DeltaSquaredXY(Vertex t)
        {
            float dx = (X - t.X);
            float dy = (Y - t.Y);
            return (dx * dx) + (dy * dy);
        }

        /// <summary>
        /// distance between this point and t. not counting heights.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public float DistanceXY(Vertex t)
        {
            return (float)System.Math.Sqrt(DeltaSquaredXY(t));
        }

        /// <summary>
        /// If this point is in the rectangle
        /// </summary>
        /// <param name="region">Bounding rectangle</param>
        /// <returns></returns>
        public bool InsideXY(System.Drawing.RectangleF region)
        {
            if (X < region.Left) return false;
            if (X > region.Right) return false;
            if (Y < region.Top) return false;
            if (Y > region.Bottom) return false;
            return true;
        }

        public override string ToString()
        {
            return X + "," + Y + "," + Z;
        }
    };
}
