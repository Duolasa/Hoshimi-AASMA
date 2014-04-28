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

#Region "Initializations"
    Public Sub New()
    End Sub

    Public Sub New(ByVal name As String, ByVal ID As Integer)
        MyBase.New(name, ID)
    End Sub

    Public Overrides ReadOnly Property Flag() As System.Drawing.Bitmap
        Get
            Return My.Resources.rcFlag
        End Get
    End Property
#End Region

    Public Enum WhatToDoNextAction
        BuildExplorer = 0
        FillNavPoints = 1
        BuildCollector = 2
        MoveToHoshimiPoint = 4
        BuildNeedle = 5
        NothingToDo = 6
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

    Private m_NeedlePoints As New List(Of Point)
    Private m_EmptyNeedlePoints As New List(Of Point)
    Private m_FullNeedlePoints As New List(Of Point)
    Public ReadOnly Property NeedlePoints() As List(Of Point)
        Get
            Return (m_NeedlePoints)
        End Get
    End Property
    Public ReadOnly Property EmptyNeedlePoints() As List(Of Point)
        Get
            Return (m_EmptyNeedlePoints)
        End Get
    End Property
    Public ReadOnly Property FullNeedlePoints() As List(Of Point)
        Get
            Return (m_FullNeedlePoints)
        End Get
    End Property


    Private Const NeedlesToBuild As Integer = 2
    Private CollectorBuilded As Integer = 0
    Private Sub UpdateInformations()
        NeedlePoints.Clear()
        EmptyNeedlePoints.Clear()
        FullNeedlePoints.Clear()
        CollectorBuilded = 0
        For Each bot As NanoBot In Me.NanoBots
            If TypeOf (bot) Is Needle Then
                NeedlePoints.Add(bot.Location)
                If bot.Stock = 100 Then
                    FullNeedlePoints.Add(bot.Location)
                Else
                    EmptyNeedlePoints.Add(bot.Location)
                End If
            ElseIf TypeOf (bot) Is Collector Then
                CollectorBuilded += 1
            End If
        Next
    End Sub
    Private Sub AI_DoActions()
        If Me.AI.State <> NanoBotState.WaitingOrders Then Return

        Select Case Me.AI_WhatToDoNext
            Case WhatToDoNextAction.BuildCollector
                If Me.AI.Build(GetType(Collector)) Then
                    If (CollectorBuilded >= Collector.SquadNumber - 1) Then
                        Me.AI_WhatToDoNext = WhatToDoNextAction.MoveToHoshimiPoint
                    End If
                End If
            Case WhatToDoNextAction.MoveToHoshimiPoint
                Me.AI.MoveTo(Utils.getNearestPoint(Me.AI.Location, HoshimiEntities, NeedlePoints))
                Me.AI_WhatToDoNext = WhatToDoNextAction.BuildNeedle
            Case WhatToDoNextAction.BuildExplorer
                If Me.AI.Build(GetType(Explorer)) Then
                    Me.AI_WhatToDoNext = WhatToDoNextAction.FillNavPoints
                End If
            Case WhatToDoNextAction.FillNavPoints
                For Each bot As NanoBot In Me.NanoBots
                    If TypeOf (bot) Is Explorer AndAlso _
                        CType(bot, Explorer).WhatToDoNext = Explorer.WhatToDoNextAction.WaitingForPoints Then
                        NEW_SelectObjectivePoints(CType(bot, Explorer))
                        Me.AI_WhatToDoNext = WhatToDoNextAction.BuildCollector
                        Exit For
                    End If
                Next
            Case WhatToDoNextAction.BuildNeedle
                If Me.AI.Build(GetType(Needle)) Then
                    If (NeedlePoints.Count >= NeedlesToBuild - 1) Then
                        Me.AI_WhatToDoNext = WhatToDoNextAction.NothingToDo
                    Else
                        Me.AI_WhatToDoNext = WhatToDoNextAction.MoveToHoshimiPoint
                    End If
                End If
            Case WhatToDoNextAction.NothingToDo
        End Select

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
        UpdateInformations()

        AI_DoActions()

        For Each bot As NanoBot In Me.NanoBots
            If TypeOf (bot) Is IActionable AndAlso bot.State = NanoBotState.WaitingOrders Then
                CType(bot, IActionable).DoActions()
            End If
        Next
    End Sub

#Region "Injection Point"
    Private m_AZNEntities As New List(Of Entity)
    Private m_HoshimiEntities As New List(Of Entity)
    Private m_AznPoints As New List(Of Point)
    Private m_HoshimiPoints As New List(Of Point)
    Private m_NavigationPoints As New List(Of Point)

    Private AZNMiddle As Point = Point.Empty
    Private HoshimiMiddle As Point = Point.Empty
    Private NavigationMiddle As Point = Point.Empty

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
    Public ReadOnly Property AZNPoints() As List(Of Point)
        Get
            Return m_AznPoints
        End Get
    End Property
    Public ReadOnly Property HoshimiPoints() As List(Of Point)
        Get
            Return m_HoshimiPoints
        End Get
    End Property
    Public ReadOnly Property NavigationPoints() As List(Of Point)
        Get
            Return m_NavigationPoints
        End Get
    End Property

    Private Sub myPlayer_ChooseInjectionPointEvent() Handles Me.ChooseInjectionPoint
        'storing AZN and Hoshimi Points
        For Each ent As Entity In Me.Tissue.Entities
            Select Case ent.EntityType
                Case EntityEnum.AZN
                    AZNEntities.Add(ent)
                    AZNPoints.Add(ent.Location)
                Case EntityEnum.HoshimiPoint
                    HoshimiEntities.Add(ent)
                    HoshimiPoints.Add(ent.Location)
            End Select
        Next

        'storing navigationPoints
        For Each obj As PH.Mission.BaseObjective In Me.Mission.Objectives
            If TypeOf (obj) Is PH.Mission.NavigationObjective AndAlso obj.Bonus > 0 Then
                Dim navObj As PH.Mission.NavigationObjective = CType(obj, PH.Mission.NavigationObjective)
                For Each np As PH.Mission.NavPoint In navObj.NavPoints
                    NavigationPoints.Add(np.Location)
                Next
            End If
            If TypeOf (obj) Is PH.Mission.UniqueNavigationObjective AndAlso obj.Bonus > 0 Then
                Dim uniqueNavObj As PH.Mission.UniqueNavigationObjective = CType(obj, PH.Mission.UniqueNavigationObjective)
                For Each np As PH.Mission.NavPoint In uniqueNavObj.NavPoints
                    NavigationPoints.Add(np.Location)
                Next
            End If
        Next

        HoshimiMiddle = Utils.getMiddlePoint(HoshimiPoints.ToArray())
        AZNMiddle = Utils.getMiddlePoint(AZNPoints.ToArray())
        NavigationMiddle = Utils.getMiddlePoint(NavigationPoints.ToArray())

        Me.InjectionPointWanted = Utils.getValidPoint(Me.Tissue, NavigationMiddle)
    End Sub
#End Region


End Class
