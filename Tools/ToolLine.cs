namespace DrawTools
{
    using System.Drawing;
    using System.Reflection;
    using System.Windows.Forms;

    using Draw;

    public class ToolLine : ToolObject
    {
        #region Constructors

        public ToolLine()
        {
            Cursor = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("VectorEditor.Tools.Resources.Line.cur"));
        }

        #endregion Constructors

        #region Methods

        public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
        {
            AddNewObject(drawArea, new DrawLine(e.X, e.Y, e.X + 1, e.Y + 1));
            IsComplete = true;
        }

        public override void OnMouseMove(DrawArea drawArea, MouseEventArgs e)
        {
            drawArea.Cursor = Cursor;
            if ( e.Button == MouseButtons.Left )
            {
                var point = new Point(e.X, e.Y);
                drawArea.GraphicsList[0].MoveHandleTo(point, 2);
                drawArea.Refresh();
            }
        }

        #endregion Methods
    }
}