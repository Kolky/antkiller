using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mogre;
using MogreFramework;

namespace AntKiller
{
  /// <summary>
  /// Class that handles all the properties / actions from the Window
  /// </summary>
  class AntKiller : OgreWindow
  {
    public AntKiller()
    {
      new AntInput(this);
    }

    protected override void CreateSceneManager()
    {
      this.SceneManager = this.Root.CreateSceneManager(SceneType.ST_EXTERIOR_CLOSE);
    }
  }
}