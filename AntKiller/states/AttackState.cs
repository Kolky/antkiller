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
        public Ant Enemy { get; private set; }
        #endregion

        public AttackState(Ant ant, Ant enemy)
            : base(ant)
        {
            Enemy = enemy;

            if (Ant.CurrentAnimation != AntAnimations.ATTACK ||
                Ant.CurrentAnimationState == null ||
                !Ant.CurrentAnimationState.Enabled)
            {
                Ant.startAnimation(AntAnimations.ATTACK, true);
            }

            Ant.Sphere.SetMaterialName("SphereRed");

            //Console.WriteLine(Ant.Name + " AttackState Enemy: " + Enemy.Name);
        }

        public override void Update(FrameEvent evt)
        {
            int enemyDmg = Options.random.Next(1, Enemy.Rank + 1);
            Ant.Health = Ant.Health - enemyDmg;

            int antDmg = Options.random.Next(1, Ant.Rank + 1);
            Enemy.Health = Enemy.Health - antDmg;

            //Console.WriteLine(Ant.Name + " Damages " + Enemy.Name + " For " + antDmg);

            if (Enemy.Health <= 0)
            {
                Ant.Rank++;
                Enemy.CurrentState = new ExplodeState(Enemy);
                Ant.CurrentState = new HomeState(Ant, new Mission(MissionType.ENEMY_KILLED));
            }
            if (Ant.Health <= 0)
            {
                Enemy.Rank++;
                Ant.CurrentState = new ExplodeState(Ant);
                Enemy.CurrentState = new HomeState(Enemy, new Mission(MissionType.ENEMY_KILLED));
            }
        }
    }
}
