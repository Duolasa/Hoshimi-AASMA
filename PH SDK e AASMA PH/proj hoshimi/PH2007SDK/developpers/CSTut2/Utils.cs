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

namespace CSTut2
{
    public class Utils
    {
        public static int Distance(Point ptA, Point ptB)
        {
            return (ptA.X - ptB.X) * (ptA.X - ptB.X) + (ptA.Y - ptB.Y) * (ptA.Y - ptB.Y);
        }

        public static Point getNearestPoint(Point currentLocation, List<Entity> entities)
        {
            Point pReturn = Point.Empty;
            int dist = 200 * 200;
            foreach (Entity ent in entities)
            {
                Point entPoint = new Point(ent.X, ent.Y);
                int entDistance = Distance(entPoint, currentLocation);
                if (entDistance < dist)
                {
                    dist = entDistance;
                    pReturn = entPoint;
                }
            }
            return pReturn;
        }


        public static int MDistance(Point p1, Point p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
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
    }
}
