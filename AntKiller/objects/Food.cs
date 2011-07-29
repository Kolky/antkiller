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
                SceneNode.Scale(new Vector3((amount * 0.05f) + 0.5f));
                //Console.WriteLine(Name + " left: " + value);
            }
        }
        #endregion

        public Food(OgreWindow win, SceneNode sceneNode, Vector3 position)
            : base(win, sceneNode)
        {
            ID = Options.counter++;
            Name = String.Concat("Food", ID);
            Amount = Options.random.Next(Options.foodMin, Options.foodMax);
            SceneNode.Position = position;

            Entity = win.SceneManager.CreateEntity(Name, "sphere.mesh");
            Entity.SetMaterialName("SphereWhite");
            SceneNode.AttachObject(Entity);
        }

        public override void Update(FrameEvent evt)
        {
            if (Amount <= 0)
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