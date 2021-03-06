﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;
using AASMAHoshimi;
using System.Diagnostics;


namespace AASMAHoshimi.ComHybrid
{

  [Characteristics(ContainerCapacity = 50, CollectTransfertSpeed = 5, Scan = 0, MaxDamage = 0, DefenseDistance = 0, Constitution = 15)]

  public class ComHybridContainer : AASMAContainer
  {
    List<Point> EmptyNeedleLocations = new List<Point>();
    List<Point> AZNLocations = new List<Point>();
    List<Point> Navpoints = new List<Point>();
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

      AASMAMessage msg = new AASMAMessage(this.InternalName, "GuardMe");
      msg.Tag = this.Location;
      getAASMAFramework().sendMessage(msg, "P" + (Convert.ToInt32(this.InternalName[1]) - 48 + 5));
    }

    private void CheckPerceptions()
    {
      AASMAPlayer framework = this.getAASMAFramework();
      List<Point> visibleAzn = framework.visibleAznPoints(this);
      List<Point> visiblePierres = framework.visiblePierres(this);
      List<Point> visibleHoshimi = framework.visibleHoshimies(this);
      List<Point> visibleNav = framework.visibleNavigationPoints(this);

      closePierres.Clear();

      foreach (Point n in visibleAzn)
      {
        if (!AZNLocations.Contains(n))
        {
          AZNLocations.Add(n);

          AASMAMessage msg = new AASMAMessage(this.InternalName, "AZNPoint");
          msg.Tag = n;
          getAASMAFramework().sendMessage(msg, "AI");

        }
      }

      foreach (Point n in visiblePierres)
      {
        closePierres.Add(n);
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

      if (Stock < ContainerCapacity && getAASMAFramework().overAZN(this))
      {
        collectAZN();
        return true;
      }
      if (Stock > 0 && getAASMAFramework().overEmptyNeedle(this))
      {
        transferAZN();
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
            previousInstructionIsFinished = true;

          }        
          break;

        case PlanCheckPoint.Actions.Unload:
          if (previousInstructionIsFinished)
          {
            if (this.getAASMAFramework().visibleFullNeedles(this).Contains(currentInstruction.location))
            {
              EmptyNeedleLocations.Remove(currentInstruction.location);
              previousInstructionIsFinished = true;
            }
            TransferTo(currentInstruction.location, 1);
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
      string content = msg.Content;
      Point p = (Point)msg.Tag;


      if (content.Equals("AZNPoint"))
      {
        if (!AZNLocations.Contains(p))
        {
          AZNLocations.Add(p);
        }
      }

      if (content.Equals("EmptyNeedle"))
      {
        if (!EmptyNeedleLocations.Contains(p))
        {
          EmptyNeedleLocations.Add(p);
        }
      }

    }
  }
}
