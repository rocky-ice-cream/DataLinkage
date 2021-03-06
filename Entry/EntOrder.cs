using System.Data;
using System.Text;

namespace DataLinkage
{
    /// <summary>
    /// 受注情報クラス
    /// </summary>
    sealed class EntOrder : Entry
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EntOrder()
        {

        }

        /// <summary>
        /// DB接続に関する情報を設定
        /// </summary>
        public override void SetDbAccessInfo() {

            this.SourceTable = "[dbo].[tmp_dtb_order]";
            this.DestTable = "[dbo].[dtb_order]";

            // SQL用のパラメータの設定
            //this.SetDBParameters("influx_source", DbType.String);
            this.SetDBParameters("order_id", DbType.Int32);
            this.SetDBParameters("customer_id", DbType.Int32);
            this.SetDBParameters("ec_order_id", DbType.Int32, true);
            this.SetDBParameters("ec_customer_id", DbType.Int32);
            this.SetDBParameters("gmo_id", DbType.Int32);
            this.SetDBParameters("order_date", DbType.Int32);
            this.SetDBParameters("broadcast_schedule_id", DbType.Int32);
            this.SetDBParameters("order_class", DbType.Int16);
            this.SetDBParameters("destination", DbType.String);
            this.SetDBParameters("order_name01", DbType.String);
            this.SetDBParameters("order_name02", DbType.String);
            this.SetDBParameters("order_kana01", DbType.String);
            this.SetDBParameters("order_kana02", DbType.String);
            this.SetDBParameters("order_tel", DbType.String);
            this.SetDBParameters("order_fax", DbType.String);
            this.SetDBParameters("order_zipcode", DbType.String);
            this.SetDBParameters("order_pref", DbType.Int16);
            this.SetDBParameters("order_addr01", DbType.String);
            this.SetDBParameters("order_addr02", DbType.String);
            this.SetDBParameters("subtotal", DbType.Int32);
            this.SetDBParameters("discount", DbType.Decimal);
            this.SetDBParameters("deliv_id", DbType.Int32);
            this.SetDBParameters("deliv_date", DbType.Int32);
            this.SetDBParameters("deliv_tzone", DbType.Int16);
            this.SetDBParameters("deliv_fee", DbType.Int32);
            this.SetDBParameters("charge", DbType.Int32);
            this.SetDBParameters("use_point", DbType.Int32);
            this.SetDBParameters("add_point", DbType.Int32);
            this.SetDBParameters("use_coupon", DbType.Int32);
            this.SetDBParameters("add_coupon_id", DbType.Int32);
            this.SetDBParameters("birth_point", DbType.Int32);
            this.SetDBParameters("tax", DbType.Int32);
            this.SetDBParameters("total", DbType.Int32);
            this.SetDBParameters("payment_total", DbType.Int32);
            this.SetDBParameters("payment_id", DbType.Int32);
            this.SetDBParameters("payment_method", DbType.String);
            this.SetDBParameters("note", DbType.String);
            this.SetDBParameters("status", DbType.Int16);
            this.SetDBParameters("confirmation_date", DbType.Int32);
            this.SetDBParameters("ship_pre_flg", DbType.Int16);
            this.SetDBParameters("invoice_flg", DbType.Int16);
            this.SetDBParameters("pickinglist_flg", DbType.Int16);
            this.SetDBParameters("deliveries_flg", DbType.Int16);
            this.SetDBParameters("commit_date", DbType.Int32);
            this.SetDBParameters("invoice_id", DbType.String);
            this.SetDBParameters("original_order_id", DbType.Int32);
            this.SetDBParameters("del_flg", DbType.Int16);
            this.SetDBParameters("create_date", DbType.Int32);
            this.SetDBParameters("create_time", DbType.Int32);
            this.SetDBParameters("create_user", DbType.Int32);
            this.SetDBParameters("update_date", DbType.Int32);
            this.SetDBParameters("update_time", DbType.Int32);
            this.SetDBParameters("update_user", DbType.Int32);
        }


        /// <summary>
        /// 参照元Select文を返す
        /// </summary>
        /// <returns>参照元検索SQL</returns>
        public override string GetSourceSelectCommandText()
        {
            StringBuilder strSelectCommand = new StringBuilder();

            strSelectCommand.Append("SELECT");
            strSelectCommand.Append("      [order_id] AS [ec_order_id]");
            strSelectCommand.Append("      ,[customer_id] AS [customer_id]");
            strSelectCommand.Append("      ,[customer_id] AS [ec_customer_id]");
            strSelectCommand.Append("      ,ISNULL(FORMAT([create_date], 'yyyyMMdd'),0) AS [order_date]");
            strSelectCommand.Append("      ,0 AS [broadcast_schedule_id]");
            strSelectCommand.Append("      ,0 AS [gmo_id]");
            strSelectCommand.Append("      ,0 AS [order_class]");
            strSelectCommand.Append("      ,[order_name01] AS [order_name01]");
            strSelectCommand.Append("      ,[order_name02] AS [order_name02]");
            strSelectCommand.Append("      ,[order_kana01] AS [order_kana01]");
            strSelectCommand.Append("      ,[order_kana02] AS [order_kana02]");
            strSelectCommand.Append("      ,''  AS [destination]");
            strSelectCommand.Append("      ,(");
            strSelectCommand.Append("        CASE ");
            strSelectCommand.Append("          WHEN ([order_tel01] IS NOT NULL AND ");
            strSelectCommand.Append("              [order_tel02] IS NOT NULL AND ");
            strSelectCommand.Append("              [order_tel03] IS NOT NULL) ");
            strSelectCommand.Append("          THEN");
            strSelectCommand.Append("            CONCAT([order_tel01],'-',[order_tel02],'-',[order_tel03])");
            strSelectCommand.Append("        ELSE");
            strSelectCommand.Append("            ''");
            strSelectCommand.Append("        END ");
            strSelectCommand.Append("        ) AS [order_tel]");
            strSelectCommand.Append("          ,(");
            strSelectCommand.Append("        CASE ");
            strSelectCommand.Append("          WHEN ([order_fax01] IS NOT NULL AND ");
            strSelectCommand.Append("              [order_fax02] IS NOT NULL AND ");
            strSelectCommand.Append("              [order_fax03] IS NOT NULL) ");
            strSelectCommand.Append("          THEN");
            strSelectCommand.Append("            CONCAT([order_fax01],'-',[order_fax02],'-',[order_fax03])");
            strSelectCommand.Append("        ELSE");
            strSelectCommand.Append("            ''");
            strSelectCommand.Append("        END ");
            strSelectCommand.Append("        ) AS [order_fax]");
            strSelectCommand.Append("      ,CONCAT([order_zip01],[order_zip02]) AS [order_zipcode]");
            strSelectCommand.Append("      ,[order_pref] AS [order_pref]");
            strSelectCommand.Append("      ,[order_addr01] AS [order_addr01]");
            strSelectCommand.Append("      ,[order_addr02] AS [order_addr02]");
            strSelectCommand.Append("      ,[subtotal] AS [subtotal]");
            strSelectCommand.Append("      ,[discount] AS [discount]");
            strSelectCommand.Append("      ,[deliv_id] AS [deliv_id]");
            strSelectCommand.Append("      ,0 AS [deliv_date]");
            strSelectCommand.Append("      ,0 AS [deliv_tzone]");
            strSelectCommand.Append("      ,[deliv_fee] AS [deliv_fee]");
            strSelectCommand.Append("      ,[charge] AS [charge]");
            strSelectCommand.Append("      ,[use_point] AS [use_point]");
            strSelectCommand.Append("      ,[add_point] AS [add_point]");
            strSelectCommand.Append("      ,0 AS [use_coupon]");
            strSelectCommand.Append("      ,0 AS [add_coupon_id]");
            strSelectCommand.Append("      ,[birth_point] AS [birth_point]");
            strSelectCommand.Append("      ,[tax] AS [tax]");
            strSelectCommand.Append("      ,[total] AS [total]");
            strSelectCommand.Append("      ,[payment_total] AS [payment_total]");
            strSelectCommand.Append("      ,[payment_id] AS [payment_id]");
            strSelectCommand.Append("      ,[payment_method] AS [payment_method]");
            strSelectCommand.Append("      ,ISNULL([note],'') AS [note]");
            strSelectCommand.Append("      ,[status] AS [status]");
            strSelectCommand.Append("      ,0 AS [confirmation_date]");
            strSelectCommand.Append("      ,0 AS [ship_pre_flg]");
            strSelectCommand.Append("      ,0 AS [invoice_flg]");
            strSelectCommand.Append("      ,0 AS [pickinglist_flg]");
            strSelectCommand.Append("      ,0 AS [deliveries_flg]");
            strSelectCommand.Append("      ,0 AS [commit_date]");
            strSelectCommand.Append("      ,'' AS [invoice_id]");
            strSelectCommand.Append("      ,0 AS [original_order_id]");
            strSelectCommand.Append("      ,[del_flg] AS [del_flg]");
            strSelectCommand.Append("      ,ISNULL(FORMAT([create_date], 'yyyyMMdd'),0) AS [create_date]");
            strSelectCommand.Append("      ,ISNULL(FORMAT([create_date], 'hhmmss'),0) AS [create_time]");
            strSelectCommand.Append("      ,'99999' AS [create_user]");
            strSelectCommand.Append("      ,ISNULL(FORMAT([update_date], 'yyyyMMdd'),0) AS [update_date]");
            strSelectCommand.Append("      ,ISNULL(FORMAT([update_date], 'hhmmss'),0) AS [update_time]");
            strSelectCommand.Append("      ,'99999' AS [update_user]");
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
            strSelectCommand.Append("SELECT ");
            strSelectCommand.Append("        [order_id]");
            strSelectCommand.Append("       ,[customer_id]");
            strSelectCommand.Append("       ,[ec_order_id]");
            strSelectCommand.Append("       ,[ec_customer_id]");
            strSelectCommand.Append("       ,[order_date]");
            strSelectCommand.Append("       ,[broadcast_schedule_id]");
            strSelectCommand.Append("       ,[order_class]");
            strSelectCommand.Append("       ,[order_name01]");
            strSelectCommand.Append("       ,[order_name02]");
            strSelectCommand.Append("       ,[order_kana01]");
            strSelectCommand.Append("       ,[order_kana02]");
            strSelectCommand.Append("       ,[order_tel]");
            strSelectCommand.Append("       ,[order_fax]");
            strSelectCommand.Append("       ,[order_zipcode]");
            strSelectCommand.Append("       ,[order_pref]");
            strSelectCommand.Append("       ,[order_addr01]");
            strSelectCommand.Append("       ,[order_addr02]");
            strSelectCommand.Append("       ,[subtotal]");
            strSelectCommand.Append("       ,[discount]");
            strSelectCommand.Append("       ,[deliv_id]");
            strSelectCommand.Append("       ,[deliv_date]");
            strSelectCommand.Append("       ,[deliv_tzone]");
            strSelectCommand.Append("       ,[deliv_fee]");
            strSelectCommand.Append("       ,[charge]");
            strSelectCommand.Append("       ,[use_point]");
            strSelectCommand.Append("       ,[add_point]");
            strSelectCommand.Append("       ,[use_coupon]");
            strSelectCommand.Append("       ,[add_coupon_id]");
            strSelectCommand.Append("       ,[birth_point]");
            strSelectCommand.Append("       ,[tax]");
            strSelectCommand.Append("       ,[total]");
            strSelectCommand.Append("       ,[payment_total]");
            strSelectCommand.Append("       ,[payment_id]");
            strSelectCommand.Append("       ,[payment_method]");
            strSelectCommand.Append("       ,[note]");
            strSelectCommand.Append("       ,[status]");
            strSelectCommand.Append("       ,[confirmation_date]");
            strSelectCommand.Append("       ,[ship_pre_flg]");
            strSelectCommand.Append("       ,[invoice_flg]");
            strSelectCommand.Append("       ,[pickinglist_flg]");
            strSelectCommand.Append("       ,[deliveries_flg]");
            strSelectCommand.Append("       ,[commit_date]");
            strSelectCommand.Append("       ,[invoice_id]");
            strSelectCommand.Append("       ,[original_order_id]");
            strSelectCommand.Append("       ,[del_flg]");
            strSelectCommand.Append("       ,[create_date]");
            strSelectCommand.Append("       ,[create_time]");
            strSelectCommand.Append("       ,[create_user]");
            strSelectCommand.Append("       ,[update_date]");
            strSelectCommand.Append("       ,[update_time]");
            strSelectCommand.Append("       ,[update_user]");
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
            strInsertCommand.Append("           ([customer_id]");
            strInsertCommand.Append("           ,[ec_order_id]");
            strInsertCommand.Append("           ,[ec_customer_id]");
            strInsertCommand.Append("           ,[order_date]");
            strInsertCommand.Append("           ,[broadcast_schedule_id]");
            strInsertCommand.Append("           ,[order_class]");
            strInsertCommand.Append("           ,[order_name01]");
            strInsertCommand.Append("           ,[order_name02]");
            strInsertCommand.Append("           ,[order_kana01]");
            strInsertCommand.Append("           ,[order_kana02]");
            strInsertCommand.Append("           ,[order_tel]");
            strInsertCommand.Append("           ,[order_fax]");
            strInsertCommand.Append("           ,[order_zipcode]");
            strInsertCommand.Append("           ,[order_pref]");
            strInsertCommand.Append("           ,[order_addr01]");
            strInsertCommand.Append("           ,[order_addr02]");
            strInsertCommand.Append("           ,[subtotal]");
            strInsertCommand.Append("           ,[discount]");
            strInsertCommand.Append("           ,[deliv_id]");
            strInsertCommand.Append("           ,[deliv_date]");
            strInsertCommand.Append("           ,[deliv_tzone]");
            strInsertCommand.Append("           ,[deliv_fee]");
            strInsertCommand.Append("           ,[charge]");
            strInsertCommand.Append("           ,[use_point]");
            strInsertCommand.Append("           ,[add_point]");
            strInsertCommand.Append("           ,[use_coupon]");
            strInsertCommand.Append("           ,[add_coupon_id]");
            strInsertCommand.Append("           ,[birth_point]");
            strInsertCommand.Append("           ,[tax]");
            strInsertCommand.Append("           ,[total]");
            strInsertCommand.Append("           ,[payment_total]");
            strInsertCommand.Append("           ,[payment_id]");
            strInsertCommand.Append("           ,[payment_method]");
            strInsertCommand.Append("           ,[note]");
            strInsertCommand.Append("           ,[status]");
            strInsertCommand.Append("           ,[confirmation_date]");
            strInsertCommand.Append("           ,[ship_pre_flg]");
            strInsertCommand.Append("           ,[invoice_flg]");
            strInsertCommand.Append("           ,[pickinglist_flg]");
            strInsertCommand.Append("           ,[deliveries_flg]");
            strInsertCommand.Append("           ,[commit_date]");
            strInsertCommand.Append("           ,[invoice_id]");
            strInsertCommand.Append("           ,[original_order_id]");
            strInsertCommand.Append("           ,[del_flg]");
            strInsertCommand.Append("           ,[create_date]");
            strInsertCommand.Append("           ,[create_time]");
            strInsertCommand.Append("           ,[create_user]");
            strInsertCommand.Append("           ,[update_date]");
            strInsertCommand.Append("           ,[update_time]");
            strInsertCommand.Append("           ,[update_user])");
            strInsertCommand.Append("     VALUES");
            strInsertCommand.Append("           (@customer_id");
            strInsertCommand.Append("           ,@ec_order_id");
            strInsertCommand.Append("           ,@ec_customer_id");
            strInsertCommand.Append("           ,@order_date");
            strInsertCommand.Append("           ,@broadcast_schedule_id");
            strInsertCommand.Append("           ,@order_class");
            strInsertCommand.Append("           ,@order_name01");
            strInsertCommand.Append("           ,@order_name02");
            strInsertCommand.Append("           ,@order_kana01");
            strInsertCommand.Append("           ,@order_kana02");
            strInsertCommand.Append("           ,@order_tel");
            strInsertCommand.Append("           ,@order_fax");
            strInsertCommand.Append("           ,@order_zipcode");
            strInsertCommand.Append("           ,@order_pref");
            strInsertCommand.Append("           ,@order_addr01");
            strInsertCommand.Append("           ,@order_addr02");
            strInsertCommand.Append("           ,@subtotal");
            strInsertCommand.Append("           ,@discount");
            strInsertCommand.Append("           ,@deliv_id");
            strInsertCommand.Append("           ,@deliv_date");
            strInsertCommand.Append("           ,@deliv_tzone");
            strInsertCommand.Append("           ,@deliv_fee");
            strInsertCommand.Append("           ,@charge");
            strInsertCommand.Append("           ,@use_point");
            strInsertCommand.Append("           ,@add_point");
            strInsertCommand.Append("           ,@use_coupon");
            strInsertCommand.Append("           ,@add_coupon_id");
            strInsertCommand.Append("           ,@birth_point");
            strInsertCommand.Append("           ,@tax");
            strInsertCommand.Append("           ,@total");
            strInsertCommand.Append("           ,@payment_total");
            strInsertCommand.Append("           ,@payment_id");
            strInsertCommand.Append("           ,@payment_method");
            strInsertCommand.Append("           ,@note");
            strInsertCommand.Append("           ,@status");
            strInsertCommand.Append("           ,@confirmation_date");
            strInsertCommand.Append("           ,@ship_pre_flg");
            strInsertCommand.Append("           ,@invoice_flg");
            strInsertCommand.Append("           ,@pickinglist_flg");
            strInsertCommand.Append("           ,@deliveries_flg");
            strInsertCommand.Append("           ,@commit_date");
            strInsertCommand.Append("           ,@invoice_id");
            strInsertCommand.Append("           ,@original_order_id");
            strInsertCommand.Append("           ,@del_flg");
            strInsertCommand.Append("           ,@create_date");
            strInsertCommand.Append("           ,@create_time");
            strInsertCommand.Append("           ,@create_user");
            strInsertCommand.Append("           ,@update_date");
            strInsertCommand.Append("           ,@update_time");
            strInsertCommand.Append("           ,@update_user)");
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
            strUpdateCommand.Append("   SET ");
            strUpdateCommand.Append("      [customer_id] = @customer_id");
            strUpdateCommand.Append("      ,[ec_order_id] = @ec_order_id");
            strUpdateCommand.Append("      ,[ec_customer_id] = @ec_customer_id");
            strUpdateCommand.Append("      ,[order_date] = @order_date");
            strUpdateCommand.Append("      ,[broadcast_schedule_id] = @broadcast_schedule_id");
            strUpdateCommand.Append("      ,[order_class] = @order_class");
            strUpdateCommand.Append("      ,[order_name01] = @order_name01");
            strUpdateCommand.Append("      ,[order_name02] = @order_name02");
            strUpdateCommand.Append("      ,[order_kana01] = @order_kana01");
            strUpdateCommand.Append("      ,[order_kana02] = @order_kana02");
            strUpdateCommand.Append("      ,[order_tel] = @order_tel");
            strUpdateCommand.Append("      ,[order_fax] = @order_fax");
            strUpdateCommand.Append("      ,[order_zipcode] = @order_zipcode");
            strUpdateCommand.Append("      ,[order_pref] = @order_pref");
            strUpdateCommand.Append("      ,[order_addr01] = @order_addr01");
            strUpdateCommand.Append("      ,[order_addr02] = @order_addr02");
            strUpdateCommand.Append("      ,[subtotal] = @subtotal");
            strUpdateCommand.Append("      ,[discount] = @discount");
            strUpdateCommand.Append("      ,[deliv_id] = @deliv_id");
            strUpdateCommand.Append("      ,[deliv_date] = @deliv_date");
            strUpdateCommand.Append("      ,[deliv_tzone] = @deliv_tzone");
            strUpdateCommand.Append("      ,[deliv_fee] = @deliv_fee");
            strUpdateCommand.Append("      ,[charge] = @charge");
            strUpdateCommand.Append("      ,[use_point] = @use_point");
            strUpdateCommand.Append("      ,[add_point] = @add_point");
            strUpdateCommand.Append("      ,[use_coupon] = @use_coupon");
            strUpdateCommand.Append("      ,[add_coupon_id] = @add_coupon_id");
            strUpdateCommand.Append("      ,[birth_point] = @birth_point");
            strUpdateCommand.Append("      ,[tax] = @tax");
            strUpdateCommand.Append("      ,[total] = @total");
            strUpdateCommand.Append("      ,[payment_total] = @payment_total");
            strUpdateCommand.Append("      ,[payment_id] = @payment_id");
            strUpdateCommand.Append("      ,[payment_method] = @payment_method");
            strUpdateCommand.Append("      ,[note] = @note");
            strUpdateCommand.Append("      ,[status] = @status");
            strUpdateCommand.Append("      ,[confirmation_date] = @confirmation_date");
            strUpdateCommand.Append("      ,[ship_pre_flg] = @ship_pre_flg");
            strUpdateCommand.Append("      ,[invoice_flg] = @invoice_flg");
            strUpdateCommand.Append("      ,[pickinglist_flg] = @pickinglist_flg");
            strUpdateCommand.Append("      ,[deliveries_flg] = @deliveries_flg");
            strUpdateCommand.Append("      ,[commit_date] = @commit_date");
            strUpdateCommand.Append("      ,[invoice_id] = @invoice_id");
            strUpdateCommand.Append("      ,[original_order_id] = @original_order_id");
            strUpdateCommand.Append("      ,[del_flg] = @del_flg");
            strUpdateCommand.Append("      ,[create_date] = @create_date");
            strUpdateCommand.Append("      ,[create_time] = @create_time");
            strUpdateCommand.Append("      ,[create_user] = @create_user");
            strUpdateCommand.Append("      ,[update_date] = @update_date");
            strUpdateCommand.Append("      ,[update_time] = @update_time");
            strUpdateCommand.Append("      ,[update_user] = @update_user");
            strUpdateCommand.Append("  WHERE [ec_order_id] = @ec_order_id");            

            return strUpdateCommand.ToString();
        }
    }
}
