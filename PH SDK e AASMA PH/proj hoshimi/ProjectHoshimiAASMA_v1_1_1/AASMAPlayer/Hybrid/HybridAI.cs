using System;
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
          OccupiedHoshimiPoints.Add(p);
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
    }

    private bool ReactiveLayer()
    {
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
        this._nanoAI.Build(typeof(BDIExplorer), "E" + this._explorerNumber++);
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
        case Desire.None:
          PlanCheckPointList.Add(new PlanCheckPoint(getNanoBot().Location, PlanCheckPoint.Actions.MoveRandom));
          break;
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

        case PlanCheckPoint.Actions.BuildNeedle:
          if (previousInstructionIsFinished)
          {
            this._nanoAI.Build(typeof(BDINeedle), "N" + this._needleNumber++);
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
