/* Template done by
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

namespace Project_Hoshimi_Tutorial3
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
            NothingToDo = 2,
        }

        private WhatToDoNextAction m_WhatToDoNext = WhatToDoNextAction.BuildExplorer;
        public WhatToDoNextAction AI_WhatToDoNext
        {
            get { return m_WhatToDoNext; }
            set { m_WhatToDoNext = value; }
        }

        private void UpdateInformations()
        { }

        private void AI_DoActions()
        {
            if (this.AI.State == NanoBotState.WaitingOrders)
            {
                switch (this.AI_WhatToDoNext)
                {
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
                                SelectObjectivePoints((Explorer)bot);
                                this.AI_WhatToDoNext = WhatToDoNextAction.NothingToDo;
                                break;
                            }
                        }
                        break;
                    case WhatToDoNextAction.NothingToDo:
                        break;
                }
            }
        }

        private void SelectObjectivePoints(Explorer explo)
        {
            explo.PointsToVisit.Clear();

            foreach (PH.Mission.BaseObjective objective in this.Mission.Objectives)
            {
                if (objective is PH.Mission.NavigationObjective)
                {
                    PH.Mission.NavigationObjective navObj = (PH.Mission.NavigationObjective)objective;
                    foreach (PH.Mission.NavPoint np in navObj.NavPoints)
                        explo.PointsToVisit.Enqueue(np.Location);
                    //break; //We need to consider multiple navigation objectives
                }
            }
            explo.WhatToDoNext = Explorer.WhatToDoNextAction.MoveToPoint;
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
        public List<Entity> AZNEntities { get { return m_AznEntities; } }
        public List<Entity> HoshimiEntities { get { return m_HoshimiEntities; } }

        private void myPlayer_ChooseInjectionPoint()
        {
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

            //I want to be injected at the first AZN point
            Entity entAZN = AZNEntities[0];
            this.InjectionPointWanted = new Point(entAZN.X, entAZN.Y);
        }
        #endregion

    }
}
