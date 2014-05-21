using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.BDI
{
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 30, MaxDamage = 0, DefenseDistance = 0, Constitution = 10)]
    public class BDIExplorer : AASMAExplorer
    {

      private List<Point> navigationObjectives = new List<Point>();

        public override void DoActions()
        {
          List<Point> objectives = getAASMAFramework().visibleNavigationPoints(this);
          if (objectives.Count > 0)
          {
            foreach (Point p in objectives)
            {
              if (!navigationObjectives.Contains(p))
              {
                navigationObjectives.Add(p);
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
            getAASMAFramework().logData(this, "received message from " + msg.Sender + " : " + msg.Content);
        }
    }
}
