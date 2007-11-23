using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using MogreFramework;

namespace AntKiller
{
  class Food : Object
  {
    #region Properties
    private int amount;
    public int Amount
    {
      get { return amount; }
      set
      {        
        amount = value;

        this.SceneNode.Scale(new Vector3((amount * 0.05f) + 0.5f));
        //Console.WriteLine(this.Name + " left: " + value);
      }
    }
    #endregion

    public Food(OgreWindow win, SceneNode sceneNode, Vector3 position)
      : base(win, sceneNode)
    {
      this.id = Options.counter++;
      this.name = String.Concat("Food", this.ID);
      this.Amount = Options.random.Next(Options.foodMin, Options.foodMax);
      this.SceneNode.Position = position;

      this.entity = win.SceneManager.CreateEntity(this.Name, "sphere.mesh");
      this.Entity.SetMaterialName("SphereWhite");
      this.SceneNode.AttachObject(this.Entity);
    }

    public override void Update(FrameEvent evt)
    {
      if (this.Amount <= 0)
      {
        if (!AntBuilder.ToBeRemoved.Contains(this))
          AntBuilder.ToBeRemoved.Add(this);
      }
      else
      {
        base.Update(evt);
      }
    }
  }
}
