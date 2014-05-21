using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using AASMAHoshimi;
using AASMAHoshimi.Examples;
using PH.Common;

namespace AASMAHoshimi.BDI
{
  public class BDIAI : AASMAAI
  {
    public BDIAI(NanoAI nano)
    {
      this._nanoAI = nano;
    }

    public override void DoActions()
    {

 /* 
   if (getAASMAFramework().containersAlive() < 10)
      {
        this._nanoAI.Build(typeof(BDIContainer), "C" + this._containerNumber++);
      } 
   */
      if (getAASMAFramework().protectorsAlive() < 30)
      {
        this._nanoAI.Build(typeof(BDIProtector), "P" + this._protectorNumber++);
      }
   
    }

    public override void receiveMessage(AASMAMessage msg)
    {
    }
  }
}
