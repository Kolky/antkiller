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
            Destination = randomDestination(Ant.SceneNode);

            if (Ant.CurrentAnimation != AntAnimations.WALK ||
                Ant.CurrentAnimationState == null ||
                !Ant.CurrentAnimationState.Enabled)
            {
                Ant.startAnimation(AntAnimations.WALK, true);
            }

            Ant.Sphere.SetMaterialName("SphereBlue");

            //Console.WriteLine(Ant.Name + " SearchState Destination: " + Destination.ToString());
        }

        public override void Update(FrameEvent evt)
        {
            float distance = Ant.moteToDestination(evt, Destination);

            if (distance <= 0.0f)
            {
                if (count < 3)
                {
                    // Try another direction
                    Destination = randomDestination(Ant.SceneNode);
                    count++;
                }
                else
                {
                    // Nothing found, go back home for new assignment
                    Ant.CurrentState = new BackState(Ant);
                }
            }
        }

        private Vector3 randomDestination(SceneNode sceneNode)
        {
            int x = Options.random.Next(
              Options.force((int)(sceneNode.Position.x - Options.pointSpread), Options.minX, Options.maxX),
              Options.force((int)(sceneNode.Position.x + Options.pointSpread), Options.minX, Options.maxX));
            int z = Options.random.Next(
              Options.force((int)(sceneNode.Position.z - Options.pointSpread), Options.minZ, Options.maxZ),
              Options.force((int)(sceneNode.Position.z + Options.pointSpread), Options.minZ, Options.maxZ));

            return new Vector3(x, 0, z);
        }
    }
}