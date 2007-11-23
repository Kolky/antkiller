using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using MogreFramework;

namespace AntKiller
{
  public enum MissionType
  {
    ENEMY_KILLED,
    NOTHING_FOUND,
    FOOD_FOUND,
    FOOD_RETURN,
    LAST_FOOD,
    FOOD_EMPTY
  };

  class Mission
  {
    #region Properties
    private MissionType missionType;
    public MissionType MissionType
    {
      get { return missionType; }
      set { missionType = value; }
    }

    private Vector3 position;
    public Vector3 Position
    {
      get { return position; }
      set { position = value; }
    }
    #endregion

    public Mission(MissionType missionType)
      : this(missionType, Vector3.ZERO)
    {
    }

    public Mission(MissionType missionType, Vector3 position)
    {
      this.MissionType = missionType;
      this.Position = position;
    }
  }
}
