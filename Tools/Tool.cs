namespace DrawTools
{
    using System;
    using System.Windows.Forms;

    public abstract class Tool
    {
        #region Fields

        public Boolean IsComplete;

        #endregion Fields

        #region Methods

        public virtual void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
        {
        }

        public virtual void OnMouseMove(DrawArea drawArea, MouseEventArgs e)
        {
        }

        public virtual void OnMouseUp(DrawArea drawArea, MouseEventArgs e)
        {
        }

        public virtual void ToolActionCompleted()
        {
        }

        #endregion Methods
    }
}