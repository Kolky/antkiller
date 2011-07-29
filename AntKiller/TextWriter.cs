using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using MogreFramework;

namespace AntKiller
{
    class TextWriter
    {
        private static Overlay overlay;
        private static OverlayContainer overlayContainer;

        public TextWriter(OgreWindow win)
        {
            TextWriter.overlay = OverlayManager.Singleton.Create("overlay");
            TextWriter.overlayContainer = (OverlayContainer)OverlayManager.Singleton.CreateOverlayElement("Panel", "container");
            TextWriter.overlayContainer.SetDimensions(1, 1);
            TextWriter.overlayContainer.SetPosition(0, 0);

            TextWriter.overlay.Add2D(overlayContainer);
            TextWriter.overlay.Show();
        }

        public void addTextBox(String id, String text, float x, float y, float width, float height, ColourValue color)
        {
            OverlayElement textBox = OverlayManager.Singleton.CreateOverlayElement("TextArea", id);
            textBox.SetDimensions(width, height);
            textBox.SetPosition(x, y);
            textBox.Width = width;
            textBox.Height = height;
            textBox.MetricsMode = GuiMetricsMode.GMM_PIXELS;
            textBox.SetParameter("font_name", "StarWars");
            textBox.SetParameter("char_height", "16");
            textBox.Colour = color;
            textBox.Caption = text;

            TextWriter.overlayContainer.AddChild(textBox);
        }

        public void removeTextBox(String id)
        {
            TextWriter.overlayContainer.RemoveChild(id);
            OverlayManager.Singleton.DestroyOverlayElement(id);
        }

        public void setText(String id, String text)
        {
            OverlayElement textBox = OverlayManager.Singleton.GetOverlayElement(id);
            textBox.Caption = text;
        }
    }
}