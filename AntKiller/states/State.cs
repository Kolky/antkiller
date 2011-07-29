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
        #endregion

        public State(Ant ant)
        {
            Ant = ant;
        }

        public abstract void Update(FrameEvent evt);
    }
}