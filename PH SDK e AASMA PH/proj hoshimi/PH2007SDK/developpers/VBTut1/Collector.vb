'Tutorial done by
' 
'Richard Clark
'Project Hoshimi Lead Manager
'Contact at ori@c2i.fr
' 
Imports PH.Common

<PH.Common.Characteristics(ContainerCapacity:=20, _
    CollectTransfertSpeed:=5, _
    Scan:=5, _
    MaxDamage:=0, _
    DefenseDistance:=0, _
    Constitution:=20)> _
  Public Class Collector
    Inherits PH.Common.NanoCollector
End Class

<PH.Common.Characteristics(ContainerCapacity:=0, _
    CollectTransfertSpeed:=5, _
    Scan:=5, _
    MaxDamage:=5, _
    DefenseDistance:=12, _
    Constitution:=20)> _
  Public Class Defender
    Inherits PH.Common.NanoCollector
End Class
