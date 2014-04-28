using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;
using PH.Map;

namespace AASMAHoshimi
{
    public abstract class AASMAAI : IActionable, ICommunicable
    {

        private Utils.direction _direction;

        protected PH.Common.NanoAI _nanoAI;

        protected int _protectorNumber = 1;
        protected int _explorerNumber = 1;
        protected int _needleNumber = 1;
        protected int _containerNumber = 1;

        public AASMAAI()
        {
            _direction = Utils.RandomDirection();
        }

        public AASMAAI(NanoAI nanoAI)
        {
            _direction = Utils.RandomDirection();
            _nanoAI = nanoAI;
        }

        public abstract void receiveMessage(AASMAMessage msg);

        public abstract void DoActions();

        public NanoAI getNanoBot()
        {
            return this._nanoAI;
        }

        public AASMAPlayer getAASMAFramework()
        {
            return (AASMAPlayer)this._nanoAI.PlayerOwner;
        }

        public Boolean frontClear()
        {
            Point p = Utils.getPointInFront(_nanoAI.Location, this._direction);
            return Utils.isPointOK(_nanoAI.PlayerOwner.Tissue, p.X, p.Y);
        }

        public void MoveForward()
        {
            Point p = Utils.getPointInFront(_nanoAI.Location, this._direction);
            _nanoAI.MoveTo(p);
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