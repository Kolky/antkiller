using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mogre;
using MogreFramework;

namespace AntKiller
{
    /// <summary>
    /// Class that builds the Scene and the Objects in it
    /// </summary>
    class AntBuilder
    {
        #region Properties
        //private Light light;
        private static List<Object> objects;
        public static List<Object> Objects
        {
            get { return objects; }
        }

        private static List<Object> toBeRemoved;
        public static List<Object> ToBeRemoved
        {
            get { return toBeRemoved; }
        }

        private static List<Object> toBeAdded;
        public static List<Object> ToBeAdded
        {
            get { return toBeAdded; }
        }

        private AntKiller antKiller;

        private static TextWriter textWriter;
        public static TextWriter TextWriter
        {
            get { return textWriter; }
        }

        private static Boolean pause;
        public static Boolean Pause
        {
            get { return pause; }
            set { pause = value; }
        }
        private static Boolean attacking;
        public static Boolean Attacking
        {
            get { return attacking; }
            set { attacking = value; }
        }

        private float time;
        private IntersectionSceneQuery intersectionSceneQuery;
        #endregion

        public AntBuilder(OgreWindow win)
        {
            AntBuilder.objects = new List<Object>();
            AntBuilder.toBeRemoved = new List<Object>();
            AntBuilder.toBeAdded = new List<Object>();

            time = 0.0f;
            antKiller = (AntKiller)win;
            antKiller.SceneCreating += new OgreWindow.SceneEventHandler(win_SceneCreating);
        }

        void win_SceneCreating(OgreWindow win)
        {
            antKiller.Root.FrameStarted += new FrameListener.FrameStartedHandler(Root_FrameStarted);

            //win.SceneManager.SetSkyDome(true, "Examples/CloudySky", 5, 8);
            win.SceneManager.SetWorldGeometry("terrain.cfg");

            createAnts(win);
            antKiller.Camera.Position = antKiller.Camera.Orientation *
                new Vector3(Options.screenX * 0.5f, Options.cameraY, Options.screenZ * 0.5f);
            antKiller.Camera.Pitch(new Degree(-90).ValueRadians);

            intersectionSceneQuery = win.SceneManager.CreateIntersectionQuery();
        }

        bool Root_FrameStarted(FrameEvent evt)
        {
            if (!AntBuilder.Pause)
            {
                foreach (Object obj in AntBuilder.Objects)
                {
                    obj.Update(evt);
                }

                while (AntBuilder.ToBeAdded.Count > 0)
                {
                    Object obj = AntBuilder.ToBeAdded[0];

                    if (!AntBuilder.Objects.Contains(obj))
                    {
                        AntBuilder.Objects.Add(obj);
                    }
                    else
                    {
                        AntBuilder.ToBeAdded.Remove(obj);
                    }
                }

                while (AntBuilder.ToBeRemoved.Count > 0)
                {
                    Object obj = AntBuilder.ToBeRemoved[0];

                    if (obj.GetType() == typeof(Food))
                    {
                        AntBuilder.Objects.Add(new Food(obj.Win, obj.Win.SceneManager.RootSceneNode.CreateChildSceneNode(), Options.randomPoint()));
                    }
                    else if (obj.GetType() == typeof(Ant))
                    {
                        ((Ant)obj).Colony.Counter--;
                    }

                    AntBuilder.Objects.Remove(obj);

                    obj.Detach();
                    //obj.Destroy();
                    AntBuilder.ToBeRemoved.Remove(obj);
                }

                time += evt.timeSinceLastFrame;
            }

            CheckCollision();

            AntBuilder.TextWriter.setText("Time", "Time: " + (int)time + (AntBuilder.Pause ? " (Paused)" : "") + (AntBuilder.Attacking ? "" : " (Attack Off)"));

            return true;
        }

        void createAnts(OgreWindow win)
        {
            AntBuilder.textWriter = new TextWriter(win);

            Colony colony1 = new Colony(
                win,
                win.SceneManager.RootSceneNode.CreateChildSceneNode(),
                AntColor.BLACK,
                new Vector3(Options.screenX * 0.25f, 0, Options.screenZ * 0.25f));
            AntBuilder.Objects.Add(colony1);

            AntBuilder.TextWriter.addTextBox(colony1.Name + "Stock", "Stock: " + colony1.Stock, 10, 10, 200, 20, new ColourValue(0.6f, 0.6f, 0.6f));
            AntBuilder.TextWriter.addTextBox(colony1.Name + "Ants", "Ants: " + colony1.Counter, 210, 10, 200, 20, new ColourValue(0.6f, 0.6f, 0.6f));

            Colony colony2 = new Colony(
                win,
                win.SceneManager.RootSceneNode.CreateChildSceneNode(),
                AntColor.RED,
                new Vector3(Options.screenX * 0.75f, 0, Options.screenZ * 0.75f));
            AntBuilder.Objects.Add(colony2);

            AntBuilder.TextWriter.addTextBox(colony2.Name + "Stock", "Stock: " + colony2.Stock, 10, 30, 200, 20, new ColourValue(1, 0, 0));
            AntBuilder.TextWriter.addTextBox(colony2.Name + "Ants", "Ants: " + colony2.Counter, 210, 30, 200, 20, new ColourValue(1, 0, 0));

            for (int i = 0; i < Options.antsPerSide; i++)
            {
                AntBuilder.Objects.Add(new Ant(win, win.SceneManager.RootSceneNode.CreateChildSceneNode(), colony1));
                AntBuilder.Objects.Add(new Ant(win, win.SceneManager.RootSceneNode.CreateChildSceneNode(), colony2));
            }
            for (int i = 0; i < Options.foodPieces; i++)
            {
                AntBuilder.Objects.Add(new Food(win, win.SceneManager.RootSceneNode.CreateChildSceneNode(), Options.randomPoint()));
            }

            AntBuilder.TextWriter.addTextBox("Time", "Time: 0", 10, 50, 200, 20, new ColourValue(0, 0, 1));
        }

        private void CheckCollision()
        {
            IntersectionSceneQueryResult result = intersectionSceneQuery.Execute();

            SceneQueryMovableIntersectionList list = result.movables2movables;
            IEnumerator<Pair<MovableObject, MovableObject>> iter = list.GetEnumerator();
            while (iter.MoveNext())
            {
                Object first = null, second = null;

                foreach (Object obj in AntBuilder.Objects)
                {
                    // Check if Moveable Objects are any Entity's of our Objects
                    if (obj.Entity == (Entity)iter.Current.first)
                        first = obj;
                    if (obj.Entity == (Entity)iter.Current.second)
                        second = obj;

                    if (first != null && second != null)
                        break;
                }

                // Did we find both Objects?
                if (first != null && second != null)
                {
                    // Did we find food?
                    if (first.GetType() == typeof(Food) ||
                        second.GetType() == typeof(Food))
                    {
                        if (first.GetType() == typeof(Ant))
                        {
                            FoundFood(first as Ant, second as Food);
                        }
                        if (second.GetType() == typeof(Ant))
                        {
                            FoundFood(second as Ant, first as Food);
                        }
                    }

                    // Is there interaction between Ants?
                    if (first.GetType() == typeof(Ant) &&
                        second.GetType() == typeof(Ant))
                    {
                        AntsInteract(first as Ant, second as Ant);
                    }
                }
            }
        }

        private void FoundFood(Ant ant, Food food)
        {
            if (ant.CurrentState.GetType() != typeof(HomeState))
            {
                food.Amount--;
                if (food.Amount > 0)
                {
                    if (ant.CurrentState.GetType() == typeof(SearchState) ||
                        ant.CurrentState.GetType() == typeof(BackState))
                    {
                        ant.CurrentState = new HomeState(ant,
                            new Mission(MissionType.FOOD_FOUND, food.Name, food.SceneNode.Position));
                    }
                    else
                    {
                        ant.CurrentState = new HomeState(ant,
                            new Mission(MissionType.FOOD_RETURN, food.Name, food.SceneNode.Position));
                    }
                }
                else if (food.Amount == 0)
                {
                    ant.CurrentState = new HomeState(ant,
                        new Mission(MissionType.LAST_FOOD, food.Name, food.SceneNode.Position));
                }
                else
                {
                    ant.CurrentState = new HomeState(ant,
                        new Mission(MissionType.FOOD_EMPTY, food.Name, food.SceneNode.Position));
                }
            }
        }

        private void AntsInteract(Ant first, Ant second)
        {
            if (first.Colony == second.Colony)
            {
                //Console.WriteLine("Friendly Interaction {0}, {1}", first.CurrentState.GetType().Name, second.CurrentState.GetType().Name);
                if (first.CurrentState.GetType() == typeof(HomeState) ||
                    second.CurrentState.GetType() == typeof(HomeState))
                {
                    if(first.CurrentState.GetType() == typeof(FoodState))
                    {
                        ExchangeFoodMessage(second, first);
                    }
                    if(second.CurrentState.GetType() == typeof(FoodState))
                    {
                        ExchangeFoodMessage(first, second);
                    }
                }
            }
            else if (AntBuilder.Attacking)
            {
                // None are exploding?
                if (first.CurrentState.GetType() != typeof(ExplodeState) &&
                    second.CurrentState.GetType() != typeof(ExplodeState))
                {
                    if (first.CurrentState.GetType() != typeof(AttackState))
                    {
                        first.CurrentState = new AttackState(first, second);
                    }
                    if (second.CurrentState.GetType() != typeof(AttackState))
                    {
                        second.CurrentState = new AttackState(second, first);
                    }
                }
            }
        }

        private void ExchangeFoodMessage(Ant home, Ant food)
        {
            Mission mission = (home.CurrentState as HomeState).Mission;
            FoodState foodAntState = food.CurrentState as FoodState;
            if ((mission.MissionType == MissionType.FOOD_EMPTY ||
                mission.MissionType == MissionType.LAST_FOOD) &&
                mission.ObjectName == foodAntState.ObjectName)
            {
                //Console.WriteLine(home.Name + " to " + food.Name + ": 'food is gone'!");
                food.CurrentState = new HomeState(food, new Mission(MissionType.FOOD_EMPTY, foodAntState.ObjectName, food.SceneNode.Position));
            }
        }
    }
}