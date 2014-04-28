using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.Examples
{
    //this is an explorer that does not move much (not much of a good explorer) but is a mouthfull
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 30, MaxDamage = 0, DefenseDistance = 0, Constitution = 10)]
    public class CommunicativeExplorer : AASMAExplorer
    {
        public override void DoActions()
        {
            List<Point> points;
            points = getAASMAFramework().visibleHoshimies(this);
            //if this explorer sees an hoshimi point, it will warn the nanoAI agent about it
            //an hoshimi point is where the AI can build needles
            if (points.Count > 0)
            {
                //the sender and receivers of the message are identified by their internal name, E1, E2,..., C1,C2,...,N1,N2, AI, etc
                AASMAMessage msg = new AASMAMessage(this.InternalName, "I've found an hoshimi point! Go there man!");
                //something nice about messages, you can add any object you like to the message as a tag
                //on the receiver side you need to cast it to the right object and then u can use it
                //in this case, we're tagging along the hoshimi point the explorer just seen
                msg.Tag = points[0];
                //this method sends a message to a particular nanobot, in this case it sends it to the AI nanobot
                //the AI nanobot is allways called AI and u should not change it's internal name
                getAASMAFramework().sendMessage(msg, "AI");

            }
            
            //navigation points are points that u can explore (go there with any nano) to increase your team's score
            points = getAASMAFramework().visibleNavigationPoints(this);
            if (points.Count > 0)
            {
                //you can use this method to move to a specified point, and your nanobot will move much faster
                //than when moving position by position
                //however, take into consideration that this type of movement can take several turns
                //and if so your agen't won't perform any other action (you can still send messsages) such as firing
                //or moving somewhere else. You can always cancel your current movement action though.
                //with the command this.StopMoving()
                //u can check the property state, if the value is waitingOrders, then u can perform actions freely
                this.MoveTo(points[0]);
            }
            else
            {
                //if this agent has nothing to do, he likes to talk to everyone
                //the broadcastmessage method sends a message to all nanobots
                getAASMAFramework().broadCastMessage(new AASMAMessage(this.InternalName, "Hi guys, I'm bored with nothing to do, any good jokes?"));

            }            
        }

        public override void receiveMessage(AASMAMessage msg)
        {
        }

    }
}
