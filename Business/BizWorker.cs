using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace DataLinkage
{
    /// <summary>
    /// DB関連処理クラス
    /// </summary>
    public class BizWorker
    {
        /// <summary>
        /// ログ出力用クラス
        /// </summary>
        protected Logger myLogger = new Logger();

        /// <summary>
        /// クラス名
        /// </summary>
        private string ClassName;

        /// <summary>
        /// ファイル名
        /// </summary>
        private string AssemblyName;

        /// <summary>
        /// 検索結果がない場合
        /// </summary>
        private const DataRow NOT_MATCHED = null;

        Entry myEntry = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BizWorker()
        {
            ClassName = this.GetType().FullName;
            AssemblyName = Path.GetFileName(this.GetType().Assembly.Location);
        }

        /// <summary>
        /// 引数つきコンストラクタ
        /// </summary>
        /// <param name="_myEntity">各エントリークラス</param>
        public BizWorker(Entry _myEntity)
        {
            //実態を渡す
            myEntry = _myEntity;
            ClassName = this.GetType().FullName;
            AssemblyName = Path.GetFileName(this.GetType().Assembly.Location);
        }


        /// <summary>
        /// 主処理関数
        /// </summary>
        public void Worker()
        {
            //トランザクション
            SqlTransaction tran = null;

            myLogger.LogWrite("ログの開始", MethodBase.GetCurrentMethod().Name, ClassName, AssemblyName);

            SqlContext.Pipe.Send("ログの開始");

            try
            {
                DataTable tempTable = new DataTable();

                // コードをここに記述してください
                using (SqlConnection conn =
                    new SqlConnection(Common.CONNECTION_STRING))
                {
                    // オープン
                    conn.Open();

                    //TEMPテーブルを取得
                    using (SqlCommand selectCommand
                        = new SqlCommand(myEntry.GetSourceSelectCommandText(), conn))
                    {
                        // TEMPテーブルの取得
                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            //TEMPテーブルの全取得
                            tempTable.Load(reader);
                        }
                    }

                    // トランザクション処理の開始
                    // トランザクション処理フラグがたっていないときは内部でtran処理しない
                    tran = BeginTransaction(conn, myEntry);
                    
                    //更新処理用テーブル
                    using (SqlDataAdapter dtadapbter = new SqlDataAdapter())
                    {
                        //Select用のパラメータを作成
                        dtadapbter.SelectCommand = SetSqlCommand(Common.FUNC_TYPE.SELECT, conn, myEntry, tran);
                        SetDbParameters(dtadapbter.SelectCommand, myEntry);

                        //Insert用のパラメータを作成
                        dtadapbter.InsertCommand = SetSqlCommand(Common.FUNC_TYPE.INSERT, conn, myEntry, tran);
                        SetDbParameters(dtadapbter.InsertCommand, myEntry);
                        
                        //Update用のパラメータを作成
                        dtadapbter.UpdateCommand = SetSqlCommand(Common.FUNC_TYPE.UPDATE, conn, myEntry, tran);
                        SetDbParameters(dtadapbter.UpdateCommand, myEntry);

                        DataSet dataset = new DataSet();
                        dtadapbter.Fill(dataset, myEntry.DestTable);

                        DataTable distTable = new DataTable();
                        distTable = dataset.Tables[myEntry.DestTable];

                        // PrimaryKey（Merge文のon句にあたるもの）の設定
                        distTable.PrimaryKey = GetPrimaryKey(myEntry,distTable);

                        //更新系の処理
                        Upsert(tempTable, distTable);

                        // updeteの実施
                        dtadapbter.Update(distTable);

                        //処理の終了
                        EndTransaction(tran, true);
                    }
                }
                myLogger.LogWrite("Mergeの正常終了", MethodBase.GetCurrentMethod().Name, ClassName, AssemblyName);
            }
            catch (SqlException e)
            {
                myLogger.LogWrite(e.Message, MethodBase.GetCurrentMethod().Name, ClassName, AssemblyName, e.ErrorCode);
                EndTransaction(tran, false);
                throw e;
            }
            catch (Exception e) 
            {
                myLogger.LogWrite(e.Message, MethodBase.GetCurrentMethod().Name, ClassName, AssemblyName);
                EndTransaction(tran, false);
                throw e;
            }

            myLogger.LogWrite("ログの終了", MethodBase.GetCurrentMethod().Name, ClassName, AssemblyName);
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="_sourceTbl">参照元TBL</param>
        /// <param name="_distTbl">参照先TBL</param>
        /// <remarks>
        /// </remarks>
        protected virtual void Upsert(DataTable _sourceTbl, DataTable _distTbl)
        {
            DataColumnCollection columns = _distTbl.Columns;

            // TEMPテーブル文の処理
            foreach (DataRow row in _sourceTbl.Rows)
            {
                // 挿入フラグ
                bool insertFlg = false;

                //primarykeyとなっているものの値配列を取得
                object[] keyValue = GetPrimaryKeyValue(row,_distTbl.PrimaryKey);
                
                //object[] primaryKeyValue = new object()
                DataRow findRow = _distTbl.Rows.Find(keyValue);

                // not matched のみ
                if (findRow == NOT_MATCHED)
                {
                    insertFlg = true;

                    //新規行の追加
                    findRow = _distTbl.NewRow();
                }

                // 参照元データカラム文の処理
                foreach (DataColumn sColumn in _sourceTbl.Columns)
                {
                    // 同一名のカラムがある場合のみ更新
                    if(columns.Contains(sColumn.ColumnName)){
                        findRow[sColumn.ColumnName] = row[sColumn.ColumnName];
                    }
                }

                // 新規挿入の場合
                if (insertFlg)
                {
                    _distTbl.Rows.Add(findRow);
                }
            }
        }

        /// <summary>
        /// プライマリーキーの配列を取得する
        /// </summary>
        /// <param name="_my">entryクラス</param>
        /// <param name="_dt">dest用table</param>
        /// <returns></returns>
        private DataColumn[] GetPrimaryKey(Entry _my, DataTable _dt)
        {
            var collect = new List<DataColumn>();

            //パラメータリストからプライマリキーに指定されているパラメータをのみをコレクションに追加
            foreach (DBParameters param in _my.DbParamList)
            {
                if (param.Primary == true)
                {
                    collect.Add(_dt.Columns[param.ParameterName]);
                }
            }
            DataColumn[] dataColumnArray = new DataColumn[collect.Count];

            collect.CopyTo(dataColumnArray, 0);

            return dataColumnArray;
        }


        /// <summary>
        /// プライマリーキーの配列の値配列を取得る
        /// </summary>
        /// <param name="_row">desttableの行データ</param>
        /// <param name="_primaryKeys">プライマリーKEY配列</param>
        /// <returns></returns>
        private object[] GetPrimaryKeyValue(DataRow _row, DataColumn[] _primaryKeys)
        {
            var collect = new List<object>();

            //パラメータリストからプライマリキーに指定されているパラメータをのみをコレクションに追加
            foreach (DataColumn col in _primaryKeys)
            {
                collect.Add(_row[col.ColumnName]);
            }
            object[] valueArray = new object[collect.Count];

            collect.CopyTo(valueArray, 0);

            return valueArray;
        }

        /// <summary>
        ///  SQLコマンドを設定する
        /// </summary>
        /// <param name="_type">機能種別定数</param>
        /// <param name="_conn">DB接続変数</param>
        /// <param name="_entry">エントリークラス</param>
        /// <param name="_tran">トランザクション・トランザクション未指定の場合,NULLが入ってくる</param>
        /// <returns></returns>
        private SqlCommand SetSqlCommand(
            Common.FUNC_TYPE _type, 
            SqlConnection _conn,
            Entry _entry,
            SqlTransaction _tran = null
            )
        {
            //SQL文の作成
            SqlCommand sqlCommand = _conn.CreateCommand();

            switch (_type)
            {
                case Common.FUNC_TYPE.INSERT:

                    sqlCommand.CommandText = _entry.GetInsertCommandText();

                    break;
                case Common.FUNC_TYPE.UPDATE:

                    sqlCommand.CommandText = _entry.GetUpdateCommandText();
                    break;

                case Common.FUNC_TYPE.SELECT:
                    sqlCommand.CommandText = _entry.GetDestSelectCommandText();
                    break;
                default:
                    SqlContext.Pipe.Send("なにも設定されていません");
                    return null;
            }

            //トランザクション処理の場合のみ
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }

            return sqlCommand;
        }

        /// <summary>
        /// DBパラメータの設定
        /// </summary>
        /// <param name="_command">SQLコマンド</param>
        /// <param name="_entry">エントリークラス</param>
        /// <returns></returns>
        private bool SetDbParameters(SqlCommand _command,Entry _entry){

            bool result = false;
            try { 

                ////パラメータの作成
                SqlParameter param = _command.CreateParameter();

                //パラメータ全てを設定
                foreach (DBParameters list in _entry.DbParamList)
                {
                    param = _command.CreateParameter();
                    param.ParameterName = string.Format("@{0}",list.ParameterName);
                    param.DbType = list.DbType;
                    param.SourceColumn = list.ParameterName;
                    _command.Parameters.Add(param);
                }
                result = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }


        /// <summary>
        /// トランザクション処理の開始を宣言
        /// </summary>
        /// <param name="_conn">コネクションクラス</param>
        /// <param name="_entry">エントリークラス</param>
        /// <returns></returns>
        private SqlTransaction BeginTransaction(SqlConnection _conn, Entry _entry)
        {
            SqlTransaction tran = null;

            //transaction処理を行う場合
            if (_entry.IsTransaction)
            {
                tran = _conn.BeginTransaction(IsolationLevel.Serializable);
            }

            return tran;
        }

        /// <summary>
        /// トランザクション処理の終了を宣言
        /// </summary>
        /// <param name="_tran">トランザクション</param>
        /// <param name="funcFlg">
        /// 処理フラグ
        /// true:処理commit false:処理rollback
        /// </param>
        private void EndTransaction(SqlTransaction _tran,bool funcFlg)
        {
            //トランザクション処理が行われる場合のみ
            if (_tran == null) return;

            //処理完了時
            if (funcFlg)
            {
                _tran.Commit();
            }
            //処理終了時
            else
            {
                _tran.Rollback();
            }
        }
    }
}

                    
