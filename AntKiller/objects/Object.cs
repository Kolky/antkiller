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
        public OgreWindow Win { get; private set; }
        public int ID { get; protected set; }
        public String Name { get; protected set; }
        public Entity Entity { get; protected set; }
        public SceneNode SceneNode { get; private set; }
        #endregion

        public Object(OgreWindow win, SceneNode sceneNode)
        {
            Win = win;
            SceneNode = sceneNode;
            raySceneQuery = Win.SceneManager.CreateRayQuery(new Ray());
        }

        #region Update Methods
        private void UpdateHeight()
        {
            Vector3 rOrigin = new Vector3(SceneNode.Position.x, 1000.0f, SceneNode.Position.z);
            Vector3 rDirection = Vector3.NEGATIVE_UNIT_Y;

            raySceneQuery.Ray = new Ray(rOrigin, rDirection);

            RaySceneQueryResult result = raySceneQuery.Execute();
            foreach (RaySceneQueryResultEntry entry in result)
            {
                if (entry.worldFragment != null)
                {
                    SceneNode.Position = entry.worldFragment.singleIntersection;
                }
            }
        }

        private void UpdateBorder()
        {
            if (SceneNode.Position.x > Options.maxX)
                SceneNode.Position = new Vector3(Options.maxX, SceneNode.Position.y, SceneNode.Position.z);

            if (SceneNode.Position.x < Options.minX)
                SceneNode.Position = new Vector3(Options.minX, SceneNode.Position.y, SceneNode.Position.z);

            if (SceneNode.Position.z > Options.maxZ)
                SceneNode.Position = new Vector3(SceneNode.Position.x, SceneNode.Position.y, Options.maxZ);

            if (SceneNode.Position.z < Options.minZ)
                SceneNode.Position = new Vector3(SceneNode.Position.x, SceneNode.Position.y, Options.minZ);
        }

        public virtual void Update(FrameEvent evt)
        {
            UpdateHeight();
            UpdateBorder();
        }

        public virtual void Detach()
        {
            SceneNode.DetachObject(Entity);
        }

        public virtual void Destroy()
        {
            Win.SceneManager.DestroyEntity(Entity);
            Win.SceneManager.DestroySceneNode(SceneNode);
        }
        #endregion
    }
}