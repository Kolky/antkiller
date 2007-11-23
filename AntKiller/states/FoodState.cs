using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using MogreFramework;

namespace AntKiller
{
  class FoodState : State
  {
    public FoodState(Ant ant, Vector3 destination)
      : base(ant)
    {
      this.Ant.resetMovement();
      this.Destination = destination;

      if (this.Ant.CurrentAnimation != AntAnimations.WALK
        || this.Ant.CurrentAnimationState == null
        || !this.Ant.CurrentAnimationState.Enabled)
        this.Ant.startAnimation(AntAnimations.WALK, true);

      this.Ant.Sphere.SetMaterialName("SphereYellow");

      //Console.WriteLine(this.Ant.Name + " FoodState Destination: " + this.Destination.ToString());
    }

    public override void Update(FrameEvent evt)
    {
      float distance = this.Ant.moteToDestination(evt, this.Destination);

      if (distance <= 0.0f)
      {
        this.Ant.CurrentState = new HomeState(this.Ant, new Mission(MissionType.FOOD_EMPTY, this.Destination));
      }
    }
  }
}
