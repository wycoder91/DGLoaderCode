using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoaderCodeManageModels;
using LoaderCodeManageDAL;

namespace LoaderCodeManageBLL
{
    /// <summary>
    /// 有关编码生成后的业务逻辑
    /// </summary>
    public class LoaderCodeManager
    {
        private int loaderCodeIdRecord = 0;
        private string[] paramNumStr = {
            "productModel", "transferMethod", "tonnage", "wheelbase","special",
            "powerForm","emission","configurationUpgrade","formatNumber","engine",
            "gearbox","boom","bucket","controlMethod","sales","optional"
        };
        private string[] paraLCStr = { "wholeCode", "wholeConfigration" }; 
        enum JudgePara
        {
            useWholeCode=0,
            usewholeConfigration=1
        };
        #region      
        /// <summary>
        /// 生成编码，有重复保存到reLoaderCode
        /// </summary>
        /// <param name="enterLoaderConfig"></param>
        /// <param name="reLoaderCode"></param>
        /// <returns></returns>
        public LoaderCode CreateLoaderCode(string enterLoaderConfig, out LoaderCode reCodeLoaderCode, out LoaderCode reConfigLoaderCode)
        {
            LoaderCode loaderCode=null;
            reCodeLoaderCode = null;
            reConfigLoaderCode = null;
            //【1】判断车型配置wholeConfigration，已有车型不做处理
            //【2】若数据库中没有此类车型，而后根据LoaderConfigrationBase类和数据库LoaderConfigrationBase表对比，
            //     生成编码LoaderCode：wholeCode
            //【3】并把LoaderConfigrationBase中的配置生成到LoaderCode：wholeConfigration
            //【4】将生成的LoaderCode加入数据库中的LoaderCode表
            if ((loaderCode = CreateCode(enterLoaderConfig, out reConfigLoaderCode)) != null)
            {
                if ((reCodeLoaderCode = JudgeLoaderCodeRepeat(loaderCode, JudgePara.useWholeCode)) == null)
                {
                    int temp = InsertLoaderCode(loaderCode);
                }
            }
            return loaderCode;
        }
        /// <summary>
        /// 判断车型配置是否和数据库中的LoaderCode表中配置重复
        /// 重复弹出重复的配置表对应条目
        /// 不重复弹出null值，注意：还有一种读不到数据的情况，事后要考虑！
        /// </summary>
        private LoaderCode JudgeLoaderCodeRepeat(LoaderCode newLoaderCode, JudgePara judgePara)
        {
            LoaderCode tempLoaderCode;
            object tempObject = null;           
            string tempString = null,tempLCstr=null;
            if (judgePara == JudgePara.useWholeCode) tempLCstr = newLoaderCode.wholeCode;
            if (judgePara == JudgePara.usewholeConfigration) tempLCstr = newLoaderCode.wholeConfigration;
            LoaderCodeService loaderCodeService = new LoaderCodeService();
            loaderCodeIdRecord = (int)loaderCodeService.GetLoaderCodeCount();
            loaderCodeIdRecord += 100;
            for (int id = 100; id < loaderCodeIdRecord; id++)
            {
                if (null == (tempObject = loaderCodeService.GetLoaderCodeDataById(id, (string)paraLCStr[(int)judgePara])))
                { break; }              
                tempString = tempObject.ToString();
                if (String.Compare(tempString, tempLCstr) == 0)
                {
                    tempLoaderCode = loaderCodeService.ReaderLoaderCodeById(id);
                    return tempLoaderCode;
                }
            }
            return null;
        }
        /// <summary>
        /// 查询当前库中LoaderCode表中，WholeCode前十六位相同的个数并返回
        /// </summary>
        /// <returns></returns>
        private int JudgeLoaderCodeRepeat(string code)
        {
            object tempObject = null;
            string tempString;
            int countNum = 0;
            if (code.Length != 16)
                return 0;
            LoaderCodeService loaderCodeService = new LoaderCodeService();
            loaderCodeIdRecord = (int)loaderCodeService.GetLoaderCodeCount();
            loaderCodeIdRecord += 100;
            for (int id = 100; id < loaderCodeIdRecord; id++)
            {
                if (null == (tempObject = loaderCodeService.GetLoaderCodeDataById(id, (string)paraLCStr[(int)JudgePara.useWholeCode])))
                { break; }
                tempString = tempObject.ToString();
                if (tempString.Length < 16) continue;
                tempString = tempString.Substring(0, 16);
                if (String.Compare(tempString, code) == 0)
                {
                    countNum++;
                }
            }        
            return countNum;
        }
        /// <summary>
        /// 生成输入的车型配置对应的编码
        /// 并封装到LoaderCode类
        /// 默认enterLoaderConfig输入是有效的，不少不多，但是允许每项输入错别字，有效性交由UI控制。
        /// </summary>
        /// <param name="loaderConfigBase"></param>
        /// <returns></returns>
        private LoaderCode CreateCode(string enterLoaderConfig, out LoaderCode reLoaderCode)
        {
            object baseSingleResult = null;
            string[] tmpArray;
            string[] loaderConfigArray = enterLoaderConfig.Split('/');
            string code = null;
            int id = 1, col = 0;//col = 0,
            bool judgeFlag = false;
            reLoaderCode = null;
            LoaderCodeService loaderCodeService = new LoaderCodeService();
            LoaderCode loaderCode = new LoaderCode();
            loaderCode.wholeConfigration = enterLoaderConfig;
            for (col = 0; col < paramNumStr.Length; col++)
            //foreach (string itemLoaderConfig in loaderConfigArray)
            {
                if (paramNumStr[col] == "optional")
                {
                    if ((reLoaderCode = JudgeLoaderCodeRepeat(loaderCode, JudgePara.usewholeConfigration)) == null)
                    { 
                        int tmpNum = JudgeLoaderCodeRepeat(code);
                        if (tmpNum < 10) code += 0;
                        code += tmpNum;
                        break;
                    }
                    return null;
                }
                while ((baseSingleResult = loaderCodeService.GetLoaderConfigrationBaseSingle(paramNumStr[col], id++)) != null)
                {
                    tmpArray = baseSingleResult.ToString().Split(':');

                    if ((id < 16) && (String.Compare(tmpArray[0], loaderConfigArray[col]) == 0))
                    {
                        code = String.Concat(code, tmpArray[1]);
                        judgeFlag = true;
                        break;
                    }
                }
                id = 1;
                if (judgeFlag == false)
                {
                    code = String.Concat(code, "*");
                }
                judgeFlag = false;
            }
            loaderCode.wholeCode = code;           
            return loaderCode;
        }       
        /// <summary>
        /// 插入数据到数据表LoaderCode中,返回受影响的行数
        /// </summary>
        /// <param name="loaderConfigBase"></param>
        /// <returns></returns>
        private int InsertLoaderCode(LoaderCode loaderCode)
        {
            LoaderCodeService loaderCodeService = new LoaderCodeService();
            if ((loaderCode == null) || (loaderCode.wholeCode.Contains("*"))) return 0;          
            return loaderCodeService.AddLoaderCode(loaderCode);             
        }
        #endregion
    }
}
