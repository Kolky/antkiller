using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using MogreFramework;

namespace AntKiller
{
    class HomeState : State
    {
        #region Properties
        public Mission Mission { get; private set; }
        #endregion

        public HomeState(Ant ant, Mission mission)
            : base(ant)
        {
            Mission = mission;

            Ant.resetMovement();
            Destination = Ant.Colony.Home;

            if (Ant.CurrentAnimation != AntAnimations.WALK ||
                Ant.CurrentAnimationState == null ||
                !Ant.CurrentAnimationState.Enabled)
            {
                Ant.startAnimation(AntAnimations.WALK, true);
            }

            switch (Mission.MissionType)
            {
                case MissionType.ENEMY_KILLED:
                    Ant.Sphere.SetMaterialName("SphereBrown");
                    break;
                case MissionType.NOTHING_FOUND:
                    Ant.Sphere.SetMaterialName("SphereOrange");
                    break;
                case MissionType.FOOD_FOUND:
                    Ant.Sphere.SetMaterialName("SphereGreen");
                    break;
                case MissionType.FOOD_RETURN:
                    Ant.Sphere.SetMaterialName("SphereGreen");
                    break;
                case MissionType.LAST_FOOD:
                    Ant.Sphere.SetMaterialName("SphereGreen");
                    break;
                case MissionType.FOOD_EMPTY:
                    Ant.Sphere.SetMaterialName("SphereOrange");
                    break;
            }

            //Console.WriteLine(Ant.Name + " HomeState Mission: " + Options.capital(mission.MissionType.ToString()));
        }

        public override void Update(FrameEvent evt)
        {
            float distance = Ant.moteToDestination(evt, Destination);

            if (distance <= 0.0f)
            {
                // Save mission data to colony
                switch (Mission.MissionType)
                {
                    case MissionType.ENEMY_KILLED:
                        Ant.Health = Options.health + (int)(Ant.Rank * Options.healthRankBonus);
                        break;
                    case MissionType.NOTHING_FOUND:
                        break;
                    case MissionType.FOOD_FOUND:
                        if (!Ant.Colony.FoodStacks.Contains(Mission.Position))
                            Ant.Colony.FoodStacks.Add(Mission.Position);
                        Ant.Colony.Stock++;
                        break;
                    case MissionType.FOOD_RETURN:
                        Ant.Colony.Foodcheck--;
                        Ant.Colony.Stock++;
                        break;
                    case MissionType.LAST_FOOD:
                        if (Ant.Colony.FoodStacks.Contains(Mission.Position))
                            Ant.Colony.FoodStacks.Remove(Mission.Position);
                        Ant.Colony.Foodcheck--;
                        break;
                    case MissionType.FOOD_EMPTY:
                        if (Ant.Colony.FoodStacks.Contains(Mission.Position))
                            Ant.Colony.FoodStacks.Remove(Mission.Position);
                        Ant.Colony.Foodcheck--;
                        break;
                }

                // Assign new assignment!
                Ant.CurrentState = Ant.Colony.newAssignment(Ant);
            }
        }
    }
}