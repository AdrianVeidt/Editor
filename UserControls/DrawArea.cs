namespace DrawTools
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml;

    using Draw;

    using SVGLib;


    public class DrawArea : UserControl
    {
        #region Fields

        public ArrayList ChartValues = new ArrayList();
        public Boolean Dirty;
        public float ScaleX, ScaleY;
        public string Title = "Default Title";
        public float Xdivs = 2, Ydivs = 2, MajorIntervals = 100;
        public float Xorigin, Yorigin;

        private IContainer components;

        private DrawToolType _activeTool; 
        private ToolStripMenuItem _bringToFrontToolStripMenuItem;
        private ContextMenuStrip _contextMenuStrip;
        private ToolStripMenuItem _copyToolStripMenuItem;
        private ToolStripMenuItem _cutToolStripMenuItem;
        private ToolStripMenuItem _deleteToolStripMenuItem;
        private GraphicsList _graphicsList; 
        private string _mDescription = "Svg picture";
        private SizeF _mOriginalSize = new SizeF(500,400);

        private SizeF _mScale = new SizeF(1.0f,1.0f);
        private SizeF _mSizePicture = new SizeF(500,400);
        private ToolStripMenuItem _pasteToolStripMenuItem;
        private ToolStripMenuItem _selectAllToolStripMenuItem;
        private ToolStripMenuItem _sendToBackToolStripMenuItem;
        private Tool[] _tools;
        private ToolStripSeparator _toolStripSeparator1;
        private ToolStripSeparator _toolStripSeparator2;
        private ToolStripSeparator _toolStripSeparator3;
        private int _width, _height;

        public String FileName { get; set; }

        #endregion Fields

        #region Constructors

        public DrawArea()
        {
            _height = 500;
            _width = 400;
            InitializeComponent();
        }

        #endregion Constructors

        #region Enumerations

        public enum DrawToolType
        {
            Pointer,
            Rectangle,
            Ellipse,
            Line,
            Polygon,
            Bitmap,
            Text,
            Pan,
            Path,
            NumberOfDrawTools
        }

        #endregion Enumerations

        #region Delegates

            public delegate void OnItemsInSelection(object sender, MouseEventArgs e);

            public delegate void OnMousePan(object sender, MouseEventArgs e);

            public delegate void OnMouseSelectionDone(object sender, EventArgs e);

        #endregion Delegates

        #region Events

            public event OnItemsInSelection ItemsSelected;

            public event OnMousePan MousePan;

            public event OnMouseSelectionDone ToolDone;

        #endregion Events

        #region Properties

        public DrawToolType ActiveTool
        {
            get
            {
                return _activeTool;
            }
            set
            {
                if (_tools != null)
                    _tools[(int)_activeTool].ToolActionCompleted();
                _activeTool = value;
            }
        }

        public string Description
        {
            get
            {
                return _mDescription;
            }
            set
            {
                _mDescription = value;
            }
        }

        public bool DrawGrid
        {
            get; set;
        }
        public bool DrawNetRectangle
        {
            get; set;
        }

        [CLSCompliant(false)]
        public GraphicsList GraphicsList
        {
            get
            {
                return _graphicsList;
            }
            set
            {
                _graphicsList = value;
            }
        }

        public RectangleF NetRectangle
        {
            get; set;
        }

        public SizeF OldScale
        {
            get
            {
                return _mScale;
            }
            set
            {
                _mScale = value;
            }
        }

        public SizeF OriginalSize
        {
            get
            {
                return _mOriginalSize;
            }
            set
            {
                _mOriginalSize = value;
            }
        }

        public Control Owner
        {
            get; set;
        }

        public SizeF ScaleDraw
        {
            get
            {
                return _mScale;
            }
            set
            {
                _mScale = value;
            }
        }

        public SizeF SizePicture
        {
            get
            {
                return _mSizePicture;
            }
            set
            {
                _mSizePicture = value;
            }
        }

        #endregion Properties

        #region Methods

        public void DoScaling(SizeF sc)
        {
            DrawObject.Zoom = sc.Height;
            _graphicsList.Resize(sc,_mScale);
            _mScale = sc;
            _mSizePicture = new SizeF(_mScale.Width*OriginalSize.Width,
                _mScale.Height*OriginalSize.Height);
        }

        public void Draw(Graphics g)
        {
            var brush = new SolidBrush(Color.FromArgb(255, 255, 255));
            g.FillRectangle(brush, ClientRectangle);
            var pen = new Pen(Color.FromArgb(0, 0, 255), 1);
            g.DrawRectangle(pen,0,0,SizePicture.Width,SizePicture.Height);
            if ( _graphicsList != null )
            {
                _graphicsList.Draw(g);
            }
            brush.Dispose();
        }

        public void DrawNetSelection(Graphics g)
        {
            if ( ! DrawNetRectangle )
                return;
            var r = new Rectangle(Convert.ToInt32(NetRectangle.X),Convert.ToInt32(NetRectangle.Y),
                Convert.ToInt32(NetRectangle.Width),Convert.ToInt32(NetRectangle.Height));
            ControlPaint.DrawFocusRectangle(g, r, Color.Black, Color.Transparent);
        }

        public void Initialize(Control owner)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            Owner = owner;
            _activeTool = DrawToolType.Pointer;

            _graphicsList = new GraphicsList();

            _tools = new Tool[(int)DrawToolType.NumberOfDrawTools];
            _tools[(int)DrawToolType.Pointer] = new ToolPointer();
            _tools[(int)DrawToolType.Rectangle] = new ToolRectangle();
            _tools[(int)DrawToolType.Ellipse] = new ToolEllipse();
            _tools[(int)DrawToolType.Line] = new ToolLine();
            _tools[(int)DrawToolType.Polygon] = new ToolPolygon();
            _tools[(int)DrawToolType.Text] = new ToolText();
            _tools[(int)DrawToolType.Bitmap] = new ToolImage();
            _tools[(int)DrawToolType.Pan] = new ToolPan();
            _tools[(int)DrawToolType.Path] = new ToolPath();

            Graphics g = Owner.CreateGraphics();
            DrawObject.Dpi = new PointF(g.DpiX,g.DpiY);
        }

        public bool LoadFromXml(XmlTextReader reader)
        {
            ErrH.Log("DrawArea", "LoadFromXML", "", ErrH._LogPriority.Info);
            _graphicsList.Clear();
            var svg = new SvgDoc();
            if (!svg.LoadFromFile(reader))
                return false;
            SvgRoot root = svg.GetSvgRoot();

            if (root == null)
                return false;
            try
            {
                SizePicture = new SizeF(DrawObject.ParseSize(root.Width,DrawObject.Dpi.X),
                    DrawObject.ParseSize(root.Height,DrawObject.Dpi.Y));
            }
            catch
            {
            }
            _mOriginalSize = SizePicture;
            SvgElement ele = root.getChild();
            _mScale = new SizeF(1,1);
            if (ele != null)
                _graphicsList.AddFromSvg(ele);

            Description = _graphicsList.Description;
            return true;
        }

        public void MkResize()
        {
            SizeF oldscale = _mScale;
            _mScale.Width = _width/_mOriginalSize.Width;
            _mScale.Height = _height/_mOriginalSize.Height;
            _graphicsList.Resize(_mScale,oldscale);
            SizePicture = new SizeF(DrawObject.RecalcFloat(SizePicture.Width, _mScale.Width,oldscale.Width),
                DrawObject.RecalcFloat(SizePicture.Height, _mScale.Height,oldscale.Height));
        }

        public void RestoreScale()
        {
            _graphicsList.Resize(new SizeF(1,1),_mScale);
            _mScale = new SizeF(1,1);
        }

        public bool SaveToXml(StreamWriter sw)
        {
            try
            {
                const string mSXmlDeclaration = "<?xml version=\"1.0\" standalone=\"no\"?>";
                const string mSXmlDocType = "<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.0//EN\" \"http://www.w3.org/TR/2001/REC-SVG-20010904/DTD/svg10.dtd\">";

                string sXml = mSXmlDeclaration + "\r\n";
                sXml += mSXmlDocType + "\r\n";
                sXml += "<svg width=\""+_mOriginalSize.Width.ToString(CultureInfo.InvariantCulture)+
                    "\" height=\""+_mOriginalSize.Height.ToString(CultureInfo.InvariantCulture)+"\">" + "\r\n";
                sXml += "<desc>"+Description+"</desc>" + "\r\n";
                sXml += _graphicsList.GetXmlString(_mScale);
                sXml += "</svg>" + "\r\n";
                sw.Write(sXml);
                sw.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SetDirty()
        {
            Dirty = true;
        }

        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        private void DrawArea_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_tools[(int)_activeTool].IsComplete)
            {
                _tools[(int)_activeTool].OnMouseDown(this, e);
                if (e.Button == MouseButtons.Right)
                {
                    if (_tools[(int)_activeTool].IsComplete)
                    {
                        _activeTool = DrawToolType.Pointer;
                        ToolDone(sender, e);
                        Refresh();
                    }
                }
                return;
            }
            if (e.Button == MouseButtons.Left)
                _tools[(int)_activeTool].OnMouseDown(this, e);
            else if (e.Button == MouseButtons.Right)
                OnContextMenu(e);

            if (_graphicsList.IsAnythingSelected() && (!_tools[(int)_activeTool].IsComplete))
            {
                if (ItemsSelected != null)
                    ItemsSelected(_graphicsList.GetAllSelected(), e);
            }
        }

        private void DrawArea_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left || e.Button == MouseButtons.None)
                {
                    if (_activeTool == DrawToolType.Pan)
                    {
                        if (MousePan != null)
                        {
                            MousePan(sender, e);
                        }
                    }

                    var ind = (int)_activeTool;
                    _tools[ind].OnMouseMove(this, e);
                }
                else
                    Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                ErrH.Log("DrawArea", "DrawArea_MouseMove", ex.ToString(), ErrH._LogPriority.Info);
                Cursor = Cursors.Default;
            }
        }

        private void DrawArea_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _tools[(int)_activeTool].OnMouseUp(this, e);
                bool res = _tools[(int)_activeTool].IsComplete;

               // if (activeTool != DrawToolType.Pan)
                if (res)
                {
                    ToolDone(sender, e);
                    ActiveTool = DrawToolType.Pointer;
                }
                else
                {
                    Refresh();
                }
            }

            if (_graphicsList.GetAllSelected().Count > 0)
            {
                if (ItemsSelected != null)
                    ItemsSelected(_graphicsList.GetAllSelected(), e);
            }
        }

        private void DrawArea_Paint(object sender, PaintEventArgs e)
        {
            var brush = new SolidBrush(Color.FromArgb(255, 255, 255));
            e.Graphics.FillRectangle(brush,
                ClientRectangle);

            if(DrawGrid)
                DrawGridsAndScale(e.Graphics);

            if ( _graphicsList != null )
                _graphicsList.Draw(e.Graphics);

            DrawNetSelection(e.Graphics);
            brush.Dispose();
        }

        private void DrawGridsAndScale(Graphics g)
        {
            var majorlinesPen = new Pen(Color.Wheat, 1);
            var minorlinesPen = new Pen(Color.LightGray, 1);

            Xorigin = Yorigin = 0;
            ScaleX = _width;
            ScaleY = _height;

            _width = (int)(SizePicture.Width);
            _height = (int)(SizePicture.Height);

            var xMajorLines = (int)(_width / MajorIntervals/ ScaleDraw.Width);
            var yMajorLines = (int)(_height/ MajorIntervals / ScaleDraw.Height);

            try
            {
                for (int i = 0; i <= xMajorLines; i++)
                {
                    float x = i * (_width / xMajorLines);
                    g.DrawLine(minorlinesPen, x, 0, x, _height);

                    for (int i1 = 1; i1 <= Xdivs; i1++)
                    {
                        float x1 = i1 * MajorIntervals / (Xdivs ) * ScaleDraw.Width;
                        g.DrawLine(majorlinesPen, x + x1, 0, x + x1, _height);
                    }
                }

                for (int i = 0; i <= yMajorLines; i++)
                {
                    float y = i * MajorIntervals * ScaleDraw.Height;
                    g.DrawLine(minorlinesPen, 0, y, _width, y);

                    for (int i1 = 1; i1 <= Ydivs; i1++)
                    {
                        float y1 = i1 * MajorIntervals / (Ydivs )* ScaleDraw.Height;
                        g.DrawLine(majorlinesPen, 0,y + y1, _width,y + y1);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void InitializeComponent()
        {
            components = new Container();
            _contextMenuStrip = new ContextMenuStrip(components);
            _selectAllToolStripMenuItem = new ToolStripMenuItem();
            _toolStripSeparator1 = new ToolStripSeparator();
            _bringToFrontToolStripMenuItem = new ToolStripMenuItem();
            _sendToBackToolStripMenuItem = new ToolStripMenuItem();
            _deleteToolStripMenuItem = new ToolStripMenuItem();
            _toolStripSeparator2 = new ToolStripSeparator();
            _cutToolStripMenuItem = new ToolStripMenuItem();
            _copyToolStripMenuItem = new ToolStripMenuItem();
            _pasteToolStripMenuItem = new ToolStripMenuItem();
            _toolStripSeparator3 = new ToolStripSeparator();
            _contextMenuStrip.SuspendLayout();
            SuspendLayout();

            _contextMenuStrip.Items.AddRange(new ToolStripItem[] {
            _selectAllToolStripMenuItem,
            _toolStripSeparator1,
            _bringToFrontToolStripMenuItem,
            _sendToBackToolStripMenuItem,
            _toolStripSeparator3,
            _deleteToolStripMenuItem,
            _toolStripSeparator2,
            _cutToolStripMenuItem,
            _copyToolStripMenuItem,
            _pasteToolStripMenuItem});
            _contextMenuStrip.Name = @"_contextMenuStrip";
            _contextMenuStrip.Size = new Size(153, 198);

            _selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            _selectAllToolStripMenuItem.Size = new Size(152, 22);
            _selectAllToolStripMenuItem.Text = @"Select All";
            _selectAllToolStripMenuItem.Click += SelectAllToolStripMenuItemClick;

            _toolStripSeparator1.Name = "toolStripSeparator1";
            _toolStripSeparator1.Size = new Size(149, 6);

            _bringToFrontToolStripMenuItem.Name = "bringToFrontToolStripMenuItem";
            _bringToFrontToolStripMenuItem.Size = new Size(152, 22);
            _bringToFrontToolStripMenuItem.Text = @"Bring to Front";
            _bringToFrontToolStripMenuItem.Click += BringToFrontToolStripMenuItemClick;

            _sendToBackToolStripMenuItem.Name = "sendToBackToolStripMenuItem";
            _sendToBackToolStripMenuItem.Size = new Size(152, 22);
            _sendToBackToolStripMenuItem.Text = @"Send to Back";
            _sendToBackToolStripMenuItem.Click += SendToBackToolStripMenuItemClick;

            _deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            _deleteToolStripMenuItem.Size = new Size(152, 22);
            _deleteToolStripMenuItem.Text = @"Delete";
            _deleteToolStripMenuItem.Click += DeleteToolStripMenuItemClick;

            _toolStripSeparator2.Name = "toolStripSeparator2";
            _toolStripSeparator2.Size = new Size(149, 6);

            _cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            _cutToolStripMenuItem.Size = new Size(152, 22);
            _cutToolStripMenuItem.Text = @"Cut";
            _cutToolStripMenuItem.Click += CutToolStripMenuItemClick;

            _copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            _copyToolStripMenuItem.Size = new Size(152, 22);
            _copyToolStripMenuItem.Text = @"Copy";
            _copyToolStripMenuItem.Click += CopyToolStripMenuItemClick;

            _pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            _pasteToolStripMenuItem.Size = new Size(152, 22);
            _pasteToolStripMenuItem.Text = @"Paste";
            _pasteToolStripMenuItem.Click += PasteToolStripMenuItemClick;

            _toolStripSeparator3.Name = "toolStripSeparator3";
            _toolStripSeparator3.Size = new Size(149, 6);

            AutoScroll = true;
            AutoSize = true;
            Name = "DrawArea";
            Size = new Size(153, 136);
            Paint += DrawArea_Paint;
            MouseMove += DrawArea_MouseMove;
            MouseDown += DrawArea_MouseDown;
            MouseUp += DrawArea_MouseUp;
            _contextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void OnContextMenu(MouseEventArgs e)
        {

            var point = new Point(e.X, e.Y);

            int n = GraphicsList.Count;
            DrawObject o = null;

            for ( int i = 0; i < n; i++ )
            {
                if ( GraphicsList[i].HitTest(point) == 0 )
                {
                    o = GraphicsList[i];
                    break;
                }
            }

            if ( o != null )
            {
                if ( ! o.Selected )
                    GraphicsList.UnselectAll();

                o.Selected = true;
                _bringToFrontToolStripMenuItem.Enabled = true;
                _sendToBackToolStripMenuItem.Enabled = true;
                _cutToolStripMenuItem.Enabled = true;
                _copyToolStripMenuItem.Enabled = true;
                _deleteToolStripMenuItem.Enabled = true;
            }
            else
            {
                _bringToFrontToolStripMenuItem.Enabled = false;
                _sendToBackToolStripMenuItem.Enabled = false;
                _cutToolStripMenuItem.Enabled = false;
                _copyToolStripMenuItem.Enabled = false;
                _deleteToolStripMenuItem.Enabled = false;
                GraphicsList.UnselectAll();
            }

            _pasteToolStripMenuItem.Enabled = GraphicsList.AreItemsInMemory();
            _contextMenuStrip.Show(MousePosition);
            Refresh();
        }

        private void CutToolStripMenuItemClick(object sender, EventArgs e)
        {
            _graphicsList.CutSelection();
            Refresh();
        }

        private void CopyToolStripMenuItemClick(object sender, EventArgs e)
        {
            _graphicsList.CopySelection();
            Refresh();
        }

        private void PasteToolStripMenuItemClick(object sender, EventArgs e)
        {
            _graphicsList.PasteSelection();
            Refresh();
        }

        private void SelectAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            _graphicsList.SelectAll();
            Refresh();
        }

        private void SendToBackToolStripMenuItemClick(object sender, EventArgs e)
        {
            _graphicsList.MoveSelectionToBack();
            Refresh();
        }


        private void BringToFrontToolStripMenuItemClick(object sender, EventArgs e)
        {
            _graphicsList.MoveSelectionToFront();
            Refresh();
        }

        private void DeleteToolStripMenuItemClick(object sender, EventArgs e)
        {
            _graphicsList.DeleteSelection();
            Refresh();
        }

        public void MoveCommand(ArrayList movedItemsList, PointF delta)
        {
            _graphicsList.Move(movedItemsList, delta);
            Refresh();
        }

        public void PropertyChanged(GridItem itemChanged, object oldVal)
        {
            _graphicsList.PropertyChanged(itemChanged, oldVal);
        }

        public void ResizeCommand(DrawObject resizedItems, PointF old, PointF newP, int handle)
        {
            _graphicsList.ResizeCommand(resizedItems, old, newP, handle);
            Refresh();
        }
        #endregion Methods
    }
}