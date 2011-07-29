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
            mWindow = win;

            mWindow.KeyDown += new KeyEventHandler(HandleKeyDown);
            mWindow.KeyUp += new KeyEventHandler(HandleKeyUp);
            mWindow.MouseDown += new MouseEventHandler(HandleMouseDown);
            mWindow.MouseUp += new MouseEventHandler(HandleMouseUp);
            mWindow.Disposed += new EventHandler(win_Disposed);
            mWindow.LostFocus += new EventHandler(win_LostFocus);
            mWindow.GotFocus += new EventHandler(win_GotFocus);

            mTimer = new System.Windows.Forms.Timer();
            mTimer.Interval = INTERVAL;
            mTimer.Enabled = true;
            mTimer.Tick += new EventHandler(Timer_Tick);
        }

        #region Event Handlers
        void win_Disposed(object sender, EventArgs e)
        {
            mTimer.Enabled = false;
        }

        void win_GotFocus(object sender, EventArgs e)
        {
            mTimer.Enabled = true;
        }

        void win_LostFocus(object sender, EventArgs e)
        {
            mTimer.Enabled = false;
            mTranslate = Vector3.ZERO;
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (mLastFocus)
            {
                // Perform the movement
                Point delta = Cursor.Position;
                delta.X -= mLastLocation.X;
                delta.Y -= mLastLocation.Y;
                HandleMouseMove(delta);
            }

            mLastLocation = Cursor.Position;
            mLastFocus = mWindow.Focused;

            if (mLastFocus)
            {
                mWindow.Camera.Position += mWindow.Camera.Orientation * mTranslate;
            }
        }

        private void HandleMouseMove(Point delta)
        {
            if (mRotating)
            {
                mWindow.Camera.Yaw(new Degree(delta.X * mRot));
                mWindow.Camera.Pitch(new Degree(delta.Y * mRot));
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
                    mTranslate.z = 0;
                    break;

                case Keys.Left:
                case Keys.A:
                case Keys.Right:
                case Keys.D:
                    mTranslate.x = 0;
                    break;

                case Keys.PageUp:
                case Keys.Q:
                case Keys.PageDown:
                case Keys.E:
                    mTranslate.y = 0;
                    break;

                case Keys.Escape:
                    mWindow.Close();
                    break;
            }
        }

        protected virtual void HandleKeyDown(object sender, KeyEventArgs e)
        {
            float amount = mTrans;
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.W:
                    mTranslate.z = -amount;
                    break;

                case Keys.Down:
                case Keys.S:
                    mTranslate.z = amount;
                    break;

                case Keys.Left:
                case Keys.A:
                    mTranslate.x = -amount;
                    break;

                case Keys.Right:
                case Keys.D:
                    mTranslate.x = amount;
                    break;

                case Keys.PageUp:
                case Keys.Q:
                    mTranslate.y = amount;
                    break;

                case Keys.PageDown:
                case Keys.E:
                    mTranslate.y = -amount;
                    break;
            }
        }

        protected virtual void HandleMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Cursor.Show();
                mRotating = false;
            }
        }

        protected virtual void HandleMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Cursor.Hide();
                mRotating = true;
            }
        }
        #endregion
    }
}