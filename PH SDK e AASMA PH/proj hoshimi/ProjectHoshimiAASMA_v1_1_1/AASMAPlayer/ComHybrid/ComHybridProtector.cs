using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.ComHybrid
{
  [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 5, MaxDamage = 5, DefenseDistance = 12, Constitution = 25)]
  class ComHybridProtector : AASMAProtector
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
          // Deliberate();
          //  Plan();
        }
        else
        {
          //  Execute();
        }

        if (goal == Desire.None)
        {
          MoveRandomly();
        }
      }
    }

    private void CheckPerceptions()
    {
      AASMAPlayer framework = this.getAASMAFramework();
      List<Point> visibleEnemies = framework.visiblePierres(this);
      List<Point> visibleAzn = framework.visibleAznPoints(this);
      List<Point> visibleHoshimi = framework.visibleHoshimies(this);
      List<Point> visibleNav = framework.visibleNavigationPoints(this);

      enemies.Clear();
      foreach (Point n in visibleEnemies)
      {
        enemies.Add(n);
      }


      foreach (Point n in visibleAzn)
      {
          AASMAMessage msg = new AASMAMessage(this.InternalName, "AZNPoint");
          msg.Tag = n;
          getAASMAFramework().sendMessage(msg, "AI");
       
      }

      foreach (Point n in visibleHoshimi)
      {
        AASMAMessage msg = new AASMAMessage(this.InternalName, "HoshimiPoint");
        msg.Tag = n;
        getAASMAFramework().sendMessage(msg, "AI");
      }

      foreach (Point n in visibleNav)
      {
        AASMAMessage msg = new AASMAMessage(this.InternalName, "NavPoint");
        msg.Tag = n;
        getAASMAFramework().sendMessage(msg, "AI");
      }

    }

    private bool ReactiveLayer()
    {
      if (enemies.Count > 0)
      {
        Point closestEnemy = Utils.getNearestPoint(Location, enemies);
        if (Utils.MDistance(Location, closestEnemy) > DefenseDistance)
        {
          int vectorX = closestEnemy.X - Location.X;
          int vectorY = closestEnemy.Y - Location.Y;

          Point CloserToPierre = new Point(Location.X + vectorX / 10, Location.Y + vectorY / 10);
          if (!MoveTo(CloserToPierre))
          {
            return false;
          }
        }
        else
        {
          DefendTo(closestEnemy, 1);
        }
        return true;
      }

      return false;
    }
    private void Deliberate()
    {
      goal = Desire.None;

    }

    private void Plan()
    {
      PlanCheckPointList.Clear();
      previousInstructionIsFinished = true;
      switch (goal)
      {
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
