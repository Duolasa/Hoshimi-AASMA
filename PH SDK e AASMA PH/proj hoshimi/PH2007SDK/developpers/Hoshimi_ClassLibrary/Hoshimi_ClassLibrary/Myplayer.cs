using System.Drawing;
using PH.Common;
using PH.Map;

namespace Hoshimi_ClassLibrary
{
    public class myPlayer : PH.Common.Player
    {
        public myPlayer() { }

        public myPlayer(string name, int ID)
            : base(name, ID)
        {

            this.ChooseInjectionPoint += new PH.Common.ChooseInjectionPointHandler(myPlayer_ChooseInjectionPoint);

            this.WhatToDoNext += new PH.Common.WhatToDoNextHandler(myPlayer_WhatToDoNext);

        }

        public override System.Drawing.Bitmap Flag
        {
            get { return Properties.Resources.iconT; }
        }

        private void myPlayer_ChooseInjectionPoint()
        {

            this.InjectionPointWanted = new Point(120, 50);

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



    }
}
