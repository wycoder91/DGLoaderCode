using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LoaderCodeManageModels;
using LoaderCodeManageBLL;
using System.Text.RegularExpressions;

namespace LoaderCodeManageUI
{
    public partial class FrmLoaderCode : Form
    {
        private string LoaderDefaultConfigration = "车型配置按如下格式输入：\r\n" +
            "山推/液力传动/5吨/长轴距/无/柴油/国2/无/煤炭版/潍柴/杭齿双变/超短臂/标准斗/非先导双手柄/国内";
        public FrmLoaderCode()
        {
            InitializeComponent();
            this.tbExampleShow.Text = this.LoaderDefaultConfigration;
        }

        #region
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private bool isInMove;
        private Point oldPoint;
        void EasyMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isInMove) return;
            Point pt = PointToScreen(e.Location);
            if (pt.X == oldPoint.X || pt.Y == oldPoint.Y) return;
            this.Location = new Point(this.Location.X + pt.X - oldPoint.X, this.Location.Y + pt.Y - oldPoint.Y);
            oldPoint = pt;
        }

        void EasyMove_MouseUp(object sender, MouseEventArgs e)
        {
            isInMove = false;
        }

        void EasyMove_MouseDown(object sender, MouseEventArgs e)
        {
            isInMove = true;
            oldPoint = PointToScreen(e.Location);
        }
        #endregion
        private void FrmLoaderCode_Load(object sender, EventArgs e)
        {

        }

        private void tbConfigEnter_TextChanged(object sender, EventArgs e)
        {
            //this.lbProductCode.Text = this.tbConfigEnter.Text;
            string loaderConfigUI = this.tbConfigEnter.Text;
                       
            if (loaderConfigUI.Contains("\r\n"))
            {
                loaderConfigUI = loaderConfigUI.Replace("\r\n", "");
                if (CheckEffective(loaderConfigUI) == true)
                {
                    LoaderCode loaderCode = null, reCodeLoaderCode = null, reConfigLoaderCode = null;

                    LoaderCodeManager loaderCodeManager = new LoaderCodeManager();
                    loaderCode = loaderCodeManager.CreateLoaderCode(loaderConfigUI, out reCodeLoaderCode, out reConfigLoaderCode);
                    if (reCodeLoaderCode == null && reConfigLoaderCode == null)
                    {
                        this.lbProductCode.Text = loaderCode.wholeCode;
                    }
                    else
                    {
                        if (reConfigLoaderCode != null)
                        {
                            this.lbProductCode.Text = reConfigLoaderCode.wholeCode;
                            this.lbProductCode.Text += "\r\n";
                            this.lbProductCode.Text += reConfigLoaderCode.wholeConfigration;
                            MessageBox.Show("配置重复！");
                        }
                        if(reCodeLoaderCode != null)
                        {
                            this.lbProductCode.Text = reCodeLoaderCode.wholeCode;
                            this.lbProductCode.Text += "\r\n";
                            this.lbProductCode.Text += reCodeLoaderCode.wholeConfigration;
                            MessageBox.Show("编码重复！");
                        }                       
                    }                   
                }
                else
                {
                    MessageBox.Show("输入出错，请重新输入！");
                }
                loaderConfigUI.Remove(0, loaderConfigUI.Length);
                this.tbConfigEnter.Text = null;
            }
        }
        /// <summary>
        /// 检查输入车型数据的有效性
        /// 检查个数是否达标
        /// </summary>
        /// <param name="loaderConfigUI"></param>
        /// <returns></returns>
        private bool CheckEffective(string loaderConfigUI)
        {
            int num = Regex.Matches(loaderConfigUI, "/").Count;
            if (num < 14) return false;
            else return true;
        }
        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("使用说明：\r\n车型配置输入如示例所示，配置信息必须按顺序用斜杆/隔开，没有填无！");
        }
    }
}
