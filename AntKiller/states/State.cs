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
		private Ant ant;
		public Ant Ant
		{
			get { return ant; }
		}
    private Vector3 destination;
    public Vector3 Destination
    {
      get { return destination; }
      set { destination = value; }
    }
		#endregion

		public State(Ant ant)
		{
			this.ant = ant;
		}

    public abstract void Update(FrameEvent evt);
	}
}
