using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mogre;
using MogreFramework;

namespace AntKiller
{
    public enum AntColor
    {
        BLACK,
        RED
    };
    public enum AntAnimations
    {
        WALK,
        ATTACK,
        STUN,
        EXPLODE,
        IDLE
    };

    class Ant : Object
    {
        #region Properties
        public int Health { get; set; }
        public int Rank { get; set; }

        public Entity Sphere { get; private set; }
        public AntAnimations CurrentAnimation { get; private set; }
        public AnimationState CurrentAnimationState { get; private set; }
        public State CurrentState { get; set; }
        public Colony Colony { get; private set; }
        #endregion

        public Ant(OgreWindow win, SceneNode sceneNode, Colony colony)
            : base(win, sceneNode)
        {
            Colony = colony;
            Colony.Counter++;

            ID = Options.counter++;
            Name = String.Concat(Colony.Name, "Ant", ID);
            Health = Options.health;
            Rank = 1;

            SceneNode.Position = Colony.Home;

            // Create Ant
            Entity = win.SceneManager.CreateEntity(Name, "ant1.mesh");
            Sphere = win.SceneManager.CreateEntity(Name + "Sphere", "sphere.mesh");

            // Apply Color
            switch (Colony.Color)
            {
                case AntColor.BLACK:
                    Entity.SetMaterialName("AntBlack");
                    break;
                case AntColor.RED:
                    Entity.SetMaterialName("AntRed");
                    break;
            }

            // Attach Object to Node
            SceneNode.AttachObject(Entity);
            SceneNode.AttachObject(Sphere);

            // Initialize Variables
            mDirection = Vector3.ZERO;
            CurrentState = new HomeState(this, new Mission(MissionType.NOTHING_FOUND));
        }

        public override void Update(FrameEvent evt)
        {
            CurrentState.Update(evt);

            if (CurrentAnimationState != null &&
                CurrentAnimationState.Enabled &&
                !CurrentAnimationState.HasEnded)
            {
                CurrentAnimationState.AddTime(evt.timeSinceLastFrame * 0.5f);
            }

            if (Health <= 0 && CurrentState.GetType() != typeof(ExplodeState))
                CurrentState = new ExplodeState(this);

            base.Update(evt);
        }

        public override void Detach()
        {
            if (Sphere == null || SceneNode == null)
            {
                // break
            }
            SceneNode.DetachObject(Sphere);
            base.Detach();
        }

        public override void Destroy()
        {
            Win.SceneManager.DestroyEntity(Sphere);
            Win.SceneManager.DestroyAnimationState("explode");
            base.Destroy();
        }

        #region Animation Options

        public void startAnimation(AntAnimations animation)
        {
            startAnimation(animation, true);
        }

        public void startAnimation(AntAnimations animation, Boolean loop)
        {
            if (CurrentAnimationState != null)
                CurrentAnimationState.Enabled = false;

            switch (animation)
            {
                case AntAnimations.WALK:
                    CurrentAnimation = AntAnimations.WALK;
                    CurrentAnimationState = Entity.GetAnimationState("walk");
                    break;
                case AntAnimations.ATTACK:
                    CurrentAnimation = AntAnimations.ATTACK;
                    CurrentAnimationState = Entity.GetAnimationState("attack");
                    break;
                case AntAnimations.STUN:
                    CurrentAnimation = AntAnimations.STUN;
                    CurrentAnimationState = Entity.GetAnimationState("stun");
                    break;
                case AntAnimations.EXPLODE:
                    CurrentAnimation = AntAnimations.EXPLODE;
                    CurrentAnimationState = Entity.GetAnimationState("explode");
                    break;
                case AntAnimations.IDLE:
                    CurrentAnimation = AntAnimations.IDLE;
                    CurrentAnimationState = Entity.GetAnimationState("idle");
                    break;
            }

            if (CurrentAnimationState != null)
            {
                CurrentAnimationState.Enabled = true;
                CurrentAnimationState.Loop = loop;
            }
        }

        public void stopAnimation()
        {
            CurrentAnimationState.Enabled = false;
        }

        #endregion

        #region Update Methods
        private Vector3 mDirection;
        private float mDistance;

        public void resetMovement()
        {
            mDirection = Vector3.ZERO;
        }

        public float moteToDestination(FrameEvent evt, Vector3 dest)
        {
            if (mDirection == Vector3.ZERO)
            {
                mDirection = dest - SceneNode.Position;
                mDirection.y = 0;
                mDistance = mDirection.Normalise();

                Vector3 src = SceneNode.Orientation * Vector3.NEGATIVE_UNIT_Z;
                if ((1.0f + src.DotProduct(mDirection)) < 0.0001f)
                    SceneNode.Yaw(new Degree(-180).ValueRadians);
                else
                    SceneNode.Rotate(src.GetRotationTo(mDirection));
            }
            else
            {
                mDistance -= Options.walkSpeed * evt.timeSinceLastFrame;

                if (mDistance <= 0.0f)
                    mDirection = Vector3.ZERO;
                else
                {
                    Vector3 translate = (mDirection * Options.walkSpeed * evt.timeSinceLastFrame);
                    // supress y movement
                    translate.y = 0;
                    SceneNode.Translate(translate);
                }
            }

            return mDistance;
        }
        #endregion
    }
}