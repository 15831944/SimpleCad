﻿using System;
using System.Drawing;

namespace SimpleCAD
{
    public abstract class Drawable
    {
        public virtual OutlineStyle OutlineStyle { get; set; }
        public virtual FillStyle FillStyle { get; set; }

        public abstract void Draw(DrawParams param);
        public abstract Extents GetExtents();
        public abstract void TransformBy(Matrix2D transformation);
        public virtual bool Contains(PointF pt) { return GetExtents().Contains(pt); }
        public virtual bool Visible { get; set; }

        protected Drawable()
        {
            OutlineStyle = OutlineStyle.Default;
            FillStyle = FillStyle.Default;
            Visible = true;
        }
    }
}