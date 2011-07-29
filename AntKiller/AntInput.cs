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
    /// Class that handles the Input from the Keyboard / Mouse
    /// </summary>
    class AntInput
    {
        #region Properties
        private const int INTERVAL = 17;

        private bool mLastFocus = false;
        private bool mRotating = false;
        private Point mLastLocation;
        private System.Windows.Forms.Timer mTimer;

        protected float mTrans = 10;
        protected float mRot = -0.2f;
        protected Vector3 mTranslate = Vector3.ZERO;
        protected OgreWindow mWindow = null;
        #endregion

        public AntInput(OgreWindow win)
        {
            this.mWindow = win;

            this.mWindow.KeyDown += new KeyEventHandler(HandleKeyDown);
            this.mWindow.KeyUp += new KeyEventHandler(HandleKeyUp);
            this.mWindow.MouseDown += new MouseEventHandler(HandleMouseDown);
            this.mWindow.MouseUp += new MouseEventHandler(HandleMouseUp);
            this.mWindow.Disposed += new EventHandler(win_Disposed);
            this.mWindow.LostFocus += new EventHandler(win_LostFocus);
            this.mWindow.GotFocus += new EventHandler(win_GotFocus);

            this.mTimer = new System.Windows.Forms.Timer();
            this.mTimer.Interval = INTERVAL;
            this.mTimer.Enabled = true;
            this.mTimer.Tick += new EventHandler(Timer_Tick);
        }

        #region Event Handlers
        void win_Disposed(object sender, EventArgs e)
        {
            this.mTimer.Enabled = false;
        }

        void win_GotFocus(object sender, EventArgs e)
        {
            this.mTimer.Enabled = true;
        }

        void win_LostFocus(object sender, EventArgs e)
        {
            this.mTimer.Enabled = false;
            this.mTranslate = Vector3.ZERO;
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (this.mLastFocus)
            {
                // Perform the movement
                Point delta = Cursor.Position;
                delta.X -= mLastLocation.X;
                delta.Y -= mLastLocation.Y;
                this.HandleMouseMove(delta);
            }

            this.mLastLocation = Cursor.Position;
            this.mLastFocus = this.mWindow.Focused;

            if (this.mLastFocus)
                this.mWindow.Camera.Position += this.mWindow.Camera.Orientation * this.mTranslate;
        }

        private void HandleMouseMove(Point delta)
        {
            if (this.mRotating)
            {
                this.mWindow.Camera.Yaw(new Degree(delta.X * this.mRot));
                this.mWindow.Camera.Pitch(new Degree(delta.Y * this.mRot));
            }
        }

        protected virtual void HandleKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.P:
                    AntBuilder.Pause = !AntBuilder.Pause;
                    break;
                case Keys.O:
                    AntBuilder.Attacking = !AntBuilder.Attacking;
                    break;

                case Keys.Up:
                case Keys.W:
                case Keys.Down:
                case Keys.S:
                    this.mTranslate.z = 0;
                    break;

                case Keys.Left:
                case Keys.A:
                case Keys.Right:
                case Keys.D:
                    this.mTranslate.x = 0;
                    break;

                case Keys.PageUp:
                case Keys.Q:
                case Keys.PageDown:
                case Keys.E:
                    this.mTranslate.y = 0;
                    break;

                case Keys.Escape:
                    this.mWindow.Close();
                    break;
            }
        }

        protected virtual void HandleKeyDown(object sender, KeyEventArgs e)
        {
            float amount = this.mTrans;
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.W:
                    this.mTranslate.z = -amount;
                    break;

                case Keys.Down:
                case Keys.S:
                    this.mTranslate.z = amount;
                    break;

                case Keys.Left:
                case Keys.A:
                    this.mTranslate.x = -amount;
                    break;

                case Keys.Right:
                case Keys.D:
                    this.mTranslate.x = amount;
                    break;

                case Keys.PageUp:
                case Keys.Q:
                    this.mTranslate.y = amount;
                    break;

                case Keys.PageDown:
                case Keys.E:
                    this.mTranslate.y = -amount;
                    break;
            }
        }

        protected virtual void HandleMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Cursor.Show();
                this.mRotating = false;
            }
        }

        protected virtual void HandleMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Cursor.Hide();
                this.mRotating = true;
            }
        }
        #endregion
    }
}