﻿using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System;

namespace SimpleCAD.View
{
    internal class Cursor : Drawable
    {
        public Point2D Location { get; set; }
        public TextStyle TextStyle { get; set; }
        public float TextHeight { get; set; }
        public string Message { get; set; }

        public Cursor()
        {
            // Assign the default system font by default
            TextStyle = new TextStyle("_Cursor", System.Drawing.SystemFonts.MessageBoxFont.FontFamily.Name, FontStyle.Regular);
            // Default text height in pixels
            TextHeight = 12;
        }

        public override void Draw(Renderer renderer)
        {
            var view = renderer.View;
            var doc = view.Document;

            Extents2D ex = view.GetViewport();
            Color c = doc.Settings.BackColor;
            var luma = (int)Math.Sqrt(c.R * c.R * .299 + c.G * c.G * .587 + c.B * c.B * .114);
            Style cursorStyle = new Style(luma > 130 ? Color.Black : Color.White);
            float emptyBoxSize = view.ScreenToWorld(new Vector2D(doc.Settings.PickBoxSize + 4, 0)).X / 2;
            float pickBoxSize = view.ScreenToWorld(new Vector2D(doc.Settings.PickBoxSize, 0)).X / 2;
            float pxSize = view.ScreenToWorld(new Vector2D(1, 0)).X / 2;

            // Draw cursor
            if (!doc.Editor.InputMode)
            {
                renderer.DrawLine(cursorStyle, new Point2D(ex.Xmin, Location.Y), new Point2D(Location.X - emptyBoxSize, Location.Y));
                renderer.DrawLine(cursorStyle, new Point2D(Location.X + emptyBoxSize, Location.Y), new Point2D(ex.Xmax, Location.Y));
                renderer.DrawLine(cursorStyle, new Point2D(Location.X, ex.Ymin), new Point2D(Location.X, Location.Y - emptyBoxSize));
                renderer.DrawLine(cursorStyle, new Point2D(Location.X, Location.Y + emptyBoxSize), new Point2D(Location.X, ex.Ymax));
                renderer.DrawRectangle(cursorStyle, new Point2D(Location.X - pickBoxSize, Location.Y - pickBoxSize),
                    new Point2D(Location.X + pickBoxSize, Location.Y + pickBoxSize));
            }
            else
            {
                renderer.DrawLine(cursorStyle, new Point2D(ex.Xmin, Location.Y), new Point2D(Location.X - pxSize, Location.Y));
                renderer.DrawLine(cursorStyle, new Point2D(Location.X + pxSize, Location.Y), new Point2D(ex.Xmax, Location.Y));
                renderer.DrawLine(cursorStyle, new Point2D(Location.X, ex.Ymin), new Point2D(Location.X, Location.Y - pxSize));
                renderer.DrawLine(cursorStyle, new Point2D(Location.X, Location.Y + pxSize), new Point2D(Location.X, ex.Ymax));
            }

            // Draw cursor prompt
            if (!string.IsNullOrEmpty(Message))
            {
                float height = Math.Abs(view.ScreenToWorld(new Vector2D(0, TextHeight)).Y);
                float margin = Math.Abs(view.ScreenToWorld(new Vector2D(4, 0)).X);
                float offset = Math.Abs(view.ScreenToWorld(new Vector2D(2, 0)).X);

                // position cursor prompt to lower-right of cursor by default
                float x = Location.X + margin + offset;
                float y = Location.Y - margin - offset;
                Vector2D sz = renderer.MeasureString(Message, TextStyle, height);
                Point2D lowerRight = new Point2D(ex.Xmax, ex.Ymin);
                // check if the prompt text fits into the window horizontally
                if (x + sz.X + offset > lowerRight.X)
                {
                    x = Location.X - margin - offset - sz.X;
                }
                // check if the prompt text fits into the window vertically
                if (y - sz.Y - offset < lowerRight.Y)
                {
                    y = Location.Y + margin + offset + sz.Y;
                }

                // Draw cursor prompt
                Style fore = new Style(doc.Settings.CursorPromptForeColor);
                Style back = new Style(doc.Settings.CursorPromptBackColor);
                renderer.FillRectangle(back, new Point2D(x - offset, y + offset), new Point2D(x + offset + sz.X, y - offset - sz.Y));
                renderer.DrawRectangle(fore, new Point2D(x - offset, y + offset), new Point2D(x + offset + sz.X, y - offset - sz.Y));
                renderer.DrawString(fore, new Point2D(x, y), Message, TextStyle, height, 0,
                    TextHorizontalAlignment.Left, TextVerticalAlignment.Top);
            }
        }

        public override Extents2D GetExtents()
        {
            return Extents2D.Infinity;
        }

        public override void TransformBy(Matrix2D transformation)
        {
            ;
        }
    }
}
