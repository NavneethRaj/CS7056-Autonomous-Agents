using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FSM
{
    public class DoHouseWork : State<MinersWife>
    {

        public override void Enter(MinersWife minersWife)
        {
            Debug.Log(minersWife.ID + " Why should wife always do housework?");
        }

        public override void Execute(MinersWife minersWife)
        {
            switch (Random.Range(0,2))
            {
                case 0:
                    Debug.Log(minersWife.ID + " Mopping the floor");
                    break;
                case 1:
                    Debug.Log(minersWife.ID + " Washing the dishes");
                    break;
                case 2:
                    Debug.Log(minersWife.ID + " Making the bed");
                    break;
                default:
                    break;
            }
        }

        public override void Exit(MinersWife minersWife)
        {

        }

        public override bool OnMessage(MinersWife minersWife, Telegram telegram)
        {
            return false;
        }

        public override bool OnSense(MinersWife agent, Sense sense)
        {
            // throw new System.NotImplementedException();
            return false;
        }
    }

    public class VisitBathroom : State<MinersWife>
    {
        public override void Enter(MinersWife minersWife)
        {
            Debug.Log(minersWife.ID + " Walking to the can. Need to put on some makeup");
        }

        public override void Execute(MinersWife minersWife)
        {
            Debug.Log(minersWife.ID + " Ahhhhhhhh!");
            minersWife.StateMachine.RevertToPreviousState();  // this completes the state blip
        }

        public override void Exit(MinersWife minersWife)
        {
            Debug.Log(minersWife.ID + " Leaving the Area");
        }

        public override bool OnMessage(MinersWife minersWife, Telegram telegram)
        {
            return false;
        }

        public override bool OnSense(MinersWife agent, Sense sense)
        {
            // throw new System.NotImplementedException();
            return false;
        }
    }

    public class CookStew : State<MinersWife>
    {
        public override void Enter(MinersWife minersWife)
        {
            if (!minersWife.Cooking)
            {
                // MinersWife sends a delayed message to herself to arrive when the food is ready
                Debug.Log(minersWife.ID + " Cooking Chicken Biryani");
                Message.DispatchMessage(2, minersWife.ID, minersWife.HusbandId, MessageType.BiryanisReady);
                minersWife.Cooking = true;
            }
        }

        public override void Execute(MinersWife minersWife)
        {
           Debug.Log(minersWife.ID + " Making Preparations for food");
        }

        public override void Exit(MinersWife minersWife)
        {
          Debug.Log(minersWife.ID + " Serving Biryani on the table");
        }

        public override bool OnMessage(MinersWife minersWife, Telegram telegram)
        {
            switch (telegram.messageType)
            {
                case MessageType.HeyHamImHome:
                    // Ignored here; handled in WifesGlobalState below
                    return false;
                case MessageType.BiryanisReady:
                    // Tell Miner that the stew is ready now by sending a message with no delay
                    Debug.Log("Message handled by " + minersWife.ID + " at time ");
                    Debug.Log(minersWife.ID + " BiryanisReady! Lets eat");
                    Message.DispatchMessage(0, minersWife.ID, minersWife.HusbandId, MessageType.BiryanisReady);
                    minersWife.Cooking = false;
                    minersWife.StateMachine.ChangeState(new DoHouseWork());
                    return true;
                default:
                    return false;
            }
        }

        public override bool OnSense(MinersWife agent, Sense sense)
        {
            // throw new System.NotImplementedException();
            return false;
        }
    }

    public class WifesGlobalState : State<MinersWife>
    {
        public override void Enter(MinersWife minersWife)
        {
           
        }

        public override void Execute(MinersWife minersWife)
        {
            // There's always a 10% chance of a state blip in which MinersWife goes to the bathroom
            if (Random.Range(0,9) == 1 && !minersWife.StateMachine.IsInState(new VisitBathroom()))
            {
                minersWife.StateMachine.ChangeState(new VisitBathroom());
            }
        }

        public override void Exit(MinersWife minersWife)
        {

        }

        public override bool OnMessage(MinersWife minersWife, Telegram telegram)
        {
            switch (telegram.messageType)
            {
                case MessageType.HeyHamImHome:
                    Debug.Log("Message handled by " + minersWife.ID + " at time ");
                    Debug.Log(minersWife.ID + "Hey Nav, let me make Biryani for you");
                    minersWife.StateMachine.ChangeState(new CookStew());
                    return true;
                case MessageType.BiryanisReady:
                    return false;
                default:
                    return false;
            }                 
        }

        public override bool OnSense(MinersWife agent, Sense sense)
        {
            // throw new System.NotImplementedException();
            return false;
        }
    }
}
