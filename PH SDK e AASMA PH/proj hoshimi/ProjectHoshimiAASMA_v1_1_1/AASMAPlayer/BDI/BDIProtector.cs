using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.BDI
{
  [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 5, MaxDamage = 5, DefenseDistance = 12, Constitution = 25)]
  class BDIProtector : AASMAProtector
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

      if (goal == Desire.None)
      {
        MoveRandomly();
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
        goal = Desire.Attack;
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
        case Desire.Attack:
        Point enemy = Utils.getNearestPoint(Location, enemies);
        PlanCheckPointList.Add(new PlanCheckPoint(enemy, PlanCheckPoint.Actions.Attack));
        planIsFinished = false;
        break;
        case Desire.None:
        PlanCheckPointList.Add(new PlanCheckPoint(Location, PlanCheckPoint.Actions.MoveRandom));
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
        case PlanCheckPoint.Actions.Attack:
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
              DefendTo(visibleEnemies[0], 1);
            }
            else
            {
              previousInstructionIsFinished = true;

            }
          }
          break;
        case PlanCheckPoint.Actions.MoveRandom:
          if (previousInstructionIsFinished)
          {
            MoveRandomly();
          }
          else
          {
            previousInstructionIsFinished = true;
          }
          break;
      }
    }

    private void MoveRandomly()
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
    }
  }
}
