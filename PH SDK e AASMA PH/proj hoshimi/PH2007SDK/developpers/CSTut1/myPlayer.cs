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

namespace CSTut1
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
        private void myPlayer_WhatToDoNext()
        {
            if (this.AI.State == NanoBotState.WaitingOrders)
            {
                if (!bCollectorBuilded)
                {
                    if (this.AI.Build(typeof(Collector)))
                        bCollectorBuilded = true;
                }
                else
                {
                    this.AI.MoveTo(new Point(100, 50));
                }
            }
        }

        private void myPlayer_ChooseInjectionPoint()
        {
            Entity ent = this.Tissue.get_EntitiesByType(EntityEnum.AZN)[0];
            this.InjectionPointWanted = new Point(120,50);
        }

        public override System.Drawing.Bitmap Flag
        {
            get{return Properties.Resources.rcFlag;}
        }
    }
}
