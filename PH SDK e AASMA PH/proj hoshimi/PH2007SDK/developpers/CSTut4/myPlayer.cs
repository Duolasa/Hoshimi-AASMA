/* Tutorial done by
 * 
 * Richard Clark
 * Project Hoshimi Lead Manager
 * Contact at ori@c2i.fr
 * 
*/
using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using PH.Map;
using System.Drawing;

namespace CSTut4
{
    public class myPlayer : PH.Common.Player
    {
        #region Initializations
        public myPlayer() { }
        public myPlayer(string name, int ID)
            : base(name, ID)
        {
            this.ChooseInjectionPoint += new PH.Common.ChooseInjectionPointHandler(myPlayer_ChooseInjectionPoint);
            this.WhatToDoNext += new PH.Common.WhatToDoNextHandler(myPlayer_WhatToDoNext);
        }
        public override System.Drawing.Bitmap Flag
        {
            get { return Properties.Resources.rcFlag; }
        } 
        #endregion

        public enum WhatToDoNextAction
        {
            BuildExplorer = 0,
            FillNavPoints = 1,
            BuildCollector = 2,
            MoveToHoshimiPoint = 4,
            BuildNeedle = 5,
            NothingToDo = 6,
        }
        private WhatToDoNextAction m_WhatToDoNext = WhatToDoNextAction.BuildExplorer;
        public WhatToDoNextAction AI_WhatToDoNext
        {
            get { return m_WhatToDoNext; }
            set { m_WhatToDoNext = value; }
        }
        
        private List<Point> m_NeedlePoints = new List<Point>();
        public List<Point> NeedlePoints{ get { return m_NeedlePoints; } }

        private List<Point> m_EmptyNeedlePoints = new List<Point>();
        public List<Point> EmptyNeedlePoints { get { return m_EmptyNeedlePoints; } }
        private List<Point> m_FullNeedlePoints = new List<Point>();
        public List<Point> FullNeedlePoints { get { return m_FullNeedlePoints; } }

        private const int NeedlesToBuild = 2;
        private int CollectorBuilded = 0;
        private void UpdateInformations()
        {
            NeedlePoints.Clear();
            EmptyNeedlePoints.Clear();
            FullNeedlePoints.Clear();
            CollectorBuilded = 0;
            foreach (NanoBot bot in this.NanoBots)
            {
                if (bot is Needle)
                {
                    NeedlePoints.Add(bot.Location);
                    if (bot.Stock == 100)
                        FullNeedlePoints.Add(bot.Location);
                    else
                        EmptyNeedlePoints.Add(bot.Location);
                }
                else if (bot is Collector)
                    CollectorBuilded++;
            }
        }

        private void AI_DoActions()
        {
            if (this.AI.State != NanoBotState.WaitingOrders) return;

            switch (this.AI_WhatToDoNext)
            {
                case WhatToDoNextAction.BuildCollector:
                    if (this.AI.Build(typeof(Collector)))
                    {
                       if (CollectorBuilded >= Collector.SquadNumber - 1)
                           this.AI_WhatToDoNext = WhatToDoNextAction.MoveToHoshimiPoint;
                    }
                    break; 
                case WhatToDoNextAction.BuildExplorer:
                    if (this.AI.Build(typeof(Explorer)))
                        this.AI_WhatToDoNext = WhatToDoNextAction.FillNavPoints;
                    break;
                case WhatToDoNextAction.FillNavPoints:
                    foreach (NanoBot bot in this.NanoBots)
                    {
                        if (bot is Explorer &&
                            ((Explorer)bot).WhatToDoNext == Explorer.WhatToDoNextAction.WaitingForPoints)
                        {
                            NEW_SelectObjectivePoints((Explorer)bot);
                            this.AI_WhatToDoNext = WhatToDoNextAction.BuildCollector;
                            break;
                        }
                    }
                    break;
                case WhatToDoNextAction.MoveToHoshimiPoint:
                    this.AI.MoveTo(Utils.getNearestPoint(this.AI.Location, HoshimiEntities, NeedlePoints));
                    this.AI_WhatToDoNext = WhatToDoNextAction.BuildNeedle;
                    break;
                case WhatToDoNextAction.BuildNeedle:
                    if (this.AI.Build(typeof(Needle)))
                    {
                        if (NeedlePoints.Count >= NeedlesToBuild - 1)
                            this.AI_WhatToDoNext = WhatToDoNextAction.NothingToDo;
                        else
                            this.AI_WhatToDoNext = WhatToDoNextAction.MoveToHoshimiPoint;
                    }
                    break;
                case WhatToDoNextAction.NothingToDo:
                    break;
            }
        }

        private void myPlayer_WhatToDoNext()
        {
            UpdateInformations();

            AI_DoActions();

            foreach (NanoBot bot in this.NanoBots)
                if (bot is IActionable && bot.State == NanoBotState.WaitingOrders)
                    ((IActionable)bot).DoActions();
        }

        private void NEW_SelectObjectivePoints(Explorer explo)
        {        
            explo.PointsToVisit.Clear();
            foreach (PH.Mission.BaseObjective objective in this.Mission.Objectives)
            {
                if (objective is PH.Mission.NavigationObjective)
                {
                    PH.Mission.NavigationObjective navObj = (PH.Mission.NavigationObjective)objective;
                    foreach (PH.Mission.NavPoint np in navObj.NavPoints)
                        explo.PointsToVisit.Enqueue(np.Location);
                    break;
                }
            }
            explo.WhatToDoNext = Explorer.WhatToDoNextAction.MoveToPoint;
        }

        #region Injection Point
        private List<Entity> m_AznEntities = new List<Entity>();
        private List<Entity> m_HoshimiEntities = new List<Entity>();
        private List<Point> m_AznPoints = new List<Point>();
        private List<Point> m_HoshimiPoints = new List<Point>();
        private List<Point> m_NavigationPoints = new List<Point>();

        public List<Entity> AZNEntities { get { return m_AznEntities; } }
        public List<Entity> HoshimiEntities { get { return m_HoshimiEntities; } }
        public List<Point> AZNPoints { get { return m_AznPoints; } }
        public List<Point> HoshimiPoints { get { return m_HoshimiPoints; } }
        public List<Point> NavigationPoints { get { return m_NavigationPoints; } }
        
        private Point AZNMiddle = Point.Empty;
        private Point HoshimiMiddle = Point.Empty;
        private Point NavigationMiddle = Point.Empty;

        private void myPlayer_ChooseInjectionPoint()
        {
            //storing AZN and Hoshimi Points
            foreach (Entity ent in this.Tissue.Entities)
            {
                switch (ent.EntityType)
                {
                    case EntityEnum.AZN:
                        AZNEntities.Add(ent);
                        AZNPoints.Add(ent.Location);
                        break;
                    case EntityEnum.HoshimiPoint:
                        HoshimiEntities.Add(ent);
                        HoshimiPoints.Add(ent.Location);
                        break;
                }
            }
            //storing navigationPoints
            foreach (PH.Mission.BaseObjective obj in this.Mission.Objectives)
            {
                if (obj is PH.Mission.NavigationObjective && obj.Bonus > 0)
                {
                    PH.Mission.NavigationObjective navObj = (PH.Mission.NavigationObjective)obj;
                    foreach (PH.Mission.NavPoint np in navObj.NavPoints)
                        NavigationPoints.Add(np.Location);
                }
                else if (obj is PH.Mission.UniqueNavigationObjective && obj.Bonus > 0)
                {
                    PH.Mission.UniqueNavigationObjective uniqueNavObj = (PH.Mission.UniqueNavigationObjective)obj;
                    foreach (PH.Mission.NavPoint np in uniqueNavObj.NavPoints)
                        NavigationPoints.Add(np.Location);
                }
            }

            HoshimiMiddle = Utils.getMiddlePoint(HoshimiPoints.ToArray());

            AZNMiddle = Utils.getMiddlePoint(AZNPoints.ToArray());
            NavigationMiddle = Utils.getMiddlePoint(NavigationPoints.ToArray());

            Point p = Utils.getNearestPoint(NavigationMiddle, AZNEntities);
            this.InjectionPointWanted = Utils.getValidPoint(this.Tissue, NavigationMiddle);
        } 
        #endregion

    }
}
