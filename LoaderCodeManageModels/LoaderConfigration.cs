using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoaderCodeManageModels
{
    //实体类，封装车型配置数据集合
   public class LoaderConfigrationBase
    {
        public int LoaderConfigrationId { set; get; }
        //前十六位车型配置数据
        public string productModel { set; get; } //产品机种
        public string transferMethod { set; get; } //传动方式
        public string tonnage { set; get; } //吨位代码
        public string wheelbase { set; get; } //轴距代号
        public string special { set; get; } //特殊代号
        public string powerForm { set; get; } //动力形式代号
        public string emission { set; get; } //排放代号
        public string configurationUpgrade { set; get; } //配置升级代号
        public string formatNumber { set; get; } //9、10位 版式代号
        public string engine { set; get; } //发动机代号
        public string gearbox { set; get; } //变速箱代号
        public string boom { set; get; } //动臂代号
        public string bucket { set; get; } //铲斗代号
        public string controlMethod { set; get; } //操控方式代号
        public string sales { set; get; } //销售代号
        //17、18位
        public string optional { set; get; } //17、18位 可选配置组合流水号

    }
}
