using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.Reactive
{
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 5, MaxDamage = 5, DefenseDistance = 12, Constitution = 25)]
    class ReactiveProtector : AASMAProtector
    {

        public override void  DoActions()
        {
            List<Point> enemies = getAASMAFramework().visiblePierres(this);
            Point enemyPosition;
            if (enemies.Count > 0)
            {
                enemyPosition = enemies[0];
                this.DefendTo(enemyPosition,1);
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
