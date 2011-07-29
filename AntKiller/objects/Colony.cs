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
        public AntColor Color { get; private set; }
        public Vector3 Home { get; private set; }

        private int stock;
        public int Stock
        {
            get { return stock; }
            set
            {
                stock = value;
                AntBuilder.TextWriter.setText(Name + "Stock", "Stock: " + value);
            }
        }

        private int counter;
        public int Counter
        {
            get { return counter; }
            set
            {
                counter = value;
                AntBuilder.TextWriter.setText(Name + "Ants", "Ants: " + value);
            }
        }

        public int Foodcheck { get; set; }
        public List<Vector3> FoodStacks { get; private set; }
        #endregion

        public Colony(OgreWindow win, SceneNode sceneNode, AntColor color, Vector3 home)
            : base(win, sceneNode)
        {
            counter = 0;
            Color = color;
            Name = "Colony" + Options.capital(Color.ToString());
            Home = home;
            stock = Options.antsPerSide * Options.foodPerAntNeed;
            FoodStacks = new List<Vector3>();

            Entity = win.SceneManager.CreateEntity(Name, "sphere.mesh");
            Entity.SetMaterialName("SphereBlack");
            SceneNode.Position = Home;
            SceneNode.AttachObject(Entity);
        }

        public State newAssignment(Ant ant)
        {
            if (FoodStacks.Count > 0)
            {
                Vector3 closestFoodStack = Vector3.ZERO;
                float checkDistance = 0f;

                foreach (Vector3 vector in FoodStacks)
                {
                    if (checkDistance == 0f)
                    {
                        Vector3 temp = Home - vector;
                        temp.y = 0;
                        checkDistance = temp.Normalise();
                        closestFoodStack = vector;
                    }
                    else
                    {
                        Vector3 temp = Home - vector;
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
                if (Foodcheck < (int)System.Math.Ceiling(counter * Options.getFoodPercentage))
                {
                    Foodcheck++;
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
            if (Stock > (counter * Options.foodPerAntNeed))
            {
                AntBuilder.ToBeAdded.Add(new Ant(Win, Win.SceneManager.RootSceneNode.CreateChildSceneNode(), this));
            }

            /*
            if (Stock > 0 && events >= Options.secondsPerFood)
            {
              Stock -= Counter * Options.foodPerAntUsed;
              events = 0;
            }

            if (Stock <= 0)
            {
              foreach (Object obj in AntBuilder.Objects)
              {
                if (obj.GetType() == typeof(Ant))
                {
                  if (((Ant)obj).Colony.Name == Name)
                  {
                    ((Ant)obj).Health -= Options.random.Next(1, ((Ant)obj).Rank);
                  }
                }
              }
              Console.WriteLine(Name + " is low on stock and ants are losing health");
            }
            */

            events += evt.timeSinceLastFrame;
            base.Update(evt);
        }
    }
}