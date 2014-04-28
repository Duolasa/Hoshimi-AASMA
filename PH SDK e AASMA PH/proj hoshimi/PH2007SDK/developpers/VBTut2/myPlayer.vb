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
        Dim entAZN As Entity = AZNEntities(0)
        Me.InjectionPointWanted = New Point(entAZN.X, entAZN.Y)
    End Sub

    Dim bCollectorBuilded As Boolean
    Dim bMoveToHoshimiPoint As Boolean
    Dim bNeedleBuilded As Boolean
    Private Sub AI_DoActions()
        If Me.AI.State = NanoBotState.WaitingOrders Then
            If Not bCollectorBuilded Then
                bCollectorBuilded = Me.AI.Build(GetType(Collector))
            ElseIf Not bMoveToHoshimiPoint Then
                Me.AI.MoveTo(Utils.getNearestPoint(Me.AI.Location, Me.HoshimiEntities))
                bMoveToHoshimiPoint = True
            ElseIf Not bNeedleBuilded Then
                bNeedleBuilded = Me.AI.Build(GetType(Needle))
            End If
        End If
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
