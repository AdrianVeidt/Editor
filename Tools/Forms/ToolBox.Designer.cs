namespace VectorEditor.Tools.ToolBoxes
{
    partial class ToolBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolBox));
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.radioButton_Pointer = new System.Windows.Forms.RadioButton();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.radioButton_rectangle = new System.Windows.Forms.RadioButton();
			this.radioButton_line = new System.Windows.Forms.RadioButton();
			this.radioButton_ellipse = new System.Windows.Forms.RadioButton();
			this.radioButton_pencil = new System.Windows.Forms.RadioButton();
			this.radioButton_path = new System.Windows.Forms.RadioButton();
			this.radioButton_text = new System.Windows.Forms.RadioButton();
			this.radioButton_pan = new System.Windows.Forms.RadioButton();
			this.radioButton_image = new System.Windows.Forms.RadioButton();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this.radioButton_Pointer);
			this.flowLayoutPanel1.Controls.Add(this.radioButton_rectangle);
			this.flowLayoutPanel1.Controls.Add(this.radioButton_line);
			this.flowLayoutPanel1.Controls.Add(this.radioButton_ellipse);
			this.flowLayoutPanel1.Controls.Add(this.radioButton_pencil);
			this.flowLayoutPanel1.Controls.Add(this.radioButton_path);
			this.flowLayoutPanel1.Controls.Add(this.radioButton_text);
			this.flowLayoutPanel1.Controls.Add(this.radioButton_pan);
			this.flowLayoutPanel1.Controls.Add(this.radioButton_image);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(94, 240);
			this.flowLayoutPanel1.TabIndex = 0;
			// 
			// radioButton_Pointer
			// 
			this.radioButton_Pointer.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButton_Pointer.ImageKey = "Pointer32x32.ico";
			this.radioButton_Pointer.ImageList = this.imageList;
			this.radioButton_Pointer.Location = new System.Drawing.Point(3, 3);
			this.radioButton_Pointer.Name = "radioButton_Pointer";
			this.radioButton_Pointer.Size = new System.Drawing.Size(35, 38);
			this.radioButton_Pointer.TabIndex = 0;
			this.radioButton_Pointer.TabStop = true;
			this.radioButton_Pointer.UseVisualStyleBackColor = true;
			this.radioButton_Pointer.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "Line32x32.ico");
			this.imageList.Images.SetKeyName(1, "Rectangle32x32.ico");
			this.imageList.Images.SetKeyName(2, "Pointer32x32.ico");
			this.imageList.Images.SetKeyName(3, "Ellipse32x32.ico");
			this.imageList.Images.SetKeyName(4, "PanHand.ico");
			this.imageList.Images.SetKeyName(5, "Pencil.ico");
			this.imageList.Images.SetKeyName(6, "Text23x32.ico");
			this.imageList.Images.SetKeyName(7, "Path32x32.ico");
			this.imageList.Images.SetKeyName(8, "image.png");
			// 
			// radioButton_rectangle
			// 
			this.radioButton_rectangle.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButton_rectangle.ImageKey = "Rectangle32x32.ico";
			this.radioButton_rectangle.ImageList = this.imageList;
			this.radioButton_rectangle.Location = new System.Drawing.Point(44, 3);
			this.radioButton_rectangle.Name = "radioButton_rectangle";
			this.radioButton_rectangle.Size = new System.Drawing.Size(35, 38);
			this.radioButton_rectangle.TabIndex = 0;
			this.radioButton_rectangle.TabStop = true;
			this.radioButton_rectangle.UseVisualStyleBackColor = true;
			this.radioButton_rectangle.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// radioButton_line
			// 
			this.radioButton_line.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButton_line.ImageKey = "Line32x32.ico";
			this.radioButton_line.ImageList = this.imageList;
			this.radioButton_line.Location = new System.Drawing.Point(3, 47);
			this.radioButton_line.Name = "radioButton_line";
			this.radioButton_line.Size = new System.Drawing.Size(35, 38);
			this.radioButton_line.TabIndex = 0;
			this.radioButton_line.TabStop = true;
			this.radioButton_line.UseVisualStyleBackColor = true;
			this.radioButton_line.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// radioButton_ellipse
			// 
			this.radioButton_ellipse.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButton_ellipse.ImageIndex = 3;
			this.radioButton_ellipse.ImageList = this.imageList;
			this.radioButton_ellipse.Location = new System.Drawing.Point(44, 47);
			this.radioButton_ellipse.Name = "radioButton_ellipse";
			this.radioButton_ellipse.Size = new System.Drawing.Size(35, 38);
			this.radioButton_ellipse.TabIndex = 0;
			this.radioButton_ellipse.TabStop = true;
			this.radioButton_ellipse.UseVisualStyleBackColor = true;
			this.radioButton_ellipse.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// radioButton_pencil
			// 
			this.radioButton_pencil.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButton_pencil.ImageKey = "Pencil.ico";
			this.radioButton_pencil.ImageList = this.imageList;
			this.radioButton_pencil.Location = new System.Drawing.Point(3, 91);
			this.radioButton_pencil.Name = "radioButton_pencil";
			this.radioButton_pencil.Size = new System.Drawing.Size(35, 38);
			this.radioButton_pencil.TabIndex = 0;
			this.radioButton_pencil.TabStop = true;
			this.radioButton_pencil.UseVisualStyleBackColor = true;
			this.radioButton_pencil.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// radioButton_path
			// 
			this.radioButton_path.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButton_path.ImageKey = "Path32x32.ico";
			this.radioButton_path.ImageList = this.imageList;
			this.radioButton_path.Location = new System.Drawing.Point(44, 91);
			this.radioButton_path.Name = "radioButton_path";
			this.radioButton_path.Size = new System.Drawing.Size(35, 38);
			this.radioButton_path.TabIndex = 0;
			this.radioButton_path.TabStop = true;
			this.radioButton_path.UseVisualStyleBackColor = true;
			this.radioButton_path.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// radioButton_text
			// 
			this.radioButton_text.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButton_text.ImageKey = "Text23x32.ico";
			this.radioButton_text.ImageList = this.imageList;
			this.radioButton_text.Location = new System.Drawing.Point(3, 135);
			this.radioButton_text.Name = "radioButton_text";
			this.radioButton_text.Size = new System.Drawing.Size(35, 38);
			this.radioButton_text.TabIndex = 0;
			this.radioButton_text.TabStop = true;
			this.radioButton_text.UseVisualStyleBackColor = true;
			this.radioButton_text.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// radioButton_pan
			// 
			this.radioButton_pan.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButton_pan.ImageKey = "PanHand.ico";
			this.radioButton_pan.ImageList = this.imageList;
			this.radioButton_pan.Location = new System.Drawing.Point(44, 135);
			this.radioButton_pan.Name = "radioButton_pan";
			this.radioButton_pan.Size = new System.Drawing.Size(35, 38);
			this.radioButton_pan.TabIndex = 0;
			this.radioButton_pan.TabStop = true;
			this.radioButton_pan.UseVisualStyleBackColor = true;
			this.radioButton_pan.Click += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// radioButton_image
			// 
			this.radioButton_image.Appearance = System.Windows.Forms.Appearance.Button;
			this.radioButton_image.ImageKey = "image.png";
			this.radioButton_image.ImageList = this.imageList;
			this.radioButton_image.Location = new System.Drawing.Point(3, 179);
			this.radioButton_image.Name = "radioButton_image";
			this.radioButton_image.Size = new System.Drawing.Size(35, 38);
			this.radioButton_image.TabIndex = 0;
			this.radioButton_image.TabStop = true;
			this.radioButton_image.UseVisualStyleBackColor = true;
			this.radioButton_image.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
			// 
			// ToolBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(97, 243);
			this.Controls.Add(this.flowLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ToolBox";
			this.Padding = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this.Text = "ToolBox";
			this.flowLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.RadioButton radioButton_line;
        private System.Windows.Forms.RadioButton radioButton_rectangle;
        private System.Windows.Forms.RadioButton radioButton_Pointer;
        private System.Windows.Forms.RadioButton radioButton_ellipse;
        private System.Windows.Forms.RadioButton radioButton_pan;
        private System.Windows.Forms.RadioButton radioButton_pencil;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.RadioButton radioButton_text;
		private System.Windows.Forms.RadioButton radioButton_path;
		private System.Windows.Forms.RadioButton radioButton_image;






    }
}