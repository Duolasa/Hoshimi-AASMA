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
using System.Drawing;
using PH.Common;
using PH.Map;

namespace CSTut4
{

    [Characteristics(ContainerCapacity = 20,
        CollectTransfertSpeed = 5,
        Scan = 5,
        MaxDamage = 0,
        DefenseDistance = 0,
        Constitution = 20)]
    public class Collector : PH.Common.NanoCollector, IActionable
    {
        public const int SquadNumber = 2;
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

        #region IAction Members
        public void DoActions()
        {
            myPlayer player = (myPlayer)this.PlayerOwner;
            switch (this.WhatToDoNext)
            {
                case WhatToDoNextAction.CollectAZN:
                    this.InternalName = "C-Collect";
                    this.CollectFrom(this.Location, 4);
                    this.WhatToDoNext = WhatToDoNextAction.MoveToHoshimi;
                    break;
                case WhatToDoNextAction.MoveToHoshimi:
                    this.InternalName = "C-Hoshimi";
                    this.MoveTo(Utils.getNearestPoint(this.Location, player.EmptyNeedlePoints));
                    this.WhatToDoNext = WhatToDoNextAction.TransfertToNeedle;
                    break;
                case WhatToDoNextAction.TransfertToNeedle:
                    this.InternalName = "C-Transfer";
                    this.TransferTo(this.Location, 4);
                    this.WhatToDoNext = WhatToDoNextAction.MoveToAZN;
                    break;
                case WhatToDoNextAction.MoveToAZN:
                    this.InternalName = "C-AZN";
                    this.MoveTo(Utils.getNearestPoint(this.Location, player.AZNEntities));
                    this.WhatToDoNext = WhatToDoNextAction.CollectAZN;
                    break;
            }
        }
        #endregion
    }

    [Characteristics(ContainerCapacity = 100,
        CollectTransfertSpeed = 0,
        Scan = 10,
        MaxDamage = 5,
        DefenseDistance = 10,
        Constitution = 25)]
    public class Needle : PH.Common.NanoNeedle, IActionable
    {
        #region IAction Members

        public void DoActions()
        { }
        #endregion
    }

    [Characteristics(ContainerCapacity = 0,
        CollectTransfertSpeed = 0,
        Scan = 30,
        MaxDamage = 0,
        DefenseDistance = 0,
        Constitution = 10)]
    public class Explorer : PH.Common.NanoExplorer, IActionable
    {
        public enum WhatToDoNextAction
        {
            WaitingForPoints = 0,
            MoveToPoint = 1,
        }

        private WhatToDoNextAction m_WhatToDoNext = WhatToDoNextAction.WaitingForPoints;
        public WhatToDoNextAction WhatToDoNext
        {
            get { return m_WhatToDoNext; }
            set { m_WhatToDoNext = value; }
        }

        #region IAction Members
        private Queue<Point> m_PointsToVisit = new Queue<Point>();
        public Queue<Point> PointsToVisit
        {
            get { return m_PointsToVisit; }
        }
        public void DoActions()
        {
            switch (this.WhatToDoNext)
            {
                case WhatToDoNextAction.WaitingForPoints:
                    break;
                case WhatToDoNextAction.MoveToPoint:
                    MakeMove();
                    break;
            }
        }
        private void MakeMove()
        {
            if (this.PointsToVisit.Count > 0)
                this.MoveTo(PointsToVisit.Dequeue());
            else
                this.ForceAutoDestruction();
        } 
        #endregion
    }

    //[Characteristics(ContainerCapacity = 0,
    //    CollectTransfertSpeed = 0,
    //    Scan = 5,
    //    MaxDamage = 5,
    //    DefenseDistance = 12,
    //    Constitution = 25)]
    //public class Protector : PH.Common.NanoCollector, IActionable
    //{
    //    public const int NumToBuild = 2;
    //    public enum WhatToDoNextAction
    //    {
    //        MoveToNeuroController = 0,
    //        Defend = 1,
    //    }
    //    private WhatToDoNextAction m_WhatToDoNext = WhatToDoNextAction.MoveToNeuroController;
    //    public WhatToDoNextAction WhatToDoNext
    //    {
    //        get { return m_WhatToDoNext; }
    //        set { m_WhatToDoNext = value; }
    //    }

    //    #region IAction Members
    //    public void DoActions()
    //    {
    //        myPlayer player = (myPlayer)this.PlayerOwner;
    //        switch (this.WhatToDoNext)
    //        {
    //            case WhatToDoNextAction.MoveToNeuroController:
    //                this.InternalName = "P-Move";
    //                this.MoveTo(Utils.getNearestPoint(this.Location, new List<Point>(player.PierreExistingNeuroControllers)));
    //                this.WhatToDoNext = WhatToDoNextAction.Defend;
    //                break;
    //            case WhatToDoNextAction.Defend:
    //                this.InternalName = "P-Defend";
    //                this.DefendTo(this.Location, 5);
    //                this.WhatToDoNext = WhatToDoNextAction.MoveToNeuroController;
    //                break;
    //        }
    //    }
    //    #endregion
    //}
}
