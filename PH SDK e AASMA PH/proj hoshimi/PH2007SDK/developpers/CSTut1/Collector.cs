/* Tutorial done by
 * 
 * Richard Clark
 * Project Hoshimi Lead Manager
 * Contact at ori@c2i.fr
 * 
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;
using PH.Map;

namespace CSTut1
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
