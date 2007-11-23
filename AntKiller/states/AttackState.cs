using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using MogreFramework;

namespace AntKiller
{
  class AttackState : State
  {
    #region Properties
    private Ant enemy;
    public Ant Enemy
    {
      get { return enemy; }
    }
    private Boolean other;
    #endregion

    public AttackState(Ant ant, Ant enemy)
      : base(ant)
    {
      this.enemy = enemy;
      this.other = false;

      if (this.Ant.CurrentAnimation != AntAnimations.ATTACK
        || this.Ant.CurrentAnimationState == null
        || !this.Ant.CurrentAnimationState.Enabled)
        this.Ant.startAnimation(AntAnimations.ATTACK, true);

      this.Ant.Sphere.SetMaterialName("SphereRed");

      //Console.WriteLine(this.Ant.Name + " AttackState Enemy: " + this.Enemy.Name);
    }

    public override void Update(FrameEvent evt)
    {
        int enemyDmg = Options.random.Next(1, this.Enemy.Rank + 1);
        this.Ant.Health = this.Ant.Health - enemyDmg;
        this.other = false;

        int antDmg = Options.random.Next(1, this.Ant.Rank + 1);
        this.Enemy.Health = this.Enemy.Health - antDmg;
        this.other = true;

      //Console.WriteLine(this.Ant.Name + " Damages " + this.Enemy.Name + " For " + antDmg);

      if (this.Enemy.Health <= 0)
      {
        this.Ant.Rank++;
        this.Enemy.CurrentState = new ExplodeState(this.Enemy);
        this.Ant.CurrentState = new HomeState(this.Ant, new Mission(MissionType.ENEMY_KILLED));
      }
      if (this.Ant.Health <= 0)
      {
        this.Enemy.Rank++;
        this.Ant.CurrentState = new ExplodeState(this.Ant);
        this.Enemy.CurrentState = new HomeState(this.Enemy, new Mission(MissionType.ENEMY_KILLED));
      }
    }
  }
}
