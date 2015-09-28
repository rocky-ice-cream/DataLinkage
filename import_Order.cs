using DataLinkage;
using Microsoft.SqlServer.Server;

public partial class StoredProcedures
{
    /// <summary>
    /// 受注ストアドプロシージャ
    /// </summary>
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void import_Order ()
    {
        // コードをここに記述してください
        Entry entry = new EntOrder();

        // DB接続情報の設定
        entry.SetDbAccessInfo();

        // 処理用クラスの実行
        new BizWorker(entry).Worker();
    }
}
