using System;
using System.Collections.Generic;
using System.Linq;


namespace Revanche.Map
{
    /// <summary>
    /// Class Representation of a (double, double) Point.
    /// </summary>
    public sealed class Point : IEquatable<Point>
    {
        public readonly double mX;
        public readonly double mY;

        public Point(double x, double y)
        {
            this.mX = x;
            this.mY = y;
        }

        public bool Equals(Point other)
        {
            double tolerance = 0.00001f;
            if (other == null)
            {
                return false;
            }

            var xDiff = Math.Abs(this.mX - other.mX);
            var yDiff = Math.Abs(this.mY - other.mY);
            var xSame = (xDiff <= tolerance || xDiff <= Math.Max(Math.Abs(this.mX), Math.Abs(other.mX)) * tolerance);
            var ySame = (yDiff <= tolerance || yDiff <= Math.Max(Math.Abs(this.mY), Math.Abs(other.mY)) * tolerance);

            return (xSame && ySame);
        }

        /// <summary>
        /// Calculates the distance to another Point object.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>double: distance from called point to other</returns>
        public double Dist(Point other)
        {
            const int i2 = 2;
            return Math.Sqrt(Math.Pow(this.mX - other.mX, i2) + Math.Pow(this.mY - other.mY, i2));
        }

        public bool IsInList(List<Point> points)
        {
            foreach (var point in points)
            {
                if (this.Equals(point))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsInArray(Point[] points)
        {
            foreach (var point in points)
            {
                if (this.Equals(point))
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Class Representation of a (point, point) Edge.
    /// </summary>
    public sealed class Edge : IEquatable<Edge>
    {
        public readonly Point mP;
        public readonly Point mQ;

        internal Edge(Point p, Point q)
        {
            this.mP = p;
            this.mQ = q;
        }

        public bool Equals(Edge other)
        {
            if (other != null)
            {
                return ((this.mP.Equals(other.mP) && (this.mQ.Equals(other.mQ))) ||
                        (this.mQ.Equals(other.mP) && (this.mP.Equals(other.mQ))));
            }

            return false;
        }

        public bool IsInList(List<Edge> edges)
        {
            foreach (var edge in edges)
            {
                if (new Edge(this.mP, this.mQ).Equals(edge) || (this.Equals(edge)))
                {
                    return true;
                }
            }
            return false;
        }

        internal bool IsInArray(IEnumerable<Edge> edges)
        {
            foreach (var edge in edges)
            {
                if ((this.Equals(edge)) || (this.Equals(new Edge(edge.mQ, edge.mP))))
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Class Representation of a Triangle defined through its corners.
    /// </summary>
    public sealed class Triangle : IEquatable<Triangle>
    {
        internal readonly Point mX;
        internal readonly Point mY;
        internal readonly Point mZ;
        internal readonly Point[] mPoints;
        internal readonly Edge[] mEdges;
        private Point mCircumCircle;

        public Triangle(Point x, Point y, Point z)
        {
            var ytoZ = new Edge(x, y);
            var xtoZ = new Edge(x, z);
            var xtoY = new Edge(y, z);
            this.mX = x;
            this.mY = y;
            this.mZ = z;
            this.mPoints = new[] { x, y, z };
            this.mEdges = new[] { xtoY, xtoZ, ytoZ };
            this.mCircumCircle = this.CalculateCircumCircle();
        }

        public bool Equals(Triangle other)
        {
            var output = true;
            if (other == null)
            {
                return true;
            }

            foreach (var point in other.mPoints)
            {
                if (!point.IsInArray(this.mPoints))
                {
                    output = false;
                }
            }

            return output;
        }

        /// <summary>
        /// Checks wether or not a permutation of the triangle is in a given list of triangles.
        /// </summary>
        /// <param name="triangles"></param>
        /// <returns>bool: is the triangle in the list?</returns>
        public bool IsInList(List<Triangle> triangles)
        {
            List<Triangle> alternative = new List<Triangle>
            {
                new(this.mX, this.mY, this.mZ),
                new(this.mX, this.mZ, this.mY),
                new(this.mY, this.mX, this.mZ),
                new(this.mY, this.mZ, this.mX),
                new(this.mZ, this.mX, this.mY),
                new(this.mZ, this.mY, this.mX)
            };
            foreach (var triangle in alternative)
            {
                foreach (var triangle2 in triangles)
                {
                    if (triangle.Equals(triangle2))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Calculates the midpoint of the Circumcircle of the triangle
        /// </summary>
        /// <returns>point: (cx, cy)</returns>
        private Point CalculateCircumCircle()
        {
            const double tolerance = 0.00001f;

            var p1 = this.mX;
            var p2 = this.mY;
            var p3 = this.mZ;


            var diff13 = Math.Abs(this.mY.mY - this.mZ.mY);
            var diff23 = Math.Abs(this.mX.mY - this.mZ.mY);

            var swap13 = diff13 <= tolerance || diff13 <= Math.Max(Math.Abs(this.mY.mY), Math.Abs(this.mZ.mY)) * tolerance;
            var swap23 = diff23 <= tolerance || diff23 <= Math.Max(Math.Abs(this.mY.mY), Math.Abs(this.mZ.mY)) * tolerance;

            if (swap23) // == true
            {
                p2 = this.mZ;
                p3 = this.mY;
            }

            if (swap13) // == true
            {
                p1 = this.mZ;
                p3 = this.mX;
            }

            var n1 = -(p2.mX - p3.mX) / (p2.mY - p3.mY);
            var n2 = -(p1.mX - p3.mX) / (p1.mY - p3.mY);
            var x1 = 1 / (2 * (n1 - n2));
            var x2 = ((Math.Pow(p1.mX, 2) - Math.Pow(p3.mX, 2)) / (p1.mY - p3.mY));
            var x3 = ((Math.Pow(p2.mX, 2) - Math.Pow(p3.mX, 2)) / (p2.mY - p3.mY));
            var x = x1 * (x2 - x3 + p1.mY - p2.mY);
            var y = (n1 * ((2 * x) - p2.mX - p3.mX) + p2.mY + p3.mY) / 2;
            return new Point(x, y);
        }

        /// <summary>
        /// Checks if the point is in the Circumcircle of a triangle.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal bool InCircle(Point p)
        {
            this.mCircumCircle = this.CalculateCircumCircle();
            return this.mCircumCircle.Dist(p) <= this.mCircumCircle.Dist(this.mX);
        }
    }

    /// <summary>
    /// Class Representation of a MapGraph (points and edges)
    /// </summary>
    public sealed class Graph
    {
        private readonly List<Point> mPoints; //intentionally left as internal
        internal readonly List<Edge> mEdges;
        private double mMinX;
        private double mMinY;
        private double mMaxX;
        private double mMaxY;

        internal Graph(List<Point> points, List<Edge> edges)
        {
            this.mPoints = points;
            this.mEdges = edges;
            this.mMinX = Double.PositiveInfinity;
            this.mMinY = Double.PositiveInfinity;
            this.mMaxX = Double.NegativeInfinity;
            this.mMaxY = Double.NegativeInfinity;
        }

        /// <summary>
        /// Calculates the maximal and minimal x and y coordinates of the points of the graph.
        /// </summary>
        private void CalculateBounds()
        {
            foreach (var point in this.mPoints)
            {
                if (point.mX < this.mMinX) { this.mMinX = point.mX; }

                if (point.mY < this.mMinY) { this.mMinY = point.mY; }

                if (point.mX > this.mMaxX) { this.mMaxX = point.mX; }

                if (point.mY > this.mMaxY) { this.mMaxY = point.mY; }
            }
        }

        /// <summary>
        /// Returns a triangle guaranteed to contain all points of the graph in its area.
        /// </summary>
        /// <returns>Triangle: triangle containing all points of the graph in its area.</returns>
        private Triangle CalculateSuperTriangle()
        {
            this.CalculateBounds();
            var a = new Point(this.mMinX - 1, this.mMinY - 1);
            var b = new Point(this.mMinX + 2 * (this.mMaxX - this.mMinX) + 2, this.mMinY - 1);
            var c = new Point(this.mMinX - 1, this.mMinY + 2 * (this.mMaxY - this.mMinY) + 2);
            return new Triangle(a, b, c);
        }

        /// <summary>
        /// Calculates a triangle-mesh for a given MapGraph using the Bowyer-Watson algorithm.
        /// </summary>
        /// <returns>List of Triangles (the polygon mesh)</returns>
        internal List<Triangle> BowyerWatson()
        {
            var triangulation = new List<Triangle>();
            var change = new List<Triangle>();
            var outList = new List<Triangle>();
            var superTriangle = this.CalculateSuperTriangle();
            triangulation.Add(superTriangle);
            var superPoints = new List<Point>() { superTriangle.mX, superTriangle.mY, superTriangle.mZ };


            foreach (var point in this.mPoints)
            {
                var badTriangles = triangulation.Where(triangle => triangle.InCircle(point)).ToList();

                var polygon = new List<Edge>();
                foreach (var triangle in badTriangles)
                {
                    foreach (var edge in triangle.mEdges)
                    {
                        var addEdge = true;
                        foreach (var unused in badTriangles.Where(triangle2 => !triangle.Equals(triangle2)).Where(triangle2 => edge.IsInArray(triangle2.mEdges)))
                        {
                            addEdge = false;
                        }

                        if (addEdge) // == true
                        {
                            polygon.Add(edge);
                        }
                    }
                }

                foreach (var triangle in triangulation)
                {
                    if (!(triangle.IsInList(badTriangles)))
                    {
                        change.Add(triangle);
                    }
                }

                triangulation = change;
                change = new List<Triangle>();

                triangulation.AddRange(polygon.Select(edge => new Triangle(edge.mP, edge.mQ, point)));
            }
            foreach (var triangle in triangulation)
            {
                var keep = true;
                foreach (var unused in superPoints.Where(spp => spp.IsInArray(triangle.mPoints)))
                {
                    keep = false;
                }
                if (keep) // == true
                { // should there be problems, add logic that checks for multiples of a triangle here
                    outList.Add(triangle);
                }
            }

            return outList;
        }

        /// <summary>
        /// Generates a MapGraph from the list of triangles. Contains each point and edge only once.
        /// </summary>
        /// <param name="bwList"></param>
        /// <returns>MapGraph: MapGraph instead of list of triangles</returns>
        internal Graph BowyerWatsonListToGraph(List<Triangle> bwList)
        {
            var points = new List<Point>();
            var edges = new List<Edge>();
            foreach (var triangle in bwList)
            {
                foreach (var point in triangle.mPoints)
                {
                    if (!(point.IsInList(points)))
                    {
                        points.Add(point);
                    }
                }

                foreach (var edge in triangle.mEdges)
                {
                    if (!(edge.IsInList(edges)))
                    {
                        edges.Add(edge);
                    }
                }
            }
            return new Graph(points, edges);
        }

        /// <summary>
        /// Calculates the adjacency matrix weighted by the distances.
        /// </summary>
        /// <returns>weighted adjacency matrix</returns>
        private double[,] ToAdjacencyMatrix()
        {
            var outMatrix = new double[this.mPoints.Count, this.mPoints.Count];
            var indexA = 0;
            foreach (var pointA in this.mPoints)
            {
                var indexB = 0;
                foreach (var pointB in this.mPoints)
                {
                    if ((new Edge(pointA, pointB).IsInList(mEdges)) || (new Edge(pointB, pointA).IsInList(mEdges)))
                    {
                        outMatrix[indexA, indexB] = pointA.Dist(pointB);
                    }
                    else
                    {
                        outMatrix[indexA, indexB] = 0;
                    }

                    indexB++;
                }
                indexA++;
            }
            return outMatrix;
        }

        /// <summary>
        /// Implementation of Prims algorithm.
        /// </summary>
        /// <returns>List of edges forming a minimum spanning tree</returns>
        private List<Edge> PrimUnconstrained()
        {
            var outEdges = new List<Edge>();
            var graphMatrix = this.ToAdjacencyMatrix();
            var selected = new List<bool>();
            for (var a = 0; a < graphMatrix.GetLength(0); a++)
            {
                selected.Add(false);
            }

            selected[0] = true;
            var nrEdges = 0;
            while (nrEdges < this.mPoints.Count - 1)
            {
                var minimum = double.PositiveInfinity;
                var x = 0;
                var y = 0;
                for (var i = 0; i < this.mPoints.Count; i++)
                {
                    if (!selected[i]) // == true
                    {
                        continue;
                    }

                    for (var j = 0; j < this.mPoints.Count; j++)
                    {
                        if ((selected[j]) || graphMatrix[i, j] == 0)
                        {
                            continue;
                        }

                        if (minimum <= graphMatrix[i, j])
                        {
                            continue;
                        }

                        minimum = graphMatrix[i, j];
                        x = i;
                        y = j;
                    }
                }
                outEdges.Add(new Edge(this.mPoints[x], this.mPoints[y]));
                selected[y] = true;
                nrEdges++;
            }

            return outEdges;
        }

        /// <summary>
        /// Builds a graph containing at least all edges of the MST.
        /// </summary>
        /// <param name="addPercent"></param>
        /// <param name="random"></param>
        /// <returns>Connected MapGraph</returns>
        public Graph ExtendedSpanningTree(int addPercent, Random random)
        {
            const int i100 = 100;
            const int i101 = 101;
            if (addPercent is < 0 or > i100)
            {
                return null;
            }

            var spanEdges = this.PrimUnconstrained();
            foreach (var edge in this.mEdges.Where(edge => !edge.IsInList(spanEdges)).Where(_ => random.Next(i101) <= addPercent))
            {
                spanEdges.Add(edge);
            }
            return new Graph(this.mPoints, spanEdges);
        }


    }

    /// <summary>
    /// Class Representation of a floor. This is basically an edge.
    /// </summary>
    public sealed class Floor
    {
        internal readonly Point mStart;
        internal readonly Point mEnd;

        internal Floor(Point start, Point end)
        {
            this.mStart = start;
            this.mEnd = end;
        }
    }

    /// <summary>
    /// Class Representation of a room.
    /// </summary>
    public sealed class Room
    {
        internal readonly Point mMid;
        internal readonly int mWidth;
        internal readonly int mHeight;
        internal readonly Point mTopLeftCorner;
        private readonly Point[] mCorners;

        internal Room(Point mid, int width, int height)
        {
            this.mMid = mid;
            this.mWidth = width;
            this.mHeight = height;
            const float f2 = 2f;
            this.mTopLeftCorner = new Point(mid.mX - Math.Floor(width / f2), mid.mY - Math.Floor(height / f2));
            var topRightCorner = new Point(mid.mX + Math.Floor(width / f2), mid.mY - Math.Floor(height / f2));
            var bottomLeftCorner = new Point(mid.mX - Math.Floor(width / f2), mid.mY + Math.Floor(height / f2));
            var bottomRightCorner = new Point(mid.mX + Math.Floor(width / f2), mid.mY + Math.Floor(height / f2));
            this.mCorners = new[] { mTopLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner };
        }

        internal bool Overlaps(Room other)
        {
            var minDist = other.mCorners.Select(point => this.mMid.Dist(point)).Prepend(double.PositiveInfinity).Min();
            const double d5 = 5;
            const int i2 = 2;
            return (this.mMid.Dist(this.mTopLeftCorner) + d5 * Math.Sqrt(i2) >= minDist);
        }

        /// <summary>
        /// Generates only horizontal and vertical Floors between rooms.
        /// Should only be used by a Dungeon.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        internal void FloorConnect(Room other, List<Floor> horizontal, List<Floor> vertical)
        {
            const double tolerance = 0.00001f;
            if (Math.Abs(this.mMid.mY - other.mMid.mY) <= tolerance || Math.Abs(this.mMid.mY - other.mMid.mY) <= Math.Max(Math.Abs(this.mMid.mY), Math.Abs(other.mMid.mY)) * tolerance)
            {
                horizontal.Add(new Floor(this.mMid, other.mMid));
            }
            else
            {
                if (Math.Abs(this.mMid.mX - other.mMid.mX) <= tolerance || Math.Abs(this.mMid.mX - other.mMid.mX) <= Math.Max(Math.Abs(this.mMid.mX), Math.Abs(other.mMid.mX)) * tolerance)
                {
                    vertical.Add(new Floor(this.mMid, other.mMid));
                }
                else
                {
                    horizontal.Add(new Floor(this.mMid, new Point(other.mMid.mX, this.mMid.mY)));
                    vertical.Add(new Floor(new Point(other.mMid.mX, this.mMid.mY), other.mMid));
                }
            }
        }
    }

    /// <summary>
    /// Class Representation of a Dungeon. Uses All of the above classes.
    /// </summary>
    public sealed class Dungeon
    {
        private const int I25 = 25;
        private const int I30 = 30;
        private const int I45 = 45;
        private const int I50 = 50;
        private const int I90 = 90;
        private const int I100 = 100;
        private const int I140 = 140;
        private const int I150 = 150;
        private const int I170 = 170;
        private const int I200 = 200;
        private const int I1000 = 1000;
        private const int I1970 = 1970;

        private Graph mMesh;
        public Grid mTiles;
        internal List<Room> mRooms;
        private List<Floor> mHorizontalFloors;
        private List<Floor> mVerticalFloors;
        private Random mRandom;

        public Dungeon()
        {
        }

        public Dungeon(int numberrooms, int dungeonx, int dungeony, int xmin, int xmax, int ymin, int ymax, int roomwidth, int roomheight, int addpercent)
        {
            this.mRooms = new List<Room>();
            this.mHorizontalFloors = new List<Floor>();
            this.mVerticalFloors = new List<Floor>();
            this.mRandom = new Random((int)((DateTime.UtcNow.Subtract(new DateTime(I1970, 1, 1))).TotalMilliseconds % int.MaxValue));
            CreateRoomList(numberrooms, xmin, xmax, ymin, ymax, roomwidth, roomheight, this.mRandom);
            CreateMesh(addpercent);
            CreateFloors();
            this.mTiles = new Grid(dungeonx, dungeony);
            this.FillGround();
            this.FillWalls();
            this.Interior();

        }
        public Dungeon CreateTechDemoDungeon()
        {
            var dungeon = new Dungeon();
            dungeon.mRooms = new List<Room>() { CreateRoomAt(I30, I100, I45, I45), CreateRoomAt(I150, I100, I45, I45), CreateRoomAt(I90, I100, I45, I45), CreateRoomAt(I90, I140, I25, I45) };
            dungeon.mHorizontalFloors = new List<Floor>() { new Floor(new Point(I30, I100), new Point(I90, I100)), new Floor(new Point(I100, I100), new Point(I150, I170)), new Floor(new Point(I90, I140), new Point(I30, I140)), new Floor(new Point(I90, I140), new Point(I150, I140)) };
            dungeon.mVerticalFloors = new List<Floor>() { new Floor(new Point(I90, I100), new Point(I90, I140)), new Floor(new Point(I30, I140), new Point(I30, I100)), new Floor(new Point(I150, I140), new Point(I150, I100)) };
            dungeon.mRandom = new Random((int)((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds % int.MaxValue));
            dungeon.mTiles = new Grid(I200, I200);
            dungeon.FillGround();
            dungeon.FillWalls();
            dungeon.TechDemoInterior();
            return dungeon;
        }

        public Dungeon CreateAiDungeon()
        {
            var dungeon = new Dungeon();
            dungeon.mRooms = new List<Room>() { CreateRoomAt(I25, I25, I25, I25) };
            dungeon.mHorizontalFloors = new List<Floor>();
            dungeon.mVerticalFloors = new List<Floor>();
            dungeon.mRandom = new Random((int)((DateTime.UtcNow.Subtract(new DateTime(I1970, 1, 1))).TotalMilliseconds % int.MaxValue));
            dungeon.mTiles = new Grid(I50, I50);
            dungeon.FillGround();
            dungeon.FillWalls();
            dungeon.AiRoomInterior();
            return dungeon;
        }

        private Room CreateRandRoomAtRandomPosition(int xMin, int xMax, int yMin, int yMax, int width, int height, Random random)
        {
            var m = new Point(random.Next(xMin, xMax), random.Next(yMin, yMax));
            return new Room(m, width, height);
        }

        private Room CreateRoomAt(int midX, int midY, int height, int width) // I forbid everyone (except me) from using this function! - Benni
        {
            return new Room(new Point(midX, midY), width, height);
        }

        private void CreateRoomList(int n, int xMin, int xMax, int yMin, int yMax, int width, int height, Random random)
        {
            var alternative = 0;
            while ((this.mRooms.Count < n) && (alternative < I1000))
            {
                var addRoom = true;
                var newRoom = this.CreateRandRoomAtRandomPosition(xMin, xMax, yMin, yMax, width, height, random);
                foreach (var room in this.mRooms)
                {
                    if (newRoom.Overlaps(room))
                    {
                        addRoom = false;
                        break;
                    }
                }
                if (addRoom) // == true
                {
                    this.mRooms.Add(newRoom);
                }
                alternative++;
            }
        }

        private void CreateMesh(int addPercent)
        {
            var pointList = this.mRooms.Select(room => room.mMid).ToList();

            var bw = new Graph(pointList, new List<Edge>());
            var triList = bw.BowyerWatson();
            var m = bw.BowyerWatsonListToGraph(triList);
            this.mMesh = m.ExtendedSpanningTree(addPercent, this.mRandom);
        }

        private void CreateFloors()
        {
            foreach (var edge in this.mMesh.mEdges)
            {
                Room r1 = null;
                Room r2 = null;
                foreach (var room in this.mRooms)
                {
                    if (room.mMid.Equals(edge.mP))
                    {
                        r1 = room;
                    }
                    if (room.mMid.Equals(edge.mQ))
                    {
                        r2 = room;
                    }
                }

                if ((r1 != null) && (r2 != null))
                {
                    r1.FloorConnect(r2, this.mHorizontalFloors, this.mVerticalFloors);
                }
            }
        }

        private void FillRooms()
        {
            foreach (var room in this.mRooms)
            {
                for (var a = 0; a < room.mHeight; a++)
                {
                    for (var b = 0; b < room.mWidth; b++)
                    {
                        this.mTiles.SetCell((int)(room.mTopLeftCorner.mX + b), (int)(room.mTopLeftCorner.mY + a), CellType.GroundCell);
                    }
                }
            }
        }

        private void FillFloors()
        {
            foreach (var vFloor in this.mVerticalFloors)
            {
                var p = vFloor.mStart;
                if (vFloor.mStart.mY > vFloor.mEnd.mY)
                {
                    p = vFloor.mEnd;
                }
                for (var a = -1; a <= 1; a++)
                {
                    const int i2 = 2;
                    for (var b = -1; b < Math.Abs(vFloor.mStart.mY - vFloor.mEnd.mY) + i2; b++) // was - a+1
                    {
                        this.mTiles.SetCell((int)(p.mX - a), (int)(b + p.mY), CellType.GroundCell);
                    }
                }
            }

            foreach (var hFloor in this.mHorizontalFloors)
            {
                var p = hFloor.mStart;
                if (hFloor.mStart.mX > hFloor.mEnd.mX)
                {
                    p = hFloor.mEnd;
                }

                for (var a = -1; a <= 1; a++)
                {
                    const int i2 = 2;
                    for (var b = -1; b < Math.Abs(hFloor.mStart.mX - hFloor.mEnd.mX) + i2; b++) //was -a+1
                    {
                        this.mTiles.SetCell((int)(b + p.mX), (int)(p.mY - a), CellType.GroundCell);
                    }
                }
            }
        }

        /// <summary>
        /// Fills all the ground Cells in the grid-matrix.
        /// </summary>
        private void FillGround()
        {
            FillFloors();
            FillRooms();
        }

        /// <summary>
        /// Fills all the Wall Cells in the grid-matrix.
        /// </summary>
        private void FillWalls()
        {
            for (var y = 1; y < this.mTiles.mHeight - 1; y++)
            {
                for (var x = 1; x < this.mTiles.mWidth - 1; x++)
                {
                    if (this.mTiles.GetCellType(x, y) != CellType.EmptyCell)
                    {
                        continue;
                    }

                    const int i2 = 2;
                    for (var a = -1; a < i2; a++)
                    {
                        for (var b = -1; b < i2; b++)
                        {
                            if (this.mTiles.GetCellType(x + b, y + a) == CellType.GroundCell)
                            {
                                this.mTiles.SetCell(x, y, CellType.WallCell);
                            }
                        }
                    }
                }
            }
        }

        private void RoomInterior(Room room, Grid roomGrid)
        {
            for (var y = 0; y < roomGrid.mHeight; y++)
            {
                for (var x = 0; x < roomGrid.mWidth; x++)
                {
                    if (roomGrid.GetCellType(x, y) != CellType.EmptyCell)
                    {
                        this.mTiles.SetCell((int)(room.mTopLeftCorner.mX + x), (int)(room.mTopLeftCorner.mY + y), roomGrid.GetCellType(x, y));
                    }
                }
            }
        }


        private void Interior()
        {
            List<RoomType> roomTypes = new List<RoomType>() { RoomType.PillarRoom, RoomType.EmptyRoom, RoomType.EmptyRoom, RoomType.PillarRoom, RoomType.LabyrinthRoom, RoomType.LayerRoom };

            RoomInterior(this.mRooms[0], new RoomTemplate(RoomType.SpawnRoom).mRoomGrid);
            RoomInterior(this.mRooms[1], new RoomTemplate(RoomType.BossRoom).mRoomGrid);

            for (var i = 2; i < this.mRooms.Count; i++)
            {
                var curType = roomTypes[i % roomTypes.Count];
                RoomInterior(this.mRooms[i], new RoomTemplate(curType).mRoomGrid);
            }
        }

        private void TechDemoInterior()
        {
            const int i3 = 3;
            const int i2 = 2;
            RoomInterior(this.mRooms[i2], new RoomTemplate(RoomType.TechDemoBig).mRoomGrid);
            RoomInterior(this.mRooms[i3], new RoomTemplate(RoomType.TechDemoRoom).mRoomGrid);
        }

        private void AiRoomInterior()
        {
            RoomInterior(this.mRooms[0], new RoomTemplate(RoomType.AiRoom).mRoomGrid);
        }

    }

}