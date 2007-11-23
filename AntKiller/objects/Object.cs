using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using MogreFramework;

namespace AntKiller
{
  abstract class Object
  {
    #region Properties    
    private RaySceneQuery raySceneQuery;   

    protected OgreWindow win;
    public OgreWindow Win
    {
      get { return win; }
    }

    protected int id;
    public int ID
    {
      get { return id; }
    }

    protected String name;
    public String Name
    {
      get { return name; }
    }

    protected Entity entity;
    public Entity Entity
    {
      get { return entity; }
    }

    private SceneNode sceneNode;
    public SceneNode SceneNode
    {
      get { return sceneNode; }
    }    
    #endregion

    public Object(OgreWindow win, SceneNode sceneNode)
    {
      this.win = win;
      this.sceneNode = sceneNode;
      this.raySceneQuery = this.win.SceneManager.CreateRayQuery(new Ray());      
    }

    #region Update Methods
    private void UpdateHeight()
    {
      Vector3 rOrigin = new Vector3(this.SceneNode.Position.x, 1000.0f, this.SceneNode.Position.z);
      Vector3 rDirection = Vector3.NEGATIVE_UNIT_Y;

      this.raySceneQuery.Ray = new Ray(rOrigin, rDirection);

      RaySceneQueryResult result = this.raySceneQuery.Execute();
      foreach (RaySceneQueryResultEntry entry in result)
      {
        if (entry.worldFragment != null)
        {
          this.SceneNode.Position = entry.worldFragment.singleIntersection;
        }
      }
    }

    private void UpdateBorder()
    {
      if (this.SceneNode.Position.x > Options.maxX)
        this.SceneNode.Position = new Vector3(Options.maxX, this.SceneNode.Position.y, this.SceneNode.Position.z);

      if (this.SceneNode.Position.x < Options.minX)
        this.SceneNode.Position = new Vector3(Options.minX, this.SceneNode.Position.y, this.SceneNode.Position.z);

      if (this.SceneNode.Position.z > Options.maxZ)
        this.SceneNode.Position = new Vector3(this.SceneNode.Position.x, this.SceneNode.Position.y, Options.maxZ);

      if (this.SceneNode.Position.z < Options.minZ)
        this.SceneNode.Position = new Vector3(this.SceneNode.Position.x, this.SceneNode.Position.y, Options.minZ);
    }

    public virtual void Update(FrameEvent evt)
    {
      this.UpdateHeight();
      this.UpdateBorder();
    }
    #endregion
  }
}
