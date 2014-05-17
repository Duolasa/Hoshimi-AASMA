using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.Examples
{

  [Characteristics(ContainerCapacity = 50, CollectTransfertSpeed = 5, Scan = 0, MaxDamage = 0, DefenseDistance = 0, Constitution = 15)]
  class ReactiveContainer : AASMAContainer
  {
    public override void DoActions()
    {
      if (Stock < ContainerCapacity && getAASMAFramework().overAZN(this))
      {
        collectAZN();
      }
      if (Stock > 0 && getAASMAFramework().overEmptyNeedle(this))
      {
        transferAZN();
      }
      if (Stock < ContainerCapacity)
      {
        List<Point> aznPoints = getAASMAFramework().visibleAznPoints(this);
        if (aznPoints.Count > 0)
        {
          Point closest = Utils.getNearestPoint(Location, aznPoints);
          if (closest != Location)
          {
            MoveTo(closest);
          }
        }
      }
      if (Stock >= ContainerCapacity * 0.8f)
      {
        List<Point> emptyNeedles = getAASMAFramework().visibleEmptyNeedles(this);
        if (emptyNeedles.Count > 0)
        {
          Point closest = Utils.getNearestPoint(Location, emptyNeedles);
          if (closest != Location)
          {
            MoveTo(closest);
          }
        }
      }
      if (frontClear())
      {
        if (Utils.randomValue(100) < 80)
        {
          this.MoveForward();
        }
        else
        {
          this.RandomTurn();
        }
      }
      else
      {
        this.RandomTurn();
      }
    }

    public override void receiveMessage(AASMAMessage msg)
    {
    }
  }
}
