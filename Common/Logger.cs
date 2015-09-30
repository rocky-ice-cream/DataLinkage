using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;

namespace DataLinkage
{
    /// <summary>
    /// ログ出力クラス
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Logger()
        {
        }


        /// <summary>
        /// ログ出力関数
        /// </summary>
        /// <param name="_text">ログ出力テキスト</param>
        /// <param name="_methodname">メソッド名</param>
        /// <param name="_classname">クラス名</param>
        /// <param name="_assemblyname">アセンブリ名</param>
        /// <param name="_err">エラーコード(デフォルト 0)</param>
        /// <returns></returns>
        public int LogWrite(
            string _text, 
            string _methodname, 
            string _classname, 
            string _assemblyname, 
            Int32 _err = 0)
        {
            // ログ用のコネクション接続
            using (SqlConnection logconn =
                new SqlConnection(Common.CONNECTION_STRING))
            {
                logconn.Open();

                //コマンドの作成
                using (SqlCommand logCommand
                    = new SqlCommand())
                {
                    logCommand.Connection = logconn;
                    logCommand.CommandText = GetInsertCommand();

                    logCommand.Parameters.AddWithValue("@log_text", _text.ToString());
                    logCommand.Parameters.AddWithValue("@error_code", _err);
                    logCommand.Parameters.AddWithValue("@method_name", _methodname.ToString());
                    logCommand.Parameters.AddWithValue("@class_name", _classname.ToString());
                    logCommand.Parameters.AddWithValue("@program_name", _assemblyname.ToString());
                    logCommand.Parameters.AddWithValue("@log_create_time",DateTime.Now);

                    return logCommand.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// ログ登録用SQLを返す
        /// </summary>
        /// <returns></returns>
        private string GetInsertCommand() {
            StringBuilder strInsertCommand = new StringBuilder();
            strInsertCommand.Append("INSERT INTO [dbo].[ctb_log]");
            strInsertCommand.Append("           ([log_text]");
            strInsertCommand.Append("           ,[error_code]");
            strInsertCommand.Append("           ,[method_name]");
            strInsertCommand.Append("           ,[class_name]");
            strInsertCommand.Append("           ,[program_name]");
            strInsertCommand.Append("           ,[log_create_time])");
            strInsertCommand.Append("     VALUES");
            strInsertCommand.Append("           (@log_text");
            strInsertCommand.Append("           ,@error_code");
            strInsertCommand.Append("           ,@method_name");
            strInsertCommand.Append("           ,@class_name");
            strInsertCommand.Append("           ,@program_name");
            strInsertCommand.Append("           ,@log_create_time)");

            return strInsertCommand.ToString();
       
        }
    }
}
