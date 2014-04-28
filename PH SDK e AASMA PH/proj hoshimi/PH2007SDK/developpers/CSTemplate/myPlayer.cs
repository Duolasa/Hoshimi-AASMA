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

namespace CSTemplate
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
        }
        private WhatToDoNextAction m_WhatToDoNext = WhatToDoNextAction.NothingToDo;
        public WhatToDoNextAction AI_WhatToDoNext
        {
            get { return m_WhatToDoNext; }
            set { m_WhatToDoNext = value; }
        }
        
        private void UpdateInformations()
        {}

        private void AI_DoActions()
        {
            switch (this.AI_WhatToDoNext)
            {
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

            //CHANGE THIS LINE
            this.InjectionPointWanted = new Point(100, 100);
        } 
        #endregion

    }
}
