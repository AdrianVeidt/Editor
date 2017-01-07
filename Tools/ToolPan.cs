namespace DrawTools
{
    using System.Reflection;
    using System.Windows.Forms;

    public class ToolPan : ToolObject
    {
        #region Fields

        readonly Cursor _closedHand;
        readonly Cursor _openHand;

        #endregion Fields

        #region Constructors

        public ToolPan()
        {
            _openHand = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("VectorEditor.Tools.Resources.pan.cur"));
            _closedHand = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("VectorEditor.Tools.Resources.pan_close.cur"));
        }

        #endregion Constructors

        #region Methods

        public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
        {
            drawArea.Cursor = _closedHand;
        }

        public override void OnMouseMove(DrawArea drawArea, MouseEventArgs e)
        {
            drawArea.Cursor = e.Button == MouseButtons.Left ? _closedHand : _openHand;
        }

        public override void OnMouseUp(DrawArea drawArea, MouseEventArgs e)
        {
            Cursor = _openHand;
        }

        #endregion Methods
    }
}