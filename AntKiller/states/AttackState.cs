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
            : base(ant, 0)
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
            int dmg = Options.random.Next(1, Ant.Rank * Options.damagePerRank);
            Enemy.Health -= dmg;

            if (Enemy.Health <= 0)
            {
                Ant.Rank++;
                Ant.CurrentState = new HomeState(Ant, new Mission(MissionType.ENEMY_KILLED));
            }
        }
    }
}
