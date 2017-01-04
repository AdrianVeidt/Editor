using System.Windows.Forms;
using Draw;

namespace DrawTools
{
	public class ToolImage : ToolRectangle
	{
		public ToolImage()
		{
            Cursor = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("VectorEditor.Resources.Text.cur"));
		}

        public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
        {
            AddNewObject(drawArea, new DrawImage(e.X, e.Y));
        }

	}
}