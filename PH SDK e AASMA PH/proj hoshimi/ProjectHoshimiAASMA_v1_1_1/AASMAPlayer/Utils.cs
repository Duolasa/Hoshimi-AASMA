using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Map;
using PH.Common;

namespace AASMAHoshimi
{
    public class Utils
    {
        private static Random _randomFactory = new Random();


        public enum direction {North, NW, West, SW, South, SE, East, NE};

        public static direction RandomDirection()
        {
            int randomDirection = Utils._randomFactory.Next(8);
            switch (randomDirection)
            {
                case 0 :
                    return direction.North;
                case 1 :
                    return direction.NW;
                case 2 :
                    return direction.West;
                case 3 :
                    return direction.SW;
                case 4 :
                    return direction.South;
                case 5 :
                    return direction.SE;
                case 6 :
                    return direction.East;
                case 7 :
                    return direction.NE;
            }
            return direction.North;
        }

        public static Point getPointInFront(Point pos, direction dir)
        {
            Point frontPoint = pos;
            switch (dir)
            {
                case direction.North:
                    frontPoint.Offset(new Point(0, 1));
                    break;
                case direction.NE:
                    frontPoint.Offset(new Point(1, 1));
                    break;
                case direction.NW:
                    frontPoint.Offset(new Point(-1, 1));
                    break;
                case direction.South:
                    frontPoint.Offset(new Point(0, -1));
                    break;
                case direction.SE:
                    frontPoint.Offset(new Point(1, -1));
                    break;
                case direction.SW:
                    frontPoint.Offset(new Point(-1, -1));
                    break;
                case direction.West:
                    frontPoint.Offset(new Point(-1, 0));
                    break;
                case direction.East:
                    frontPoint.Offset(new Point(1, 0));
                    break;
            }

            return frontPoint;
        }

        public static direction DirectionLeft(direction dir)
        {
            switch (dir)
            {
                case direction.North:
                    return direction.NW;
                case direction.NW:
                    return direction.West;
                case direction.West:
                    return direction.SW;
                case direction.SW:
                    return direction.South;
                case direction.South:
                    return direction.SE;
                case direction.SE:
                    return direction.East;
                case direction.East:
                    return direction.NE;
                case direction.NE:
                    return direction.North;
            }

            return direction.North;
        }

        public static direction DirectionRight(direction dir)
        {
            switch (dir)
            {
                case direction.North:
                    return direction.NE;
                case direction.NE:
                    return direction.East;
                case direction.East:
                    return direction.SE;
                case direction.SE:
                    return direction.South;
                case direction.South:
                    return direction.SW;
                case direction.SW:
                    return direction.West;
                case direction.West:
                    return direction.NW;
                case direction.NW:
                    return direction.North;
            }

            return direction.North;
        }

        public static int SquareDistance(Point ptA, Point ptB)
        {
            return (ptA.X - ptB.X) * (ptA.X - ptB.X) + (ptA.Y - ptB.Y) * (ptA.Y - ptB.Y);
        }
        public static int MDistance(Point p1, Point p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }
        public static Point getNearestPoint(Point currentLocation, List<Entity> entities)
        {
            Point pReturn = Point.Empty;
            int dist = 200 * 200;
            foreach (Entity ent in entities)
            {
                Point entPoint = new Point(ent.X, ent.Y);
                int entDistance = SquareDistance(entPoint, currentLocation);
                if (entDistance < dist)
                {
                    dist = entDistance;
                    pReturn = entPoint;
                }
            }
            return pReturn;
        }
        public static Point getNearestPoint(Point currentLocation, List<Point> points)
        {
            Point pReturn = Point.Empty;
            int dist = 200 * 200;
            foreach (Point p in points)
            {
                int entDistance = SquareDistance(p, currentLocation);
                if (entDistance < dist)
                {
                    dist = entDistance;
                    pReturn = p;
                }
            }
            return pReturn;
        }
        public static Point getNearestPoint(Point currentLocation, List<Entity> entities, List<Point> exceptPoints)
        {
            Point pReturn = Point.Empty;
            int dist = 200 * 200;
            foreach (Entity ent in entities)
            {
                Point entPoint = new Point(ent.X, ent.Y);
                bool bExcept = false;
                if (exceptPoints != null)
                {
                    foreach (Point exceptPoint in exceptPoints)
                    {
                        if (entPoint == exceptPoint)
                        {
                            bExcept = true;
                            break;
                        }
                    }
                }
                if (!bExcept)
                {

                    int entDistance = MDistance(entPoint, currentLocation);
                    if (entDistance < dist)
                    {
                        dist = entDistance;
                        pReturn = entPoint;
                    }
                }
            }
            return pReturn;
        }
        public static Point getNearestPoint(Point currentLocation, List<Point> availablePoints, List<Point> exceptPoints)
        {
            Point pReturn = Point.Empty;
            int dist = 200 * 200;
            foreach (Point p in availablePoints)
            {
                bool bExcept = false;
                if (exceptPoints != null)
                {
                    foreach (Point exceptPoint in exceptPoints)
                    {
                        if (p == exceptPoint)
                        {
                            bExcept = true;
                            break;
                        }
                    }
                }
                if (!bExcept)
                {

                    int entDistance = MDistance(p, currentLocation);
                    if (entDistance < dist)
                    {
                        dist = entDistance;
                        pReturn = p;
                    }
                }
            }
            return pReturn;
        }

        public static Point getMiddlePoint(Point[] points)
        {
            if (points == null || points.Length == 0) return Point.Empty;

            int sumX = 0;
            int sumY = 0;
            foreach (Point p in points)
            {
                sumX += p.X;
                sumY += p.Y;
            }

            int x = (int)Math.Round(1f * sumX / points.Length);
            int y = (int)Math.Round(1f * sumY / points.Length);

            return new Point(x, y);
        }
        public static Point getValidPoint(Tissue tissue, Point p)
        {
            if (isPointOK(tissue, p.X, p.Y))
                return p;
            int dist = 1;
            while (true)
            {
                //up
                for (int iX = -dist; iX < dist + 1; iX++)
                    if (isPointOK(tissue, p.X + iX, p.Y + dist))
                        return new Point(p.X + iX, p.Y + dist);
                //down
                for (int iX = -dist; iX < dist + 1; iX++)
                    if (isPointOK(tissue, p.X + iX, p.Y - dist))
                        return new Point(p.X + iX, p.Y - dist);
                //left
                for (int iY = -dist; iY < dist + 1; iY++)
                    if (isPointOK(tissue, p.X - dist, p.Y + iY))
                        return new Point(p.X - dist, p.Y + iY);
                //right
                for (int iY = -dist; iY < dist + 1; iY++)
                    if (isPointOK(tissue, p.X + dist, p.Y + iY))
                        return new Point(p.X + dist, p.Y + iY);
                dist++;
            }
        }
        public static bool isPointOK(Tissue tissue, int X, int Y)
        {
            if (!tissue.IsInMap(X, Y)) return false;
            return tissue[X, Y].AreaType == AreaEnum.HighDensity |
                tissue[X, Y].AreaType == AreaEnum.MediumDensity |
                tissue[X, Y].AreaType == AreaEnum.LowDensity;
        }

        public static Point randomNeighbour(Point point)
        {
            Point neighbourPoint = point;
            int randomDirection = Utils._randomFactory.Next(0, 4);

            Point[] directions ={ new Point(1,0),
                                  new Point(1,1),
                                  new Point(1,-1),
                                  new Point(0,-1),
                                  new Point(0,1),
                                  new Point(-1,-1),
                                  new Point(-1,0),
                                  new Point(-1,1)};
            neighbourPoint.Offset(directions[randomDirection]);

            return neighbourPoint;
        }

        public static Point randomPoint(List<Point> pointList)
        {
            int pointCount = pointList.Count;
            int randomPointIndex = Utils._randomFactory.Next(0, pointCount + 1);

            return pointList[randomPointIndex];
        }

        public static int randomValue(int maximum)
        {
            return Utils._randomFactory.Next(0, maximum + 1);
        }

    }
}
