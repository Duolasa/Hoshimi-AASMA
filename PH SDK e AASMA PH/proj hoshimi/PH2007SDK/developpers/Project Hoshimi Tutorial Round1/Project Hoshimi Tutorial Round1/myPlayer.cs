/* Template done by
 * 
 * Richard Clark
 * Project Hoshimi Lead Manager
 * Contact at ori@c2i.fr
 * 
 * Changes done by
 * 
 * Paulo Gomes
 * 
*/
using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using PH.Map;
using System.Drawing;

namespace Project_Hoshimi_Tutorial_Round1
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
            NothingToDo = 0,
            BuildExplorer = 1,
            FillNavPoints = 2,
            BuildCollector = 3,
            MoveToHoshimiPoint = 4,
            BuildNeedle = 5,
        }

        private WhatToDoNextAction m_WhatToDoNext = WhatToDoNextAction.BuildCollector;
        public WhatToDoNextAction AI_WhatToDoNext
        {
            get { return m_WhatToDoNext; }
            set { m_WhatToDoNext = value; }
        }

        private int CollectorBuilded = 0;
        private List<Point> m_NeedlePoints = new List<Point>();
        private List<Point> m_EmptyNeedlePoints = new List<Point>();
        private List<Point> m_FullNeedlePoints = new List<Point>();
        public List<Point> NeedlePoints { get { return m_NeedlePoints; } }
        public List<Point> EmptyNeedlePoints { get { return m_EmptyNeedlePoints; } }
        public List<Point> FullNeedlePoints { get { return m_FullNeedlePoints; } }

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
            switch (this.AI_WhatToDoNext)
            {
                case WhatToDoNextAction.BuildCollector:
                    if (this.AI.Build(typeof(Collector)))
                    {
                        if (CollectorBuilded >= Collector.SquadNumber - 1)
                            this.AI_WhatToDoNext = WhatToDoNextAction.MoveToHoshimiPoint;
                    }
                    break;

                case WhatToDoNextAction.MoveToHoshimiPoint:
                    this.AI.MoveTo(Utils.getNearestPoint(this.AI.Location, HoshimiEntities, NeedlePoints));
                    this.AI_WhatToDoNext = WhatToDoNextAction.BuildNeedle;
                    break;

                case WhatToDoNextAction.BuildNeedle:
                    //TODO: Verify if it can build a needle
                    this.AI.Build(typeof(Needle));
                    this.AI_WhatToDoNext = WhatToDoNextAction.MoveToHoshimiPoint;
                    break;

                case WhatToDoNextAction.NothingToDo:
                    break;
            }
        }

        private void myPlayer_WhatToDoNext()
        {
            UpdateInformations();

            if (this.AI.State == NanoBotState.WaitingOrders)
                AI_DoActions();

            foreach (NanoBot bot in this.NanoBots)
                if (bot is IActionable && bot.State == NanoBotState.WaitingOrders)
                    ((IActionable)bot).DoActions();
        }

        #region Injection Point
        private List<Entity> m_AznEntities = new List<Entity>();
        private List<Entity> m_HoshimiEntities = new List<Entity>();
        private List<Point> m_NavigationPoints = new List<Point>();
        public List<Entity> AZNEntities { get { return m_AznEntities; } }
        public List<Entity> HoshimiEntities { get { return m_HoshimiEntities; } }
        public List<Point> NavigationPoints { get { return m_NavigationPoints; } }

        private void myPlayer_ChooseInjectionPoint()
        {
            #region Storing AZN and Hoshimi Points
            //storing AZN and Hoshimi Points
            foreach (Entity ent in this.Tissue.Entities)
            {
                switch (ent.EntityType)
                {
                    case EntityEnum.AZN:
                        AZNEntities.Add(ent);
                        break;
                    case EntityEnum.HoshimiPoint:
                        HoshimiEntities.Add(ent);
                        break;
                }
            }
            #endregion

            #region Storing Navigation Objectives
            //storing Navigation Objecives
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
            #endregion

            //Choosing Injection Point
            Point NavigationMiddle = Utils.getMiddlePoint(NavigationPoints.ToArray());
            this.InjectionPointWanted = Utils.getValidPoint(this.Tissue, NavigationMiddle);

        }
        #endregion
    }
}
