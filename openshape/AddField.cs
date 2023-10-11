using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Controls;

namespace openshape
{
    public partial class AddField : Form
    {
        public AddField()
        {
            InitializeComponent();
        }
        private AxMapControl buddyMap;

        public AxMapControl BuddyMap
        {
            get
            {
                return buddyMap;
            }
            set
            {
                buddyMap = value;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IFeatureLayer firstlay = buddyMap.get_Layer(0) as IFeatureLayer;
            IFeatureClass firstclass = firstlay.FeatureClass;
            IFieldEdit filed = new FieldClass();
            filed.Name_2 = textName.Text;
            filed.Length_2 = int.Parse(textLength.Text);
            filed.Type_2 = esriFieldType.esriFieldTypeString;
            firstclass.AddField(filed as IField);
            MessageBox.Show("添加字段完成！");
        }

        private void textName_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }



    }
}
