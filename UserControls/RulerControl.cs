using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
#if !FRAMEWORKMENUS
using Lyquidity.UtilityLibrary.MenusA;
using Lyquidity.UtilityLibrary.General;
using System.ComponentModel.Design;
#endif

namespace Lyquidity.UtilityLibrary.Controls
{
	#region Enumerations

	public enum enumOrientation
	{
		orHorizontal = 0,
		orVertical = 1
	}

	public enum enumScaleMode
	{
		smPoints = 0,
		smPixels = 1,
		smCentimetres = 2,
		smInches = 3,
		smMillimetres = 4
	}

	public enum enumRulerAlignment
	{
		raTopOrLeft,
		raMiddle,
		raBottomOrRight
	}


	internal enum Msg
	{
		WM_MOUSEMOVE              = 0x0200,
		WM_MOUSELEAVE             = 0x02A3,
		WM_NCMOUSELEAVE           = 0x02A2,
	}

	#endregion


	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(RulerControl), "Ruler.bmp")]
	public class RulerControl : System.Windows.Forms.Control, IMessageFilter
	{

		#region Internal Variables

#if !FRAMEWORKMENUS
		private PopupMenu			_mnuContext			= null;
#endif
		private int					_Scale;
		private bool				_bDrawLine			= false;
		private bool				_bInControl			= false;
		private int					_iMousePosition		= 1;
		private int					_iOldMousePosition	= -1;
		private Bitmap				_Bitmap				= null;

		#endregion

		#region Property variable

		private enumOrientation		_Orientation;
		private enumScaleMode		_ScaleMode;
		private enumRulerAlignment	_RulerAlignment     = enumRulerAlignment.raBottomOrRight;
		private Border3DStyle		_i3DBorderStyle     = Border3DStyle.Etched;
		private int					_iMajorInterval     = 100;
		private int					_iNumberOfDivisions = 10;
		private int					_DivisionMarkFactor = 5;
		private int					_MiddleMarkFactor	= 3;
		private double				_ZoomFactor         = 1;
		private double				_StartValue			= 0;
		private bool				_bMouseTrackingOn   = false;
		private bool				_VerticalNumbers	= true;
		private bool				_bActualSize		= true;
		private float				_DpiX				= 96;

		#endregion

		#region Event Arguments

		public class ScaleModeChangedEventArgs : EventArgs
		{
			public enumScaleMode Mode;

			public ScaleModeChangedEventArgs(enumScaleMode Mode) : base()
			{
				this.Mode = Mode;
			}
		}

		public class HooverValueEventArgs : EventArgs
		{
			public double Value;

			public HooverValueEventArgs(double Value) : base()
			{
				this.Value = Value;
			}
		}


		#endregion

		#region Delegates

		public delegate void ScaleModeChangedEvent(object sender, ScaleModeChangedEventArgs e);
		public delegate void HooverValueEvent(object sender, HooverValueEventArgs e);
		// public delegate void ClickEvent(object sender, ClickEventArgs e);

		#endregion

		#region Events

		public event ScaleModeChangedEvent ScaleModeChanged;
		public event HooverValueEvent HooverValue;

		#endregion


		private System.ComponentModel.Container components = null;

		#region Constrcutors/Destructors

		public RulerControl()
		{
			base.BackColor = System.Drawing.Color.White;
			base.ForeColor = System.Drawing.Color.Black;
			InitializeComponent();

#if !FRAMEWORKMENUS

			_mnuContext = new PopupMenu();

			MenuCommand mnuPoints = new MenuCommand("Points", new EventHandler(Popup_Click));
			mnuPoints.Tag = enumScaleMode.smPoints;
			MenuCommand mnuPixels = new MenuCommand("Pixels", new EventHandler(Popup_Click));
			mnuPixels.Tag = enumScaleMode.smPixels;
			MenuCommand mnuCentimetres = new MenuCommand("Centimetres", new EventHandler(Popup_Click));
			mnuCentimetres.Tag = enumScaleMode.smCentimetres;
			MenuCommand mnuInches = new MenuCommand("Inches", new EventHandler(Popup_Click));
			mnuInches.Tag = enumScaleMode.smInches;
			MenuCommand mnuInches = new MenuCommand("Millimetres", new EventHandler(Popup_Click));
			mnuInches.Tag = enumScaleMode.smMillimetres;

			_mnuContext.MenuCommands.AddRange(new MenuCommand[]{mnuPoints, mnuPixels, mnuCentimetres, mnuInches});

			_mnuContext.Style = VisualStyle.IDE;

#endif

#if FRAMEWORKMENUS

			System.Windows.Forms.MenuItem mnuPoints = new System.Windows.Forms.MenuItem("Points", new EventHandler(Popup_Click));
			System.Windows.Forms.MenuItem mnuPixels = new System.Windows.Forms.MenuItem("Pixels", new EventHandler(Popup_Click));
			System.Windows.Forms.MenuItem mnuCentimetres = new System.Windows.Forms.MenuItem("Centimetres", new EventHandler(Popup_Click));
			System.Windows.Forms.MenuItem mnuInches = new System.Windows.Forms.MenuItem("Inches", new EventHandler(Popup_Click));
			System.Windows.Forms.MenuItem mnuMillimetres = new System.Windows.Forms.MenuItem("Millimetres", new EventHandler(Popup_Click));
			ContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {mnuPoints, mnuPixels, mnuCentimetres, mnuInches, mnuMillimetres});
#endif
			Graphics g = this.CreateGraphics();
			_DpiX = g.DpiX;
			ScaleMode = enumScaleMode.smPoints;
		}

		#endregion

		#region Methods

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool PreFilterMessage(ref Message m)
		{
			if (!this._bMouseTrackingOn) return false;

			if (m.Msg == (int)Msg.WM_MOUSEMOVE)
			{
				int X = 0;
				int Y = 0;

				Point pointScreen = Control.MousePosition;

				Point pointClientOrigin = new Point(X, Y);
				pointClientOrigin = PointToScreen(pointClientOrigin);

				_bDrawLine = false;
				_bInControl = false;

				HooverValueEventArgs eHoover = null;

				if (this.Orientation == enumOrientation.orHorizontal)
				{
					_bDrawLine = (pointScreen.X >= pointClientOrigin.X) && (pointScreen.X <= pointClientOrigin.X + this.Width);
				}
				else
				{
					_bDrawLine = (pointScreen.Y >= pointClientOrigin.Y) && (pointScreen.Y <= pointClientOrigin.Y + this.Height);
				}

				if (_bDrawLine)
				{
					X = pointScreen.X-pointClientOrigin.X;
					Y = pointScreen.Y-pointClientOrigin.Y;

					_bInControl = (this.ClientRectangle.Contains(new Point(X, Y)));

					ChangeMousePosition((this.Orientation == enumOrientation.orHorizontal) ? X : Y);
					eHoover = new HooverValueEventArgs(CalculateValue(_iMousePosition));

				} 
				else
				{
					ChangeMousePosition(-1);
					eHoover = new HooverValueEventArgs(_iMousePosition);
				}

				PaintEventArgs e = null;
				try
				{
					e = new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle);
					OnPaint(e);
				}
				finally
				{
					e.Graphics.Dispose();
				}

				OnHooverValue(eHoover);
			}

			if ((m.Msg == (int)Msg.WM_MOUSELEAVE) || 
				(m.Msg == (int)Msg.WM_NCMOUSELEAVE))
			{
				_bDrawLine = false;
				PaintEventArgs paintArgs = null;
				try
				{
					paintArgs = new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle);
					this.OnPaint(paintArgs);
				}
				finally
				{
					paintArgs.Graphics.Dispose();
				}
			}

			return false;  
		}


		public double PixelToScaleValue(int iOffset)
		{
			return this.CalculateValue(iOffset);
		}

		public int ScaleValueToPixel(double nScaleValue)
		{
			return CalculatePixel(nScaleValue);
		}

		#endregion

		#region Component Designer generated code

		private void InitializeComponent()
		{

			this.Name = "RulerControl";
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RulerControl_MouseUp);

			base.SetStyle(System.Windows.Forms.ControlStyles.DoubleBuffer, true);

#if FRAMEWORKMENUS
			this.ContextMenu = new ContextMenu();
			this.ContextMenu.Popup +=new EventHandler(ContextMenu_Popup);
#endif

		}
		#endregion

		#region Overrides

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

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);

			_Bitmap = null;
			this.Invalidate();
		}

		public override void Refresh()
		{
			base.Refresh ();
			this.Invalidate();
		}


		[Description("Draws the ruler marks in the scale requested.")]
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);
			DrawControl(e.Graphics);
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged (e);

			if (this.Visible)
			{
				if (_bMouseTrackingOn) Application.AddMessageFilter(this);
			}
			else
			{
				if (_bMouseTrackingOn) RemoveMessageFilter(); 
			}
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			base.OnHandleDestroyed (e);
			RemoveMessageFilter();
			_bMouseTrackingOn = false;
		}

		protected override void OnControlRemoved(ControlEventArgs e)
		{
			base.OnControlRemoved (e);
			RemoveMessageFilter();
			_bMouseTrackingOn = false;
		}

		private void RemoveMessageFilter()
		{
			try
			{
				if (_bMouseTrackingOn) 
				{
					Application.RemoveMessageFilter(this);
				}
			} 
			catch {}
		}

		#endregion

		#region Event Handlers

		private void RulerControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
//			if (e.Button.Equals(MouseButtons.Right)) 
		}

		private void RulerControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
#if FRAMEWORKMENUS
			if ((Control.MouseButtons & MouseButtons.Right) != 0) 
			{
				this.ContextMenu.Show(this, PointToClient(Control.MousePosition));
#else
			if ((e.Button & MouseButtons.Right) != 0)
			{
				_mnuContext.TrackPopup(this.PointToScreen(new Point(e.X, e.Y)));
#endif
			}
			else
			{
				EventArgs eClick = new EventArgs();
				this.OnClick(eClick);
			}
		}

		private void Popup_Click(object sender, EventArgs e)
		{
#if FRAMEWORKMENUS
			System.Windows.Forms.MenuItem item = (System.Windows.Forms.MenuItem)sender;
			ScaleMode = (enumScaleMode)item.Index;
#else
			MenuCommand item = (MenuCommand)sender;
			ScaleMode = (enumScaleMode)item.Tag;
#endif
			_Bitmap = null;
			Invalidate();
		}

		protected void OnHooverValue(HooverValueEventArgs e)
		{
			if (HooverValue != null) HooverValue(this, e);
		}

		protected void OnScaleModeChanged(ScaleModeChangedEventArgs e)
		{
			if (ScaleModeChanged != null) ScaleModeChanged(this, e);
		}

		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter (e);
			_bDrawLine = false;
			Invalidate();
		}

		protected override void OnLeave(EventArgs e)
		{
			base.OnLeave (e);
			Invalidate();
		}

		private void ContextMenu_Popup(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Popup");
		}

		#endregion

		#region Properties

		[
		DefaultValue(typeof(Border3DStyle),"Etched"),
		Description("The border style use the Windows.Forms.Border3DStyle type"),
		Category("Ruler"),
		]
		public Border3DStyle BorderStyle
		{
			get
			{
				return _i3DBorderStyle;
			}
			set
			{
				_i3DBorderStyle = value;
				_Bitmap = null;
				Invalidate();
			}
		}

		[Description("Horizontal or vertical layout")]
		[Category("Ruler")]
		public enumOrientation Orientation
		{ 
			get { return _Orientation; }
			set 
			{
				_Orientation = value;
				_Bitmap = null;
				Invalidate();
			}
		}

		[Description("A value from which the ruler marking should be shown.  Default is zero.")]
		[Category("Ruler")]
		public double StartValue
		{
			get { return _StartValue; }
			set 
			{
				_StartValue = value;
				_Bitmap = null;
				Invalidate();
			}
		}

		[Description("The scale to use")]
		[Category("Ruler")]
		public enumScaleMode ScaleMode
		{
			get { return _ScaleMode; }
			set 
			{
				enumScaleMode iOldScaleMode = _ScaleMode;
				_ScaleMode = value;    

				if (_iMajorInterval == DefaultMajorInterval(iOldScaleMode))
				{
					_Scale = DefaultScale(_ScaleMode);
					_iMajorInterval = DefaultMajorInterval(_ScaleMode);

				} 
				else
				{
					MajorInterval = _iMajorInterval;
				}

				this.StartValue = this._StartValue;

				for(int i = 0; i <= 4; i++)
#if FRAMEWORKMENUS
					ContextMenu.MenuItems[i].Checked = false;

				ContextMenu.MenuItems[(int)value].Checked = true;
#else
					_mnuContext.MenuCommands[i].Checked = false;

				_mnuContext.MenuCommands[(int)value].Checked = true;
#endif

				ScaleModeChangedEventArgs e = new ScaleModeChangedEventArgs(value);
				this.OnScaleModeChanged(e);
			}
		}

		[Description("")]
		[Category("Ruler")]
		public int MajorInterval
		{
			get { return _iMajorInterval; }
			set 
			{ 
				if (value <=0) throw new Exception("The major interval value cannot be less than one");
				_iMajorInterval = value;
				_Scale = DefaultScale(_ScaleMode) * _iMajorInterval / DefaultMajorInterval(_ScaleMode);
				_Bitmap = null;
				Invalidate();
			}
		}

		[Description("")]
		[Category("Ruler")]
		public int Divisions
		{
			get { return _iNumberOfDivisions; }
			set 
			{
				if (value <=0) throw new Exception("The number of divisions cannot be less than one");
				_iNumberOfDivisions = value;
				_Bitmap = null;
				Invalidate();
			}
		}

		[Description("")]
		[Category("Ruler")]
		public int DivisionMarkFactor
		{
			get { return _DivisionMarkFactor; }
			set 
			{ 
				if (value <=0) throw new Exception("The Division Mark Factor cannot be less than one");
				_DivisionMarkFactor = value;
				_Bitmap = null;
				Invalidate();
			}
		}

		[Description("")]
		[Category("Ruler")]
		public int MiddleMarkFactor
		{
			get { return _MiddleMarkFactor; }
			set
			{
				if (value <=0) throw new Exception("The Middle Mark Factor cannot be less than one");
				_MiddleMarkFactor = value;
				_Bitmap = null;
				Invalidate();
			}
		}

		[Description("")]
		[Category("Ruler")]
		public double ScaleValue
		{
			get {return CalculateValue(_iMousePosition); }
		}

		[Description("")]
		[Category("Ruler")]
		public bool MouseTrackingOn
		{
			get { return _bMouseTrackingOn; }
			set 
			{ 
				if (value == _bMouseTrackingOn) return;
				
				if (value)
				{
					if (this.Visible) Application.AddMessageFilter(this);
				}
				else
				{
					Application.RemoveMessageFilter(this);
					ChangeMousePosition(-1);
				}

				_bMouseTrackingOn = value;

				_Bitmap = null;
				Invalidate();
			}
		}

		[Description("")]
		public override Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				base.Font = value;
				_Bitmap = null;
				Invalidate();
			}
		}

		[Description("")]
		[Category("Ruler")]
		public int MouseLocation
		{
			get { return _iMousePosition; }
		}

		[DefaultValue(typeof(Color), "ControlDarkDark")]
		[Description("")]
		public override Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
				_Bitmap = null;
				Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "White")]
		[Description("")]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
				_Bitmap = null;
				Invalidate();
			}
		}


		[Description("")]
		[Category("Ruler")]
		public bool VerticalNumbers
		{
			get { return _VerticalNumbers; }
			set
			{
				_VerticalNumbers = value;
				_Bitmap = null;
				Invalidate();
			}
		}

		[Description("A factor between 0.1 and 10 by which the displayed scale will be zoomed.")]
		[Category("Ruler")]
		public double ZoomFactor
		{
			get { return _ZoomFactor; }
			set 
			{
				if ((value < 0.1) || (value > 10)) throw new Exception("Zoom factor can be between 10% and 1000%");
				if (_ZoomFactor == value) return;
				_ZoomFactor = value;
				this.ScaleMode = _ScaleMode;
				_Bitmap = null;
				Invalidate();
			}
		}


		[Description("")]
		[Category("Ruler")]
		public bool ActualSize
		{
			get { return _bActualSize; }
			set 
			{
				if (_bActualSize == value) return;
				_bActualSize = value;
				this.ScaleMode = _ScaleMode;
				_Bitmap = null;
				Invalidate();
			}
		}

		[Description("")]
		[Category("Ruler")]
		public enumRulerAlignment RulerAlignment
		{
			get { return _RulerAlignment; }
			set 
			{
				if (_RulerAlignment == value) return;
				_RulerAlignment = value;
				_Bitmap = null;
				Invalidate();
			}
		}


		#endregion

		#region Private functions

		private double CalculateValue(int iOffset)
		{
			if (iOffset < 0) return 0;

			double nValue = ((double)iOffset-Start()) / (double)_Scale * (double)_iMajorInterval;
			return nValue + this._StartValue;
		}

		[Description("")]
		private int CalculatePixel(double nScaleValue)
		{

			double nValue = nScaleValue - this._StartValue;
			if (nValue < 0) return Start();  

			int iOffset = Convert.ToInt32(nValue / (double)_iMajorInterval * (double)_Scale);

			return iOffset + Start();
		}

		public void RenderTrackLine(Graphics g)
		{
			if (_bMouseTrackingOn & _bDrawLine)
			{
				int iOffset = Offset();

				switch(Orientation)
				{
					case enumOrientation.orHorizontal:
						Line(g, _iMousePosition, iOffset, _iMousePosition, Height - iOffset);
						break;
					case enumOrientation.orVertical:
						Line (g, iOffset, _iMousePosition, Width - iOffset, _iMousePosition);
						break;
				}
			}
		}

		private void DrawControl(Graphics graphics)
		{
			Graphics g = null;

			if (!this.Visible) return;

			if (this.Width < 1 || this.Height < 1) 
			{
#if DEBUG
				System.Diagnostics.Trace.WriteLine("Minimised?");
#endif
				return;
			}

			if (_Bitmap == null)
			{

				int  iValueOffset = 0;
				int  iScaleStartValue;

				_Bitmap = new Bitmap(this.Width, this.Height);

				g = Graphics.FromImage(_Bitmap);

				try
				{
					g.FillRectangle(new SolidBrush(this.BackColor), 0, 0, _Bitmap.Width, _Bitmap.Height);

					if (this.StartValue >= 0) 
					{
						iScaleStartValue = Convert.ToInt32(_StartValue * _Scale / _iMajorInterval);  
					} 
					else
					{
						double dStartValue = Math.Ceiling(Math.Abs(_StartValue))-Math.Abs(_StartValue);

						iScaleStartValue = Convert.ToInt32(dStartValue * _Scale / _iMajorInterval); 
						iValueOffset = Convert.ToInt32(Math.Ceiling(Math.Abs(_StartValue)));
					};

					int iScale = _Scale;

					int iStart = Start(); 
					int iEnd = (this.Orientation == enumOrientation.orHorizontal) ? Width : Height;

#if DEBUG
					if (this.Orientation == enumOrientation.orVertical)
					{
						System.Diagnostics.Debug.WriteLine("Vert");
					}
#endif
					for(int j = iStart; j <= iEnd; j += iScale)
					{
						int iLeft = _Scale;  
						int jOffset = j+iScaleStartValue;

						iScale = ((jOffset-iStart) % _Scale);  

						if (iScale == 0)
						{
							if (_RulerAlignment != enumRulerAlignment.raMiddle)
							{
								if (this.Orientation == enumOrientation.orHorizontal)
									Line(g, j, 0, j, Height);
								else
									Line (g, 0, j, Width, j);
							}

							iLeft = _Scale;     
						} 
						else
						{
							iLeft = _Scale - Math.Abs(iScale);     
						}

						iScale = iLeft;

						int iValue = (((jOffset-iStart)/_Scale)+1) * _iMajorInterval;

						iValue -= iValueOffset;
						DrawValue(g, iValue, j - iStart, iScale);

						int iUsed = 0;

						for(int i = 0; i < _iNumberOfDivisions; i++)
						{
							int iX = Convert.ToInt32(Math.Round((double)(_Scale-iUsed)/(double)(_iNumberOfDivisions - i),0)); // Use a spreading algorithm rather that using expensive floating point numbers

							iUsed += iX;

							if (iUsed >= (_Scale-iLeft))
							{
								iX = iUsed+j-(_Scale-iLeft);

								bool bMiddleMark = ((_iNumberOfDivisions & 0x1) == 0) & (i+1==_iNumberOfDivisions/2);
								bool bShowMiddleMark = bMiddleMark;
								bool bLastDivisionMark = (i+1 == _iNumberOfDivisions);
								bool bLastAlignMiddleDivisionMark =  bLastDivisionMark & (_RulerAlignment == enumRulerAlignment.raMiddle);
								bool bShowDivisionMark = !bMiddleMark & !bLastAlignMiddleDivisionMark;

								if (bShowMiddleMark)
								{
									DivisionMark(g, iX, _MiddleMarkFactor);  
								} 
								else if (bShowDivisionMark)
								{
									DivisionMark(g, iX, _DivisionMarkFactor); 
								}
							}
						}
					}

					if (_i3DBorderStyle != Border3DStyle.Flat)
						ControlPaint.DrawBorder3D(g, this.ClientRectangle, this._i3DBorderStyle );

				}
				catch(Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex.Message);
				}
				finally 
				{
					g.Dispose();
				}
			}

			g = graphics;

			try
			{
				g.DrawImage(_Bitmap, this.ClientRectangle);

				RenderTrackLine(g);
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			finally
			{
				GC.Collect();
			}

		}

		private void DivisionMark(Graphics g, int iPosition, int iProportion)
		{

			int iMarkStart = 0, iMarkEnd = 0;

			if (this.Orientation == enumOrientation.orHorizontal)
			{

				switch(_RulerAlignment)
				{
					case enumRulerAlignment.raBottomOrRight:
					{
						iMarkStart = Height - Height/iProportion;
						iMarkEnd = Height;
						break;
					}
					case enumRulerAlignment.raMiddle:
					{
						iMarkStart = (Height - Height/iProportion)/2 -1;
						iMarkEnd = iMarkStart + Height/iProportion;
						break;
					}
					case enumRulerAlignment.raTopOrLeft:
					{
						iMarkStart = 0;
						iMarkEnd = Height/iProportion;
						break;
					}
				}

				Line(g, iPosition, iMarkStart, iPosition, iMarkEnd);
			}
			else
			{

				switch(_RulerAlignment)
				{
					case enumRulerAlignment.raBottomOrRight:
					{
						iMarkStart = Width - Width/iProportion;
						iMarkEnd = Width;
						break;
					}
					case enumRulerAlignment.raMiddle:
					{
						iMarkStart = (Width - Width/iProportion)/2 -1;
						iMarkEnd = iMarkStart + Width/iProportion;
						break;
					}
					case enumRulerAlignment.raTopOrLeft:
					{
						iMarkStart = 0;
						iMarkEnd = Width/iProportion;
						break;
					}
				}

				Line(g, iMarkStart, iPosition, iMarkEnd, iPosition);
			}
		}

		private void DrawValue(Graphics g, int iValue, int iPosition, int iSpaceAvailable)
		{
			StringFormat format = new StringFormat(StringFormatFlags.MeasureTrailingSpaces);
			if (_VerticalNumbers)
				format.FormatFlags |= StringFormatFlags.DirectionVertical;
			
			SizeF size = g.MeasureString((iValue).ToString(), this.Font, iSpaceAvailable, format);

			Point drawingPoint;
			int iX = 0;
			int iY = 0;

			if (this.Orientation == enumOrientation.orHorizontal)
			{
				switch(_RulerAlignment)
				{
					case enumRulerAlignment.raBottomOrRight:
					{
						iX = iPosition + iSpaceAvailable - (int)size.Width - 2;
						iY = 2;
						break;
					}
					case enumRulerAlignment.raMiddle:
					{
						iX = iPosition + iSpaceAvailable - (int)size.Width/2;
						iY = (Height - (int)size.Height)/2 - 2;
						break;
					}
					case enumRulerAlignment.raTopOrLeft:
					{
						iX = iPosition + iSpaceAvailable - (int)size.Width - 2;
						iY = Height - 2 - (int)size.Height;
						break;
					}
				}

				drawingPoint = new Point(iX, iY);
			}
			else
			{
				switch(_RulerAlignment)
				{
					case enumRulerAlignment.raBottomOrRight:
					{
						iX = 2;
						iY = iPosition + iSpaceAvailable - (int)size.Height - 2;
						break;
					}
					case enumRulerAlignment.raMiddle:
					{
						iX = (Width - (int)size.Width)/2 - 1;
						iY = iPosition + iSpaceAvailable - (int)size.Height/2;
						break;
					}
					case enumRulerAlignment.raTopOrLeft:
					{
						iX = Width - 2 - (int)size.Width;
						iY = iPosition + iSpaceAvailable - (int)size.Height - 2;
						break;
					}
				}

				drawingPoint = new Point(iX, iY);
			}

			g.DrawString(iValue.ToString(), this.Font, new SolidBrush(this.ForeColor), drawingPoint, format);
		}

		private void Line(Graphics g, int x1, int y1, int x2, int y2)
		{
			using(SolidBrush brush = new SolidBrush(this.ForeColor))
			{
				using(Pen pen = new Pen(brush))
				{
					g.DrawLine(pen, x1, y1, x2, y2);
					pen.Dispose();
					brush.Dispose();
				}
			}
		}

		private int DefaultScale(enumScaleMode iScaleMode)
		{
			int iScale = 100;

			switch(iScaleMode)
			{
				case enumScaleMode.smPoints:
					iScale = 660; // 132;
					break;
				case enumScaleMode.smPixels:
					iScale = 100;
					break;
				case enumScaleMode.smCentimetres:
					iScale = 262; // 53;
					break;
				case enumScaleMode.smInches:
					iScale = 660; // 132;
					break;
				case enumScaleMode.smMillimetres:
					iScale = 27;
					break;
/*
				case enumScaleMode.smPoints:
					iScale = 96;
					break;
				case enumScaleMode.smPixels:
					iScale = 100;
					break;
				case enumScaleMode.smCentimetres:
					iScale = 38;
					break;
				case enumScaleMode.smInches:
					iScale = 96;
					break;
				case enumScaleMode.smMillimetres:
					iScale = 4;
					break;
*/
			}

			if (iScaleMode == enumScaleMode.smPixels)
				return Convert.ToInt32((double)iScale * _ZoomFactor);
			else
				return Convert.ToInt32((double)iScale * _ZoomFactor * (double)(_bActualSize ? (double)_DpiX/(float)480 : 0.2));
		}

		private int DefaultMajorInterval(enumScaleMode iScaleMode)
		{
			int iInterval = 10;

			switch(iScaleMode)
			{
				case enumScaleMode.smPoints:
					iInterval = 72;
					break;
				case enumScaleMode.smPixels:
					iInterval = 100;
					break;
				case enumScaleMode.smCentimetres:
					iInterval = 1;
					break;
				case enumScaleMode.smInches:
					iInterval = 1;
					break;
				case enumScaleMode.smMillimetres:
					iInterval = 1;
					break;
			}

			return iInterval;
		}

		private int Offset()
		{
			int iOffset = 0;

			switch(this._i3DBorderStyle)
			{
				case Border3DStyle.Flat: iOffset = 0; break;
				case Border3DStyle.Adjust: iOffset = 0; break;
				case Border3DStyle.Sunken: iOffset = 2; break;
				case Border3DStyle.Bump: iOffset = 2; break;
				case Border3DStyle.Etched: iOffset = 2; break;
				case Border3DStyle.Raised: iOffset = 2; break;
				case Border3DStyle.RaisedInner: iOffset = 1; break;
				case Border3DStyle.RaisedOuter: iOffset = 1; break;
				case Border3DStyle.SunkenInner: iOffset = 1; break;
				case Border3DStyle.SunkenOuter: iOffset = 1; break;
				default: iOffset = 0; break;
			}

			return iOffset;
		}

		private int Start()
		{
			int iStart = 0;

			switch(this._i3DBorderStyle)
			{
				case Border3DStyle.Flat: iStart = 0; break;
				case Border3DStyle.Adjust: iStart = 0; break;
				case Border3DStyle.Sunken: iStart = 1; break;
				case Border3DStyle.Bump: iStart = 1; break;
				case Border3DStyle.Etched: iStart = 1; break;
				case Border3DStyle.Raised: iStart = 1; break;
				case Border3DStyle.RaisedInner: iStart = 0; break;
				case Border3DStyle.RaisedOuter: iStart = 0; break;
				case Border3DStyle.SunkenInner: iStart = 0; break;
				case Border3DStyle.SunkenOuter: iStart = 0; break;
				default: iStart = 0; break;
			}
			return iStart;
		}

		private void ChangeMousePosition(int iNewPosition)
		{
			this._iOldMousePosition = this._iMousePosition;
			this._iMousePosition = iNewPosition;
		}

	}

	#endregion

}