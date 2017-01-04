using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VectorEditor.Tools.ToolBoxes
{
    public partial class ToolBox : Form
    {
        public String ToolSelection = "";
        public delegate void ToolSelectionChangedEventHandler(object sender, EventArgs e);
        public event ToolSelectionChangedEventHandler ToolSelectionChanged;

        public ToolBox()
        {
            InitializeComponent();
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            String ToolSelected = ((RadioButton)(sender)).Name;

            if (!((RadioButton)(sender)).Checked)
                return;

            switch (ToolSelected)
            {
                case "radioButton_Pointer":
                    ToolSelection = "Pointer";
                    break;
                case "radioButton_rectangle":
                    ToolSelection = "Rectangle";
                        break;
                case "radioButton_line":
                    ToolSelection = "Line";
                    break;
                case "radioButton_ellipse":
                    ToolSelection = "Ellipse";
                    break;
                case "radioButton_pan":
                    ToolSelection = "Pan";
                    break;
                case "radioButton_pencil":
                    ToolSelection = "Pencil";
                    break;
                case "radioButton_text":
                    ToolSelection = "Text";
                    break;
                case "radioButton_path":
                    ToolSelection = "Path";
                    break;
                case "radioButton_image":
                    ToolSelection = "Image";
                    break;
            }

            if (ToolSelectionChanged != null)
                ToolSelectionChanged(ToolSelection, null);   
        }

        public void SetToolSelection(DrawTools.DrawArea.DrawToolType tool)
        {
            switch (tool)
            {
                case DrawTools.DrawArea.DrawToolType.Pointer:
                    radioButton_Pointer.Checked = true;
                    break;
                case DrawTools.DrawArea.DrawToolType.Ellipse:
                    radioButton_ellipse.Checked = true;
                    break;
                case DrawTools.DrawArea.DrawToolType.Rectangle:
                    radioButton_rectangle.Checked = true;
                    break;
                case DrawTools.DrawArea.DrawToolType.Line:
                    radioButton_line.Checked = true;
                    break;
                case DrawTools.DrawArea.DrawToolType.Pan:
                    radioButton_pan.Checked = true;
                    break;
                case DrawTools.DrawArea.DrawToolType.Polygon:
                    radioButton_pencil.Checked = true;
                    break;
                default:
                    radioButton_Pointer.Checked = true;
                    break;
            }
        }
    }
}
