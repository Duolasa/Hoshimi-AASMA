'Tutorial done by
' 
'Richard Clark
'Project Hoshimi Lead Manager
'Contact at ori@c2i.fr
' 
Imports PH.Common

<Characteristics(ContainerCapacity:=20, _
    CollectTransfertSpeed:=5, _
    Scan:=5, _
    MaxDamage:=0, _
    DefenseDistance:=0, _
    Constitution:=20)> _
  Public Class Collector
    Inherits PH.Common.NanoCollector
    Implements IActionable

    Public Enum WhatToDoNextAction
        MoveToAZN = 0
        CollectAZN = 1
        MoveToHoshimi = 2
        TransfertToNeedle = 3
    End Enum

    Private m_WhatToDoNext As WhatToDoNextAction = WhatToDoNextAction.CollectAZN
    Public Property WhatToDoNext() As WhatToDoNextAction
        Get
            Return Me.m_WhatToDoNext
        End Get
        Set(ByVal value As WhatToDoNextAction)
            m_WhatToDoNext = value
        End Set
    End Property

    Public Sub DoActions() Implements IActionable.DoActions
        Select Case Me.WhatToDoNext
            Case WhatToDoNextAction.CollectAZN
                Me.CollectFrom(Me.Location, 4)
                Me.WhatToDoNext = WhatToDoNextAction.MoveToHoshimi
            Case WhatToDoNextAction.MoveToHoshimi
                Me.MoveTo(Utils.getNearestPoint(Me.Location, CType(Me.PlayerOwner, myPlayer).HoshimiEntities))
                Me.WhatToDoNext = WhatToDoNextAction.TransfertToNeedle
            Case WhatToDoNextAction.TransfertToNeedle
                Me.TransferTo(Me.Location, 4)
                Me.WhatToDoNext = WhatToDoNextAction.MoveToAZN
            Case WhatToDoNextAction.MoveToAZN
                Me.MoveTo(Utils.getNearestPoint(Me.Location, CType(Me.PlayerOwner, myPlayer).AZNEntities))
                Me.WhatToDoNext = WhatToDoNextAction.CollectAZN
        End Select
    End Sub
End Class

<Characteristics(ContainerCapacity:=100, _
CollectTransfertSpeed:=0, _
Scan:=10, _
MaxDamage:=5, _
DefenseDistance:=10, _
Constitution:=25)> _
Public Class Needle
    Inherits PH.Common.NanoNeedle

End Class
