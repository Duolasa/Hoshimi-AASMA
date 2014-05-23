using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.BDI
{
  [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 30, MaxDamage = 0, DefenseDistance = 0, Constitution = 10)]
  public class BDIExplorer : AASMAExplorer
  {

    private List<Point> navigationObjectives = new List<Point>();
    private List<Point> discoveredNavigationObjectives = new List<Point>();

    List<PlanCheckPoint> PlanCheckPointList = new List<PlanCheckPoint>();
    PlanCheckPoint currentInstruction;
    bool planIsFinished = true;
    bool previousInstructionIsFinished = true;
    enum Desire
    {
      None,
      Explore
    };

    Desire goal = Desire.None;

    public override void DoActions()
    {
      CheckPerceptions();
      if (planIsFinished)
      {
        Deliberate();
        Plan();
      }
      else
      {
        Execute();
      }
    }

    private void CheckPerceptions()
    {
      List<Point> objectives = getAASMAFramework().visibleNavigationPoints(this);
      if (objectives.Count > 0)
      {
        foreach (Point p in objectives)
        {
          if (!navigationObjectives.Contains(p) && !discoveredNavigationObjectives.Contains(p))
          {
            navigationObjectives.Add(p);
          }
        }
      }
    }

    private void Deliberate()
    {
      if (navigationObjectives.Count > 0)
      {
        goal = Desire.Explore;
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
        case Desire.Explore:
          Point nearest = Utils.getNearestPoint(Location, navigationObjectives);
          PlanCheckPointList.Add(new PlanCheckPoint(nearest, PlanCheckPoint.Actions.Move));
          planIsFinished = false;
          break;
        case Desire.None:
          PlanCheckPointList.Add(new PlanCheckPoint(Location, PlanCheckPoint.Actions.MoveRandom));
          planIsFinished = false;
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
        case PlanCheckPoint.Actions.Move:
          if (previousInstructionIsFinished)
          {
            MoveTo(currentInstruction.location);
            previousInstructionIsFinished = false;
          }
          else
          {
            if (this.Location == currentInstruction.location)
            {
              navigationObjectives.Remove(currentInstruction.location);
              discoveredNavigationObjectives.Add(currentInstruction.location);
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
      getAASMAFramework().logData(this, "received message from " + msg.Sender + " : " + msg.Content);
    }
  }
}
