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
        public MissionType MissionType { get; private set; }
        public Vector3 Position { get; private set; }
        #endregion

        public Mission(MissionType missionType)
            : this(missionType, Vector3.ZERO)
        {
        }

        public Mission(MissionType missionType, Vector3 position)
        {
            MissionType = missionType;
            Position = position;
        }
    }
}