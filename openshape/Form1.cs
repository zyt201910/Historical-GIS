using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
/*注意引用项*/
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;


//using ESRI.ArcGIS.Controls;

namespace openshape
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog pOpenFileDialog = new OpenFileDialog();
            pOpenFileDialog.CheckFileExists = true;
            pOpenFileDialog.Title = "打开Shape文件";
            pOpenFileDialog.Filter = "Shape文件(*.shp)|*.shp";
            pOpenFileDialog.ShowDialog();
            //获取文件路径
            IWorkspaceFactory pWorkspaceFactory;
            IFeatureWorkspace pFeatureWorkspace;
            IFeatureLayer pFeatureLayer;
            string pFullpath = pOpenFileDialog.FileName; //获取文件的完整路径
            MessageBox.Show(pFullpath); //展示一下获取的pFullpath
            if (pFullpath == "") {
                return;
            }
            int pIndex = pFullpath.LastIndexOf("\\"); //查找pFullpath中最后一个\的位置，并将其赋值给整型变量pIndex。这个位置是文件名前的路径的末尾。
            string pFilePath = pFullpath.Substring(0, pIndex); //使用Substring方法从pFullpath中截取0到pIndex之间的字符，得到文件的路径，并将其赋值给字符串变量pFilePath。
            string pFileName = pFullpath.Substring(pIndex + 1); //使用Substring方法从pFullpath中截取pIndex + 1到字符串末尾的字符，得到文件的名称，并将其赋值给字符串变量pFileName。
            pWorkspaceFactory = new ShapefileWorkspaceFactory();
            /*
                    ShapefileWorkspaceFactory CoClass
            接口	                                描述
            IWorkspaceFactory （esriGeoDatabase）	提供对创建和打开工作区以及提供工作区工厂信息的成员的访问权限。
            IWorkspaceFactory2 （esriGeoDatabase）	提供对创建和打开工作区以及提供工作区工厂信息的成员的访问权限。
            ShapefileWorkspaceFactory 是其类的唯一实例。
             */

            // 打开工作空间
            pFeatureWorkspace = (IFeatureWorkspace)pWorkspaceFactory.OpenFromFile(pFilePath, 0);
            // 打开要素类
            IFeatureClass pFeatureClass = pFeatureWorkspace.OpenFeatureClass(pFileName);
            // 创建要素图层
            pFeatureLayer = new FeatureLayer();
            //将打开的要素类赋值给要素图层
            pFeatureLayer.FeatureClass = pFeatureClass;
            pFeatureLayer.Name = pFeatureLayer.FeatureClass.AliasName; //将pFeatureLayer的Name属性设置为pFeatureLayer所关联的FeatureClass的AliasName属性的值。
            //ClearAllData();
            axMapControl2.Map.AddLayer(pFeatureLayer);
            axMapControl2.ActiveView.Refresh();
            axMapControl1.Map.AddLayer(pFeatureLayer);
            axMapControl1.ActiveView.Refresh();
            //SynchronizeEagleEye();

            /*通过MapControl的AddShapefile方法加载Shapefile文件*/
            //axMapControl2.AddShapeFile(pFilePath,pFileName); //文件路径，不带扩展名的文件名
        }

        private void axMapControl1_OnMouseDown(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseDownEvent e)
        {

        }

        private void axMapControl2_OnMouseDown(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseDownEvent e)
        {
            axMapControl2.Pan();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("此应用为打开ShapeFile文件");
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            OpenFileDialog pOpenFileDialog = new OpenFileDialog();
            pOpenFileDialog.CheckFileExists = true;
            pOpenFileDialog.Title = "打开地图文档";
            pOpenFileDialog.Filter = "ArcMap 文档(*.mxd)|*.mxd;|ArcMap 模板(*.mxt)|*.mxt|发布地图文件(*.pmf)|*.pmf|所有地图格式(*mxd;*.mxt;*.pmf)|*.mxd;*.mxt;*.pmf";
            pOpenFileDialog.Multiselect = false;
            pOpenFileDialog.RestoreDirectory = true;
            if (pOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                string pFileName = pOpenFileDialog.FileName;
                if (pFileName == "")
                {
                    return;
                }
                if (axMapControl2.CheckMxFile(pFileName))
                {
                    //ClearAllData();
                    axMapControl2.LoadMxFile(pFileName);
                }
                else
                {
                    MessageBox.Show(pFileName + "是无效的地图文档！", "信息提示");
                    return;
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            OpenFileDialog pOpenFileDialog = new OpenFileDialog();
            pOpenFileDialog.CheckFileExists = true;
            pOpenFileDialog.Title = "打开栅格文件";
            pOpenFileDialog.Filter = "栅格文件(*.*)|*.bmp;*.tif;*.jpg;*.img|(*.bmp)|*.bmp|(*.tif)|*.tif|(*.jpg)|*.jpg|(*.img)|*.jpg|(*.img)|*.img";
            pOpenFileDialog.ShowDialog();
            string pRasterFileName = pOpenFileDialog.FileName;
            if (pRasterFileName == "") {
                return;
            }
            string pPath = System.IO.Path.GetDirectoryName(pRasterFileName);
            String pFileName = System.IO.Path.GetFileName(pRasterFileName);
            IWorkspaceFactory pWorkspaceFactory = new RasterWorkspaceFactory(); //RasterWorkspaceFactory()
            IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(pPath, 0);
            IRasterWorkspace pRasterWorkspace = pWorkspace as IRasterWorkspace;
            IRasterDataset pRasterDataset = pRasterWorkspace.OpenRasterDataset(pFileName);
            //影像金字塔判断与创建
            IRasterPyramid3 pRasPyrmid;
            pRasPyrmid = pRasterDataset as IRasterPyramid3;
            if (pRasPyrmid != null)
            {
                if (!(pRasPyrmid.Present)) 
                {
                    pRasPyrmid.Create();//创建金字塔
                }
            }
            IRaster pRaster;
            pRaster = pRasterDataset.CreateDefaultRaster();
            IRasterLayer pRasterLayer;
            pRasterLayer = new RasterLayerClass();
            pRasterLayer.CreateFromRaster(pRaster);
            ILayer pLayer = pRasterLayer as ILayer;
            axMapControl2.AddLayer(pLayer, 0);

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            //获取第一个图层名
            IFeatureLayer rn = axMapControl2.get_Layer(0) as IFeatureLayer; //as接口跳转
            string Layername = rn.Name;
            MessageBox.Show(Layername);
        }  

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            // 获取用户输入的新名称
            string newLayerName = toolStripTextBox1.Text;
            // 获取第一个图层
            IFeatureLayer layer = axMapControl2.get_Layer(0) as IFeatureLayer;
            if (layer != null && newLayerName != null)
            {
                // 修改图层的名称
                layer.Name = newLayerName;
                // 更新图层列表
                axTOCControl1.Update();
            }     
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            IFeatureLayer scal = axMapControl2.get_Layer(0) as IFeatureLayer;
            scal.MaximumScale = 100000;
            scal.MinimumScale = 1000000;
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            axMapControl2.Extent = axMapControl2.FullExtent;
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            ExportMap f = new ExportMap();
            f.ShowDialog();
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {

        }

        private void 显示图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IFeatureLayer TVisible = axMapControl2.get_Layer(0) as IFeatureLayer;
            TVisible.Visible = true;
        }

        private void 关闭图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IFeatureLayer NVisible = axMapControl2.get_Layer(0) as IFeatureLayer;
            NVisible.Visible = false;
        }

        private void 添加字段ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddField ad = new AddField();
            ad.BuddyMap = axMapControl2;
            ad.ShowDialog();
            
        }




    }
}