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

        private static float GetMissionSpeed(MissionType missionType)
        {
            switch (missionType)
            {
                case MissionType.ENEMY_KILLED:
                    return Options.returnSpeed;
                case MissionType.FOOD_FOUND:
                case MissionType.FOOD_RETURN:
                case MissionType.LAST_FOOD:
                    return Options.carrySpeed;
                case MissionType.NOTHING_FOUND:
                case MissionType.FOOD_EMPTY:
                    return Options.walkSpeed;
                default:
                    return 0;
            }
        }

        public HomeState(Ant ant, Mission mission)
            : base(ant, GetMissionSpeed(mission.MissionType))
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
                        if (!Ant.Colony.FoodStacks.ContainsKey(Mission.ObjectName))
                        {
                            Ant.Colony.FoodStacks.Add(Mission.ObjectName, Mission.Position);
                        }
                        Ant.Colony.Stock++;
                        break;
                    case MissionType.FOOD_RETURN:
                        Ant.Colony.Stock++;
                        break;
                    case MissionType.LAST_FOOD:
                    case MissionType.FOOD_EMPTY:
                        if (Ant.Colony.FoodStacks.ContainsKey(Mission.ObjectName))
                        {
                            Ant.Colony.FoodStacks.Remove(Mission.ObjectName);
                        }
                        break;
                }

                // Assign new assignment!
                Ant.CurrentState = Ant.Colony.newAssignment(Ant);
            }
        }
    }
}