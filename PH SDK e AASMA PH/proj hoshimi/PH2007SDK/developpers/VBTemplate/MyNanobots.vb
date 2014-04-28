'Template done by
' 
'Richard Clark
'Project Hoshimi Lead Manager
'Contact at ori@c2i.fr
' 
Imports PH.Common
Imports System.Drawing

<Characteristics(ContainerCapacity:=20, CollectTransfertSpeed:=5, Scan:=5, MaxDamage:=0, DefenseDistance:=0, Constitution:=20)> _
  Public Class Collector
    Inherits PH.Common.NanoCollector
    Implements IActionable

    Public Const SquadNumber As Integer = 0
    Public Sub DoActions() Implements IActionable.DoActions
    End Sub
End Class

<Characteristics(ContainerCapacity:=100, CollectTransfertSpeed:=0, Scan:=10, MaxDamage:=5, DefenseDistance:=10, Constitution:=25)> _
Public Class Needle
    Inherits PH.Common.NanoNeedle
    Implements IActionable

    Public Const SquadNumber As Integer = 0
    Public Sub DoActions() Implements IActionable.DoActions
    End Sub
End Class

<Characteristics(ContainerCapacity:=0, CollectTransfertSpeed:=0, Scan:=30, MaxDamage:=0, DefenseDistance:=0, Constitution:=10)> _
Public Class Explorer
    Inherits PH.Common.NanoExplorer
    Implements IActionable

    Public Const SquadNumber As Integer = 0
    Public Sub DoActions() Implements IActionable.DoActions
    End Sub
End Class

<Characteristics(ContainerCapacity:=0, CollectTransfertSpeed:=0, Scan:=5, MaxDamage:=5, DefenseDistance:=12, Constitution:=25)> _
  Public Class Protector
    Inherits PH.Common.NanoCollector
    Implements IActionable

    Public Const SquadNumber As Integer = 0
    Public Sub DoActions() Implements IActionable.DoActions
    End Sub
End Class

