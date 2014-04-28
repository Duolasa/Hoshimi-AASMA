'Template done by
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
        NothingToDo = 0
    End Enum


    Private m_WhatToDoNext As WhatToDoNextAction = WhatToDoNextAction.NothingToDo
    Public Property AI_WhatToDoNext() As WhatToDoNextAction
        Get
            Return m_WhatToDoNext
        End Get
        Set(ByVal value As WhatToDoNextAction)
            m_WhatToDoNext = value
        End Set
    End Property

    Private Sub UpdateInformations()
    End Sub
    Private Sub AI_DoActions()

        Select Case Me.AI_WhatToDoNext
            Case WhatToDoNextAction.NothingToDo
        End Select
    End Sub

    Private Sub myPlayer_WhatToDoNextEvent() Handles Me.WhatToDoNext
        UpdateInformations()

        If Me.AI.State <> NanoBotState.WaitingOrders Then AI_DoActions()

        For Each bot As NanoBot In Me.NanoBots
            If TypeOf (bot) Is IActionable AndAlso bot.State = NanoBotState.WaitingOrders Then
                CType(bot, IActionable).DoActions()
            End If
        Next
    End Sub

#Region "Injection Point"
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
                    AZNEntities.Add(ent)
                Case EntityEnum.HoshimiPoint
                    HoshimiEntities.Add(ent)
            End Select
        Next

        'CHANGE THIS LINE
        Me.InjectionPointWanted = New Point(100, 100)
    End Sub
#End Region


End Class
