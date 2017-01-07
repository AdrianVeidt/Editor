namespace DrawTools
{
    using System.Drawing;
    using System.Reflection;
    using System.Windows.Forms;

    using Draw;

    public class ToolPath : ToolObject
    {
        #region Fields

        private DrawPath _newPath;
        bool _startPathDraw = true;

        #endregion Fields

        #region Constructors

        public ToolPath()
        {
            Cursor = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("VectorEditor.Tools.Resources.Pencil.cur"));
        }

        #endregion Constructors

        #region Methods

        public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ToolActionCompleted();
                return;
            }

            if (_startPathDraw)
            {
                _newPath = new DrawPath(e.X, e.Y);
                AddNewObject(drawArea, _newPath);
                _startPathDraw = false;
                IsComplete = false;
            }
            else
            {
                _newPath.AddPoint(e.Location);
            }
        }
        public override void OnMouseMove(DrawArea drawArea, MouseEventArgs e)
        {
            drawArea.Cursor = Cursor;
            if (e.Button == MouseButtons.Left)
            {
                var point = new Point(e.X, e.Y);
                _newPath.MoveHandleTo(point, _newPath.HandleCount);
                drawArea.Refresh();
            }
        }

        public override void OnMouseUp(DrawArea drawArea, MouseEventArgs e)
        {
        }

        public override void ToolActionCompleted()
        {
            if(_newPath != null)
            _newPath.CloseFigure();
            _startPathDraw = true;
            IsComplete = true;
            _newPath = null;
        }

        #endregion Methods
    }
}