using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mogre;
using MogreFramework;

namespace AntKiller
{
	class Colony : Object
	{
		#region Properties
    private AntColor color;
    public AntColor Color
    {
      get { return color; }
    }

    private Vector3 home;
    public Vector3 Home
    {
      get { return home; }
    }

    private int stock;
    public int Stock
    {
      get { return stock; }
      set
      {
        stock = value;
        AntBuilder.TextWriter.setText(this.Name + "Stock", "Stock: " + value);
      }
    }

    private int counter;
    public int Counter
    {
      get { return counter; }
      set
      {
        counter = value;
        AntBuilder.TextWriter.setText(this.Name + "Ants", "Ants: " + value);
      }
    }

    private int foodcheck = 0;
    public int Foodcheck
    {
      get { return foodcheck; }
      set { foodcheck = value; }
    }

    private List<Vector3> foodStacks;
    public List<Vector3> FoodStacks
    {
      get { return foodStacks; }
      set { foodStacks = value; }
    }	
		#endregion

    public Colony(OgreWindow win, SceneNode sceneNode, AntColor color, Vector3 home)
      : base(win, sceneNode)
		{
      this.counter = 0;
      this.color = color;
      this.name = "Colony" + Options.capital(this.Color.ToString()) + Options.counter++;      
      this.home = home;
      this.stock = Options.antsPerSide * Options.foodPerAntNeed;
      this.foodStacks = new List<Vector3>();

      this.entity = this.win.SceneManager.CreateEntity(this.Name, "sphere.mesh");
      this.Entity.SetMaterialName("SphereBlack");
      this.SceneNode.Position = this.Home;
      this.SceneNode.AttachObject(this.Entity);
		}

    public State newAssignment(Ant ant)
    {
      if (this.FoodStacks.Count > 0)
      {
        Vector3 closestFoodStack = Vector3.ZERO;
        float checkDistance = 0f;

        foreach (Vector3 vector in this.FoodStacks)
        {
          if (checkDistance == 0f)
          {
            Vector3 temp = this.Home - vector;
            temp.y = 0;
            checkDistance = temp.Normalise();
            closestFoodStack = vector;
          }
          else
          {
            Vector3 temp = this.Home - vector;
            temp.y = 0;
            float tempDistance = temp.Normalise();
            if (tempDistance < checkDistance)
            {
              checkDistance = tempDistance;
              closestFoodStack = vector;
            }
          }
        }       

        // Not everyone should get food, always people need to search
        if(this.foodcheck < (int)System.Math.Ceiling(this.counter * Options.getFoodPercentage))
        {
          this.foodcheck++;
          return new FoodState(ant, closestFoodStack);
        }
        else
          return new SearchState(ant);
      }
      else
        return new SearchState(ant);
    }

    private float events = 0;

    public override void Update(FrameEvent evt)
    {
      if (this.Stock > (this.counter * Options.foodPerAntNeed))
      {
        AntBuilder.ToBeAdded.Add(new Ant(this.Win, this.Win.SceneManager.RootSceneNode.CreateChildSceneNode(), this));
      }

      /*
      if (this.Stock > 0 && this.events >= Options.secondsPerFood)
      {
        this.Stock -= this.Counter * Options.foodPerAntUsed;
        this.events = 0;
      }

      if (this.Stock <= 0)
      {
        foreach (Object obj in AntBuilder.Objects)
        {
          if (obj.GetType() == typeof(Ant))
          {
            if (((Ant)obj).Colony.Name == this.Name)
            {
              ((Ant)obj).Health -= Options.random.Next(1, ((Ant)obj).Rank);
            }
          }
        }
        Console.WriteLine(this.Name + " is low on stock and ants are losing health");
      }
      */

      this.events += evt.timeSinceLastFrame;
      base.Update(evt);
    }
	}
}
