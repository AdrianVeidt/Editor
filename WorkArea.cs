namespace VectorEditor
{
    using System;
    using System.Windows.Forms;

    using UserControls;

    public partial class WorkArea : Form
    {
        #region Constructors

        public WorkArea()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Delegates

            public delegate void OnItemsInSelection(object sender, MouseEventArgs e);

            public delegate void OnTabPageChanged(object sender, EventArgs e);

            public delegate void OnToolDone(object sender, EventArgs e);

        #endregion Delegates

        #region Events

            public event OnItemsInSelection ItemsSelected;

            public event OnTabPageChanged PageChanged;

            public event OnToolDone ToolDone;

        #endregion Events

        #region Methods

            public void AddNewPage(String fileName)
            {
                var svgForm = new WorkspaceHolder {Dock = DockStyle.Fill, Name = fileName};
                svgForm.svgDrawForm.ToolDone += OnToolDoneComplete;
                svgForm.svgDrawForm.ItemsSelected += SvgDrawFormItemsSelected;
                tabbedView.Add(svgForm);
            }

            public void CloseActiveDocument()
            {
                if (tabbedView.Count > 0)
                {
                    var svgForm = (WorkspaceHolder)tabbedView.GetPageAt(tabbedView.SelectedIndex);
                    if (!svgForm.svgDrawForm.CheckDirty())
                    {
                        tabbedView.Remove(svgForm);
                    }
                    else
                    {
                        if ((MessageBox.Show(@"Changes have been made. Exit without Saving?", @"SVG Editor", MessageBoxButtons.YesNo) == DialogResult.Yes))
                        {
                            tabbedView.Remove(svgForm);
                        }
                    }
                }
            }

            public bool CloseAll()
            {
                bool retVal = true;
                int count = tabbedView.Count;
                if (tabbedView.Count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        var svgForm = (WorkspaceHolder)tabbedView.GetPageAt(0);
                        if (!svgForm.svgDrawForm.CheckDirty())
                        {
                            tabbedView.Remove(svgForm);
                        }
                        else
                        {
                            if ((MessageBox.Show(@"Changes have been made. Exit without Saving?", @"SVG Editor", MessageBoxButtons.YesNo) == DialogResult.Yes))
                            {
                                tabbedView.SelectedIndex = 0;
                                tabbedView.Remove(svgForm);
                            }
                            else
                            {
                                tabbedView.SelectedIndex = 0;
                                retVal = false;
                            }
                        }
                    }
                }
                return retVal;
            }

            public DrawTools.DrawArea.DrawToolType GetCurrentTool()
            {
                var svgForm = (WorkspaceHolder)tabbedView.GetPageAt(tabbedView.SelectedIndex);
                return svgForm.svgDrawForm.GetCurrentTool();
            }

            public WorkspaceHolder GetCurrentWorkForm()
            {
                return (WorkspaceHolder)tabbedView.GetPageAt(tabbedView.SelectedIndex);
            }

            public bool GetGridOption()
            {
                var svgForm = (WorkspaceHolder)tabbedView.GetPageAt(tabbedView.SelectedIndex);
                return svgForm.svgDrawForm.GetGridOption();
            }

            public int GetMinorGrids()
            {
                return GetCurrentForm().GetMinorGrids();
            }

            public WorkSpace GetCurrentForm()
            {
                return ((WorkspaceHolder)tabbedView.GetPageAt(tabbedView.SelectedIndex)).svgDrawForm;
            }

            public int GetWorkAreaSize()
            {
                return GetCurrentForm().GetMinorGrids();
            }

            public int GetZoom()
            {
                return (int)(GetCurrentForm().GetCurrentZoom());
            }

            public void GridOptionChanges(object sender, EventArgs e)
            {
                GetCurrentForm().GridOptionChanged(sender, e);
            }

            public void OnZoomChange(object sender, EventArgs e)
            {
                var tb = (TrackBar)sender;
                if (tb != null)
                {
                    GetCurrentForm().SetZoom(tb.Value);
                }
            }

            public void OpenDocument(String fileName)
            {
                var svgForm = new WorkspaceHolder {Dock = DockStyle.Fill, Name = fileName};
                svgForm.svgDrawForm.ToolDone += OnToolDoneComplete;
                svgForm.svgDrawForm.ItemsSelected += SvgDrawFormItemsSelected;
                svgForm.svgDrawForm.OpenFile(fileName);
                tabbedView.Add(svgForm);
                svgForm.Refresh();
            }

            public void PropertyChanged(GridItem itemChanged, object oldVal)
            {
                GetCurrentForm().PropertyChanged(itemChanged, oldVal);
            }

            public void SaveDocument(String fileName)
            {
                GetCurrentForm().SaveFile(fileName);
            }

            public void SetTool(String tool)
            {
                GetCurrentForm().SetTool(tool);
            }

            private void OnToolDoneComplete(object sender, EventArgs e)
            {
                ToolDone(sender, e);
            }

            void SvgDrawFormItemsSelected(object sender, MouseEventArgs e)
            {
                if (ItemsSelected != null)
                    ItemsSelected(sender, e);
            }

            private void TabbedViewPageSelectionMade(object sender, EventArgs e)
        {
            PageChanged(sender, e);
        }

        #endregion Methods
    }
}