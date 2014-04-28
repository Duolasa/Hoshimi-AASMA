using System;
using System.Collections.Generic;
using System.Text;
using AASMAHoshimi;
using AASMAHoshimi.Examples;
using PH.Common;

namespace AASMAHoshimi.Reactive
{
  public class ReactiveAI : AASMAAI
  {
    public ReactiveAI(NanoAI nano)
    {
      this._nanoAI = nano;
    }

    public override void DoActions()
    {
      if (getAASMAFramework().protectorsAlive() < 5)
      {
        this._nanoAI.Build(typeof(ReactiveProtector), "P" + this._protectorNumber++);
      }
      if (getAASMAFramework().containersAlive() < 5)
      {
        this._nanoAI.Build(typeof(ReactiveContainer), "C" + this._containerNumber++);
      }
      if (getAASMAFramework().explorersAlive() < 5)
      {
        this._nanoAI.Build(typeof(ReactiveExplorer), "E" + this._explorerNumber++);
      }
      if (getAASMAFramework().overHoshimiPoint(getNanoBot())) 
      {
        this._nanoAI.Build(typeof(ReactiveNeedle), "N" + this._needleNumber++);
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
