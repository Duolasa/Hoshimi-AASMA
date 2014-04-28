using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;
using PH.Map;

namespace AASMAHoshimi
{
    [Characteristics(ContainerCapacity = 100, CollectTransfertSpeed = 0, Scan = 10, MaxDamage = 5, DefenseDistance = 10, Constitution = 25)]
    public abstract class AASMANeedle : PH.Common.NanoNeedle, IActionable, ICommunicable
    {

        public abstract void receiveMessage(AASMAMessage msg);

        public abstract void DoActions();

        public AASMAPlayer getAASMAFramework()
        {
            return (AASMAPlayer)this.PlayerOwner;
        }


    }
}
