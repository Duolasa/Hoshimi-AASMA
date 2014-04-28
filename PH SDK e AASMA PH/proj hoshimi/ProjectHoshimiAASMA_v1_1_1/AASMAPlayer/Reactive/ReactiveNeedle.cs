using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using AASMAHoshimi;
using PH.Common;

namespace AASMAHoshimi.Reactive
{
  [Characteristics(ContainerCapacity = 100, CollectTransfertSpeed = 0, Scan = 10, MaxDamage = 5, DefenseDistance = 10, Constitution = 25)]
  public class ReactiveNeedle : AASMANeedle
  {
    public override void DoActions()
    {
      List<Point> enemies = getAASMAFramework().visiblePierres(this);
      Point enemyPosition;
      if (enemies.Count > 0)
      {
        enemyPosition = enemies[Utils.randomValue(enemies.Count)];
        int sqrDefenceDistance = this.DefenseDistance * this.DefenseDistance;
        int sqrDistanceToEnemy = Utils.SquareDistance(this.Location, enemyPosition);
        if (sqrDistanceToEnemy <= sqrDefenceDistance)
        {
          this.DefendTo(enemyPosition, 1);
        }
      } 
    }

    public override void receiveMessage(AASMAMessage msg)
    {
    }
  }
}
