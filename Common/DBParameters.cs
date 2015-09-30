using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataLinkage
{

    /// <summary>
    /// DBのパラメータクラス
    /// </summary>
    /// <remarks>
    /// 別にEntity.csにまとめても良かったが可読性を上げる為、別ファイル化
    /// </remarks>
    public class DBParameters
    {
        /// <summary>
        /// パラメータ名
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// パラメータの種別
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        /// パラメータがプライマリキーかどうか
        /// </summary>
        public bool Primary { get; set; } 

        /// <summary>
        /// 引数つきコンストラクタ
        /// </summary>
        /// <param name="name">パラメータ名</param>
        /// <param name="type">パラメータ型</param>
        /// <param name="primary">プライマリキーかどうか</param>
        public DBParameters(string name, DbType type,bool primary)
        {
            this.ParameterName = name;
            this.DbType = type;
            this.Primary = primary;
        }
    }
}
