namespace DrawTools
{
    using System;
    using System.Windows.Forms;
    using Draw;
using System.Drawing;

    public abstract class ToolObject : Tool
    {
        #region Properties
        protected Cursor Cursor
        {
            get; 
			set;
        }
        protected Size MinSize { get; set; }

        #endregion Properties

        #region Methods

        public override void OnMouseUp(DrawArea drawArea, MouseEventArgs e)
        {
            if(drawArea.GraphicsList[0] != null)
                drawArea.GraphicsList[0].Normalize();

            adjustForMinimumSize(drawArea);

            drawArea.Capture = false;
            IsComplete = true;
            drawArea.Refresh();
        }

        [CLSCompliant(false)]
        protected void AddNewObject(DrawArea drawArea, DrawObject o)
        {
            drawArea.GraphicsList.UnselectAll();

            o.Selected = true;
            drawArea.GraphicsList.Add(o);

            drawArea.Capture = true;
            drawArea.Refresh();

            drawArea.SetDirty();
        }

        protected virtual void adjustForMinimumSize(DrawArea drawArea)
        {
        }
        #endregion Methods
    }
}