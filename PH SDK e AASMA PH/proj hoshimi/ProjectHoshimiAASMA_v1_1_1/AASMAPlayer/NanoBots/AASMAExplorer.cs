using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;
using PH.Map;

namespace AASMAHoshimi
{
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 30, MaxDamage = 0, DefenseDistance = 0, Constitution = 10)]
    public abstract class AASMAExplorer : PH.Common.NanoExplorer, IActionable, ICommunicable
    {
        private Utils.direction _direction;

        public AASMAExplorer()
        {
            _direction = Utils.RandomDirection();
        }

        public abstract void receiveMessage(AASMAMessage msg);

        public abstract void DoActions();

        public AASMAPlayer getAASMAFramework()
        {
            return (AASMAPlayer)this.PlayerOwner;
        }

        public Boolean frontClear()
        {
            Point p = Utils.getPointInFront(this.Location, this._direction);
            return Utils.isPointOK(PlayerOwner.Tissue, p.X, p.Y);
        }

        public void MoveForward()
        {
            Point p = Utils.getPointInFront(this.Location, this._direction);
            this.MoveTo(p);
        }

        public void TurnLeft()
        {
            this._direction = Utils.DirectionLeft(this._direction);
        }

        public void TurnRight()
        {
            this._direction = Utils.DirectionRight(this._direction);
        }

        public void RandomTurn()
        {
            if (Utils.randomValue(2) == 1)
            {
                this._direction = Utils.DirectionLeft(this._direction);
            }
            else
            {
                this._direction = Utils.DirectionRight(this._direction);
            }
        }

    }
}
