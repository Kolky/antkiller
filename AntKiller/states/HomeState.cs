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
    private Mission mission;
    public Mission Mission
    {
      get { return mission; }
      set { mission = value; }
    }
    #endregion

    public HomeState(Ant ant, Mission mission)
      : base(ant)
    {
      this.Mission = mission;

      this.Ant.resetMovement();
      this.Destination = this.Ant.Colony.Home;

      if (this.Ant.CurrentAnimation != AntAnimations.WALK
        || this.Ant.CurrentAnimationState == null
        || !this.Ant.CurrentAnimationState.Enabled)
        this.Ant.startAnimation(AntAnimations.WALK, true);

      switch (this.Mission.MissionType)
      {
        case MissionType.ENEMY_KILLED:
          this.Ant.Sphere.SetMaterialName("SphereBrown");
          break;
        case MissionType.NOTHING_FOUND:
          this.Ant.Sphere.SetMaterialName("SphereOrange");
          break;
        case MissionType.FOOD_FOUND:
          this.Ant.Sphere.SetMaterialName("SphereGreen");
          break;
        case MissionType.FOOD_RETURN:
          this.Ant.Sphere.SetMaterialName("SphereGreen");
          break;
        case MissionType.LAST_FOOD:
          this.Ant.Sphere.SetMaterialName("SphereGreen");
          break;
        case MissionType.FOOD_EMPTY:
          this.Ant.Sphere.SetMaterialName("SphereOrange");
          break;
      }      

      //Console.WriteLine(this.Ant.Name + " HomeState Mission: " + Options.capital(mission.MissionType.ToString()));
    }

    public override void Update(FrameEvent evt)
    {
      float distance = this.Ant.moteToDestination(evt, this.Destination);

      if (distance <= 0.0f)
      {
        // Save mission data to colony
        switch (this.Mission.MissionType)
        {
          case MissionType.ENEMY_KILLED:
            this.Ant.Health = Options.health + (int)(this.Ant.Rank * Options.healthRankBonus);
            break;
          case MissionType.NOTHING_FOUND:
            break;
          case MissionType.FOOD_FOUND:
            if (!this.Ant.Colony.FoodStacks.Contains(this.Mission.Position))
              this.Ant.Colony.FoodStacks.Add(this.Mission.Position);            
            this.Ant.Colony.Stock++;
            break;
          case MissionType.FOOD_RETURN:
            this.Ant.Colony.Foodcheck--;
            this.Ant.Colony.Stock++;
            break;
          case MissionType.LAST_FOOD:
            if (this.Ant.Colony.FoodStacks.Contains(this.Mission.Position))
              this.Ant.Colony.FoodStacks.Remove(this.Mission.Position);
            this.Ant.Colony.Foodcheck--;
            break;
          case MissionType.FOOD_EMPTY:
            if (this.Ant.Colony.FoodStacks.Contains(this.Mission.Position))
              this.Ant.Colony.FoodStacks.Remove(this.Mission.Position);
            this.Ant.Colony.Foodcheck--;
            break;
        }

        // Assign new assignment!
        this.Ant.CurrentState = this.Ant.Colony.newAssignment(this.Ant);
      }
    }
  }
}
