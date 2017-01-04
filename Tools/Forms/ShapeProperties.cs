using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VectorEditor
{
    public partial class shapeProperties : Form
    {
        public delegate void OnPropertyChanged(object sender, System.Windows.Forms.PropertyValueChangedEventArgs e);
        public event OnPropertyChanged PropertyChanged;

        public shapeProperties()
        {
            InitializeComponent();
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(s, e);
        }
    }
}