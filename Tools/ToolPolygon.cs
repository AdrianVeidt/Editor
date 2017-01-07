namespace DrawTools
{
    using System;
    using System.Drawing;
    using System.Reflection;
    using System.Windows.Forms;

    using Draw;

    using SVGLib;

    public class ToolPolygon : ToolObject
    {
        #region Fields

        private const int MinDistance = 15*15;

        private int _lastX, _lastY;
        private DrawPolygon _newPolygon;

        #endregion Fields

        #region Constructors

        public ToolPolygon()
        {
            Cursor = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("VectorEditor.Tools.Resources.Pencil.cur"));
        }

        #endregion Constructors

        #region Methods

        public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
        {
            // создаем новый полигон, добавляем к листу 
            // храним референс на него
            _newPolygon = new DrawPolygon(e.X, e.Y, e.X + 1, e.Y + 1);
            AddNewObject(drawArea, _newPolygon);
            _lastX = e.X;
            _lastY = e.Y;
        }

        public override void OnMouseMove(DrawArea drawArea, MouseEventArgs e)
        {
            drawArea.Cursor = Cursor;

            if ( e.Button != MouseButtons.Left )
                return;

            if ( _newPolygon == null )
                return;                 

            var point = new Point(e.X, e.Y);
            int distance = (e.X - _lastX)*(e.X - _lastX) + (e.Y - _lastY)*(e.Y - _lastY);
            try
            {
                if ( distance < MinDistance )
                {
                    // дистанция между 2 последними точками меньше минимума
                    _newPolygon.MoveHandleTo(point, _newPolygon.HandleCount);
                }
                else
                {
                    // добавляем новую точку
                    _newPolygon.AddPoint(point);
                    _lastX = e.X;
                    _lastY = e.Y;
                }
                drawArea.Refresh();
            }
            catch(Exception ex)
            {
                ErrH.Log("ToolPolygon", "OnMouse", ex.ToString(), ErrH._LogPriority.Info);
            }
        }

        public override void OnMouseUp(DrawArea drawArea, MouseEventArgs e)
        {
            _newPolygon = null;
            IsComplete = true;

            base.OnMouseUp (drawArea, e);
        }

        #endregion Methods
    }
}