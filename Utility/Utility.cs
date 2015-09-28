using System;
using System.Collections.Generic;
using System.Text;

namespace DataLinkage
{
    public static class Utility
    {
        /// <summary>
        /// 接続文字列
        /// </summary>
        public const string CONNECTION_STRING = "context connection=true";

        /// <summary>
        /// 処理種別 定数
        /// </summary>
        public enum FUNC_TYPE { INSERT, UPDATE, SELECT };
    }
}
