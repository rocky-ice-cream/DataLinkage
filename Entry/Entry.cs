using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataLinkage
{
    /// <summary>
    /// DB処理実行用クラス（基底クラス）
    /// </summary>
    /// <remarks>
    /// DB処理に関するSQL、TABLE名、パラメータなどを設定するクラス
    /// SQLなどは処理固有になる為、派生クラス側で記述する。
    /// </remarks>
    public class Entry : IDisposable
    {
        // Dispose したかどうか 
        private bool _disposed = false;

        /// <summary>
        /// 参照元テーブル
        /// </summary>
        public string SourceTable { get; set; }

        /// <summary>
        /// 参照先テーブル
        /// </summary>
        public string DestTable { get; set; }

        /// <summary>
        /// パラメータリスト
        /// </summary>
        public List<DBParameters> DbParamList = null;

        /// <summary>
        /// トランザクション処理を行うかの判定
        /// </summary>
        public bool IsTransaction { get; set; }
                
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Entry(){
            this.DbParamList = new List<DBParameters>();
            this.IsTransaction = false;
            this._disposed = false;
            this.SourceTable = string.Empty;
            this.DestTable = string.Empty;
        }

        /// <summary>
        /// DB接続に関する情報を設定
        /// </summary>
        public virtual void SetDbAccessInfo() { }

        /// <summary>
        /// DBパラメータの設定
        /// </summary>
        /// <param name="name">パラメータ名</param>
        /// <param name="type">パラメータ型</param>
        /// <param name="primary">プライマリキーかどうか</param>
        /// <returns></returns>
        protected bool SetDBParameters(string name,DbType type, bool primary = false) {

            //Listに格納
            this.DbParamList.Add(new DBParameters(name, type, primary));

            return true;
        }

        // IDisposable に必須のメソッドの実装 
        public void Dispose() { 
            Dispose(true); 
            // Dispose() によってリソースの解放を行ったので、 
            // GC での解放が必要が無いことを GC に通知。 
            GC.SuppressFinalize(this); 
        }

        /// <summary>
        /// 解放処理の実行
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) { 
	        // Dispose がまだ実行されていないときだけ実行 
            if (!_disposed)
            {
                // disposing が true の場合(Dispose() が実行された場合)は 
                // マネージリソースも解放。 
                if (disposing)
                {
                    // マネージリソースの解放 
                    if (this.DbParamList != null)
                    {
                        this.DbParamList.Clear();
                        this.DbParamList = null;                       
                    }
                } 
                // アンマネージリソースの解放 
                this.DestTable = string.Empty;
                this.SourceTable = string.Empty;
                _disposed = true;
            }
        }

        /// <summary>
        /// 参照元Select文を返す
        /// </summary>
        /// <returns>参照元検索SQL</returns>
        public virtual string GetSourceSelectCommandText()
        {
            StringBuilder strSelectCommand = new StringBuilder();

            return strSelectCommand.ToString();
        }

        /// <summary>
        /// 参照先Select文を返す
        /// </summary>
        /// <returns>参照先検索SQL</returns>
        public virtual string GetDestSelectCommandText()
        {
            StringBuilder strSelectCommand = new StringBuilder();

            return strSelectCommand.ToString();
        }

        /// <summary>
        /// Update文を返す
        /// </summary>
        /// <returns>参照先更新SQL</returns>
        public virtual string GetUpdateCommandText()
        {
            return string.Empty;
        }


        /// <summary>
        /// Insert文を返す
        /// </summary>
        /// <returns>参照先挿入SQL</returns>
        public virtual string GetInsertCommandText()
        {
            return string.Empty;
        }
        
    }

}
