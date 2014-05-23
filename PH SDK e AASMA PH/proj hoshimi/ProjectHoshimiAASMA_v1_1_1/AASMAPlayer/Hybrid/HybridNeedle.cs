using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using AASMAHoshimi;
using PH.Common;
using System.Diagnostics;

namespace AASMAHoshimi.Hybrid
{
  [Characteristics(ContainerCapacity = 100, CollectTransfertSpeed = 0, Scan = 10, MaxDamage = 5, DefenseDistance = 10, Constitution = 25)]
  public class HybridNeedle : AASMANeedle
  {
    List<PlanCheckPoint> PlanCheckPointList = new List<PlanCheckPoint>();
    PlanCheckPoint currentInstruction;
    bool planIsFinished = true;
    bool previousInstructionIsFinished = true;
    List<Point> enemies = new List<Point>();
    Desire goal = Desire.None;

    enum Desire
    {
      None,
      Attack,
      Defend
    };


    public override void DoActions()
    {
      CheckPerceptions();
      if (!ReactiveLayer())
      {
        if (planIsFinished)
        {
          Deliberate();
          Plan();
          Execute();
        }
        else
        {
          Execute();
        }
      }
    }

    private bool ReactiveLayer()
    {
      if (enemies.Count > 0)
      {
        Point enemy = Utils.getNearestPoint(Location, enemies);

          this.DefendTo(enemy, 1);
        
        return true;
      }
      return false;
    }

    private void CheckPerceptions()
    {
      List<Point> visibleEnemies = this.getAASMAFramework().visiblePierres(this);
      enemies.Clear();
      foreach (Point n in visibleEnemies)
      {
        enemies.Add(n);
      }
    }

    private void Deliberate()
    {
      goal = Desire.None;
    }

    private void Plan()
    {
    }

    private void Execute()
    {
      if (previousInstructionIsFinished)
      {
        if (PlanCheckPointList.Count == 0)
        {
          planIsFinished = true;
          return;
        }
        currentInstruction = PlanCheckPointList[0];
        PlanCheckPointList.RemoveAt(0);
      }


      /*switch (currentInstruction.action)
      {
      }*/
    }

    public override void receiveMessage(AASMAMessage msg)
    {
    }
  }
}
