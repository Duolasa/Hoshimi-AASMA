﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using AASMAHoshimi;
using AASMAHoshimi.Examples;
using PH.Common;
using AASMAHoshimi.BDI;   ///remove this after no longer needed

namespace AASMAHoshimi.Hybrid
{
  public class HybridAI : AASMAAI
  {
    public HybridAI(NanoAI nano)
    {
      this._nanoAI = nano;
    }

    List<Point> HoshimiPoints = new List<Point>();
    List<Point> OccupiedHoshimiPoints = new List<Point>();
    List<Point> closePierres = new List<Point>();

    List<PlanCheckPoint> PlanCheckPointList = new List<PlanCheckPoint>();
    PlanCheckPoint currentInstruction;
    bool planIsFinished = true;
    bool previousInstructionIsFinished = true;
    enum Desire
    {
      None,
      BuildContainer,
      BuildProtector,
      BuildExplorer,
      BuildNeedle
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
      List<Point> visibleFullNeedles = framework.visibleFullNeedles(getNanoBot());
      List<Point> visibleEmptyNeedles = framework.visibleEmptyNeedles(getNanoBot());
      List<Point> visibleHoshimiPoints = framework.visibleHoshimies(getNanoBot());

      foreach (Point p in visibleHoshimiPoints)
      {
        if (visibleFullNeedles.Contains(p) || visibleEmptyNeedles.Contains(p))
        {
          if (!OccupiedHoshimiPoints.Contains(p))
          {
            OccupiedHoshimiPoints.Add(p);
          }
          if (HoshimiPoints.Contains(p))
          {
            HoshimiPoints.Remove(p);
          }
        }
        else if (!HoshimiPoints.Contains(p) && !OccupiedHoshimiPoints.Contains(p))
        {
          HoshimiPoints.Add(p);
        }
      }

      List<Point> visiblePierres = framework.visiblePierres(getNanoBot());
      closePierres.Clear();
      foreach (Point p in visiblePierres)
      {
        closePierres.Add(p);
      }
    }

    private bool ReactiveLayer()
    {
      if (closePierres.Count > 0)
      {
        Point closestEnemy = Utils.getNearestPoint(getNanoBot().Location, closePierres);
        int awayVectorX = getNanoBot().Location.X - closestEnemy.X;
        int awayVectorY = getNanoBot().Location.Y - closestEnemy.Y;

        Point awayFromPierre = new Point(getNanoBot().Location.X + awayVectorX / 2, getNanoBot().Location.Y + awayVectorY / 2);
        getNanoBot().MoveTo(awayFromPierre);

        return true;
      }

      if (getAASMAFramework().protectorsAlive() < 10)
      {
        this._nanoAI.Build(typeof(HybridProtector), "P" + this._protectorNumber++);
        return true;
      }
      if (getAASMAFramework().containersAlive() < 10)
      {
        this._nanoAI.Build(typeof(HybridContainer), "C" + this._containerNumber++);
        return true;
      }
      if (getAASMAFramework().explorersAlive() < 10)
      {
        this._nanoAI.Build(typeof(HybridExplorer), "E" + this._explorerNumber++);
        return true;
      }

      return false;

    }

    private void Deliberate()
    {
      if (HoshimiPoints.Count > 0)
      {
        goal = Desire.BuildNeedle;
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
        case Desire.BuildNeedle:
          Point nearest = Utils.getNearestPoint(getNanoBot().Location, HoshimiPoints);
          PlanCheckPointList.Add(new PlanCheckPoint(nearest, PlanCheckPoint.Actions.Move));
          PlanCheckPointList.Add(new PlanCheckPoint(nearest, PlanCheckPoint.Actions.BuildNeedle));
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
            getNanoBot().MoveTo(currentInstruction.location);
            previousInstructionIsFinished = false;
          }
          else
          {
            if (getNanoBot().Location == currentInstruction.location)
            {
              previousInstructionIsFinished = true;
            }
          }
          break;

        case PlanCheckPoint.Actions.BuildNeedle:
          if (previousInstructionIsFinished)
          {
            this._nanoAI.Build(typeof(HybridNeedle), "N" + this._needleNumber++);
            OccupiedHoshimiPoints.Add(currentInstruction.location);
            HoshimiPoints.Remove(currentInstruction.location);
          }
          else
          {
            if (OccupiedHoshimiPoints.Contains(currentInstruction.location))
            {
              planIsFinished = true;
            }
            else
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
    }
  }
}
