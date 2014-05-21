using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using PH.Common;
using PH.Map;
using AASMAHoshimi.Examples;
using AASMAHoshimi.Reactive;
using AASMAHoshimi.BDI;

namespace AASMAHoshimi
{
    public class AASMAPlayer : PH.Common.Player
    {
        protected int _containersAlive;
        protected int _protectorsAlive;
        protected int _explorersAlive;
        protected int _needlesAlive;
        protected AASMAAI _nanoBotAI;

        #region Needle Members
        private List<Point> _needlePoints = new List<Point>();
        private List<Point> _emptyNeedlePoints = new List<Point>();
        private List<Point> _fullNeedlePoints = new List<Point>();
       
        #endregion

        private List<Entity> _aznEntities = new List<Entity>();
        private List<Entity> _hoshimiEntities = new List<Entity>();
        private List<Point> _navigationPoints = new List<Point>();
        

        private List<Point> _hoshimiPoints = new List<Point>();

        private List<Point> _startNeuroControllers = new List<Point>();

        private List<AASMAMessage> _broadCastPool = new List<AASMAMessage>();
        private List<AASMAMessage> _msgPool = new List<AASMAMessage>();

        
        
        public virtual AASMAAI buildAIAgent(NanoAI nanoAI)
        {
            return new BDIAI(nanoAI);
        }

        #region AASMA_STUDENTS-DO_NOT_CHANGE_ANYTHING_HERE
        public AASMAPlayer() { }
        public AASMAPlayer(string name, int ID)
            : base(name, ID)
        {
            this.ChooseInjectionPoint += new PH.Common.ChooseInjectionPointHandler(playerChooseInjectionPoint);
            this.WhatToDoNext += new PH.Common.WhatToDoNextHandler(playerWhatToDoNext);

            initLogger();
            initialization();
        }

        

        public override System.Drawing.Bitmap Flag
        {
            get { return Properties.Resources.rcFlag; }
        }

        public void initialization()
        {
            _containersAlive = 0;
            _protectorsAlive = 0;
            _explorersAlive = 0;
            _needlesAlive = 0;
            base.AI.InternalName = "AI";
            _nanoBotAI = buildAIAgent(base.AI);
        }

        private void updateCommunication()
        {
            foreach (AASMAMessage message in _broadCastPool)
            {
                
                foreach (NanoBot bot in this.NanoBots)
                {
                    if (bot is ICommunicable)
                        ((ICommunicable)bot).receiveMessage(message);
                }

                _nanoBotAI.receiveMessage(message);

            }

            foreach (AASMAMessage message in _msgPool)
            {   
                if (message.Receiver.Equals("AI")) 
                {
                    _nanoBotAI.receiveMessage(message);
                }
                else
                {
                    foreach (NanoBot bot in this.NanoBots)
                    {
                        if (bot is ICommunicable && bot.InternalName.Equals(message.Receiver))
                        {
                            ((ICommunicable)bot).receiveMessage(message);
                            break;
                        }
                    }
                }
            }

            _broadCastPool.Clear();
            _msgPool.Clear();
        }

        public void broadCastMessage(AASMAMessage msg)
        {
            _broadCastPool.Add(msg);
        }

        public void sendMessage(AASMAMessage msg, String receiver)
        {
            msg.Receiver = receiver;
            _msgPool.Add(msg);
        }

        private void playerChooseInjectionPoint()
        {
           
            #region Storing AZN and Hoshimi Points
            //storing AZN and Hoshimi Points
            foreach (Entity ent in this.Tissue.Entities)
            {
                switch (ent.EntityType)
                {
                    case EntityEnum.AZN:
                        _aznEntities.Add(ent);
                        break;
                    case EntityEnum.HoshimiPoint:
                        _hoshimiEntities.Add(ent);
                        _hoshimiPoints.Add(ent.Location);
                        break;
                }
            }
            #endregion

            #region Storing Navigation Objectives
            //storing Navigation Objecives
            foreach (PH.Mission.BaseObjective obj in this.Mission.Objectives)
            {
                if (obj is PH.Mission.NavigationObjective && obj.Bonus > 0)
                {
                    PH.Mission.NavigationObjective navObj = (PH.Mission.NavigationObjective)obj;
                    foreach (PH.Mission.NavPoint np in navObj.NavPoints)
                        _navigationPoints.Add(np.Location);
                }
                else if (obj is PH.Mission.UniqueNavigationObjective && obj.Bonus > 0)
                {
                    PH.Mission.UniqueNavigationObjective uniqueNavObj = (PH.Mission.UniqueNavigationObjective)obj;
                    foreach (PH.Mission.NavPoint np in uniqueNavObj.NavPoints)
                        _navigationPoints.Add(np.Location);
                }
            }
            #endregion

            //Choosing Injection Point
            Point NavigationMiddle = Utils.getMiddlePoint(_navigationPoints.ToArray());

            //Point injectionPoint = new Point(106, 100);
            this.InjectionPointWanted = Utils.getValidPoint(this.Tissue, NavigationMiddle);

            #region storing Neurocontrollers

            for (int i = 0; i < PierreExistingNeuroControllers.Length; i++)
                _startNeuroControllers.Add(PierreExistingNeuroControllers[i]);

            #endregion

        }

        private void playerWhatToDoNext()
        {
            UpdateInformations();
            updateCommunication();

            _nanoBotAI.DoActions();

            foreach (NanoBot bot in this.NanoBots)
            {
                if (bot is IActionable)
                {
                    ((IActionable)bot).DoActions();
                }
            }
                //if (bot is IActionable && bot.State == NanoBotState.WaitingOrders)
                    

        }

        private void UpdateInformations()
        {
            _needlePoints.Clear();
            _emptyNeedlePoints.Clear();
            _fullNeedlePoints.Clear();
            _containersAlive = 0;
            _protectorsAlive = 0;
            _explorersAlive = 0;
            _needlesAlive = 0;

            foreach (NanoBot bot in this.NanoBots)
            {
                if (bot is AASMANeedle)
                {
                    _needlesAlive++;
                    _needlePoints.Add(bot.Location);
                    if (bot.Stock == 100)
                        _fullNeedlePoints.Add(bot.Location);
                    else
                        _emptyNeedlePoints.Add(bot.Location);
                }
                else if (bot is AASMAContainer)
                    _containersAlive++;
                else if (bot is AASMAProtector)
                    _protectorsAlive++;
                else if (bot is AASMAExplorer)
                    _explorersAlive++;
            }
        }

        public int containersAlive()
        {
            return this._containersAlive;
        }

        public int protectorsAlive()
        {
            return this._protectorsAlive;
        }

        public int explorersAlive()
        {
            return this._explorersAlive;
        }

        public int needlesAlive()
        {
            return this._needlesAlive;
        }

        // Returns if there is an AZN point under the robot
        public bool overAZN(NanoBot bot)
        {
            foreach (Entity azn in this._aznEntities)
            {
                if (azn.Location.Equals(bot.Location))
                    return true;
            }

            return false;
        }

        // Returns if there is a Needle under the robot
        public bool overNeedle(NanoBot bot)
        {
            foreach (Point point in this._needlePoints)
            {
                if (point.Equals(bot.Location))
                    return true;
            }

            return false;
        }

        // Returns if there is an Empty Needle under the robot
        public bool overEmptyNeedle(NanoBot bot)
        {
            foreach (Point point in this._emptyNeedlePoints)
            {
                if (point.Equals(bot.Location))
                    return true;
            }

            return false;
        }

        // Returns true if there is an Hoshimi point under the robot
        public bool overHoshimiPoint(NanoBot bot)
        {
            foreach (Entity hoshimiPoint in this._hoshimiEntities)
            {
                if (hoshimiPoint.Location.Equals(bot.Location))
                    return true;
            }

            return false;
        }

        // Verifies if a robot can move to a point
        public bool isMovablePoint(Point p)
        {
            return Utils.isPointOK(this.Tissue, p.X, p.Y);
        }

        public List<Point> visibleHoshimies(NanoBot bot)
        {
            List<Point> visibleHoshimies = new List<Point>();
            int robotScanDistance = bot.Scan + PH.Common.Utils.ScanLength;
            int squaredRobotScanDistance = robotScanDistance * robotScanDistance;

            foreach (Point hoshimiPoint in this._hoshimiPoints)
            {
                if (Utils.SquareDistance(bot.Location, hoshimiPoint) < squaredRobotScanDistance)
                    visibleHoshimies.Add(hoshimiPoint);
            }

            return visibleHoshimies;
        }

        public List<Point> visibleAznPoints(NanoBot bot)
        {
            List<Point> visibleAznPoints = new List<Point>();
            int robotScanDistance = bot.Scan + PH.Common.Utils.ScanLength;
            int squaredRobotScanDistance = robotScanDistance * robotScanDistance;

            foreach (Entity Azn in this._aznEntities)
            {
                if (Utils.SquareDistance(bot.Location, Azn.Location) < squaredRobotScanDistance)
                    visibleAznPoints.Add(Azn.Location);
            }

            return visibleAznPoints;
        }

        public List<Point> visibleNavigationPoints(NanoBot bot)
        {
            List<Point> visibleNavPoints = new List<Point>();
            int robotScanDistance = bot.Scan + PH.Common.Utils.ScanLength;
            int squaredRobotScanDistance = robotScanDistance * robotScanDistance;

            foreach (Point nav in this._navigationPoints)
            {
                if (Utils.SquareDistance(bot.Location, nav) < squaredRobotScanDistance)
                    visibleNavPoints.Add(nav);
            }

            return visibleNavPoints;
        }

        public List<Point> visibleEmptyNeedles(NanoBot bot)
        {
            List<Point> visibleEmptyNeedles = new List<Point>();
            int robotScanDistance = bot.Scan + PH.Common.Utils.ScanLength;
            int squaredRobotScanDistance = robotScanDistance * robotScanDistance;

            foreach (Point emptyNeedlePoint in this._emptyNeedlePoints)
            {
                if (Utils.SquareDistance(bot.Location, emptyNeedlePoint) < squaredRobotScanDistance)
                    visibleEmptyNeedles.Add(emptyNeedlePoint);
            }

            return visibleEmptyNeedles;
        }

        public List<Point> visibleFullNeedles(NanoBot bot)
        {
            List<Point> visibleFullNeedles = new List<Point>();


            int robotScanDistance = bot.Scan + PH.Common.Utils.ScanLength;
            int squaredRobotScanDistance = robotScanDistance * robotScanDistance;

            foreach (Point fullNeedlePoint in this._fullNeedlePoints)
            {
                if (Utils.SquareDistance(bot.Location, fullNeedlePoint) < squaredRobotScanDistance)
                    visibleFullNeedles.Add(fullNeedlePoint);
            }

            return visibleFullNeedles;
        }

        public List<Point> visiblePierres(NanoBot bot)
        {

            List<Point> visibleNeuroControllers = new List<Point>();

            if (this.OtherNanoBotsInfo == null) return visibleNeuroControllers;

            int robotScanDistance = bot.Scan + PH.Common.Utils.ScanLength;
            int squaredVisionDistance = robotScanDistance * robotScanDistance;

            foreach (NanoBotInfo botInfo in this.OtherNanoBotsInfo)
            {
                int squaredBotDistance = Utils.SquareDistance(bot.Location, botInfo.Location);
                if (squaredBotDistance < squaredVisionDistance &&
                    botInfo.PlayerID == 0)
                    visibleNeuroControllers.Add(botInfo.Location);
            }

            return visibleNeuroControllers;
        }

        #region Debugging

        public void initLogger()
        {
            Debug.Listeners.Clear();
            Debug.Listeners.Add(new DefaultTraceListener());
            Debug.Listeners.Add(new TextWriterTraceListener("log " +
              DateTime.Now.ToString().Replace(':', '.').Replace('/', '-') + ".txt"));
            Debug.AutoFlush = true;
        }

        public void logData(NanoBot bot, string text)
        {

            Debug.WriteLine("[" + this.CurrentTurn.ToString() + "] " + bot.InternalName + ": " + text);
        }

        #endregion
        #endregion

    }
}
