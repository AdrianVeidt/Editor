namespace DrawTools
{
    using System.Reflection;
    using System.Windows.Forms;

    using Draw;
    using System.Drawing;


    public class ToolText : ToolRectangle
    {
        #region Constructors

        public ToolText()
        {
            Cursor = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("VectorEditor.Tools.Resources.Text.cur"));
            MinSize = new System.Drawing.Size(40, 20);
        }

        #endregion Constructors

        #region Methods

        public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
        {
            AddNewObject(drawArea, new DrawText(e.X, e.Y));
        }

        protected override void adjustForMinimumSize(DrawArea drawArea)
        {
            var objectAdded = (DrawText)drawArea.GraphicsList[0];
            Rectangle rect;

            rect = objectAdded.Rect;

            if (MinSize.Width > 0)
            {
                if (objectAdded.Rect.Width < MinSize.Width)
                {
                    rect.Width = (int)(MinSize.Width * DrawObject.Zoom);
                }
            }
            if (MinSize.Height > 0)
            {
                if (objectAdded.Rect.Height < MinSize.Height)
                {
                    rect.Height = (int)(MinSize.Height * DrawObject.Zoom);
                }
            }

            objectAdded.Rect = rect;
        }

        #endregion Methods
    }
}