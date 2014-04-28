/* Tutorial done by
 * 
 * Richard Clark
 * Project Hoshimi Lead Manager
 * Contact at ori@c2i.fr
 * 
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Map;
using PH.Common;

namespace Project_Hoshimi_Tutorial3
{
    public class Utils
    {
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
            if (IsPointOK(tissue, p.X, p.Y))
                return p;
            int dist = 1;
            while (true)
            {
                //up
                for (int iX = -dist; iX < dist + 1; iX++)
                    if (IsPointOK(tissue, p.X + iX, p.Y + dist))
                        return new Point(p.X + iX, p.Y + dist);
                //down
                for (int iX = -dist; iX < dist + 1; iX++)
                    if (IsPointOK(tissue, p.X + iX, p.Y - dist))
                        return new Point(p.X + iX, p.Y - dist);
                //left
                for (int iY = -dist; iY < dist + 1; iY++)
                    if (IsPointOK(tissue, p.X - dist, p.Y + iY))
                        return new Point(p.X - dist, p.Y + iY);
                //right
                for (int iY = -dist; iY < dist + 1; iY++)
                    if (IsPointOK(tissue, p.X + dist, p.Y + iY))
                        return new Point(p.X + dist, p.Y + iY);
                dist++;
            }
        }
        private static bool IsPointOK(Tissue tissue, int X, int Y)
        {
            if (!tissue.IsInMap(X, Y)) return false;
            return tissue[X, Y].AreaType == AreaEnum.HighDensity |
                tissue[X, Y].AreaType == AreaEnum.MediumDensity |
                tissue[X, Y].AreaType == AreaEnum.LowDensity;
        }
    }
}
