using DataLinkage;
using Microsoft.SqlServer.Server;

public partial class StoredProcedures
{
    /// <summary>
    /// 受注詳細ストアドプロシージャ
    /// </summary>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void import_OrderDetail ()
    {
        // コードをここに記述してください
        Entry entry = new EntOrderDetail();

        // DB接続情報の設定
        entry.SetDbAccessInfo();

        // 処理用クラスの実行
        new BizWorker(entry).Worker();
    }
}
