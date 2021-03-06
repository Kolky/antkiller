using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using MogreFramework;

namespace AntKiller
{
    class FoodState : State
    {
        public string ObjectName { get; private set; }

        public FoodState(Ant ant, string objectName, Vector3 destination)
            : base(ant, Options.walkSpeed)
        {
            ObjectName = objectName;
            Ant.resetMovement();
            Destination = destination;

            if (Ant.CurrentAnimation != AntAnimations.WALK ||
                Ant.CurrentAnimationState == null ||
                !Ant.CurrentAnimationState.Enabled)
            {
                Ant.startAnimation(AntAnimations.WALK, true);
            }

            Ant.Sphere.SetMaterialName("SphereYellow");

            //Console.WriteLine(Ant.Name + " FoodState Destination: " + Destination.ToString());
        }

        public override void Update(FrameEvent evt)
        {
            float distance = Ant.moteToDestination(evt, Destination);

            if (distance <= 0.0f)
            {
                Ant.CurrentState = new HomeState(Ant, new Mission(MissionType.FOOD_EMPTY, ObjectName, Destination));
            }
        }
    }
}