using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using MogreFramework;

namespace AntKiller
{
    class BackState : State
    {
        public BackState(Ant ant)
            : base(ant, Options.returnSpeed)
        {
            Destination = Ant.Colony.Home;

            if (Ant.CurrentAnimation != AntAnimations.WALK ||
                Ant.CurrentAnimationState == null ||
                !Ant.CurrentAnimationState.Enabled)
            {
                Ant.startAnimation(AntAnimations.WALK, true);
            }

            Ant.Sphere.SetMaterialName("SpherePink");

            //Console.WriteLine(Ant.Name + " BackState");
        }

        public override void Update(FrameEvent evt)
        {
            float distance = Ant.moteToDestination(evt, Destination);

            if (distance <= 0.0f)
            {
                Ant.CurrentState = Ant.Colony.newAssignment(Ant);
            }
        }
    }
}