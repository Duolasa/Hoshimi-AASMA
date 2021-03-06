﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using AASMAHoshimi;
using AASMAHoshimi.Examples;
using PH.Common;
using AASMAHoshimi.BDI;   ///remove this after no longer needed
using AASMAHoshimi.Hybrid;

namespace AASMAHoshimi.ComHybrid
{
  public class ComHybridAI : AASMAAI
  {
    public ComHybridAI(NanoAI nano)
    {
      this._nanoAI = nano;
    }

    List<Point> HoshimiPoints = new List<Point>();
    List<Point> OccupiedHoshimiPoints = new List<Point>();
    List<Point> closePierres = new List<Point>();
    List<Point> AZNpoints = new List<Point>();
    List<Point> Navpoints = new List<Point>();

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

      for (int i = 1; i <= 5; i++)
      {
        AASMAMessage msg = new AASMAMessage(this.getNanoBot().InternalName, "GuardMe");
        msg.Tag = this.getNanoBot().Location;
        getAASMAFramework().sendMessage(msg, "P" + i);
      }
    }

    private void CheckPerceptions()
    {
      AASMAPlayer framework = this.getAASMAFramework();
      List<Point> visibleFullNeedles = framework.visibleFullNeedles(getNanoBot());
      List<Point> visibleEmptyNeedles = framework.visibleEmptyNeedles(getNanoBot());
      List<Point> visibleHoshimiPoints = framework.visibleHoshimies(getNanoBot());
      List<Point> visibleAZNPoints = framework.visibleAznPoints(getNanoBot());
      List<Point> visibleNavPoints = framework.visibleNavigationPoints(getNanoBot());

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

      foreach (Point p in visibleAZNPoints)
      {
        if (!AZNpoints.Contains(p))
        {
          AZNpoints.Add(p);

          for (int i = 1; i <= _containerNumber; i++)
          {
            AASMAMessage msg = new AASMAMessage(this.getNanoBot().InternalName, "AZNPoint");
            msg.Tag = p;
            getAASMAFramework().sendMessage(msg, "C" + i);
          }

        }
      }

      foreach (Point p in visibleNavPoints)
      {
        if (!visibleNavPoints.Contains(p))
        {
          visibleNavPoints.Add(p);
          AASMAMessage msg = new AASMAMessage(this.getNanoBot().InternalName, "NavPoint");
          msg.Tag = p;
          getAASMAFramework().broadCastMessage(msg);
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
        getNanoBot().StopMoving();
        planIsFinished = true;
        goal = Desire.None;
        if (!getNanoBot().MoveTo(awayFromPierre))
        {
          MoveRandomly();
        }
        return true;
      }



      if (getAASMAFramework().protectorsAlive() < 10)
      {
        this._nanoAI.Build(typeof(ComHybridProtector), "P" + this._protectorNumber++);
        return true;
      }
      if (getAASMAFramework().containersAlive() < 5)
      {
        this._nanoAI.Build(typeof(ComHybridContainer), "C" + this._containerNumber++);
        return true;
      }
      if (getAASMAFramework().explorersAlive() < 5)
      {
        this._nanoAI.Build(typeof(ComHybridExplorer), "E" + this._explorerNumber++);
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
            for (int i = 1; i <= _containerNumber; i++)
            {
              AASMAMessage msg = new AASMAMessage(this.getNanoBot().InternalName, "EmptyNeedle");
              msg.Tag = currentInstruction.location;
              getAASMAFramework().sendMessage(msg, "C" + i);
            }
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

      string content = msg.Content;
      Point p = (Point) msg.Tag;

      if (content.Equals("HoshimiPoint"))
      {
        if (!OccupiedHoshimiPoints.Contains(p) && !HoshimiPoints.Contains(p))
        {
          HoshimiPoints.Add(p);
        } 
      }

      if (content.Equals("AZNPoint"))
      {

        if (!AZNpoints.Contains(p))
        {

          AZNpoints.Add(p);

          for (int i = 1; i <= _containerNumber; i++)
          {

            AASMAMessage broadcastmsg = new AASMAMessage(this.getNanoBot().InternalName, "AZNPoint");
            broadcastmsg.Tag = p;
            getAASMAFramework().sendMessage(broadcastmsg, "C" + i);
          }
        }
      }

      if (content.Equals("NavPoint"))
      {
        if (!Navpoints.Contains(p))
        {
          Navpoints.Add(p);


          for (int i = 1; i <= _explorerNumber; i++)
          {
            AASMAMessage broadcastmsg = new AASMAMessage(this.getNanoBot().InternalName, "NavPoint");
            broadcastmsg.Tag = p;
            getAASMAFramework().sendMessage(broadcastmsg, "E" + i);
          }

        }
      }
    }
  }
}
