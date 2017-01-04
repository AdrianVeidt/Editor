namespace VectorEditor
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using Crom.Controls.Docking;
    using Tools.ToolBoxes;

    public partial class VectorEditorForm : Form
    {
        #region Fields

        static int _counter;

        DockableFormInfo _infoDocumentProperties;
        DockableFormInfo _infoFilesMain;
        DockableFormInfo _infoShapeProperties;
        DockableFormInfo _infoToolbar;
        shapeProperties _shapeProperties;
        WorkArea _svgMainFiles;
        WorkSpaceControlBox _svgProperties;
        ToolBox _toolBox;

        #endregion Fields

        #region Constructors

        public VectorEditorForm()
        {
            InitializeComponent();
            Intialize();
        }

        #endregion Constructors

        #region Methods

        public void OnZoomChanged(object sender, EventArgs e)
        {
            _svgMainFiles.OnZoomChange(sender, e);
        }

      /*  private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            var objAbout = new AboutBox();
            objAbout.ShowDialog();
        }*/

        private void BringToFrontToolStripMenuItemClick(object sender, EventArgs e)
        {
            _svgMainFiles.GetCurrentWorkForm().svgDrawForm.BringShapelToFront();
        }

        private void CopyToolStripMenuItemClick(object sender, EventArgs e)
        {
            _svgMainFiles.GetCurrentWorkForm().svgDrawForm.Copy();
        }

        private void CutToolStripMenuItemClick(object sender, EventArgs e)
        {
            _svgMainFiles.GetCurrentWorkForm().svgDrawForm.Cut();
        }

        private void DeleteToolStripMenuItemClick(object sender, EventArgs e)
        {
            _svgMainFiles.GetCurrentWorkForm().svgDrawForm.Delete();
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_svgMainFiles.CloseAll())
            {
                Close();
            }
        }

        private void GridOptionChaged(object sender, EventArgs e)
        {
            _svgMainFiles.GridOptionChanges(sender, e);
        }

        private void Intialize()
        {
            _svgMainFiles = new WorkArea();
            _svgMainFiles.PageChanged += OnPageSelectionChanged;
            _svgMainFiles.ToolDone += OnToolDone;
            _svgMainFiles.ItemsSelected += SvgMainFilesItemsSelected;

            _toolBox = new ToolBox {Size = new Size(113, 165)};
            _toolBox.ToolSelectionChanged += ToolSelectionChanged;

            _svgProperties = new WorkSpaceControlBox();
            _svgProperties.ZoomChange += OnZoomChanged;
            _svgProperties.GridOptionChange += GridOptionChaged;
            _svgProperties.WorkAreaOptionChange += SvgPropertiesWorkAreaOptionChange;

            _infoFilesMain = _docker.Add(_svgMainFiles, zAllowedDock.Fill, new Guid("a6402b80-2ebd-4fd3-8930-024a6201d001"));
            _infoFilesMain.ShowCloseButton = false;

            _infoToolbar = _docker.Add(_toolBox, zAllowedDock.All, new Guid("a6402b80-2ebd-4fd3-8930-024a6201d002"));
            _infoToolbar.ShowCloseButton = false;

            _infoDocumentProperties = _docker.Add(_svgProperties, zAllowedDock.All, new Guid("a6402b80-2ebd-4fd3-8930-024a6201d003"));
            _infoDocumentProperties.ShowCloseButton = false;

            _shapeProperties = new shapeProperties();
            _shapeProperties.PropertyChanged += ShapePropertiesPropertyChanged;
            _infoShapeProperties = _docker.Add(_shapeProperties, zAllowedDock.All, new Guid("a6402b80-2ebd-4fd3-8930-024a6201d004"));
            _infoShapeProperties.ShowCloseButton = false;
        }

        private void NewToolStripMenuItemNewClick(object sender, EventArgs e)
        {
            _svgMainFiles.AddNewPage("New:" + _counter++);
        }

        private void OnPageSelectionChanged(object sender, EventArgs e)
        {
            var tool = _svgMainFiles.GetCurrentTool();

            _svgProperties.SetZoom(_svgMainFiles.GetZoom());

            bool opt = _svgMainFiles.GetGridOption();
            int minorGrids = _svgMainFiles.GetMinorGrids();
            Size gridSize = _svgMainFiles.GetCurrentWorkForm().svgDrawForm.GetWorkAreaSize();
            String desc = _svgMainFiles.GetCurrentWorkForm().svgDrawForm.GetSvgDescription();

            _svgProperties.SetGridOption(opt, minorGrids, gridSize, desc);
            _toolBox.SetToolSelection(tool);
        }

        private void OnToolDone(object sender, EventArgs e)
        {
            _toolBox.SetToolSelection(DrawTools.DrawArea.DrawToolType.Pointer);
            if (((DrawTools.DrawArea)sender).GraphicsList.SelectionCount == 0)
            {
                _shapeProperties.propertyGrid.SelectedObject = null;
            }
        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {        
            var flgOpenFileDialog = new OpenFileDialog();
            flgOpenFileDialog.Filter = @"SVG files (*.svg)|*.svg|All files (*.*)|*.*";

            if (flgOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                _svgMainFiles.OpenDocument(flgOpenFileDialog.FileName);
            }
        }

        private void PasteToolStripMenuItemClick(object sender, EventArgs e)
        {
            _svgMainFiles.GetCurrentWorkForm().svgDrawForm.Paste();
        }

        private void RedoToolStripMenuItemClick(object sender, EventArgs e)
        {
            _svgMainFiles.GetCurrentWorkForm().svgDrawForm.Redo();
        }

        private void SaveToolStripMenuItemClick(object sender, EventArgs e)
        {
            var dlgSaveFileDialog = new SaveFileDialog();
            dlgSaveFileDialog.Filter = @"SVG files (*.svg)|*.svg|All files (*.*)|*.*";
            if (dlgSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _svgMainFiles.SaveDocument(dlgSaveFileDialog.FileName);
            }
        }

        private void SendBackToolStripMenuItemClick(object sender, EventArgs e)
        {
            _svgMainFiles.GetCurrentWorkForm().svgDrawForm.SendShapeToBack();
        }

        void ShapePropertiesPropertyChanged(object sender, PropertyValueChangedEventArgs e)
        {
            _svgMainFiles.PropertyChanged(e.ChangedItem, e.OldValue);
        }

        void SvgMainFilesItemsSelected(object sender, MouseEventArgs e)
        {
            int i = 0;
            var selectedItems = (List<Draw.DrawObject>)sender;
            var obj = new object[selectedItems.Count];
            foreach (Draw.DrawObject dob in selectedItems)
            {
                obj[i++] = dob;
            }

            if (selectedItems.Count > 0)
            {
                _shapeProperties.propertyGrid.SelectedObjects = obj;
            }
        }

        private void SvgMainShown(object sender, EventArgs e)
        {
            _docker.DockForm(_infoToolbar, DockStyle.Left, zDockMode.Inner);
            _docker.DockForm(_infoFilesMain, DockStyle.Fill, zDockMode.Inner);
            _docker.DockForm(_infoDocumentProperties, DockStyle.Right, zDockMode.Inner);
            _docker.DockForm(_infoShapeProperties,_infoToolbar, DockStyle.Bottom, zDockMode.Outer);
            _svgMainFiles.AddNewPage("New:" + _counter++);
        }

        void SvgPropertiesWorkAreaOptionChange(object sender, WorkSpaceControlBox.ControlBoxEventArgs e)
        {
            _svgMainFiles.GetCurrentWorkForm().svgDrawForm.SetDrawAreaProperties(e.Size, e.Description);
        }

        private void ToolSelectionChanged(object sender, EventArgs e)
        {
            _svgMainFiles.SetTool((String)sender);
        }

        private void ToolStripButtonCloseClick(object sender, EventArgs e)
        {
            _svgMainFiles.CloseActiveDocument();
        }

        private void TsSelectAllClick(object sender, EventArgs e)
        {
            _svgMainFiles.GetCurrentWorkForm().svgDrawForm.SelectAll();
        }

        private void UndoToolStripMenuItemClick(object sender, EventArgs e)
        {
            _svgMainFiles.GetCurrentWorkForm().svgDrawForm.Undo();
        }

        private void UnSelectAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            _svgMainFiles.GetCurrentWorkForm().svgDrawForm.UnSelectAll();
        }

        #endregion Methods

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _svgMainFiles.SaveDocument("");
        }
    }
}