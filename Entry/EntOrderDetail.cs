using System.Data;
using System.Text;

namespace DataLinkage
{
    /// <summary>
    /// 受注詳細情報クラス
    /// </summary>
    sealed class EntOrderDetail : Entry
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EntOrderDetail()
        {

        }

        /// <summary>
        /// DB接続に関する情報を設定
        /// </summary>
        public override void SetDbAccessInfo() {

            this.SourceTable = "[dbo].[tmp_dtb_order_detail]";
            this.DestTable = "[dbo].[dtb_order_detail]";

            // SQL用のパラメータの設定
            this.SetDBParameters("influx_source", DbType.String, true);
            this.SetDBParameters("order_id", DbType.Int32, true);
            this.SetDBParameters("order_detail_id", DbType.Int32, true);
            this.SetDBParameters("product_id", DbType.Int32);
            this.SetDBParameters("product_name", DbType.String);
            this.SetDBParameters("price", DbType.Decimal);
            this.SetDBParameters("quantity", DbType.Decimal);
            this.SetDBParameters("point_rate", DbType.Decimal);
            this.SetDBParameters("tax_rate", DbType.Decimal);
        }

        /// <summary>
        /// 参照元Select文を返す
        /// </summary>
        /// <returns>参照元検索SQL</returns>
        public override string GetSourceSelectCommandText()
        {
            StringBuilder strSelectCommand = new StringBuilder();

            strSelectCommand.Append("SELECT [influx_source]");
            strSelectCommand.Append("      ,[order_id]");
            strSelectCommand.Append("      ,[order_detail_id]");
            strSelectCommand.Append("      ,[product_id]");
            strSelectCommand.Append("      ,[product_name]");
            strSelectCommand.Append("      ,[price]");
            strSelectCommand.Append("      ,[quantity]");
            strSelectCommand.Append("      ,[point_rate]");
            strSelectCommand.Append("      ,[tax_rate]");
            strSelectCommand.Append("FROM ");
            strSelectCommand.Append(SourceTable);
            strSelectCommand.Append(" WITH (NOLOCK) ");

            return strSelectCommand.ToString();
        }

        /// <summary>
        /// 参照先Select文を返す
        /// </summary>
        /// <returns>参照先SQL</returns>
        public override string GetDestSelectCommandText()
        {
            StringBuilder strSelectCommand = new StringBuilder();
            strSelectCommand.Append("SELECT [influx_source]");
            strSelectCommand.Append("      ,[order_id]");
            strSelectCommand.Append("      ,[order_detail_id]");
            strSelectCommand.Append("      ,[product_id]");
            strSelectCommand.Append("      ,[product_name]");
            strSelectCommand.Append("      ,[price]");
            strSelectCommand.Append("      ,[quantity]");
            strSelectCommand.Append("      ,[point_rate]");
            strSelectCommand.Append("      ,[tax_rate]");
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
            strInsertCommand.Append("           ,[order_id]");
            strInsertCommand.Append("           ,[order_detail_id]");
            strInsertCommand.Append("           ,[product_id]");
            strInsertCommand.Append("           ,[product_name]");
            strInsertCommand.Append("           ,[price]");
            strInsertCommand.Append("           ,[quantity]");
            strInsertCommand.Append("           ,[point_rate]");
            strInsertCommand.Append("           ,[tax_rate])");
            strInsertCommand.Append("     VALUES");
            strInsertCommand.Append("           (@influx_source");
            strInsertCommand.Append("           ,@order_id");
            strInsertCommand.Append("           ,@order_detail_id");
            strInsertCommand.Append("           ,@product_id");
            strInsertCommand.Append("           ,@product_name");
            strInsertCommand.Append("           ,@price");
            strInsertCommand.Append("           ,@quantity");
            strInsertCommand.Append("           ,@point_rate");
            strInsertCommand.Append("           ,@tax_rate)");
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
            strUpdateCommand.Append("      ,[order_id] = @order_id");
            strUpdateCommand.Append("      ,[order_detail_id] = @order_detail_id");
            strUpdateCommand.Append("      ,[product_id] = @product_id");
            strUpdateCommand.Append("      ,[product_name] = @product_name");
            strUpdateCommand.Append("      ,[price] = @price");
            strUpdateCommand.Append("      ,[quantity] = @quantity");
            strUpdateCommand.Append("      ,[point_rate] = @point_rate");
            strUpdateCommand.Append("      ,[tax_rate] = @tax_rate");

            return strUpdateCommand.ToString();
        }
    }
}
