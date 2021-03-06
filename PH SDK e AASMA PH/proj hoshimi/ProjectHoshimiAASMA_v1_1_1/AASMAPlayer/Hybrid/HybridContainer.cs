﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;
using AASMAHoshimi;

namespace AASMAHoshimi.Hybrid
{

  [Characteristics(ContainerCapacity = 50, CollectTransfertSpeed = 5, Scan = 0, MaxDamage = 0, DefenseDistance = 0, Constitution = 15)]

  class HybridContainer : AASMAContainer
  {
    List<Point> EmptyNeedleLocations = new List<Point>();
    List<Point> AZNLocations = new List<Point>();
    List<Point> closePierres = new List<Point>();
    List<PlanCheckPoint> PlanCheckPointList = new List<PlanCheckPoint>();
    PlanCheckPoint currentInstruction;
    bool planIsFinished = true;
    bool previousInstructionIsFinished = true;
    enum Desire
    {
      None,
      Unload,
      Collect
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

    private void CheckPerceptions()
    {
      AASMAPlayer framework = this.getAASMAFramework();
      List<Point> visibleNeedles = framework.visibleEmptyNeedles(this);
      List<Point> visibleAzn = framework.visibleAznPoints(this);
      List<Point> visiblePierres = framework.visiblePierres(this);
      closePierres.Clear();


      foreach (Point n in visibleNeedles)
      {
        if (!EmptyNeedleLocations.Contains(n))
        {
          EmptyNeedleLocations.Add(n);
        }
      }

      foreach (Point n in visibleAzn)
      {
        if (!AZNLocations.Contains(n))
        {
          AZNLocations.Add(n);
        }
      }

      foreach (Point n in visiblePierres)
      {
        closePierres.Add(n);      
      }

    }

    private bool ReactiveLayer()
    {
      if (closePierres.Count > 0)
      {
        Point closestEnemy = Utils.getNearestPoint(Location, closePierres);
        int awayVectorX =  Location.X - closestEnemy.X;
        int awayVectorY =  Location.Y - closestEnemy.Y;

        Point awayFromPierre = new Point(Location.X + awayVectorX/2, Location.Y + awayVectorY/2);
        this.MoveTo(awayFromPierre);

        return true;
      }

      return false;
    }

    private void Deliberate()
    {
      if (AZNLocations.Count > 0 && Stock < ContainerCapacity)
      {
        goal = Desire.Collect;
        return;
      }

      if (Stock > 0 && EmptyNeedleLocations.Count > 0)
      {
        goal = Desire.Unload;
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
        case Desire.Collect:
          Point closestAZN = Utils.getNearestPoint(Location, AZNLocations);
          PlanCheckPointList.Add(new PlanCheckPoint(closestAZN, PlanCheckPoint.Actions.Move));
          PlanCheckPointList.Add(new PlanCheckPoint(closestAZN, PlanCheckPoint.Actions.Collect));
          planIsFinished = false;
          break;
        case Desire.Unload:
          Point closestNeedle = Utils.getNearestPoint(Location, EmptyNeedleLocations);
          PlanCheckPointList.Add(new PlanCheckPoint(closestNeedle, PlanCheckPoint.Actions.Move));
          PlanCheckPointList.Add(new PlanCheckPoint(closestNeedle, PlanCheckPoint.Actions.Unload));
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
              previousInstructionIsFinished = true;
            }
          }
          break;

        case PlanCheckPoint.Actions.Collect:
          if (previousInstructionIsFinished)
          {
            CollectFrom(currentInstruction.location, 1);
          }
          else
          {
            if (Stock == ContainerCapacity)
            {
              previousInstructionIsFinished = true;
            }
          }
          break;

        case PlanCheckPoint.Actions.Unload:
          if (previousInstructionIsFinished)
          {
            TransferTo(currentInstruction.location, 1);
          }
          else
          {
            if (this.Location == currentInstruction.location)
            {
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
      getAASMAFramework().logData(this, "received message from " + msg.Sender + " : " + msg.Content);
    }
  }
}
