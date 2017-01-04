namespace VectorEditor
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public partial class WorkSpaceControlBox : Form
    {
        #region Constructors

        public WorkSpaceControlBox()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Delegates

        public delegate void OnGridOptionChange(object sender, EventArgs e);

        public delegate void OnWorkAreaOptionChange(object sender, ControlBoxEventArgs e);

        public delegate void OnZoomChange(object sender, EventArgs e);

        #endregion Delegates

        #region Events

        public event OnGridOptionChange GridOptionChange;

        public event OnWorkAreaOptionChange WorkAreaOptionChange;

        public event OnZoomChange ZoomChange;

        #endregion Events

        #region Methods

        public void SetGridOption(bool isGridOn, int minorGrid, Size drawAreaSize, String description)
        {
            checkBox_Grid.Checked = isGridOn;
            numericUpDown_minorGrids.Value = minorGrid;
            numHeight.Value = drawAreaSize.Height;
            numWidth.Value = drawAreaSize.Width;
            textBox_description.Text = description;
        }

        public void SetZoom(float f)
        {
            if((f >= trackBarZoom.Minimum) && (f <= trackBarZoom.Maximum))
            trackBarZoom.Value = (int)f;
        }

        private void ButtonNoZoomClick(object sender, EventArgs e)
        {
            trackBarZoom.Value = 1;
        }

        private void ButtonZoominClick(object sender, EventArgs e)
        {
            if (trackBarZoom.Value < 10)
                trackBarZoom.Value++;
        }

        private void ButtonZoomoutClick(object sender, EventArgs e)
        {
            if (trackBarZoom.Value > 1)
                trackBarZoom.Value--;
        }

        private void CheckBoxGridCheckedChanged(object sender, EventArgs e)
        {
            if(GridOptionChange!= null)
                GridOptionChange(sender, e);
        }

        private void FillControlboxEventArgs(ref ControlBoxEventArgs ev)
        {
            Size newSize = ev.Size;
            newSize.Height = (int)numHeight.Value;
            newSize.Width = (int)numWidth.Value;
            ev.Description = textBox_description.Text;
            ev.Size = newSize;
        }

        private void NumericUpDownMinorGridsValueChanged(object sender, EventArgs e)
        {
            if (GridOptionChange != null)
                GridOptionChange(sender, e);
        }

        private void NumHeightValueChanged(object sender, EventArgs e)
        {
            var ev = new ControlBoxEventArgs();
            FillControlboxEventArgs(ref ev);
            if (WorkAreaOptionChange != null)
                WorkAreaOptionChange(sender, ev);
        }

        private void NumWidthValueChanged(object sender, EventArgs e)
        {
            var ev = new ControlBoxEventArgs();
            FillControlboxEventArgs(ref ev);
            if(WorkAreaOptionChange != null)
                WorkAreaOptionChange(sender, ev);
        }

        private void TrackBarZoomValueChanged(object sender, EventArgs e)
        {
            label_Zoom.Text = trackBarZoom.Value + @"X";
            ZoomChange(sender, e);
        }

        private void textBox_description_TextChanged(object sender, EventArgs e)
        {
            var ev = new ControlBoxEventArgs();
            FillControlboxEventArgs(ref ev);
            if (WorkAreaOptionChange != null)
                WorkAreaOptionChange(sender, ev);
        }

        #endregion Methods

        #region Nested Types

        public class ControlBoxEventArgs : EventArgs
        {
            #region Constructors

            public ControlBoxEventArgs()
            {
                Description = String.Empty;
            }

            #endregion Constructors

            #region Properties

            public string Description
            {
                get; set;
            }

            public Size Size
            {
                get; set;
            }

            #endregion Properties
        }

        #endregion Nested Types

    }
}