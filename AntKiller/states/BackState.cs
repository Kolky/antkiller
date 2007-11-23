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
      : base(ant)
    {
      this.Destination = this.Ant.Colony.Home;

      if (this.Ant.CurrentAnimation != AntAnimations.WALK
        || this.Ant.CurrentAnimationState == null
        || !this.Ant.CurrentAnimationState.Enabled)
        this.Ant.startAnimation(AntAnimations.WALK, true);

      this.Ant.Sphere.SetMaterialName("SphereOrange");

      //Console.WriteLine(this.Ant.Name + " BackState");
    }

    public override void Update(FrameEvent evt)
    {
      float distance = this.Ant.moteToDestination(evt, this.Destination);

      if (distance <= 0.0f)
      {
        this.Ant.CurrentState = this.Ant.Colony.newAssignment(this.Ant);
      }
    }
  }
}
