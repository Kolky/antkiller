using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using MogreFramework;

namespace AntKiller
{
    abstract class State
    {
        #region Properties
        public Ant Ant { get; private set; }
        public Vector3 Destination { get; protected set; }
        public float Speed { get; protected set; }
        #endregion

        public State(Ant ant, float speed)
        {
            Ant = ant;
            Speed = speed;
        }

        public abstract void Update(FrameEvent evt);
    }
}