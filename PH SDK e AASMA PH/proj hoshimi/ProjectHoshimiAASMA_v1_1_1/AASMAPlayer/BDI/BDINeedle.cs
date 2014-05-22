using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.BDI
{
  [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 5, MaxDamage = 5, DefenseDistance = 12, Constitution = 25)]
  class BDINeedle : AASMANeedle
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

    private void CheckPerceptions()
    {
      AASMAPlayer framework = this.getAASMAFramework();
      List<Point> visibleEnemies = framework.visiblePierres(this);
      enemies.Clear();
      foreach (Point n in visibleEnemies)
      {
        if (!enemies.Contains(n))
        {
          enemies.Add(n);
        }
      }
    }

    private void Deliberate()
    {
      if (enemies.Count > 0)
      {
        goal = Desire.Defend;
        return;
      }
      goal = Desire.None;
    }

    private void Plan()
    {
      PlanCheckPointList.Clear();
      previousInstructionIsFinished = true;
      switch (goal)
      {
        case Desire.Defend:
          Point enemy = Utils.getNearestPoint(Location, enemies);
          int sqrDefenceDistance = this.DefenseDistance * this.DefenseDistance;
          int sqrDistanceToEnemy = Utils.SquareDistance(this.Location, enemy);
          if (sqrDistanceToEnemy <= sqrDefenceDistance)
          {
            PlanCheckPointList.Add(new PlanCheckPoint(enemy, PlanCheckPoint.Actions.Defend));
            planIsFinished = false;
          }
          break;
      }
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


      switch (currentInstruction.action)
      {
        case PlanCheckPoint.Actions.Defend:
          if (previousInstructionIsFinished)
          {
            DefendTo(currentInstruction.location, 1);
            previousInstructionIsFinished = false;
          }
          else
          {
            List<Point> visibleEnemies = this.getAASMAFramework().visiblePierres(this);
            if (visibleEnemies.Count > 0)
            {
              int sqrDefenceDistance = this.DefenseDistance * this.DefenseDistance;
              int sqrDistanceToEnemy = Utils.SquareDistance(this.Location, visibleEnemies[0]);
              if (sqrDistanceToEnemy <= sqrDefenceDistance)
              {
                this.DefendTo(visibleEnemies[0], 1);
              }
            }
            else
            {
              previousInstructionIsFinished = true;
            }
          }
          break;
      }
    }

    public override void receiveMessage(AASMAMessage msg)
    {
    }
  }
}
