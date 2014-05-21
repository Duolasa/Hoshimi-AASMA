using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;


namespace AASMAHoshimi.BDI
{
  class PlanCheckPoint
  {
   public enum Actions
    {
      Move,
      Collect,
      Unload,
      Attack, 
      Run,
      Defend
    };
    public Point location;
    public Actions action;

    public PlanCheckPoint(Point p, Actions a)
    {
      location = p;
      action = a;
    }
  }
}
