using System.Data;
using System.Text;

namespace DataLinkage
{
    /// <summary>
    /// Customer用クラス
    /// </summary>
    sealed class EntCustomer : Entry
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EntCustomer()
        {

        }

        /// <summary>
        /// DB接続に関する情報を設定
        /// </summary>
        public override void SetDbAccessInfo() {

            this.SourceTable = "[dbo].[tmp_dtb_customer]";
            this.DestTable = "[dbo].[dtb_customer_test]";

            // SQL用のパラメータの設定
            this.SetDBParameters("common_no", DbType.Int32, true);

            this.SetDBParameters("customer_id", DbType.Int32);
            this.SetDBParameters("influx_source", DbType.String);
            this.SetDBParameters("name01", DbType.String);
            this.SetDBParameters("name02", DbType.String);
            this.SetDBParameters("kana01", DbType.String);
            this.SetDBParameters("kana02", DbType.String);
            this.SetDBParameters("zipcode", DbType.String);
            this.SetDBParameters("addr01", DbType.String);
            this.SetDBParameters("addr02", DbType.String);
            this.SetDBParameters("tel", DbType.String);
            this.SetDBParameters("email", DbType.String);
            this.SetDBParameters("note", DbType.String);
            this.SetDBParameters("pref", DbType.Int16);
            this.SetDBParameters("sex", DbType.Int16);
            this.SetDBParameters("job", DbType.Int16);
            this.SetDBParameters("birth", DbType.Int32);
            this.SetDBParameters("mailmaga_flg", DbType.Int16);
            this.SetDBParameters("first_buy_date", DbType.Int32);
            this.SetDBParameters("last_buy_date", DbType.Int32);
            this.SetDBParameters("point", DbType.Int32);
            this.SetDBParameters("use_point", DbType.Int32);
            this.SetDBParameters("coupon", DbType.Int32);
            this.SetDBParameters("status", DbType.Int16);
            this.SetDBParameters("create_date", DbType.Int32);
            this.SetDBParameters("create_time", DbType.Int32);
            this.SetDBParameters("create_user", DbType.Int32);
            this.SetDBParameters("update_date", DbType.Int32);
            this.SetDBParameters("update_time", DbType.Int32);
            this.SetDBParameters("update_user", DbType.Int32);
            this.SetDBParameters("buy_times", DbType.Int32);
            this.SetDBParameters("del_flg", DbType.Int16);
            this.SetDBParameters("buy_total", DbType.Int16);
        }


        /// <summary>
        /// 参照元Select文を返す
        /// </summary>
        /// <returns>参照元検索SQL</returns>
        public override string GetSourceSelectCommandText()
        {
            StringBuilder strSelectCommand = new StringBuilder();

            strSelectCommand.Append("SELECT [influx_source]");
            strSelectCommand.Append("      ,[customer_id]");
            strSelectCommand.Append("      ,[name01]");
            strSelectCommand.Append("      ,[name02]");
            strSelectCommand.Append("      ,[kana01]");
            strSelectCommand.Append("      ,[kana02]");
            strSelectCommand.Append("      ,[zipcode]");
            strSelectCommand.Append("      ,[pref]");
            strSelectCommand.Append("      ,[addr01]");
            strSelectCommand.Append("      ,[addr02]");
            strSelectCommand.Append("      ,[tel]");
            strSelectCommand.Append("      ,[sex]");
            strSelectCommand.Append("      ,[job]");
            strSelectCommand.Append("      ,[birth]");
            strSelectCommand.Append("      ,[email]");
            strSelectCommand.Append("      ,[mailmaga_flg]");
            strSelectCommand.Append("      ,[first_buy_date]");
            strSelectCommand.Append("      ,[last_buy_date]");
            strSelectCommand.Append("      ,[buy_times]");
            strSelectCommand.Append("      ,[buy_total]");
            strSelectCommand.Append("      ,[point]");
            strSelectCommand.Append("      ,[use_point]");
            strSelectCommand.Append("      ,[coupon]");
            strSelectCommand.Append("      ,[note]");
            strSelectCommand.Append("      ,[status]");
            strSelectCommand.Append("      ,[create_date]");
            strSelectCommand.Append("      ,[create_time]");
            strSelectCommand.Append("      ,[create_user]");
            strSelectCommand.Append("      ,[update_date]");
            strSelectCommand.Append("      ,[update_time]");
            strSelectCommand.Append("      ,[update_user]");
            strSelectCommand.Append("      ,[del_flg]");
            strSelectCommand.Append("      ,[common_no] ");
            strSelectCommand.Append("FROM ");
            strSelectCommand.Append(SourceTable);
            strSelectCommand.Append(" WITH (NOLOCK) ");

            return strSelectCommand.ToString();
        }

        /// <summary>
        /// 参照先Select文を返す
        /// </summary>
        /// <returns>参照先検索SQL</returns>
        public override string GetDestSelectCommandText()
        {
            StringBuilder strSelectCommand = new StringBuilder();
            strSelectCommand.Append("SELECT [influx_source]");
            strSelectCommand.Append("      ,[customer_id]");
            strSelectCommand.Append("      ,[name01]");
            strSelectCommand.Append("      ,[name02]");
            strSelectCommand.Append("      ,[kana01]");
            strSelectCommand.Append("      ,[kana02]");
            strSelectCommand.Append("      ,[zipcode]");
            strSelectCommand.Append("      ,[pref]");
            strSelectCommand.Append("      ,[addr01]");
            strSelectCommand.Append("      ,[addr02]");
            strSelectCommand.Append("      ,[tel]");
            strSelectCommand.Append("      ,[sex]");
            strSelectCommand.Append("      ,[job]");
            strSelectCommand.Append("      ,[birth]");
            strSelectCommand.Append("      ,[email]");
            strSelectCommand.Append("      ,[mailmaga_flg]");
            strSelectCommand.Append("      ,[first_buy_date]");
            strSelectCommand.Append("      ,[last_buy_date]");
            strSelectCommand.Append("      ,[buy_times]");
            strSelectCommand.Append("      ,[buy_total]");
            strSelectCommand.Append("      ,[point]");
            strSelectCommand.Append("      ,[use_point]");
            strSelectCommand.Append("      ,[coupon]");
            strSelectCommand.Append("      ,[note]");
            strSelectCommand.Append("      ,[status]");
            strSelectCommand.Append("      ,[create_date]");
            strSelectCommand.Append("      ,[create_time]");
            strSelectCommand.Append("      ,[create_user]");
            strSelectCommand.Append("      ,[update_date]");
            strSelectCommand.Append("      ,[update_time]");
            strSelectCommand.Append("      ,[update_user]");
            strSelectCommand.Append("      ,[del_flg]");
            strSelectCommand.Append("      ,[common_no] ");
            strSelectCommand.Append("FROM ");
            strSelectCommand.Append(DestTable);

            return strSelectCommand.ToString();
        }



        /// <summary>
        /// InsertSQL（commandText）を返す
        /// </summary>
        /// <returns>参照先挿入SQL</returns>
        public override string GetInsertCommandText()
        {
            StringBuilder strInsertCommand = new StringBuilder();

            strInsertCommand.Append("INSERT INTO ");
            strInsertCommand.Append(DestTable);
            strInsertCommand.Append("           ([influx_source]");
            //strInsertCommand.Append("           ,[customer_id]");
            strInsertCommand.Append("           ,[name01]");
            strInsertCommand.Append("           ,[name02]");
            strInsertCommand.Append("           ,[kana01]");
            strInsertCommand.Append("           ,[kana02]");
            strInsertCommand.Append("           ,[zipcode]");
            strInsertCommand.Append("           ,[pref]");
            strInsertCommand.Append("           ,[addr01]");
            strInsertCommand.Append("           ,[addr02]");
            strInsertCommand.Append("           ,[tel]");
            strInsertCommand.Append("           ,[sex]");
            strInsertCommand.Append("           ,[job]");
            strInsertCommand.Append("           ,[birth]");
            strInsertCommand.Append("           ,[email]");
            strInsertCommand.Append("           ,[mailmaga_flg]");
            strInsertCommand.Append("           ,[first_buy_date]");
            strInsertCommand.Append("           ,[last_buy_date]");
            strInsertCommand.Append("           ,[buy_times]");
            strInsertCommand.Append("           ,[buy_total]");
            strInsertCommand.Append("           ,[point]");
            strInsertCommand.Append("           ,[use_point]");
            strInsertCommand.Append("           ,[coupon]");
            strInsertCommand.Append("           ,[note]");
            strInsertCommand.Append("           ,[status]");
            strInsertCommand.Append("           ,[create_date]");
            strInsertCommand.Append("           ,[create_time]");
            strInsertCommand.Append("           ,[create_user]");
            strInsertCommand.Append("           ,[update_date]");
            strInsertCommand.Append("           ,[update_time]");
            strInsertCommand.Append("           ,[update_user]");
            strInsertCommand.Append("           ,[del_flg]");
            strInsertCommand.Append("           ,[common_no])"); //テスト的に共通番号列を作成
            strInsertCommand.Append("     VALUES");
            strInsertCommand.Append("           (@influx_source");
            //strInsertCommand.Append("           ,@customer_id");
            strInsertCommand.Append("           ,@name01");
            strInsertCommand.Append("           ,@name02");
            strInsertCommand.Append("           ,@kana01");
            strInsertCommand.Append("           ,@kana02");
            strInsertCommand.Append("           ,@zipcode");
            strInsertCommand.Append("           ,@pref");
            strInsertCommand.Append("           ,@addr01");
            strInsertCommand.Append("           ,@addr02");
            strInsertCommand.Append("           ,@tel");
            strInsertCommand.Append("           ,@sex");
            strInsertCommand.Append("           ,@job");
            strInsertCommand.Append("           ,@birth");
            strInsertCommand.Append("           ,@email");
            strInsertCommand.Append("           ,@mailmaga_flg");
            strInsertCommand.Append("           ,@first_buy_date");
            strInsertCommand.Append("           ,@last_buy_date");
            strInsertCommand.Append("           ,@buy_times");
            strInsertCommand.Append("           ,@buy_total");
            strInsertCommand.Append("           ,@point");
            strInsertCommand.Append("           ,@use_point");
            strInsertCommand.Append("           ,@coupon");
            strInsertCommand.Append("           ,@note");
            strInsertCommand.Append("           ,@status");
            strInsertCommand.Append("           ,@create_date");
            strInsertCommand.Append("           ,@create_time");
            strInsertCommand.Append("           ,@create_user");
            strInsertCommand.Append("           ,@update_date");
            strInsertCommand.Append("           ,@update_time");
            strInsertCommand.Append("           ,@update_user");
            strInsertCommand.Append("           ,@del_flg");
            strInsertCommand.Append("           ,@common_no)");//テスト的に共通番号列を作成
            return strInsertCommand.ToString();
        }

        /// <summary>
        /// UpdateSQL（commandText）を返す
        /// </summary>
        /// <returns>参照先更新SQL</returns>
        public override string GetUpdateCommandText()
        {
            StringBuilder strUpdateCommand = new StringBuilder();

            strUpdateCommand.Append("UPDATE ");
            strUpdateCommand.Append(DestTable);
            strUpdateCommand.Append("   SET [influx_source] = @influx_source");
            strUpdateCommand.Append("      ,[name01] = @name01");
            strUpdateCommand.Append("      ,[name02] = @name02");
            strUpdateCommand.Append("      ,[kana01] = @kana01");
            strUpdateCommand.Append("      ,[kana02] = @kana02");
            strUpdateCommand.Append("      ,[zipcode] = @zipcode");
            strUpdateCommand.Append("      ,[pref] = @pref");
            strUpdateCommand.Append("      ,[addr01] = @addr01");
            strUpdateCommand.Append("      ,[addr02] = @addr02");
            strUpdateCommand.Append("      ,[tel] = @tel");
            strUpdateCommand.Append("      ,[sex] = @sex");
            strUpdateCommand.Append("      ,[job] = @job");
            strUpdateCommand.Append("      ,[birth] = @birth");
            strUpdateCommand.Append("      ,[email] = @email");
            strUpdateCommand.Append("      ,[mailmaga_flg] = @mailmaga_flg");
            strUpdateCommand.Append("      ,[first_buy_date] = @first_buy_date");
            strUpdateCommand.Append("      ,[last_buy_date] = @last_buy_date");
            strUpdateCommand.Append("      ,[buy_times] = @buy_times");
            strUpdateCommand.Append("      ,[buy_total] = @buy_total");
            strUpdateCommand.Append("      ,[point] = @point");
            strUpdateCommand.Append("      ,[use_point] = @use_point");
            strUpdateCommand.Append("      ,[coupon] = @coupon");
            strUpdateCommand.Append("      ,[note] = @note");
            strUpdateCommand.Append("      ,[status] = @status");
            strUpdateCommand.Append("      ,[create_date] = @create_date");
            strUpdateCommand.Append("      ,[create_time] = @create_time");
            strUpdateCommand.Append("      ,[create_user] = @create_user");
            strUpdateCommand.Append("      ,[update_date] = @update_date");
            strUpdateCommand.Append("      ,[update_time] = @update_time");
            strUpdateCommand.Append("      ,[update_user] = @update_user");
            strUpdateCommand.Append("      ,[del_flg] = @del_flg");
            strUpdateCommand.Append("      ,[common_no] = @common_no");//テスト的に共通番号列を作成

            return strUpdateCommand.ToString();
        }
    }
}
