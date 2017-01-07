namespace DrawTools
{
    using System.Reflection;
    using System.Windows.Forms;

    using Draw;

    public class ToolEllipse : ToolRectangle
    {
        #region Constructors

        public ToolEllipse()
        {
            Cursor = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("VectorEditor.Tools.Resources.Ellipse.cur"));
        }

        #endregion Constructors

        #region Methods

        public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
        {
            AddNewObject(drawArea, new DrawEllipse(e.X, e.Y, 1, 1));
        }

        #endregion Methods
    }
}