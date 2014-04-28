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
using System.Drawing;
using PH.Common;
using PH.Map;

namespace Project_Hoshimi_Tutorial_Round1
{

    [Characteristics(ContainerCapacity = 20, CollectTransfertSpeed = 5, Scan = 5, MaxDamage = 0, DefenseDistance = 0, Constitution = 20)]
    public class Collector : PH.Common.NanoCollector, IActionable
    {
        public const int SquadNumber = 2;

        #region IAction Members
        public enum WhatToDoNextAction
        {
            MoveToAZN = 0,
            CollectAZN = 1,
            MoveToHoshimi = 2,
            TransfertToNeedle = 3,
        }

        private WhatToDoNextAction m_WhatToDoNext = WhatToDoNextAction.MoveToAZN;
        public WhatToDoNextAction WhatToDoNext
        {
            get { return m_WhatToDoNext; }
            set { m_WhatToDoNext = value; }
        }

        public void DoActions()
        {
            switch (this.WhatToDoNext)
            {
                case WhatToDoNextAction.MoveToAZN:
                    this.MoveTo(Utils.getNearestPoint(this.Location, ((myPlayer)this.PlayerOwner).AZNEntities));
                    this.WhatToDoNext = WhatToDoNextAction.CollectAZN;
                    break;

                case WhatToDoNextAction.CollectAZN:
                    //TODO: 4 should be replaced by a number dependent on the current number of squad members
                    this.CollectFrom(this.Location, 4);
                    this.WhatToDoNext = WhatToDoNextAction.MoveToHoshimi;
                    break;

                case WhatToDoNextAction.MoveToHoshimi:
                    this.MoveTo(Utils.getNearestPoint(this.Location, ((myPlayer)this.PlayerOwner).EmptyNeedlePoints));
                    this.WhatToDoNext = WhatToDoNextAction.TransfertToNeedle;
                    break;

                case WhatToDoNextAction.TransfertToNeedle:
                    //TODO: we need to recheck if this is a empty needle
                    //TODO: 4 should be replaced by a number dependent on the current number of squad members
                    this.TransferTo(this.Location, 4);
                    this.WhatToDoNext = WhatToDoNextAction.MoveToAZN;
                    break;
            }
        }
        #endregion
    }

    [Characteristics(ContainerCapacity = 100, CollectTransfertSpeed = 0, Scan = 10, MaxDamage = 5, DefenseDistance = 10, Constitution = 25)]
    public class Needle : PH.Common.NanoNeedle, IActionable
    {
        public const int SquadNumber = 0;
        #region IAction Members
        public void DoActions()
        { }
        #endregion
    }

    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 30, MaxDamage = 0, DefenseDistance = 0, Constitution = 10)]
    public class Explorer : PH.Common.NanoExplorer, IActionable
    {
        public const int SquadNumber = 0;
        #region IAction Members
        public void DoActions()
        { }
        #endregion
    }

    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 5, MaxDamage = 5, DefenseDistance = 12, Constitution = 25)]
    public class Protector : PH.Common.NanoCollector, IActionable
    {
        public const int SquadNumber = 0;
        #region IAction Members
        public void DoActions()
        { }
        #endregion
    }
}
