using System;
using System.Linq;
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

        public Dictionary<string, Vector3> FoodStacks { get; private set; }
        #endregion

        public Colony(OgreWindow win, SceneNode sceneNode, AntColor color, Vector3 home)
            : base(win, sceneNode)
        {
            counter = 0;
            Color = color;
            Name = "Colony" + Options.capital(Color.ToString());
            Home = home;
            stock = Options.antsPerSide;
            FoodStacks = new Dictionary<string, Vector3>();

            Entity = win.SceneManager.CreateEntity(Name, "sphere.mesh");
            Entity.SetMaterialName("SphereBlack");
            SceneNode.Position = Home;
            SceneNode.AttachObject(Entity);
        }

        public State newAssignment(Ant ant)
        {
            if (FoodStacks.Count > 0)
            {
                var closestFoodStack = FoodStacks
                    .OrderBy(x => new Vector3(Home.x - x.Value.x, Home.y - x.Value.y, 0).SquaredLength)
                    .First();

                // Not everyone should get food, always people need to search
                int gatheringAnts = AntBuilder.Objects
                                              .Cast<Ant>()
                                              .Count<Ant>(x => x.Colony == this && x.CurrentState.GetType() == typeof(FoodState));
                if (gatheringAnts < (int)System.Math.Ceiling(counter * Options.getFoodPercentage))
                {
                    return new FoodState(ant, closestFoodStack.Key, closestFoodStack.Value);
                }
                else
                {
                    return new SearchState(ant);
                }
            }
            else
            {
                return new SearchState(ant);
            }
        }

        private float events = 0;

        public override void Update(FrameEvent evt)
        {
            if (Stock > (counter * Options.foodPerAntNeed))
            {
                AntBuilder.ToBeAdded.Add(new Ant(Win, Win.SceneManager.RootSceneNode.CreateChildSceneNode(), this));
            }

            //*
            if (Stock > 0 && events >= Options.secondsPerFood)
            {
                Stock -= (int)(Counter * Options.foodPerAntUsed);
                events = 0;
            }

            if (Stock <= 0)
            {
                foreach (Ant ant in AntBuilder.Objects.Where(x => x.GetType() == typeof(Ant)).Cast<Ant>())
                {
                    if (ant.Colony.Name == Name)
                    {
                        ant.Health -= Options.random.Next(1, ant.Rank);
                    }
                }
                Console.WriteLine(Name + " is low on stock and ants are losing health");
            }
            //*/

            events += evt.timeSinceLastFrame;
            base.Update(evt);
        }
    }
}