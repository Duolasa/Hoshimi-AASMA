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

namespace CSTut2
{
    public class myPlayer : PH.Common.Player
    {
        public myPlayer() { }
        public myPlayer(string name, int ID): base(name, ID)
        {
            this.ChooseInjectionPoint += new PH.Common.ChooseInjectionPointHandler(myPlayer_ChooseInjectionPoint);
            this.WhatToDoNext += new PH.Common.WhatToDoNextHandler(myPlayer_WhatToDoNext);
        }

        bool bCollectorBuilded;
        bool bMoveToHoshimiPoint;
        bool bNeedleBuilded;
        private void AI_DoActions()
        {
            if (this.AI.State == NanoBotState.WaitingOrders)
            {
                if (!bCollectorBuilded)
                    bCollectorBuilded = this.AI.Build(typeof(Collector));
                else if (!bMoveToHoshimiPoint)
                {
                    this.AI.MoveTo(Utils.getNearestPoint(this.AI.Location, this.HoshimiEntities));
                    bMoveToHoshimiPoint = true;
                }
                else if (!bNeedleBuilded)
                    bNeedleBuilded = this.AI.Build(typeof(Needle));
            }
        }

        private void myPlayer_WhatToDoNext()
        {
            AI_DoActions();
        

            foreach (NanoBot bot in this.NanoBots)
                if (bot is IActionable && bot.State == NanoBotState.WaitingOrders)
                    ((IActionable)bot).DoActions();
        }

        private List<Entity> m_AznEntities = new List<Entity>();
        private List<Entity> m_HoshimiEntities = new List<Entity>();

        public List<Entity> AznEntities{ get { return m_AznEntities; } }
        public List<Entity> HoshimiEntities{ get { return m_HoshimiEntities; } }

        private void myPlayer_ChooseInjectionPoint()
        {
            //storing AZN and Hoshimi Points
            foreach (Entity ent in this.Tissue.Entities)
            {
                switch (ent.EntityType)
                {
                    case EntityEnum.AZN:
                        m_AznEntities.Add(ent);
                        break;
                    case EntityEnum.HoshimiPoint:
                        m_HoshimiEntities.Add(ent);
                        break;
                }
            }

            //I want to be injected at the first AZN point
            Entity entAZN = AznEntities[0];
            this.InjectionPointWanted = new Point(entAZN.X, entAZN.Y);
        }

        public override System.Drawing.Bitmap Flag
        {
            get{return Properties.Resources.rcFlag;}
        }
    }
}
