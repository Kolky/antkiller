using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using MogreFramework;

namespace AntKiller
{
  class ExplodeState : State
  {
    #region Properties
    private float time = 0.0f;
    #endregion

    public ExplodeState(Ant ant)
      : base(ant)
    {
      if (this.Ant.CurrentAnimation != AntAnimations.EXPLODE
        || this.Ant.CurrentAnimationState == null
        || !this.Ant.CurrentAnimationState.Enabled)
        this.Ant.startAnimation(AntAnimations.EXPLODE, true);

      this.Ant.Sphere.SetMaterialName("SpherePurple");

      //Console.WriteLine(this.Ant.Name + " ExplodeState");
    }

    public override void Update(FrameEvent evt)
    {
      time += evt.timeSinceLastFrame;

      //if (this.CurrentState.GetType() == typeof(ExplodeState) && this.CurrentAnimationState.HasEnded)
      if (time >= 1.0f)
        AntBuilder.ToBeRemoved.Add(this.Ant);
    }
  }
}
