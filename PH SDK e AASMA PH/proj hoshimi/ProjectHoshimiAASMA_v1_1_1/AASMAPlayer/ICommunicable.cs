using System;
using System.Collections.Generic;
using System.Text;

namespace AASMAHoshimi
{
    interface ICommunicable
    {
        void receiveMessage(AASMAMessage msg);
    }
}
