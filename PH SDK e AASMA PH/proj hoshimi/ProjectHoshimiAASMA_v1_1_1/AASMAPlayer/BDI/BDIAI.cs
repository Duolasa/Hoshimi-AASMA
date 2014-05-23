using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using AASMAHoshimi;
using AASMAHoshimi.Examples;
using PH.Common;

namespace AASMAHoshimi.BDI
{
  public class BDIAI : AASMAAI
  {
    public BDIAI(NanoAI nano)
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

    private void Deliberate()
    {
      if (getAASMAFramework().containersAlive() < 10)
      {
        goal = Desire.BuildContainer;
        return;
      }
      if (getAASMAFramework().protectorsAlive() < 10)
      {
        goal = Desire.BuildProtector;
        return;
      }
      if (getAASMAFramework().explorersAlive() < 10)
      {
        goal = Desire.BuildExplorer;
        return;
      }
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
        case Desire.BuildContainer:
          PlanCheckPointList.Add(new PlanCheckPoint(getNanoBot().Location, PlanCheckPoint.Actions.BuildContainer));
          planIsFinished = false;
          break;
        case Desire.BuildProtector:
          PlanCheckPointList.Add(new PlanCheckPoint(getNanoBot().Location, PlanCheckPoint.Actions.BuildProtector));
          planIsFinished = false;
          break;
        case Desire.BuildExplorer:
          PlanCheckPointList.Add(new PlanCheckPoint(getNanoBot().Location, PlanCheckPoint.Actions.BuildExplorer));
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
        case PlanCheckPoint.Actions.BuildContainer:
          if (previousInstructionIsFinished)
          {
            this._nanoAI.Build(typeof(BDIContainer), "C" + this._containerNumber++);
          }
          else
          {
            previousInstructionIsFinished = true;
          }
          break;
        case PlanCheckPoint.Actions.BuildProtector:
          if (previousInstructionIsFinished)
          {
            this._nanoAI.Build(typeof(BDIProtector), "P" + this._protectorNumber++);
          }
          else
          {
            previousInstructionIsFinished = true;
          }
          break;
        case PlanCheckPoint.Actions.BuildExplorer:
          if (previousInstructionIsFinished)
          {
            this._nanoAI.Build(typeof(BDIExplorer), "E" + this._explorerNumber++);
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
