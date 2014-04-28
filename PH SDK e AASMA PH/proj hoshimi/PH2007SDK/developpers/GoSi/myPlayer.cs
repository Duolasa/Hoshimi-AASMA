/* Template done by
 * 
 * Richard Clark
 * Project Hoshimi Lead Manager
 * Contact at ori@c2i.fr
 * 
 * Changes done by
 * 
 * Bruno Silva & Paulo Gomes
 * 
*/
/**
 * IDEA: ver quais os nanos q se conseguem defender. Os q não podem precisam de escolta.
 *       O AI precisa sempre de escolta. Pelo menos uns 5 pra defesa. Ele não pode morrer.
 * TODO: criar exploradores aleatórios e respectivos Protectors para limpar o tecido dos
 *       inimigos (como no tutorial do intermediate)
 *      como é que se percebe quando um Nanobot é destruído???
 **/
using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using PH.Map;
using System.Drawing;
using PH.Mission; /* hummm */

namespace GoSi
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
            BuildExplorer = 1,
            FillNavPoints = 2,
            BuildCollector = 3,
            MoveToHoshimiPoint = 4,
            BuildNeedle = 5,
            NothingToDo = 0,
            /* TODO: considerar a construção de outros Nanos*/
        }

        // Vamos primeiro criar os exploradores. Dependerá do tipo de nível?
        private WhatToDoNextAction m_WhatToDoNext = WhatToDoNextAction.BuildExplorer;
        public WhatToDoNextAction AI_WhatToDoNext
        {
            get { return m_WhatToDoNext; }
            set { m_WhatToDoNext = value; }
        }

        #region InfoNeedles
        /**
         * summary: information about Needles
         **/
        private List<Point> m_NeedlePoints = new List<Point>();
        private List<Point> m_EmptyNeedlePoints = new List<Point>();
        private List<Point> m_FullNeedlePoints = new List<Point>();

        public List<Point> NeedlePoints { get { return m_NeedlePoints; } }
        public List<Point> EmptyNeedlePoints { get { return m_EmptyNeedlePoints; } }
        public List<Point> FullNeedlePoints { get { return m_FullNeedlePoints; } }

        private int m_CollectorBuilded;
        private int m_ExplorerBuilded;

        public int CollectorBuilded
        {
            get { return m_CollectorBuilded; }
            set { m_CollectorBuilded = value; }
        }

        public int ExplorerBuilded
        {
            get { return m_ExplorerBuilded; }
            set { m_ExplorerBuilded = value; }
        }
        #endregion

        private void UpdateInformations()
        {
            NeedlePoints.Clear();
            EmptyNeedlePoints.Clear();
            FullNeedlePoints.Clear();
            CollectorBuilded = 0;
            ExplorerBuilded = 0;
            /* TODO: ver os pontos para os quais nenhum Explorer está a navegar */
            foreach (NanoBot bot in this.NanoBots)
            {
                if (bot is Needle)
                {
                    NeedlePoints.Add(bot.Location);
                    if (bot.Stock == 100)
                        FullNeedlePoints.Add(bot.Location);
                    else
                        EmptyNeedlePoints.Add(bot.Location);
                }
                else if (bot is Collector)
                    CollectorBuilded++;
                else if (bot is Explorer)
                    ExplorerBuilded++;
            }
        }

        /*TODO: ao fazer o TODO anterior,possivelmente esta função salta 
         *     PG : Os exploradores são precisos para duas coisas
         *          - explorar o mapa e recolher informação;(que informação?)
         *          - visitar os pontos de navegação;
         *          Esta função talvez se mantenha.
         */
        private void SelectObjectivePoints(Explorer explo)
        {
            explo.PointsToVisit.Clear();
            foreach (PH.Mission.BaseObjective objective in this.Mission.Objectives)
            {
                if (objective is PH.Mission.NavigationObjective)
                {
                    PH.Mission.NavigationObjective navObj = (PH.Mission.NavigationObjective)objective;
                    foreach (PH.Mission.NavPoint np in navObj.NavPoints)
                        explo.PointsToVisit.Enqueue(np.Location);
                }
            }
            explo.WhatToDoNext = Explorer.WhatToDoNextAction.MoveToPoint;
        }

        private void AI_DoActions()
        {
            switch (this.AI_WhatToDoNext)
            {
                /* TODO: e quando um Explorer é morto? temos de o substituir 
                 *   PG: ExplorerBuilded é actualizado em cada Update. ExplorerBuilded
                 *       é o número de explorers actuais, não o número de explorers construídos.
                 */
                case WhatToDoNextAction.BuildExplorer:
                    if (this.AI.Build(typeof(Explorer)))
                    {
                        if (ExplorerBuilded >= Explorer.SquadNumber - 1)
                            this.AI_WhatToDoNext = WhatToDoNextAction.FillNavPoints;
                    }
                    break;

                case WhatToDoNextAction.FillNavPoints:
                    foreach (NanoBot bot in this.NanoBots)
                    {
                        if (bot is Explorer && ((Explorer)bot).WhatToDoNext == Explorer.WhatToDoNextAction.WaitingForPoints)
                        {
                            /* atribuir apenas 1 ponto a cada explorer*/
                            /* escusa de ser um estado... estamos a perder 1 turno */
                            /* deve ser feito pelo explorer. Quando não tem ponto, acede 
                             à lista de pontos por ir e retira o mais perto 
                             * 
                             * TODO: PG: Lista de pontos atribuídos
                             *           Será diferente para unique nevigation points
                             * 
                             */
                            SelectObjectivePoints((Explorer)bot);
                        }
                    }
                    this.AI_WhatToDoNext = WhatToDoNextAction.BuildCollector;
                    break;

                /* E quando um Collector é morto? temos de o substitur 
                 * PG: PG: CollectorBuilded é actualizado em cada Update. CollectorBuilded
                 *       é o número de collectors actuais, não o número de collectors construídos.
                 */
                case WhatToDoNextAction.BuildCollector:
                    if (this.AI.Build(typeof(Collector)))
                    {
                        if (CollectorBuilded >= Collector.SquadNumber - 1)
                            this.AI_WhatToDoNext = WhatToDoNextAction.MoveToHoshimiPoint;
                    }
                    break;
                /*
                 * TODO: Construir outras squads
                 *       - unidades em excesso implicam um grande atraso inicial
                 */
                case WhatToDoNextAction.MoveToHoshimiPoint:
                    this.AI.MoveTo(Utils.getNearestPoint(this.AI.Location, HoshimiEntities, NeedlePoints));
                    this.AI_WhatToDoNext = WhatToDoNextAction.BuildNeedle;
                    break;
                    /* Needles podem ser destruídos?? */
                case WhatToDoNextAction.BuildNeedle:
                    if (this.AI.Build(typeof(Needle)))
                    {
                        this.AI_WhatToDoNext = WhatToDoNextAction.MoveToHoshimiPoint;
                    }
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

        /**
         * summary: constants that define InjectionPoint choosing criteria
         **/
        /* TODO: PG: Valor de ObjectivesPointsWeight deve depender da pontuação que os objectivos têm
         *           HoshimiPointsWeight deve ter mais peso que AZNPointsWeight já que se não tivermos
         *           Hoshimi points livres, de nada nos serve termos muitas fontes AZN
         *           Deve depender negativamente do ponto de inserção do Pierre e dos seus NeuroControllers;
         *           Aplicar Command?
         * 
         *           >PierreExistingNeuroControllers List of Points where Pierre NeuroControllers are at the beginning of the Game.  
         *           >PierreExistingNeuroControllersCount Numbers of NeuroControllers of Pierre's Team at the beginig of the game  
         *           >PierreTeamInjectionPoint
         */
        private float HoshimiPointsWeight = 5;
        private float AZNPointsWeight = 2;
        private float ObjectivesPointsWeight = 1;
        private float PierreTeamInjectionPointWeight = -4;//ççç

        /**
         * Lists that holds information about current items on the tissue.
         **/
        private List<Entity> m_AznEntities = new List<Entity>();
        private List<Entity> m_HoshimiEntities = new List<Entity>();
        private List<Point>  m_NavigationPoints = new List<Point>();

        public List<Entity> AznEntities { get { return m_AznEntities; } }
        public List<Entity> HoshimiEntities { get { return m_HoshimiEntities; } }
        public List<Point>  NavigationPoints { get { return m_NavigationPoints; } }

        /**
         * summary: function that calculates the best Injection Point based on Hoshimi/Azn/Objective Points
         * returns: calculated Point
         * 
         * PG: Poderia receber lista de listas
         * 
         **/
        public Point FindInjectionPoint()
        {
            Point hoshimiBarycenter = UtilsFx.PointBarycenter(this.HoshimiEntities);
            Point aznBarycenter = UtilsFx.PointBarycenter(this.AznEntities);
            Point objectivesBarycenter = UtilsFx.PointBarycenter(this.NavigationPoints);
            //Point objectivesBarycenter = UtilsFx.PointBarycenter(this.NavigationPoints);
            Point PierreTeamInjectionBarycenter = PierreTeamInjectionPoint;//ççç
            return new Point((int)Math.Round(((PierreTeamInjectionPointWeight * PierreTeamInjectionBarycenter.X) + (HoshimiPointsWeight * hoshimiBarycenter.X) + (AZNPointsWeight * aznBarycenter.X) + (ObjectivesPointsWeight * objectivesBarycenter.X)) / (HoshimiPointsWeight + AZNPointsWeight + ObjectivesPointsWeight + -PierreTeamInjectionPointWeight)),
                (int)Math.Round(((PierreTeamInjectionPointWeight * PierreTeamInjectionBarycenter.X) + (HoshimiPointsWeight * hoshimiBarycenter.Y) + (AZNPointsWeight * aznBarycenter.Y) + (ObjectivesPointsWeight * objectivesBarycenter.Y)) / (HoshimiPointsWeight + AZNPointsWeight + ObjectivesPointsWeight + -PierreTeamInjectionPointWeight)));//ççç
        }

        private void myPlayer_ChooseInjectionPoint()
        {
            //storing AZN and Hoshimi Points
            foreach (Entity ent in this.Tissue.Entities)
                switch (ent.EntityType)
                {
                    case EntityEnum.AZN:
                        AznEntities.Add(ent);
                        break;
                    case EntityEnum.HoshimiPoint:
                        HoshimiEntities.Add(ent);
                        break;
                }
            //storing NavigationObjective Points
            foreach (BaseObjective obj in this.Mission.Objectives)
                if (obj is NavigationObjective || obj is UniqueNavigationObjective)
                    this.NavigationPoints.AddRange(obj.GetObjectiveLocations());

            Point bestPoint = FindInjectionPoint();
            this.InjectionPointWanted = Utils.getNearestPoint(bestPoint,this.HoshimiEntities);
        }
        #endregion

    }
}
