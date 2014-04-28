using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;
using PH.Map;

namespace AASMAHoshimi
{
    [Characteristics(ContainerCapacity = 50, CollectTransfertSpeed = 5, Scan = 0, MaxDamage = 0, DefenseDistance = 0, Constitution = 15)]
    public abstract class AASMAContainer : PH.Common.NanoContainer, IActionable, ICommunicable
    {
        private Utils.direction _direction;

        public AASMAContainer(){

            _direction = Utils.RandomDirection();
        }

        public abstract void DoActions();

        public abstract void receiveMessage(AASMAMessage msg);


        public AASMAPlayer getAASMAFramework()
        {
            return (AASMAPlayer)this.PlayerOwner;
        }

        public Boolean frontClear()
        {
            Point p = Utils.getPointInFront(this.Location,this._direction);
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

        public bool collectAZN()
        {
            return base.CollectFrom(this.Location, 1);
        }

        public bool transferAZN()
        {
            return base.TransferTo(this.Location, 1);
        }
    }
}
