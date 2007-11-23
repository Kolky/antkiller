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
		private int health;
		public int Health
		{
			get { return health; }
			set { health = value; }
		}

		private int rank;
		public int Rank
		{
			get { return rank; }
			set { rank = value; }
		}

		private Entity sphere;
		public Entity Sphere
		{
			get { return sphere; }
		}

		private SceneNode sphereNode;
		public SceneNode SphereNode
		{
			get { return sphereNode; }
		}

    private AntAnimations currentAnimation;
    public AntAnimations CurrentAnimation
    {
      get { return currentAnimation; }
      set { currentAnimation = value; }
    }	

		private AnimationState currentAnimationState;
		public AnimationState CurrentAnimationState
		{
      get { return currentAnimationState; }
      set { currentAnimationState = value; }
		}

		private State currentState;
		public State CurrentState
		{
			get { return currentState; }
			set { currentState = value; }
		}

    private Colony colony;
    public Colony Colony
    {
      get { return colony; }
    }	
		#endregion

    public Ant(OgreWindow win, SceneNode sceneNode, Colony colony)
      : base(win, sceneNode)
		{
      this.colony = colony;
      this.Colony.Counter++;

      this.id = Options.counter++;
			this.name = String.Concat(this.Colony.Name, "Ant", this.ID);
			this.Health = Options.health;
			this.Rank = 1;

      this.SceneNode.Position = this.Colony.Home;
			this.sphereNode = this.SceneNode;			

			// Create Ant
			this.entity = this.win.SceneManager.CreateEntity(this.Name, "ant1.mesh");
      this.sphere = this.win.SceneManager.CreateEntity(this.Name + "Sphere", "sphere.mesh");

			// Apply Color
      switch (this.Colony.Color)
			{
				case AntColor.BLACK:
					this.Entity.SetMaterialName("AntBlack");
					break;
				case AntColor.RED:
					this.Entity.SetMaterialName("AntRed");
					break;
			}			
			
			// Attach Object to Node
			this.SceneNode.AttachObject(this.Entity);
			this.SphereNode.AttachObject(this.Sphere);

			// Initialize Variables
			this.mDirection = Vector3.ZERO;
			this.CurrentState = new HomeState(this, new Mission(MissionType.NOTHING_FOUND));
		}

		public override void Update(FrameEvent evt)
		{
			this.CurrentState.Update(evt);

      if (this.CurrentAnimationState != null && this.CurrentAnimationState.Enabled)
      {
        this.CurrentAnimationState.AddTime(evt.timeSinceLastFrame * 0.5f);
      }       

      if (this.Health <= 0 &&  this.CurrentState.GetType() != typeof(ExplodeState))
        this.CurrentState = new ExplodeState(this);

      base.Update(evt);
		}

		#region Animation Options
		public void startAnimation(AntAnimations animation)
		{
			this.startAnimation(animation, true);
		}
		public void startAnimation(AntAnimations animation, Boolean loop)
		{
			if (this.CurrentAnimationState != null)
				this.CurrentAnimationState.Enabled = false;

			switch (animation)
			{
				case AntAnimations.WALK:
          this.CurrentAnimation = AntAnimations.WALK;
					this.CurrentAnimationState = this.Entity.GetAnimationState("walk");
					break;
				case AntAnimations.ATTACK:
          this.CurrentAnimation = AntAnimations.ATTACK;
					this.CurrentAnimationState = this.Entity.GetAnimationState("attack");
					break;
				case AntAnimations.STUN:
          this.CurrentAnimation = AntAnimations.STUN;
					this.CurrentAnimationState = this.Entity.GetAnimationState("stun");
					break;
				case AntAnimations.EXPLODE:
          this.CurrentAnimation = AntAnimations.EXPLODE;
					this.CurrentAnimationState = this.Entity.GetAnimationState("explode");
					break;
				case AntAnimations.IDLE:
          this.CurrentAnimation = AntAnimations.IDLE;
					this.CurrentAnimationState = this.Entity.GetAnimationState("idle");
					break;
			}

			if (this.CurrentAnimationState != null)
			{
				this.CurrentAnimationState.Enabled = true;
				this.CurrentAnimationState.Loop = loop;
			}
		}
		public void stopAnimation()
		{
			this.CurrentAnimationState.Enabled = false;
		}
		#endregion		

		#region Update Methods
		private Vector3 mDirection;
		private float mDistance;

    public void resetMovement()
    {
      this.mDirection = Vector3.ZERO;
    }
		public float moteToDestination(FrameEvent evt, Vector3 dest)
		{
			if (this.mDirection == Vector3.ZERO)
			{
				this.mDirection = dest - this.SceneNode.Position;
        this.mDirection.y = 0;
				this.mDistance = this.mDirection.Normalise();

				Vector3 src = this.SceneNode.Orientation * Vector3.NEGATIVE_UNIT_Z;
        if ((1.0f + src.DotProduct(this.mDirection)) < 0.0001f)
          this.SceneNode.Yaw(new Degree(-180).ValueRadians);
        else
          this.SceneNode.Rotate(src.GetRotationTo(mDirection));
			}
			else
			{
				this.mDistance -= Options.walkSpeed * evt.timeSinceLastFrame;

        if (mDistance <= 0.0f)
          this.mDirection = Vector3.ZERO;
        else
        {
          Vector3 translate = (mDirection * Options.walkSpeed * evt.timeSinceLastFrame);
          // supress y movement
          translate.y = 0;
          this.SceneNode.Translate(translate);
        }
			}

			return mDistance;
		}		
		#endregion
	}
}
