using System;
using System.Collections.Generic;
using System.Text;

namespace DataLinkage
{
    /// <summary>
    /// 共通クラス
    /// </summary>
    public static class Common
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
