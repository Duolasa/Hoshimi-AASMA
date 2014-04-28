using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.Reactive
{
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 30, MaxDamage = 0, DefenseDistance = 0, Constitution = 10)]
    public class ReactiveExplorer : AASMAExplorer
    {
        public override void DoActions()
        {
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
