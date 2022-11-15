using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Isoline_Grapher
{
    public partial class Form1 : Form
    {
        public static int drawingHeight = 300;
        public static int drawingWidth = 300;
        public static int offsetx = 5; // offset from the bounds of the box, to see the points better
        public static int offsety = 5;
        public static int gridStep = 10;
        public static int isoNum = 12;
        public static bool dataNormal = false;
        public static bool dataTriangulated = false;
        public static bool gridCalculated = false;
        Graphics DataCanvas = null;
        Graphics LinesCanvas = null;
        Triangulation triangulation = null;
        List<Vertex> inputData = null;
        Vertex[,] apprGrid = null;
        public float maxHeight = 0;
        public float minheight = 0;

        // proportionally fixes data: makes X and Y in the range from 0 to drawingHeight/Width, heights from 0 to 1
        void NormalizeAndFix(ref List<Vertex> points)
        {
            float maxX = 0;
            float maxY = 0;
            float minX = 0;
            float minY = 0;
            foreach (Vertex point in points)
            {
                //if (Math.Abs(point.X) > maxX)
                //    maxX = Math.Abs(point.X);
                //if (Math.Abs(point.Y) > maxY)
                //    maxY = Math.Abs(point.Y);
                if (point.X > maxX)
                    maxX = point.X;
                if (point.X < minX)
                    minX = point.X;
                if (point.Y > maxY)
                    maxY = point.Y;
                if (point.Y < minY)
                    minY = point.Y;
                if (point.Z > maxHeight)
                    maxHeight = point.Z;
                if (point.Z < minheight)
                    minheight = point.Z;
            }
            maxHeight -= minheight; // so maxheight will have the height range length
            maxX -= minX;
            maxY -= minY;
            bool normX = true, normY = true, normH = true;
            if (maxX < 0.00001F) // check for 0
                // all points on one line or the center does not a good material make.
                // only trying to normalize if the coordinates are somewhat different, so we won't divide by basically 0
                normX = false;
            if (maxY < 0.00001F) // check for 0
                normY = false;
            if (maxHeight < 0.00001F) // check for 0
                normH = false;

            float multX = drawingWidth / maxX;
            float multY = drawingHeight / maxY;

            for (int i = 0; i < points.Count; i++)
            {
                Vertex point = points.ElementAt(i);
                point.Z -= minheight;
                point.X -= minX;
                point.Y -= minY;
                //if (normX)
                //    point.X /= maxX;
                //if (normY)
                //    point.Y /= maxY;
                if (normX)
                    point.X *= multX;
                if (normY)
                    point.Y *= multY;
                if (normH)
                    point.Z /= maxHeight;
                points[i] = point;
            }
        }
        // turns heights from 0 to 1 into colors. The redder, the higher.
        Color HeightToColour(float H)
        {
            H = Math.Min(1 - H, 0.99F); // map lowest to violet, highest to pure red
            // capping at 0.99 so that one end would not loop back to another
            float H1 = H * 360; // Hue in degrees
            int C = 255; // simplified from HSV to RGB formulas when Saturation is full and Brightness is half
            float fmd = H1 - ((int)H1/2*2);
            float X = 1 - Math.Abs(fmd - 1);// simplified from HSV to RGB formulas when Saturation is full and Brightness is half
            int Hrem = (int)(H * 6); // for the piecewise formula
            int r, g, b;
            switch (Hrem) // refer to Wikipedia for formulas
            {
                case 0: r = C; g = (int)(X*255); b = 0; break;
                case 1: r = (int)(X * 255); g = C; b = 0; break;
                case 2: r = 0; g = C; b = (int)(X * 255); break;
                case 3: r = 0; g = (int)(X*255); b = C; break;
                case 5: r = (int)(X * 255); g = 0; b = C; break;
                default: r = C; g = 0; b = (int)(X * 255); break;
            }
            return Color.FromArgb(r, g, b);
        }

        // sets data to the test example
        void ResetData()
        {
            inputData = new List<Vertex> {
                new Vertex(0F, 0F, 5F),
                new Vertex(0.5F, 0.5F, 3F),
                new Vertex(0.25F,0.75F,3),
                new Vertex(-0.5F,0.5F,3),
                new Vertex(-0.25F,0.75F,3),
                new Vertex(0.5F,-0.5F,3),
                new Vertex(0.25F,-0.75F,3),
                new Vertex(-0.5F,-0.5F,3),
                new Vertex(-0.25F,-0.75F,3),
                new Vertex(2,0,2),
                new Vertex(0,2,2),
                new Vertex(-1,-1,2),
                new Vertex(1,-1,2),
                new Vertex(-1,1,2),
                new Vertex(3,0,0),
                new Vertex(0,3,0),
                new Vertex(-3,0,0),
                new Vertex(0,-3,0)
            };
            dataNormal = false;
            dataTriangulated = false;
            gridCalculated = false;
        }
        // clears and divides into quaters
        void PrepareDataCanvas()
        {
            DataCanvas = dataDrawBox.CreateGraphics();
            // Fill with white
            DataCanvas.Clear(Color.White);
            Pen penbkg = new Pen(Color.Gray, 1);
            DataCanvas.Clear(Color.White);
            int centerx = offsetx + drawingWidth / 2;
            int centery = offsety + drawingHeight / 2;
            DataCanvas.DrawLine(penbkg, 0, centery, centerx * 2, centery);
            DataCanvas.DrawLine(penbkg, centerx, 0, centerx, centery * 2);

        }
        // clears and divides into quaters
        void PrepareLinesCanvas()
        {
            LinesCanvas = lineDrawBox.CreateGraphics();
            // Fill with white
            LinesCanvas.Clear(Color.White);
            Pen penbkg = new Pen(Color.Gray, 1);
            LinesCanvas.Clear(Color.White);
            int centerx = offsetx + drawingWidth / 2;
            int centery = offsety + drawingHeight / 2;
            LinesCanvas.DrawLine(penbkg, 0, centery, centerx * 2, centery);
            LinesCanvas.DrawLine(penbkg, centerx, 0, centerx, centery * 2);
        }
        void DrawPointWithHeightShownByColor(ref Graphics canvas, Vertex point, int penSize = 4)
        {
            float x = point.X - 1 + offsetx;
            float y = point.Y - 1 + offsety;
            Pen pen = new Pen(HeightToColour(point.Z), penSize);
            canvas.DrawEllipse(pen, x, y, 1, 1);
        }
        // draws contents of inputData
        void DrawPureData(bool onLinesCanvas = false)
        {
            if (onLinesCanvas)
            {
                PrepareLinesCanvas();
            }
            else
            {
                PrepareDataCanvas();
            }
            Graphics canvas = onLinesCanvas ? LinesCanvas : DataCanvas;

            if (!dataNormal)
            {
                NormalizeAndFix(ref inputData);
                dataNormal = true;
            }
            foreach (Vertex point in inputData)
            {
                DrawPointWithHeightShownByColor(ref canvas, point);
            }
        }
        // Delanuey triangulation
        void TriangulateData()
        {
            triangulation = new Triangulation();
            triangulation.Compute(inputData, new RectangleF(0, 0, Form1.drawingWidth, Form1.drawingHeight));
            dataTriangulated = true;
            gridCalculated = false;
        }
        void ApproximateGrid()
        {
            int gridSizeX = Form1.drawingWidth / Form1.gridStep; 
            int gridSizeY = Form1.drawingHeight / Form1.gridStep;
            if (Form1.drawingWidth % Form1.gridStep == 0)
            {
                gridSizeX++;
            }
            if (Form1.drawingHeight % Form1.gridStep == 0)
            {
                gridSizeY++;
            }
            apprGrid = new Vertex[gridSizeY, gridSizeX];
            int x = 0;
            for (int i = 0; i < gridSizeY; i++)
            {
                int y = 0;
                for (int j = 0; j < gridSizeX; j++)
                {
                    apprGrid[i, j] = new Vertex(x, y, 0);
                    y += gridStep;
                }
                x += gridStep;
            }

            Triangle currTriangle = triangulation.Facets.ElementAt(0);
            for (int i = 0; i < gridSizeY; i++) // can't use foreach because grid is modified
            {
                for (int j = 0; j < gridSizeX; j++)
                {
                    Vertex temp = apprGrid[i, j];
                    if (currTriangle.Contains(temp))
                    {
                        currTriangle.ApproxHeight(ref temp);
                        apprGrid[i, j].Z = temp.Z;// just changing the element does not compile
                    }
                    else
                    {
                        foreach (Triangle triangle in triangulation.Facets)
                        {
                            if (triangle.Contains(temp))
                            {
                                currTriangle = triangle;
                                currTriangle.ApproxHeight(ref temp);
                                apprGrid[i, j].Z = temp.Z;
                                break;
                            }
                        }
                    }
                }
            }
            gridCalculated = true;
        }
        void DrawTriangulation()
        {
            PrepareLinesCanvas();
            DrawPureData(onLinesCanvas: true);
            int minx = 0;
            int miny = 0;
            int maxx = Form1.drawingWidth;
            int maxy = Form1.drawingHeight;

            System.Drawing.Pen[] pens = {
                System.Drawing.Pens.Gray,
                System.Drawing.Pens.DarkSlateGray,
                System.Drawing.Pens.LightGray,
                System.Drawing.Pens.DarkGray};

            for (int i = 0; i < triangulation.Facets.Count; i++)
                {
                    float x = triangulation.Facets[i].OpositeOfEdge(0).X;
                    float y = triangulation.Facets[i].OpositeOfEdge(0).Y;
                    int k = i % pens.Length;
                    for (int j = 1; j < 4; j++)
                    {
                         x = x < minx ? minx : x;
                         y = y < miny ? miny : y;
                         x = x > maxx ? maxx : x;
                         y = y > maxy ? maxy : y;
                    
                         float nx = triangulation.Facets[i].OpositeOfEdge(j).X;
                         float ny = triangulation.Facets[i].OpositeOfEdge(j).Y;
                         nx = nx < minx ? minx : nx;
                         ny = ny < miny ? miny : ny;
                         nx = nx > maxx ? maxx : nx;
                         ny = ny > maxy ? maxy : ny;
                         LinesCanvas.DrawLine(pens[k], x+offsetx, y + offsety, nx + offsetx, ny + offsety);
                         x = nx;
                         y = ny;
                    }
             }
        }
        void DravApprGrid()
        {
            PrepareDataCanvas();
            DrawPureData(onLinesCanvas: false);
            int gridSizeX = apprGrid.GetLength(1);
            int gridSizeY = apprGrid.GetLength(0);
            for (int i = 0; i < gridSizeY; i++)
            {
                for (int j = 0; j < gridSizeX; j++)
                {
                    DrawPointWithHeightShownByColor(ref DataCanvas, apprGrid[i, j], 3);
                }
            }
        }
        void AddIsoline(float height)
        {
            int gridWidth = apprGrid.GetLength(1);
            int gridHeight = apprGrid.GetLength(0);
            bool[,] mask = new bool[gridHeight, gridWidth];
            for (int i = 0; i < gridHeight; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                {
                    mask[i, j] = apprGrid[i,j].Z >= height ? true : false;
                }
            }
            Pen pen = new Pen(HeightToColour(height));
            int halfstep = gridStep / 2;
            int x = 0;
            for (int i = 0; i < gridHeight - 1; i++) // -1 because we iterate over squares, not vertices
            {
                int y = 0;
                for (int j = 0; j < gridWidth - 1; j++)
                {
                    short cellIndex = 0;
                    if (mask[i, j]) cellIndex += 8;
                    if (mask[i+1, j]) cellIndex += 4;
                    if (mask[i, j+1]) cellIndex += 1;
                    if (mask[i+1, j+1]) cellIndex += 2;
                    float average;
                    switch (cellIndex)
                    {
                        case 3: case 12: LinesCanvas.DrawLine(pen, x, y+halfstep, x+gridStep, y+halfstep); break;
                        case 6: case 9: LinesCanvas.DrawLine(pen, x + halfstep, y, x + halfstep, y + gridStep); break;
                        case 1: case 14: LinesCanvas.DrawLine(pen, x, y + halfstep, x + halfstep, y + gridStep); break;
                        case 2: case 13: LinesCanvas.DrawLine(pen, x + halfstep, y + gridStep, x + gridStep, y + halfstep); break;
                        case 4: case 11: LinesCanvas.DrawLine(pen, x + halfstep, y, x + gridStep, y + halfstep); break;
                        case 7: case 8: LinesCanvas.DrawLine(pen, x, y + halfstep, x + halfstep, y); break;
                        case 5: 
                            average = (apprGrid[i, j].Z + apprGrid[i, j].Z + apprGrid[i, j].Z + apprGrid[i, j].Z) / 4;
                            if (average >= height)
                            {
                                LinesCanvas.DrawLine(pen, x, y + halfstep, x + halfstep, y);
                                LinesCanvas.DrawLine(pen, x + halfstep, y + gridStep, x + gridStep, y + halfstep);
                            }
                            else
                            {
                                LinesCanvas.DrawLine(pen, x + halfstep, y, x + gridStep, y + halfstep);
                                LinesCanvas.DrawLine(pen, x, y + halfstep, x + halfstep, y + gridStep);
                            }
                            break;
                        case 10:
                            average = (apprGrid[i, j].Z + apprGrid[i, j].Z + apprGrid[i, j].Z + apprGrid[i, j].Z) / 4;
                            if (average < height)
                            {
                                LinesCanvas.DrawLine(pen, x, y + halfstep, x + halfstep, y);
                                LinesCanvas.DrawLine(pen, x + halfstep, y + gridStep, x + gridStep, y + halfstep);
                            }
                            else
                            {
                                LinesCanvas.DrawLine(pen, x + halfstep, y, x + gridStep, y + halfstep);
                                LinesCanvas.DrawLine(pen, x, y + halfstep, x + halfstep, y + gridStep);
                            }
                            break;
                    }
                    y += gridStep;
                }
                x += gridStep;
            }
        }
        void DrawIsolines()
        {
            PrepareLinesCanvas();
            float step = 1F / (isoNum + 1);
            float isHeight = step;
            for (int i = 0; i < isoNum; i++)
            {
                AddIsoline(isHeight);
                isHeight += step;
            }
        }

        public Form1()
        {
            InitializeComponent();
            ResetData();
        }

        private void FillSampleButton_Click(object sender, EventArgs e)
        {
            ResetData();
            DrawPureData();
        }

        private void triangulateButton_Click(object sender, EventArgs e)
        {
            if (inputData is null)
            {
                ResetData();
            }
            TriangulateData();
            DrawTriangulation();
        }

        private void approxGridButton_Click(object sender, EventArgs e)
        {
            if (!dataTriangulated)
            {
                TriangulateData();
            }
            if (!gridCalculated)
            {
                ApproximateGrid();
            }
            DravApprGrid();
        }

        private void drawButton_Click(object sender, EventArgs e)
        {
            DrawIsolines();
        }

        private void generateDataButton_Click(object sender, EventArgs e)
        {
            inputData = new List<Vertex>();
            Random random = new Random();
            int dataNum = (int)(random.NextDouble() * 20) + 10;
            for (int i = 0; i < dataNum; i++)
            {
                float z = (float)random.NextDouble();
                float x = (float)(random.NextDouble()* Form1.drawingWidth);
                float y = (float)(random.NextDouble()* Form1.drawingHeight);
                inputData.Add(new Vertex(x, y, z));
            }
            dataNormal = true;
            DrawPureData();
        }
    }
}
