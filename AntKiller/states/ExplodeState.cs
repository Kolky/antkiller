using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using MogreFramework;

namespace AntKiller
{
    class ExplodeState : State
    {
        public ExplodeState(Ant ant)
            : base(ant)
        {
            if (Ant.CurrentAnimation != AntAnimations.EXPLODE || 
                Ant.CurrentAnimationState == null ||
                !Ant.CurrentAnimationState.Enabled)
            {
                Ant.startAnimation(AntAnimations.EXPLODE, false);
            }

            Ant.Sphere.SetMaterialName("SpherePurple");

            //Console.WriteLine(Ant.Name + " ExplodeState");
        }

        public override void Update(FrameEvent evt)
        {
            if (Ant.CurrentAnimationState != null &&
                Ant.CurrentAnimationState.HasEnded)
            {
                AntBuilder.ToBeRemoved.Add(Ant);
            }
        }
    }
}
        