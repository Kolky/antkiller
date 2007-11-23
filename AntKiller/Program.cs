using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mogre;
using MogreFramework;

namespace AntKiller
{
	/// <summary>
	/// Class that starts the program and handles overall exceptions
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				AntKiller antKiller = new AntKiller();
				new AntBuilder(antKiller);
				antKiller.Go();
			}
			catch (System.Runtime.InteropServices.SEHException)
			{
				if (OgreException.IsThrown)
					MessageBox.Show(OgreException.LastException.FullDescription, "An Ogre exception has occurred!");
				else
					throw;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}
	}
}
