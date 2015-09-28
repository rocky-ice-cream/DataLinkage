using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataLinkage
{
    /// <summary>
    /// 顧客テーブル用の処理
    /// </summary>
    public class BizWorker
    {
        /// <summary>
        /// 検索結果がない場合
        /// </summary>
        private const DataRow NOT_MATCHED = null;

        Entry myEntry = null;

        ///// <summary>
        ///// 参照元テーブル名
        ///// </summary>
        //private string m_SourceTable = "[dbo].[tmp_dtb_customer]";

        ///// <summary>
        ///// 参照先テーブル名
        ///// </summary>
        //private string m_DestTable = "[dbo].[dtb_customer_test]";

        ///// <summary>
        ///// 出力元SQL
        ///// </summary>
        //private StringBuilder m_SourceSQL = new StringBuilder(); 

        /////// <summary>
        /////// 出力先SQL
        /////// </summary>
        //private StringBuilder m_DestSQL = new StringBuilder();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BizWorker()
        {
            //m_SourceSQL.AppendFormat("select * from {0}",m_SourceTable);
            //m_DestSQL.AppendFormat("select * from {0}", m_DestTable);
        }

        /// <summary>
        /// 引数つきコンストラクタ
        /// </summary>
        /// <param name="_myEntity">各エントリークラス</param>
        public BizWorker(Entry _myEntity)
        {
            //実態を渡す
            myEntry = _myEntity;

            //m_SourceSQL.AppendFormat("select * from {0}", m_SourceTable);
            //m_DestSQL.AppendFormat("select * from {0}", m_DestTable);
        }


        /// <summary>
        /// 主処理関数
        /// </summary>
        public void Worker()
        {
            //トランザクション
            SqlTransaction tran = null;

            try
            {
                DataTable tempTable = new DataTable();

                // コードをここに記述してください
                using (SqlConnection conn =
                    new SqlConnection(Utility.CONNECTION_STRING))
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
                        dtadapbter.SelectCommand = SetSqlCommand(Utility.FUNC_TYPE.SELECT, conn, tran);
                        SetDbParameters(dtadapbter.SelectCommand);

                        //Insert用のパラメータを作成
                        dtadapbter.InsertCommand = SetSqlCommand(Utility.FUNC_TYPE.INSERT, conn, tran);
                        SetDbParameters(dtadapbter.InsertCommand);
                        
                        //Update用のパラメータを作成
                        dtadapbter.UpdateCommand = SetSqlCommand(Utility.FUNC_TYPE.UPDATE, conn, tran);
                        SetDbParameters(dtadapbter.UpdateCommand);

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
                SqlContext.Pipe.Send("Mergeの正常終了");
            }
            catch (SqlException e)
            {
                SqlContext.Pipe.Send(e.Message);
                EndTransaction(tran, false);
                throw e;
            }
            catch (Exception e) 
            {
                SqlContext.Pipe.Send(e.Message);
                EndTransaction(tran, false);
                throw e;
            }
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="_sourceTbl">参照元TBL</param>
        /// <param name="_distTbl">参照先TBL</param>
        /// <remarks>
        /// </remarks>
        private void Upsert(DataTable _sourceTbl, DataTable _distTbl)
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

                // 行の追加・更新
                // 条件が必要なときはここを修正
                // ↑↑↑ 上の処理で対応
                //OverWriteRows(findRow, row);

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
        /// <param name="_my"></param>
        /// <param name="_dt"></param>
        /// <returns></returns>
        private DataColumn[] GetPrimaryKey(Entry _my, DataTable _dt)
        {
            var collect = new List<DataColumn>();

            //パラメータリストからプライマリキーに指定されているパラメータをのみをコレクションに追加
            foreach (DBParameters param in myEntry.DbParamList)
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
        /// <param name="_my"></param>
        /// <param name="_dt"></param>
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

        ///// <summary>
        ///// 参照先データの上書き
        ///// </summary>
        ///// <param name="_findRow">参照先テーブル</param>
        ///// <param name="_row">参照元テーブル</param>
        ///// <remarks>
        ///// 該当レコードがない場合は新規行、
        ///// すでにある場合は既存行の更新を行う
        ///// </remarks>
        //private void OverWriteRows(DataRow _findRow,DataRow _row)
        //{
            

        //    //_findRow["influx_source"] = _row["influx_source"];
        //    //_findRow["customer_id"] = _row["customer_id"];
        //    //_findRow["name01"] = _row["name01"];
        //    //_findRow["name02"] = _row["name02"];
        //    //_findRow["kana01"] = _row["kana01"];
        //    //_findRow["kana02"] = _row["kana02"];
        //    //_findRow["zipcode"] = _row["zipcode"];
        //    //_findRow["pref"] = _row["pref"];
        //    //_findRow["addr01"] = _row["addr01"];
        //    //_findRow["addr02"] = _row["addr02"];
        //    //_findRow["tel"] = _row["tel"];
        //    //_findRow["sex"] = _row["sex"];
        //    //_findRow["job"] = _row["job"];
        //    //_findRow["birth"] = _row["birth"];
        //    //_findRow["email"] = _row["email"];
        //    //_findRow["mailmaga_flg"] = _row["mailmaga_flg"];
        //    //_findRow["first_buy_date"] = _row["first_buy_date"];
        //    //_findRow["last_buy_date"] = _row["last_buy_date"];
        //    //_findRow["buy_times"] = _row["buy_times"];
        //    //_findRow["buy_total"] = _row["buy_total"];
        //    //_findRow["point"] = _row["point"];
        //    //_findRow["use_point"] = _row["use_point"];
        //    //_findRow["coupon"] = _row["coupon"];
        //    //_findRow["note"] = _row["note"];
        //    //_findRow["status"] = _row["status"];
        //    //_findRow["create_date"] = _row["create_date"];
        //    //_findRow["create_time"] = _row["create_time"];
        //    //_findRow["create_user"] = _row["create_user"];
        //    //_findRow["update_date"] = _row["update_date"];
        //    //_findRow["update_time"] = _row["update_time"];
        //    //_findRow["update_user"] = _row["update_user"];
        //    //_findRow["del_flg"] = _row["del_flg"];
        //    //_findRow["common_no"] = _row["common_no"];
        //}


        /// <summary>
        ///  SQLコマンドを設定する
        /// </summary>
        /// <param name="_type">機能種別定数</param>
        /// <param name="_conn">DB接続変数</param>
        /// <param name="_tran"></param>
        /// <returns></returns>
        private SqlCommand SetSqlCommand(
            Utility.FUNC_TYPE _type, 
            SqlConnection _conn,
            SqlTransaction _tran = null)
        {
            //SQL文の作成
            SqlCommand sqlCommand = _conn.CreateCommand();

            switch (_type)
            {
                case Utility.FUNC_TYPE.INSERT:

                    sqlCommand.CommandText = myEntry.GetInsertCommandText();

                    break;
                case Utility.FUNC_TYPE.UPDATE:

                    sqlCommand.CommandText =  myEntry.GetUpdateCommandText();
                    break;

                case Utility.FUNC_TYPE.SELECT:
                    sqlCommand.CommandText = myEntry.GetDestSelectCommandText();
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
        /// <param name="_command"></param>
        /// <returns></returns>
        private bool SetDbParameters(SqlCommand _command){

            bool result = false;
            try { 

                ////パラメータの作成
                SqlParameter param = _command.CreateParameter();

                //パラメータ全てを設定
                foreach (DBParameters list in myEntry.DbParamList)
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

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@influx_source";
            //param.DbType = DbType.String;
            //param.SourceColumn = "influx_source";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@name01";
            //param.DbType = DbType.String;
            //param.SourceColumn = "name01";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@name02";
            //param.DbType = DbType.String;
            //param.SourceColumn = "name02";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@kana01";
            //param.DbType = DbType.String;
            //param.SourceColumn = "kana01";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@kana02";
            //param.DbType = DbType.String;
            //param.SourceColumn = "kana02";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@zipcode";
            //param.DbType = DbType.String;
            //param.SourceColumn = "zipcode";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@addr01";
            //param.DbType = DbType.String;
            //param.SourceColumn = "addr01";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@addr02";
            //param.DbType = DbType.String;
            //param.SourceColumn = "addr02";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@tel";
            //param.DbType = DbType.String;
            //param.SourceColumn = "tel";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@email";
            //param.DbType = DbType.String;
            //param.SourceColumn = "email";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@note";
            //param.DbType = DbType.String;
            //param.SourceColumn = "note";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@customer_id";
            //param.DbType = DbType.Int32;
            //param.SourceColumn = "customer_id";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@pref";
            //param.DbType = DbType.Int16;
            //param.SourceColumn = "pref";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@sex";
            //param.DbType = DbType.Int16;
            //param.SourceColumn = "sex";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@job";
            //param.DbType = DbType.Int16;
            //param.SourceColumn = "job";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@birth";
            //param.DbType = DbType.Int32;
            //param.SourceColumn = "birth";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@mailmaga_flg";
            //param.DbType = DbType.Int16;
            //param.SourceColumn = "mailmaga_flg";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@first_buy_date";
            //param.DbType = DbType.Int32;
            //param.SourceColumn = "first_buy_date";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@last_buy_date";
            //param.DbType = DbType.Int32;
            //param.SourceColumn = "last_buy_date";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@point";
            //param.DbType = DbType.Int32;
            //param.SourceColumn = "point";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@use_point";
            //param.DbType = DbType.Int32;
            //param.SourceColumn = "use_point";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@coupon";
            //param.DbType = DbType.Int32;
            //param.SourceColumn = "coupon";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@status";
            //param.DbType = DbType.Int16;
            //param.SourceColumn = "status";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@create_date";
            //param.DbType = DbType.Int32;
            //param.SourceColumn = "create_date";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@create_time";
            //param.DbType = DbType.Int32;
            //param.SourceColumn = "create_time";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@create_user";
            //param.DbType = DbType.Int32;
            //param.SourceColumn = "create_user";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@update_date";
            //param.DbType = DbType.Int32;
            //param.SourceColumn = "update_date";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@update_time";
            //param.DbType = DbType.Int32;
            //param.SourceColumn = "update_time";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@update_user";
            //param.DbType = DbType.Int32;
            //param.SourceColumn = "update_user";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@del_flg";
            //param.DbType = DbType.Int16;
            //param.SourceColumn = "del_flg";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@buy_times";
            //param.DbType = DbType.Int32;
            //param.SourceColumn = "buy_times";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@buy_total";
            //param.DbType = DbType.Int16;
            //param.SourceColumn = "buy_total";
            //sqlCommand.Parameters.Add(param);

            //param = sqlCommand.CreateParameter();
            //param.ParameterName = "@common_no";
            //param.DbType = DbType.Int32;
            //param.SourceColumn = "common_no";
            //sqlCommand.Parameters.Add(param);

        }

        /// <summary>
        /// トランザクション処理の開始を宣言
        /// </summary>
        /// <param name="_entry"></param>
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

        ///// <summary>
        ///// InsertSQL（commandText）を返す
        ///// </summary>
        ///// <returns>CommandText</returns>
        //private string GetInsertCommandText()
        //{
        //    StringBuilder strInsertCommand = new StringBuilder();

        //    strInsertCommand.Append("INSERT INTO ");
        //    strInsertCommand.Append(myEntry.DestTable);
        //    strInsertCommand.Append("           ([influx_source]");
        //    //strInsertCommand.Append("           ,[customer_id]");
        //    strInsertCommand.Append("           ,[name01]");
        //    strInsertCommand.Append("           ,[name02]");
        //    strInsertCommand.Append("           ,[kana01]");
        //    strInsertCommand.Append("           ,[kana02]");
        //    strInsertCommand.Append("           ,[zipcode]");
        //    strInsertCommand.Append("           ,[pref]");
        //    strInsertCommand.Append("           ,[addr01]");
        //    strInsertCommand.Append("           ,[addr02]");
        //    strInsertCommand.Append("           ,[tel]");
        //    strInsertCommand.Append("           ,[sex]");
        //    strInsertCommand.Append("           ,[job]");
        //    strInsertCommand.Append("           ,[birth]");
        //    strInsertCommand.Append("           ,[email]");
        //    strInsertCommand.Append("           ,[mailmaga_flg]");
        //    strInsertCommand.Append("           ,[first_buy_date]");
        //    strInsertCommand.Append("           ,[last_buy_date]");
        //    strInsertCommand.Append("           ,[buy_times]");
        //    strInsertCommand.Append("           ,[buy_total]");
        //    strInsertCommand.Append("           ,[point]");
        //    strInsertCommand.Append("           ,[use_point]");
        //    strInsertCommand.Append("           ,[coupon]");
        //    strInsertCommand.Append("           ,[note]");
        //    strInsertCommand.Append("           ,[status]");
        //    strInsertCommand.Append("           ,[create_date]");
        //    strInsertCommand.Append("           ,[create_time]");
        //    strInsertCommand.Append("           ,[create_user]");
        //    strInsertCommand.Append("           ,[update_date]");
        //    strInsertCommand.Append("           ,[update_time]");
        //    strInsertCommand.Append("           ,[update_user]");
        //    strInsertCommand.Append("           ,[del_flg]");
        //    strInsertCommand.Append("           ,[common_no])"); //テスト的に共通番号列を作成
        //    strInsertCommand.Append("     VALUES");
        //    strInsertCommand.Append("           (@influx_source");
        //    //strInsertCommand.Append("           ,@customer_id");
        //    strInsertCommand.Append("           ,@name01");
        //    strInsertCommand.Append("           ,@name02");
        //    strInsertCommand.Append("           ,@kana01");
        //    strInsertCommand.Append("           ,@kana02");
        //    strInsertCommand.Append("           ,@zipcode");
        //    strInsertCommand.Append("           ,@pref");
        //    strInsertCommand.Append("           ,@addr01");
        //    strInsertCommand.Append("           ,@addr02");
        //    strInsertCommand.Append("           ,@tel");
        //    strInsertCommand.Append("           ,@sex");
        //    strInsertCommand.Append("           ,@job");
        //    strInsertCommand.Append("           ,@birth");
        //    strInsertCommand.Append("           ,@email");
        //    strInsertCommand.Append("           ,@mailmaga_flg");
        //    strInsertCommand.Append("           ,@first_buy_date");
        //    strInsertCommand.Append("           ,@last_buy_date");
        //    strInsertCommand.Append("           ,@buy_times");
        //    strInsertCommand.Append("           ,@buy_total");
        //    strInsertCommand.Append("           ,@point");
        //    strInsertCommand.Append("           ,@use_point");
        //    strInsertCommand.Append("           ,@coupon");
        //    strInsertCommand.Append("           ,@note");
        //    strInsertCommand.Append("           ,@status");
        //    strInsertCommand.Append("           ,@create_date");
        //    strInsertCommand.Append("           ,@create_time");
        //    strInsertCommand.Append("           ,@create_user");
        //    strInsertCommand.Append("           ,@update_date");
        //    strInsertCommand.Append("           ,@update_time");
        //    strInsertCommand.Append("           ,@update_user");
        //    strInsertCommand.Append("           ,@del_flg");
        //    strInsertCommand.Append("           ,@common_no)");//テスト的に共通番号列を作成
        //    return strInsertCommand.ToString();
        //}

        ///// <summary>
        ///// UpdateSQL（commandText）を返す
        ///// </summary>
        ///// <returns>CommandText</returns>
        //private string GetUpdateCommandText()
        //{
        //    StringBuilder strUpdateCommand = new StringBuilder();

        //    strUpdateCommand.Append("UPDATE ");
        //    strUpdateCommand.Append(myEntry.DestTable);
        //    strUpdateCommand.Append("   SET [influx_source] = @influx_source");
        //    strUpdateCommand.Append("      ,[name01] = @name01");
        //    strUpdateCommand.Append("      ,[name02] = @name02");
        //    strUpdateCommand.Append("      ,[kana01] = @kana01");
        //    strUpdateCommand.Append("      ,[kana02] = @kana02");
        //    strUpdateCommand.Append("      ,[zipcode] = @zipcode");
        //    strUpdateCommand.Append("      ,[pref] = @pref");
        //    strUpdateCommand.Append("      ,[addr01] = @addr01");
        //    strUpdateCommand.Append("      ,[addr02] = @addr02");
        //    strUpdateCommand.Append("      ,[tel] = @tel");
        //    strUpdateCommand.Append("      ,[sex] = @sex");
        //    strUpdateCommand.Append("      ,[job] = @job");
        //    strUpdateCommand.Append("      ,[birth] = @birth");
        //    strUpdateCommand.Append("      ,[email] = @email");
        //    strUpdateCommand.Append("      ,[mailmaga_flg] = @mailmaga_flg");
        //    strUpdateCommand.Append("      ,[first_buy_date] = @first_buy_date");
        //    strUpdateCommand.Append("      ,[last_buy_date] = @last_buy_date");
        //    strUpdateCommand.Append("      ,[buy_times] = @buy_times");
        //    strUpdateCommand.Append("      ,[buy_total] = @buy_total");
        //    strUpdateCommand.Append("      ,[point] = @point");
        //    strUpdateCommand.Append("      ,[use_point] = @use_point");
        //    strUpdateCommand.Append("      ,[coupon] = @coupon");
        //    strUpdateCommand.Append("      ,[note] = @note");
        //    strUpdateCommand.Append("      ,[status] = @status");
        //    strUpdateCommand.Append("      ,[create_date] = @create_date");
        //    strUpdateCommand.Append("      ,[create_time] = @create_time");
        //    strUpdateCommand.Append("      ,[create_user] = @create_user");
        //    strUpdateCommand.Append("      ,[update_date] = @update_date");
        //    strUpdateCommand.Append("      ,[update_time] = @update_time");
        //    strUpdateCommand.Append("      ,[update_user] = @update_user");
        //    strUpdateCommand.Append("      ,[del_flg] = @del_flg");
        //    strUpdateCommand.Append("      ,[common_no] = @common_no");//テスト的に共通番号列を作成

        //    return strUpdateCommand.ToString();
        //}
    }
}

                    
