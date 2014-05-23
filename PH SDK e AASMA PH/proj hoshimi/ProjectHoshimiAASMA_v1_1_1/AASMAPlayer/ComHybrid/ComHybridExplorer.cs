using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using PH.Common;

namespace AASMAHoshimi.ComHybrid
{
  [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 30, MaxDamage = 0, DefenseDistance = 0, Constitution = 10)]
  public class ComHybridExplorer : AASMAExplorer
  {

    private List<Point> navigationObjectives = new List<Point>();
    private List<Point> discoveredNavigationObjectives = new List<Point>();
    private List<Point> closePierres = new List<Point>();

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

        if (goal == Desire.None)
        {
          MoveRandomly();
        }
      }
    }

    private bool ReactiveLayer()
    {
      if (closePierres.Count > 0)
      {
        Point closestEnemy = Utils.getNearestPoint(Location, closePierres);
        int awayVectorX = Location.X - closestEnemy.X;
        int awayVectorY = Location.Y - closestEnemy.Y;

        Point awayFromPierre = new Point(Location.X + awayVectorX / 2, Location.Y + awayVectorY / 2);
        StopMoving();
        planIsFinished = true;
        goal = Desire.None;
        if (!MoveTo(awayFromPierre))
        {
          MoveRandomly();
        }
        return true;
      }
      if (goal == Desire.Explore)
      {
        Point navPoint = Utils.getNearestPoint(Location, navigationObjectives);
        int currentInstDistance = Utils.SquareDistance(this.Location, currentInstruction.location);
        int closestDistance = Utils.SquareDistance(this.Location, navPoint);
        if (closestDistance < currentInstDistance)
        {
          this.MoveTo(navPoint);
          currentInstruction.location = navPoint;
        }
      }
      return false;

    }

    private void CheckPerceptions()
    {
      List<Point> objectives = getAASMAFramework().visibleNavigationPoints(this);
      List<Point> visiblePierres = getAASMAFramework().visiblePierres(this);
      closePierres.Clear();

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

      foreach (Point n in visiblePierres)
      {
        closePierres.Add(n);
      }



      List<Point> visibleAzn = getAASMAFramework().visibleAznPoints(this);
      List<Point> visibleHoshimi = getAASMAFramework().visibleHoshimies(this);

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
    }

    private void Deliberate()
    {
      if (navigationObjectives.Count > 0 && Utils.randomValue(100) < 60)
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
              if (navigationObjectives.Contains(currentInstruction.location))
              {
                navigationObjectives.Remove(currentInstruction.location);
                discoveredNavigationObjectives.Add(currentInstruction.location);
              }
              previousInstructionIsFinished = true;
            }
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
      string content = msg.Content;
      Point p = (Point)msg.Tag;


      if (content.Equals("NavPoint"))
      {
        if (!navigationObjectives.Contains(p))
        {

          navigationObjectives.Add(p);
        }
      }
    }
  }
}
