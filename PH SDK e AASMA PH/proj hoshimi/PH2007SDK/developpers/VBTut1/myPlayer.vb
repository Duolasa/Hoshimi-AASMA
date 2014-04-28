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

    Private Sub myPlayer_ChooseInjectionPointEvent() Handles Me.ChooseInjectionPoint
        Me.InjectionPointWanted = New Point(140, 80)
    End Sub

    Dim bCollectorBuilded As Boolean
    Private Sub myPlayer_WhatToDoNextEvent() Handles Me.WhatToDoNext
        If Me.AI.State = NanoBotState.WaitingOrders Then
            If Not bCollectorBuilded Then
                If Me.AI.Build(GetType(Collector)) Then
                    bCollectorBuilded = True
                End If
            Else
                Me.AI.MoveTo(New Point(100, 50))
            End If
        End If
    End Sub

    Public Overrides ReadOnly Property Flag() As System.Drawing.Bitmap
        Get
            Return My.Resources.rcFlag
        End Get
    End Property

End Class



