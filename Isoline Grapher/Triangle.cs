using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Isoline_Grapher
{
    /// <summary>
    /// Triangle class.
    /// </summary>
    public class Triangle
    {
        /// <summary>
        /// Reset the unique ID giver for the triangulation
        /// </summary>
        public static void ResetIndex() { m_index = 0; }

        #region Constructors: (), (Triangle src), (Vertex a, Vertex b, Vertex c)
        /// <summary>
        ///Default Constructor. Just give an index.
        /// </summary>
        public Triangle()
        {
            Index = m_index;
            m_index++;
        }

        /// <summary>
        /// Copy constructor. Copy all fields except index-ID, give a new one
        /// </summary>
        /// <param name="src"></param>
        public Triangle(Triangle src)
        {
            A = src.A;
            B = src.B;
            C = src.C;
            AB = src.AB;
            BC = src.BC;
            CA = src.CA;
            Index = m_index;
            m_index++;
        }

        /// <summary>
        /// Construct from vertexes and give a new ID
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public Triangle(Vertex a, Vertex b, Vertex c)
        {
            A = a;
            B = b;
            C = c;
            Index = m_index;
            m_index++;
        }
        #endregion

        #region Protected data.

        // Vertexes.
        protected Vertex m_a = null;
        protected Vertex m_b = null;
        protected Vertex m_c = null;

        // Lengths.
        protected float m_abLen = 0;
        protected float m_bcLen = 0;
        protected float m_caLen = 0;
        protected bool m_abLenCalcd = false;
        protected bool m_bcLenCalcd = false;
        protected bool m_caLenCalcd = false;

        // Side determinations.
        protected bool m_abDet = false;
        protected bool m_bcDet = false;
        protected bool m_caDet = false;
        protected bool m_abDetCalcd = false;
        protected bool m_bcDetCalcd = false;
        protected bool m_caDetCalcd = false;

        /// <summary>
        /// Index of this triangle for debug.
        /// </summary>
        protected static int m_index = 0;

        // Sides
        protected Triangle m_ab = null;
        protected Triangle m_bc = null;
        protected Triangle m_ca = null;

        // Center
        protected bool m_centerComputed = false;
        protected Vertex m_center = null;

        #endregion

        /// <summary>
        /// Index.
        /// </summary>
        public int Index { get; protected set; }

        /// <summary>
        /// Which search region it is in.
        /// </summary>
        public int RegionCode { get; set; }

        /// <summary>
        /// To string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Index + ": " + A.ToString() + " => " + B.ToString() + " => " + C.ToString();
        }

        /// <summary>
        /// Compute the center
        /// </summary>
        public Vertex Center
        {
            get
            {
                if (m_centerComputed) return m_center;
                m_center = new Vertex(
                    (A.X + B.X + C.X) / 3f,
                    (A.Y + B.Y + C.Y) / 3f,
                    (A.Z + B.Z + C.Z) / 3f);

                float delta = m_center.DeltaSquaredXY(A);
                float tmp = m_center.DeltaSquaredXY(B);
                delta = delta > tmp ? delta : tmp;
                tmp = m_center.DeltaSquaredXY(C);
                delta = delta > tmp ? delta : tmp;
                FarthestFromCenter = delta;
                m_centerComputed = true;

                return m_center;
            }
        }

        /// <summary>
        /// Farthest distance a point is from center is distance squared.
        /// </summary>
        public float FarthestFromCenter { get; protected set; }

        /// <summary>
        /// Vertex A
        /// </summary>
        public Vertex A
        {
            get { return m_a; }
            set
            {
                if (m_a == value) return;
                m_abDetCalcd = false;
                m_caDetCalcd = false;
                m_abLenCalcd = false;
                m_caLenCalcd = false;
                m_centerComputed = false;
                m_a = value;
            }
        }

        /// <summary>
        /// Vertex B
        /// </summary>
        public Vertex B
        {
            get { return m_b; }
            set
            {
                if (m_b == value) return;
                m_abDetCalcd = false;
                m_bcDetCalcd = false;
                m_abLenCalcd = false;
                m_bcLenCalcd = false;
                m_centerComputed = false;
                m_b = value;
            }
        }

        /// <summary>
        /// Vertex C
        /// </summary>
        public Vertex C
        {
            get { return m_c; }
            set
            {
                if (m_c == value) return;
                m_caDetCalcd = false;
                m_bcDetCalcd = false;
                m_caLenCalcd = false;
                m_bcLenCalcd = false;
                m_centerComputed = false;
                m_c = value;
            }
        }

        /// <summary>
        /// Triangle AB shares side AB.
        /// </summary>
        public Triangle AB { get { return m_ab; } set { m_ab = value; } }

        /// <summary>
        /// Triangle BC shares side BC.
        /// </summary>
        public Triangle BC { get { return m_bc; } set { m_bc = value; } }

        /// <summary>
        /// Triangle CA shares side CA.
        /// </summary>
        public Triangle CA { get { return m_ca; } set { m_ca = value; } }


        /// <summary>
        /// AB det.
        /// </summary>
        protected bool abDet
        {
            get
            {
                if (!m_abDetCalcd)
                {
                    m_abDet = vertexTest(A, B, C);
                }
                return m_abDet;
            }
        }

        /// <summary>
        /// BC det.
        /// </summary>
        protected bool bcDet
        {
            get
            {
                if (!m_bcDetCalcd)
                {
                    m_bcDet = vertexTest(B, C, A);
                }
                return m_bcDet;
            }
        }

        /// <summary>
        /// CA det.
        /// </summary>
        protected bool caDet
        {
            get
            {
                if (!m_caDetCalcd)
                {
                    m_caDet = vertexTest(C, A, B);
                }
                return m_caDet;
            }
        }

        /// <summary>
        /// Vertex sidedness test.
        /// </summary>
        /// <param name="la"></param>
        /// <param name="lb"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        protected bool vertexTest(Vertex la, Vertex lb, Vertex t)
        {
            // y = mx + b
            if (la.X == lb.X)
            {
                // Vertical at X.
                return t.X > la.X;
            }
            if (la.Y == lb.Y)
            {
                return t.Y > la.Y;
            }
            float m = (la.Y - lb.Y) / (la.X - lb.X);
            float b = la.Y - (m * la.X);
            return (m * t.X + b - t.Y) > 0;
        }

        /// <summary>
        /// Does this contain t.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Contains(Vertex t)
        {
            float delta = t.DeltaSquaredXY(Center);
            if (delta > FarthestFromCenter) return false;
            if (abDet != vertexTest(A, B, t)) return false;
            if (bcDet != vertexTest(B, C, t)) return false;
            if (caDet != vertexTest(C, A, t)) return false;
            return true;
        }

        /// <summary>
        /// Length of AB, cached and lazy calculated.
        /// </summary>
        public float AB_LengthSquared
        {
            get
            {
                if (m_abLenCalcd == true)
                {
                    return m_abLen;
                }
                if ((A == null) || (B == null)) return -1;
                m_abLen = A.DeltaSquaredXY(B);
                m_abLenCalcd = true;
                return m_abLen;
            }
        }

        /// <summary>
        /// Length of BC, cached and lazy calculated.
        /// </summary>
        public float BC_LengthSquared
        {
            get
            {
                if (m_bcLenCalcd == true)
                {
                    return m_bcLen;
                }
                if ((B == null) || (C == null)) return -1;
                m_bcLen = B.DeltaSquaredXY(C);
                m_bcLenCalcd = true;
                return m_bcLen;
            }
        }

        /// <summary>
        /// Length of CA, cached and lazy calculated.
        /// </summary>
        public float CA_LengthSquared
        {
            get
            {
                if (m_caLenCalcd == true)
                {
                    return m_caLen;
                }
                if ((C == null) || (A == null)) return -1;
                m_caLen = C.DeltaSquaredXY(A);
                m_caLenCalcd = true;
                return m_caLen;
            }
        }

        /// <summary>
        /// Area of the triangle.
        /// </summary>
        public float Area
        {
            get
            {
                float a = AB_LengthSquared;
                float b = BC_LengthSquared;
                float c = CA_LengthSquared;
                a = (float)System.Math.Sqrt(a);
                b = (float)System.Math.Sqrt(b);
                c = (float)System.Math.Sqrt(c);

                // Herons formula.
                float s = 0.5f * (a + b + c);
                return (float)System.Math.Sqrt(s * (s - a) * (s - b) * (s - c));
            }
        }

        /// <summary>
        /// Return the indexed edge length;
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public float Edge_Length(int i)
        {
            i = i < 0 ? i + 3 : i > 2 ? i - 3 : i;
            return i == 0 ? AB_LengthSquared : i == 1 ? BC_LengthSquared : CA_LengthSquared;
        }

        /// <summary>
        /// Return the oposite of the edge.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Vertex OpositeOfEdge(int i)
        {
            i = i < 0 ? i + 3 : i > 2 ? i - 3 : i;
            return i == 0 ? C : i == 1 ? A : B;
        }

        /// <summary>
        /// Set the vertex by index.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="v"></param>
        public void SetVertex(int i, Vertex v)
        {
            i = i < 0 ? i + 3 : i > 2 ? i - 3 : i;
            if (i == 0) A = v;
            if (i == 1) B = v;
            if (i == 2) C = v;
        }

        /// <summary>
        /// Get the cosine angle associated with a vertex.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public float VertexCosineAngle(int i)
        {
            i = i < 0 ? i + 3 : i > 2 ? i - 3 : i;
            float dx1 = 0;
            float dx2 = 0;
            float dy1 = 0;
            float dy2 = 0;
            if (i == 0)
            {
                dx1 = B.X - A.X;
                dy1 = B.Y - A.Y;
                dx2 = C.X - A.X;
                dy2 = C.Y - A.Y;
            }
            else
            {
                if (i == 1)
                {
                    dx1 = C.X - B.X;
                    dy1 = C.Y - B.Y;
                    dx2 = A.X - B.X;
                    dy2 = A.Y - B.Y;
                }
                else
                {
                    dx1 = A.X - C.X;
                    dy1 = A.Y - C.Y;
                    dx2 = B.X - C.X;
                    dy2 = B.Y - C.Y;
                }
            }
            float mag1 = (dx1 * dx1) + (dy1 * dy1);
            float mag2 = (dx2 * dx2) + (dy2 * dy2);
            float mag = (float)System.Math.Sqrt(mag1 * mag2);
            float dot = (float)((dx1 * dx2) + (dy1 * dy2)) / mag;

            // dot is 0 to 1 result of the cosine.
            return dot;
        }


        /// <summary>
        /// Get the angle of a vertex in radians.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public float VertexAngleRadians(int i)
        {
            return (float)System.Math.Acos(VertexCosineAngle(i));
        }

        /// <summary>
        /// Is this rectangle within the region.
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public bool Inside(System.Drawing.RectangleF region)
        {
            if (!A.InsideXY(region)) return false;
            if (!B.InsideXY(region)) return false;
            if (!C.InsideXY(region)) return false;
            return true;
        }

        /// <summary>
        /// Repair any Edge links, both ways.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public void RepairEdges(Triangle a)
        {
            // Check if a.AB is in this.
            if (this.Index == a.Index) return;
            if (bothIn(a, a.A, a.B)) { a.AB = this; return; }
            if (bothIn(a, a.B, a.C)) { a.BC = this; return; }
            if (bothIn(a, a.C, a.A)) { a.CA = this; return; }
        }

        /// <summary>
        /// Are both vertexes in?
        /// </summary>
        /// <param name="t"></param>
        /// <param name="vt"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected bool bothIn(Triangle t, Vertex a, Vertex b)
        {
            if (a == A)
            {
                if (b == B) { AB = t; return true; }
                if (b == C) { CA = t; return true; }
            }
            if (a == B)
            {
                if (b == A) { AB = t; return true; }
                if (b == C) { BC = t; return true; }
            }
            if (a == C)
            {
                if (b == A) { CA = t; return true; }
                if (b == B) { BC = t; return true; }
            }
            return false;
        }

        /// <summary>
        /// Vertex by number.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Vertex GetVertex(int i)
        {
            i = i < 0 ? i + 3 : i > 2 ? i - 3 : i;
            if (i == 0) return A;
            if (i == 1) return B;
            return C;
        }

        /// <summary>
        /// Set the edge by index.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="t"></param>
        public void SetEdge(int i, Triangle t)
        {
            i = i < 0 ? i + 3 : i > 2 ? i - 3 : i;
            if (i == 0) AB = t;
            if (i == 1) BC = t;
            if (i == 2) CA = t;
        }

        /// <summary>
        /// Get the indexed edge.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Triangle Edge(int i)
        {
            return i == 0 ? AB : i == 1 ? BC : CA;
        }

        /// <summary>
        /// Set the z value for v, linearly approximmating by the verteces
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public void ApproxHeight(ref Vertex v)
        {
            Vertex ab = new Vertex(A.X - B.X, A.Y - B.Y, A.Z - B.Z);
            Vertex ac = new Vertex(A.X - C.X, A.Y - C.Y, A.Z - C.Z);
            Vertex nrmal = new Vertex(ab.Y * ac.Z - ab.Z * ac.Y, ab.Z * ac.X - ab.X * ac.Z, ab.X * ac.Y - ab.Y * ac.X);
            // normal vector for the plane - nx, ny, nz
            float zbase = A.X * nrmal.X + A.Y * nrmal.Y + A.Z * nrmal.Z; // normal mult by ab
            // x * nx + y * ny + z * nz = zbase - plane equation
            v.Z = (zbase - v.X * nrmal.X - v.Y * nrmal.Y) / nrmal.Z;
            if (Math.Abs(nrmal.Z) < 0.0001F)
            {
                Console.WriteLine();
            }
        }

        public bool IsDegenerate()
        {
            float AB_Length = (float)Math.Sqrt(AB_LengthSquared);
            float BC_Length = (float)Math.Sqrt(BC_LengthSquared);
            float CA_Length = (float)Math.Sqrt(CA_LengthSquared);
            if (Math.Abs(AB_Length + BC_Length - CA_Length) < 0.00001F)
            {
                return true;
            }
            if (Math.Abs(CA_Length + BC_Length - AB_Length) < 0.00001F)
            {
                return true;
            }
            if (Math.Abs(AB_Length + CA_Length - BC_Length) < 0.00001F)
            {
                return true;
            }
            return false;
        }
    }
}