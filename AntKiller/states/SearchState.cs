using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using MogreFramework;

namespace AntKiller
{
	class SearchState : State
  {
    #region Properties
    private int count;
    #endregion

    public SearchState(Ant ant)
			: base(ant)
		{
      this.Destination = this.randomDestination(this.Ant.SceneNode);

      if (this.Ant.CurrentAnimation != AntAnimations.WALK
        || this.Ant.CurrentAnimationState == null
        || !this.Ant.CurrentAnimationState.Enabled)
        this.Ant.startAnimation(AntAnimations.WALK, true);

      this.Ant.Sphere.SetMaterialName("SphereBlue");

      //Console.WriteLine(this.Ant.Name + " SearchState Destination: " + this.Destination.ToString());
		}

		public override void Update(FrameEvent evt)
		{
			float distance = this.Ant.moteToDestination(evt, this.Destination);

			if (distance <= 0.0f)
			{
        if (this.count < 3)
        {
          // Try another direction
          this.Destination = this.randomDestination(this.Ant.SceneNode);
          this.count++;
        }
        else
        {
          // Nothing found, go back home for new assignment
          this.Ant.CurrentState = new BackState(this.Ant);
        }
			}
		}    

    private Vector3 randomDestination(SceneNode sceneNode)
    {
      int x = Options.random.Next(
        Options.force((int)(sceneNode.Position.x - Options.pointSpread), 20, 1480),
        Options.force((int)(sceneNode.Position.x + Options.pointSpread), 20, 1480));
      int z = Options.random.Next(
        Options.force((int)(sceneNode.Position.z - Options.pointSpread), 20, 1480),
        Options.force((int)(sceneNode.Position.z + Options.pointSpread), 20, 1480));

      return new Vector3(x, 0, z);
    }
	}
}
