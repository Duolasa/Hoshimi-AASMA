using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Map;
using PH.Common;

namespace GoSi
{
    public class UtilsFx
    {
        /**
         * summary: function that calculates the best Injection Point based on given Points list
         * returns: calculated Point
         **/
        public static Point PointBarycenter(List<Entity> Entities)
        {
            Point sum = Point.Empty;
            int pointsCount = 0;
            foreach (Entity entity in Entities)
            {
                Point pointLocation = new Point(entity.X, entity.Y);
                sum += (Size)pointLocation;
                pointsCount++;
            }
            Point result = new Point((int)Math.Floor((double)sum.X / (double)pointsCount),
                (int)Math.Floor((double)sum.Y / (double)pointsCount));
            return result;
        }
        public static Point PointBarycenter(List<Point> Points)
        {
            Point sum = Point.Empty;
            int pointsCount = 0;
            foreach (Point point in Points)
            {
                Point pointLocation = new Point(point.X, point.Y);
                sum += (Size)pointLocation;
                pointsCount++;
            }
            Point result = new Point((int)Math.Floor((double)sum.X / (double)pointsCount),
                (int)Math.Floor((double)sum.Y / (double)pointsCount));
            return result;
        }

    }
}
