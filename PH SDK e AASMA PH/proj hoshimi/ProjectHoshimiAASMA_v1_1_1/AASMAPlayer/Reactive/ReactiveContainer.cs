using System;
using System.Collections.Generic;
using System.Text;
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
