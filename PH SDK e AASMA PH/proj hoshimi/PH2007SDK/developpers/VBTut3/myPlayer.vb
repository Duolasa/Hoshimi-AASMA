'Tutorial done by
' 
'Richard Clark
'Project Hoshimi Lead Manager
'Contact at ori@c2i.fr
' 
Imports PH.Common
Imports PH.Map
Imports System.Drawing

Public Class myPlayer
    Inherits PH.Common.Player

    Public Sub New()
    End Sub

    Public Sub New(ByVal name As String, ByVal ID As Integer)
        MyBase.New(name, ID)
    End Sub

    Private m_AZNEntities As New List(Of Entity)
    Private m_HoshimiEntities As New List(Of Entity)

    Public ReadOnly Property AZNEntities() As List(Of Entity)
        Get
            Return m_AZNEntities
        End Get
    End Property
    Public ReadOnly Property HoshimiEntities() As List(Of Entity)
        Get
            Return m_HoshimiEntities
        End Get
    End Property

    Private Sub myPlayer_ChooseInjectionPointEvent() Handles Me.ChooseInjectionPoint
        'storing AZN and Hoshimi Points
        For Each ent As Entity In Me.Tissue.Entities
            Select Case ent.EntityType
                Case EntityEnum.AZN
                    m_AZNEntities.Add(ent)
                Case EntityEnum.HoshimiPoint
                    m_HoshimiEntities.Add(ent)
            End Select
        Next

        'I want to be injected at the first AZN point
        Dim entAZN As Entity = AZNEntities(2)
        Me.InjectionPointWanted = New Point(entAZN.X, entAZN.Y)
    End Sub

    Public Enum WhatToDoNextAction
        BuildExplorer = 0
        FillNavPoints = 1
        NothingToDo = 2
    End Enum

    Private m_WhatToDoNext As WhatToDoNextAction = WhatToDoNextAction.BuildExplorer
    Public Property AI_WhatToDoNext() As WhatToDoNextAction
        Get
            Return m_WhatToDoNext
        End Get
        Set(ByVal value As WhatToDoNextAction)
            m_WhatToDoNext = value
        End Set
    End Property

    Private Sub AI_DoActions()
        If Me.AI.State = NanoBotState.WaitingOrders Then
            Select Case Me.AI_WhatToDoNext
                Case WhatToDoNextAction.BuildExplorer
                    If Me.AI.Build(GetType(Explorer)) Then
                        Me.AI_WhatToDoNext = WhatToDoNextAction.FillNavPoints
                    End If
                Case WhatToDoNextAction.FillNavPoints
                    For Each bot As NanoBot In Me.NanoBots
                        If TypeOf (bot) Is Explorer AndAlso _
                            CType(bot, Explorer).WhatToDoNext = Explorer.WhatToDoNextAction.WaitingForPoints Then
                            SelectObjectivePoints(CType(bot, Explorer))
                            Me.AI_WhatToDoNext = WhatToDoNextAction.NothingToDo
                            Exit For
                        End If
                    Next
                Case WhatToDoNextAction.NothingToDo
            End Select
        End If
    End Sub
    Private Sub NEW_SelectObjectivePoints(ByVal explo As Explorer)
        explo.PointsToVisit.Clear()

        For Each objective As PH.Mission.BaseObjective In Me.Mission.Objectives
            If TypeOf (objective) Is PH.Mission.NavigationObjective Then
                Dim navObj As PH.Mission.NavigationObjective = CType(objective, PH.Mission.NavigationObjective)
                For Each np As PH.Mission.NavPoint In navObj.NavPoints
                    explo.PointsToVisit.Enqueue(np.Location)
                Next
            End If
        Next

        explo.WhatToDoNext = Explorer.WhatToDoNextAction.MoveToPoint
    End Sub
    Private Sub SelectObjectivePoints(ByVal explo As Explorer)
        explo.PointsToVisit.Clear()
        explo.PointsToVisit.Enqueue(New Point(117, 142))
        explo.PointsToVisit.Enqueue(New Point(111, 154))
        explo.PointsToVisit.Enqueue(New Point(128, 195))
        explo.WhatToDoNext = Explorer.WhatToDoNextAction.MoveToPoint
    End Sub

    Private Sub myPlayer_WhatToDoNextEvent() Handles Me.WhatToDoNext
        AI_DoActions()

        For Each bot As NanoBot In Me.NanoBots
            If TypeOf (bot) Is IActionable AndAlso bot.State = NanoBotState.WaitingOrders Then
                CType(bot, IActionable).DoActions()
            End If
        Next
    End Sub

    Public Overrides ReadOnly Property Flag() As System.Drawing.Bitmap
        Get
            Return My.Resources.rcFlag
        End Get
    End Property


End Class
