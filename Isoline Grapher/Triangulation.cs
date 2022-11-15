using System.Collections.Generic;

namespace Isoline_Grapher
{
    /// <summary>
    /// Triangulation
    /// </summary>
    public class Triangulation
    {
        #region Protected data
        /// <summary>
        /// Recursion limit.
        /// </summary>
        protected int m_recursion = 4;

        /// <summary>
        /// The points.
        /// </summary>
        protected List<Vertex> m_points = new List<Vertex>();

        /// <summary>
        /// The facets.
        /// </summary>
        protected List<Triangle> m_facets = new List<Triangle>();

        /// <summary>
        /// Bounds. Had to hardcode for the easier construction
        /// </summary>
        protected System.Drawing.RectangleF m_bounds = new System.Drawing.RectangleF(0, 0, Form1.drawingWidth, Form1.drawingHeight);

        #endregion

        #region Properties: Points, Facets, Bounds, Recursion.
        /// <summary>
        /// The points.
        /// </summary>
        public List<Vertex> Points
        {
            get { return m_points; }
            set { m_points = value; }
        }

        /// <summary>
        /// The facets.
        /// </summary>
        public List<Triangle> Facets
        {
            get { return m_facets; }
            set { m_facets = value; }
        }

        /// <summary>
        /// Bounds.
        /// </summary>
        public System.Drawing.RectangleF Bounds
        {
            get { return m_bounds; }
            set { m_bounds = value; }
        }

        /// <summary>
        /// Recursion level.
        /// </summary>
        public int Recursion
        {
            get { return m_recursion; }
            set { if (value < 0) value = 0; m_recursion = value; }
        }

        #endregion

        /// <summary>
        /// Get the indexes for the triangular mesh.
        /// Points is the points. Every 3 indicies is a triangle.
        /// </summary>
        /// <returns></returns>
        public int[] GetVertexIndicies()
        {
            int[] indicies = new int[3 * Facets.Count];
            int k = 0;
            for (int i = 0; i < m_points.Count; i++)
            {
                m_points[i].Index = i;
            }
            for (int i = 0; i < Facets.Count; i++)
            {
                indicies[k++] = Facets[i].A.Index;
                indicies[k++] = Facets[i].B.Index;
                indicies[k++] = Facets[i].C.Index;
            }
            return indicies;
        }

        /// <summary>
        /// Compute.
        /// </summary>
        /// <param name="set"></param>
        /// <param name="bounds"></param>
        public void Compute(List<Vertex> set, System.Drawing.RectangleF bounds)
        {
            Setup(bounds);
            for (int i = 0; i < set.Count; i++)
            {
                Append(set[i]);
            }

            List<Triangle> temp = new List<Triangle>();
            foreach (Triangle t in m_facets)
            {
                if (!t.IsDegenerate())
                    temp.Add(t);
                else
                {
                    float tab = t.AB_LengthSquared;
                    float tac = t.CA_LengthSquared;
                    float tbc = t.BC_LengthSquared;
                }
            }
            m_facets = temp;
        }

        /// <summary>
        /// Append point.
        /// </summary>
        /// <param name="v"></param>
        public void Append(Vertex v)
        {
            // Find a triangle containing v.
            for (int i = 0; i < Facets.Count; i++)
            {
                if (Facets[i].Contains(v))
                {
                    Insert(v, Facets[i]);
                }
            }
        }

        /// <summary>
        /// Setup.
        /// </summary>
        /// <param name="bounds"></param>
        public void Setup(System.Drawing.RectangleF bounds)
        {
            Triangle.ResetIndex();
            Facets.Clear();
            Points.Clear();
            Bounds = bounds;

            Vertex tl = new Vertex(Bounds.Left, Bounds.Top, 0);
            Vertex tr = new Vertex(Bounds.Right, Bounds.Top, 0);
            Vertex bl = new Vertex(Bounds.Left, Bounds.Bottom, 0);
            Vertex br = new Vertex(Bounds.Right, Bounds.Bottom, 0);
            Triangle t1 = new Triangle();
            Triangle t2 = new Triangle();
            t1.A = bl;
            t1.B = tr;
            t1.C = tl;
            t2.A = bl;
            t2.B = br;
            t2.C = tr;
            t1.AB = t2;
            t2.CA = t1;
            Facets.Add(t1);
            Facets.Add(t2);
        }


        /// <summary>
        /// Draw the mesh.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="minx"></param>
        /// <param name="miny"></param>
        /// <param name="maxx"></param>
        /// <param name="maxy"></param>
        public void Draw(System.Drawing.Graphics g, int minx, int miny, int maxx, int maxy)
        {
            System.Drawing.Pen[] pens = {
                System.Drawing.Pens.Red,
                System.Drawing.Pens.Green,
                System.Drawing.Pens.Blue,
                System.Drawing.Pens.Orange,
                System.Drawing.Pens.Purple,
                System.Drawing.Pens.Brown,
                System.Drawing.Pens.Violet,
                System.Drawing.Pens.Lime,
                System.Drawing.Pens.DarkBlue,
                System.Drawing.Pens.Magenta,
                System.Drawing.Pens.Cyan,
                System.Drawing.Pens.DarkRed};

            maxx -= 2;
            maxy -= 2;
            for (int i = 0; i < Facets.Count; i++)
            {
                float x = Facets[i].OpositeOfEdge(0).X;
                float y = Facets[i].OpositeOfEdge(0).Y;
                int k = i % pens.Length;
                for (int j = 1; j < 4; j++)
                {
                    x = x < minx ? minx : x;
                    y = y < miny ? miny : y;
                    x = x > maxx ? maxx : x;
                    y = y > maxy ? maxy : y;

                    float nx = Facets[i].OpositeOfEdge(j).X;
                    float ny = Facets[i].OpositeOfEdge(j).Y;
                    nx = nx < minx ? minx : nx;
                    ny = ny < miny ? miny : ny;
                    nx = nx > maxx ? maxx : nx;
                    ny = ny > maxy ? maxy : ny;
                    g.DrawLine(pens[k], x, y, nx, ny);
                    x = nx;
                    y = ny;
                }
            }

        }

        /// <summary>
        /// Insert.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="old"></param>
        protected void Insert(Vertex v, Triangle old)
        {
            // Avoid duplicates, if this facet contains v as a vertex,
            // just return.
            if ((old.A.X == v.X) && (old.A.Y == v.Y)) return;
            if ((old.B.X == v.X) && (old.B.Y == v.Y)) return;
            if ((old.C.X == v.X) && (old.C.Y == v.Y)) return;

            m_points.Add(v);

            // Split old into 3 triangles,
            // Because old is counter clockwise, when duplicated,
            // ab, bc, ca is counter clockwise.
            // By changing one point and keeping to the commutation, 
            // they remain counter clockwise.
            Triangle ab = new Triangle(old); // contains old ab, v is new C.
            Triangle bc = new Triangle(old); // contains old bc, v is new A.
            Triangle ca = new Triangle(old); // contains old ca, v is new B.
            ab.C = v;
            bc.A = v;
            ca.B = v;

            // This also makes assigning the sides easy.
            ab.BC = bc;
            ab.CA = ca;
            bc.AB = ab;
            bc.CA = ca;
            ca.AB = ab;
            ca.BC = bc;

            // The existing trianges that share an edge with old, 
            // now share an edge with one of the three new triangles.
            // Repair the existing.

            // One way of looking at it:
            // for (int j = 0; j < 3; j++)
            // {
            //    if ((ab.AB != null) && (ab.AB.Edge(j) == old)) ab.AB.SetEdge(j, ab);
            //    if ((bc.BC != null) && (bc.BC.Edge(j) == old)) bc.BC.SetEdge(j, bc);
            //    if ((ca.CA != null) && (ca.CA.Edge(j) == old)) ca.CA.SetEdge(j, ca);
            // } 
            // This is faster, null check is once per edge, and default logic
            // reduces the compares by one. Instead of 3*3*2 comparisons = 18,
            // Worst case is 3*3 = 9, Average is 2+3+3=8.
            Triangle[] ta = { ab.AB, bc.BC, ca.CA };
            Triangle[] tb = { ab, bc, ca };
            for (int j = 0; j < 3; j++)
            {
                if (ta[j] == null) continue;
                if (ta[j].Edge(0) == old)
                {
                    ta[j].SetEdge(0, tb[j]);
                    continue;
                }
                if (ta[j].Edge(1) == old)
                {
                    ta[j].SetEdge(1, tb[j]);
                    continue;
                }
                ta[j].SetEdge(2, tb[j]);
            }

            // Add the new, remove the old.
            Facets.Add(ab);
            Facets.Add(bc);
            Facets.Add(ca);
            Facets.Remove(old);

            // Check for 1st order flipping.
            // Triangle ab has neighbor ab.AB.
            // Depth of up to recursion deep.
            // Remember that due to commutators, same.same is outward, 
            // same.different is inward.

            flipIfNeeded(ab, ab.AB, Recursion);
            flipIfNeeded(bc, bc.BC, Recursion);
            flipIfNeeded(ca, ca.CA, Recursion);

            return;
        }


        /// <summary>
        /// Flip if needed.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="depth"></param>
        protected void flipIfNeeded(Triangle a, Triangle b, int depth)
        {
            if (depth <= 0) return;
            if (a == null) return;
            if (b == null) return;
            depth--;

            // Triangle a and b share a border, together there are 4 points.
            // As they are counter clockwise, the directions on the boarder are different
            // and the indexing on one border is different than the other.
            // For example if "a" has edge AB in common we know the same direction on that edge
            // is clockwise in triangle "b", but we don't know which vertex in b starts that
            // edge. Luckily edges also contain the reference to the next triangle
            // That makes things easy.
            int ai = 0;
            int bi = 0;
            // Rather than roll a loop, since only 3, and default is zero.
            if (a.Edge(1) == b) ai = 1;
            if (a.Edge(2) == b) ai = 2;
            if (b.Edge(1) == a) bi = 1;
            if (b.Edge(2) == a) bi = 2;

            // The vertex index of the oposite is:
            //     edge index (ai,bi)    vertex index (vai, vbi)
            //       0                      2
            //       1                      0
            //       2                      1
            //       x                      (x+2)%3
            int[] table = { 2, 0, 1 };
            int vai = table[ai];
            int vbi = table[bi];

            // The delaunay condition is that the sum of the interior angles that span
            // the oposite vertexes must be less than 180 degrees (pi in radians) 
            // if it is, then no need to flip.
            float fa = a.VertexAngleRadians(vai);
            float fb = b.VertexAngleRadians(vbi);
            if (fa + fb <= System.Math.PI)
            {
                return;
            }

            // Replace a and b, flipping replacements as needed.
            // opposite and next of opposite remains as an edge in each, and the oposites switch!

            Triangle[] ts = { a.Edge(0), a.Edge(1), a.Edge(2), b.Edge(0), b.Edge(1), b.Edge(2) };

            // The oposite is simple, if the edges are 0=AB, 1=BC, 2=CA, then 
            // the oposites are C, A, B (counter clockwise conjugate). 
            Vertex aOp = a.OpositeOfEdge(ai);
            Vertex bOp = b.OpositeOfEdge(bi);

            a.SetVertex(ai + 1, bOp);
            b.SetVertex(bi + 1, aOp);

            a.AB = null;
            a.BC = null;
            a.CA = null;
            b.AB = null;
            b.BC = null;
            b.CA = null;

            // Remake edge a.AB.
            for (int i = 0; i < 6; i++)
            {
                if (ts[i] == null) continue;
                ts[i].RepairEdges(a);
                ts[i].RepairEdges(b);
            }

            // Check if -1, 0 need flipping.
            // for (int j = 0; j < 2; j++)
            // {
            //    if (j != ai) flipIfNeeded(a, a.Edge(j), depth);
            //    if (j != bi) flipIfNeeded(b, b.Edge(j), depth);
            // }
            // With a commutator index, to get the other two, add +1, +2.
            flipIfNeeded(a, a.Edge(ai + 1), depth);
            flipIfNeeded(b, b.Edge(bi + 1), depth);
            flipIfNeeded(a, a.Edge(ai + 2), depth);
            flipIfNeeded(b, b.Edge(bi + 2), depth);
        }
    }
}