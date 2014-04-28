namespace Hoshimi_ClassLibrary
{
    [PH.Common.Characteristics(ContainerCapacity = 20,

     CollectTransfertSpeed = 5,

     Scan = 5,

     MaxDamage = 0,

     DefenseDistance = 0,

     Constitution = 20)]

    public class Collector : PH.Common.NanoCollector

    { }



    [PH.Common.Characteristics(ContainerCapacity = 0,

    CollectTransfertSpeed = 5,

    Scan = 5,

    MaxDamage = 5,

    DefenseDistance = 12,

    Constitution = 20)]

    public class Defender : PH.Common.NanoCollector

    { }



}
