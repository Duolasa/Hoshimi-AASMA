using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;

namespace AASMAHoshimi.Examples
{
    //this explorer always moves forward and turns when it cannot move. Uses the basic movement functions (turn, movefront)
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 30, MaxDamage = 0, DefenseDistance = 0, Constitution = 10)]
    public class ForwardExplorer : AASMAExplorer
    {
        public override void DoActions()
        {
            //the frontClear method returns true if the agent can move to the position in front of his current direction
            if (frontClear())
            {
                this.MoveForward();
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
