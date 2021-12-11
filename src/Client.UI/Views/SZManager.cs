using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// ZIPファイル操作クラス(パスワード付きZIP対応)
/// </summary>
public static class SZManager
{
    /// <summary>
    /// ZIP解凍[パスワード指定]
    /// </summary>
    /// <param name="zipFilePath">7zファイル名</param>
    /// <param name="sDestFolder">出力先フォルダ</param>
    /// <param name="sPassword">パスワード(半角英数字)</param>
    public static void ExtractWithPassword(string zipFilePath, string sDestFolder, string sPassword)
    {
        lock (typeof(SZManager))
        {
            StringBuilder sbOutput = new StringBuilder(1024);

            //---------------------------------------------------------------------------------
            // sDestFolderの最後が\の場合、\を取り払う
            //---------------------------------------------------------------------------------
            while (sDestFolder[sDestFolder.Length - 1] == '\\')
            {
                sDestFolder = sDestFolder.Substring(0, sDestFolder.Length - 1);
            }

            //---------------------------------------------------------------------------------
            // コマンドライン文字列の作成
            //---------------------------------------------------------------------------------
            // x:解凍
            // -tzip:種別ZIPを指定
            // -aoa:確認なしで上書き
            // -hide:処理状況ダイアログ表示の抑止
            // -y:全ての質問に yes を仮定
            // -r:サブディレクトリの再帰的検索
            // -o{dir_path}:出力先ディレクトリの設定
            // -p{password}:パスワードの設定
            string sCmdLine = $"x -tzip -aoa -hide -y -r \"{zipFilePath}\" -o\"{sDestFolder}\" -p{sPassword}";

            //---------------------------------------------------------------------------------
            // 解凍実行
            //---------------------------------------------------------------------------------
            int iSevenZipRtn = SevenZip((IntPtr)0, sCmdLine, sbOutput, sbOutput.Capacity);

            //---------------------------------------------------------------------------------
            // 成功判定
            //---------------------------------------------------------------------------------
            CheckProc(iSevenZipRtn, sbOutput);
        }
    }

    /// <summary>
    /// SevenZipメソッド成功判定
    /// </summary>
    /// <param name="iSevenZipRtn">SevenZipメソッドの戻り値</param>
    /// <param name="sbLzhOutputString">SevenZipメソッドの第3引数</param>
    private static void CheckProc(int iSevenZipRtn, StringBuilder sbLzhOutputString)
    {
        //-------------------------------------------------------------------------------------
        // メソッドの戻り値=0なら正常終了
        //-------------------------------------------------------------------------------------
        if (iSevenZipRtn == 0)
        {
            return;
        }

        //-------------------------------------------------------------------------------------
        // 例外スロー
        //-------------------------------------------------------------------------------------
        string sMsg = $"解凍処理に失敗:エラーコード={iSevenZipRtn}:{sbLzhOutputString}";
        throw new ApplicationException(sMsg);
    }

    /// <summary>
    /// [DLL Import] SevenZipのコマンドラインメソッド
    /// </summary>
    /// <param name="hwnd">ウィンドウハンドル(=0)</param>
    /// <param name="szCmdLine">コマンドライン</param>
    /// <param name="szOutput">実行結果文字列</param>
    /// <param name="dwSize">実行結果文字列格納サイズ</param>
    /// <returns>
    /// 0:正常、0以外:異常終了
    /// </returns>
    [DllImport("7-zip32.dll", CharSet = CharSet.Ansi)]
    private static extern int SevenZip(IntPtr hwnd, string szCmdLine, StringBuilder szOutput, int dwSize);
}