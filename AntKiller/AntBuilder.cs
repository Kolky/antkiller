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
    private Light light;
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

      this.time = 0.0f;
      this.antKiller = (AntKiller)win;      
      this.antKiller.SceneCreating += new OgreWindow.SceneEventHandler(win_SceneCreating);      
    }

    void win_SceneCreating(OgreWindow win)
    {
      this.antKiller.Root.FrameStarted += new FrameListener.FrameStartedHandler(Root_FrameStarted);

      win.SceneManager.SetSkyDome(true, "Examples/CloudySky", 5, 8);
      win.SceneManager.SetWorldGeometry("terrain.cfg");

      this.createAnts(win);
      //this.antKiller.Camera.Pitch(new Degree(-35).ValueRadians);

      this.intersectionSceneQuery = win.SceneManager.CreateIntersectionQuery();
    }

    bool Root_FrameStarted(FrameEvent evt)
    {
      if (!AntBuilder.Pause)
      {
        foreach (Object obj in AntBuilder.Objects)
        {
          obj.Update(evt);
        }

        foreach (Object obj in AntBuilder.ToBeAdded)
        {
          if (!AntBuilder.Objects.Contains(obj))
            AntBuilder.Objects.Add(obj);
        }

        foreach (Object obj in AntBuilder.ToBeRemoved)
        {
          if (obj.GetType() == typeof(Food))
          {
            AntBuilder.Objects.Add(new Food(obj.Win, obj.Win.SceneManager.RootSceneNode.CreateChildSceneNode(), Options.randomPoint()));
          }
          else if (obj.GetType() == typeof(Ant))
          {
            ((Ant)obj).Colony.Counter--;
          }

          AntBuilder.Objects.Remove(obj);

          obj.SceneNode.DetachObject(obj.Entity);
          obj.Win.SceneManager.DestroyEntity(obj.Entity);
          obj.Win.SceneManager.DestroySceneNode(obj.SceneNode.Name);
        }
        AntBuilder.ToBeRemoved.Clear();

        this.time += evt.timeSinceLastFrame;
      }

      this.CheckCollision();

      AntBuilder.TextWriter.setText("Time", "Time: " + (int)this.time + (AntBuilder.Pause ? " (Paused)" : "") + (AntBuilder.Attacking ? "" : " (Attack Off)"));

      return true;
    }

    void createAnts(OgreWindow win)
    {
      Colony colony1 = new Colony(win, win.SceneManager.RootSceneNode.CreateChildSceneNode(), AntColor.BLACK, new Vector3(500, 0, 500));
      Colony colony2 = new Colony(win, win.SceneManager.RootSceneNode.CreateChildSceneNode(), AntColor.RED, new Vector3(1000, 0, 1000));

      AntBuilder.textWriter = new TextWriter(win);
      
      AntBuilder.TextWriter.addTextBox(colony1.Name + "Stock", "Stock: " + colony1.Stock, 10, 10, 200, 20, new ColourValue(0.6f, 0.6f, 0.6f));
      AntBuilder.TextWriter.addTextBox(colony1.Name + "Ants", "Ants: " + colony1.Counter, 210, 10, 200, 20, new ColourValue(0.6f, 0.6f, 0.6f));

      AntBuilder.TextWriter.addTextBox(colony2.Name + "Stock", "Stock: " + colony2.Stock, 10, 30, 200, 20, new ColourValue(1, 0, 0));
      AntBuilder.TextWriter.addTextBox(colony2.Name + "Ants", "Ants: " + colony2.Counter, 210, 30, 200, 20, new ColourValue(1, 0, 0));

      AntBuilder.Objects.Add(colony1);
      AntBuilder.Objects.Add(colony2);

      for (int i = 0; i < Options.antsPerSide; i++)
        AntBuilder.Objects.Add(new Ant(win, win.SceneManager.RootSceneNode.CreateChildSceneNode(), colony1));
      for (int i = 0; i < Options.antsPerSide; i++)
        AntBuilder.Objects.Add(new Ant(win, win.SceneManager.RootSceneNode.CreateChildSceneNode(), colony2));

      for (int i = 0; i < Options.foodPieces; i++)
        AntBuilder.Objects.Add(new Food(win, win.SceneManager.RootSceneNode.CreateChildSceneNode(), Options.randomPoint()));

      AntBuilder.TextWriter.addTextBox("Time", "Time: 0", 10, 50, 200, 20, new ColourValue(0, 0, 1));
    }

    private void CheckCollision()
    {
      IntersectionSceneQueryResult result = this.intersectionSceneQuery.Execute();

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
          // First Type is Food
          if (first.GetType() == typeof(Food))
          {
            Food food = (Food)first;

            if (second.GetType() == typeof(Ant))
            {
              Ant ant = (Ant)second;
              // Found Food
              if (ant.CurrentState.GetType() != typeof(HomeState))
              {
                food.Amount--;
                if (food.Amount > 0)
                {
                  if (ant.CurrentState.GetType() == typeof(SearchState) || ant.CurrentState.GetType() == typeof(BackState))
                    ant.CurrentState = new HomeState(ant, new Mission(MissionType.FOOD_FOUND, food.SceneNode.Position));
                  else
                    ant.CurrentState = new HomeState(ant, new Mission(MissionType.FOOD_RETURN, food.SceneNode.Position));
                }
                else
                  ant.CurrentState = new HomeState(ant, new Mission(MissionType.FOOD_EMPTY, food.SceneNode.Position));
              }
            }
          }

          // Second Type is Ant
          if (first.GetType() == typeof(Ant))
          {
            Ant ant = (Ant)first;
            if (second.GetType() == typeof(Food))
            {
              Food food = (Food)second;
              // Found Food!
              if (ant.CurrentState.GetType() != typeof(HomeState))
              {
                food.Amount--;
                if (food.Amount > 0)
                {
                  if(ant.CurrentState.GetType() == typeof(SearchState) || ant.CurrentState.GetType() == typeof(BackState))
                    ant.CurrentState = new HomeState(ant, new Mission(MissionType.FOOD_FOUND, food.SceneNode.Position));
                  else
                    ant.CurrentState = new HomeState(ant, new Mission(MissionType.FOOD_RETURN, food.SceneNode.Position));
                }
                else if(food.Amount == 0)
                  ant.CurrentState = new HomeState(ant, new Mission(MissionType.LAST_FOOD, food.SceneNode.Position));
                else
                  ant.CurrentState = new HomeState(ant, new Mission(MissionType.FOOD_EMPTY, food.SceneNode.Position));
              }
            }
            else if (second.GetType() == typeof(Ant))
            {
              // Same Colony?               
              if (ant.Colony.Name != ((Ant)second).Colony.Name)
              {
                if (AntBuilder.Attacking)
                {
                  Ant enemy = (Ant)second;
                  // None are exploding?
                  if (ant.CurrentState.GetType() != typeof(ExplodeState) && enemy.CurrentState.GetType() != typeof(ExplodeState))
                  {
                    if (ant.CurrentState.GetType() != typeof(AttackState))
                    {
                      ant.CurrentState = new AttackState(ant, enemy);
                    }
                    if (enemy.CurrentState.GetType() != typeof(AttackState))
                    {
                      enemy.CurrentState = new AttackState(enemy, ant);
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
  }
}
