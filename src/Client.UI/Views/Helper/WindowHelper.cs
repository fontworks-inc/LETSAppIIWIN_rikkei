﻿using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Client.UI.Views.Helper
{
    /// <summary>
    /// ウィンドウヘルパークラス
    /// </summary>
    public static class WindowHelper
    {
        /// <summary>
        /// 拡張ウィンドウスタイル
        /// </summary>
        private const int GWLEXSTYLE = -20;

        /// <summary>
        /// 二重の境界線を持つウィンドウを作成
        /// </summary>
        private const int WSEXDLGMODALFRAME = 0x0001;

        /// <summary>
        /// cxとcyを使わないことを示す
        /// </summary>
        private const int SWPNOSIZE = 0x0001;

        /// <summary>
        /// XとYを使わないことを示す
        /// </summary>
        private const int SWPNOMOVE = 0x0002;

        /// <summary>
        /// hWndInsertAfterを使わないことを示す
        /// </summary>
        private const int SWPNOZORDER = 0x0004;

        /// <summary>
        /// ウインドウをアクティブ化しないことを示す
        /// </summary>
        private const int SWPNOACTIVATE = 0x0010;

        /// <summary>
        /// SetWindowLongによるスタイル変更を有効にすることを示す
        /// </summary>
        private const int SWPFRAMECHANGED = 0x0020;

        /// <summary>
        /// アイコンをウィンドウに関連付けすることを示す
        /// </summary>
        private const uint WMSETICON = 0x0080;

        /// <summary>
        /// ウィンドウを閉じる（閉じるメニュー削除で利用）
        /// </summary>
        private const int SCCLOSE = 0xf060;

        /// <summary>
        /// 項目のID（閉じるメニュー削除で利用）
        /// </summary>
        private const int MFBYCOMMAND = 0x0000;

        /// <summary>
        /// ウィンドウスタイルを示す（ボタン削除で利用）
        /// </summary>
        private const int GWLSTYLE = -16;

        /// <summary>
        /// ウィンドウメニューボックスを示す（ボタン削除で利用）
        /// </summary>
        private const int WSSYSMENU = 0x80000;

        /// <summary>
        /// 閉じるメニューを削除する（フレームの閉じるボタンを無効化する）
        /// </summary>
        /// <param name="window">対象のWindow</param>
        public static void RemoveCloseMenu(Window window)
        {
            IntPtr handle = new WindowInteropHelper(window).Handle;

            IntPtr hMenu = GetSystemMenu(handle, false);
            RemoveMenu(hMenu, SCCLOSE, MFBYCOMMAND);
        }

        /// <summary>
        /// アイコンを解除する
        /// </summary>
        /// <param name="window">アイコンを表示しているWindow</param>
        public static void RemoveIcon(Window window)
        {
            if (window == null)
            {
                return;
            }

            var hWnd = new WindowInteropHelper(window).Handle;

            var exStyle = GetWindowLong(hWnd, GWLEXSTYLE);
            SetWindowLong(hWnd, GWLEXSTYLE, exStyle | WSEXDLGMODALFRAME);

            SendMessage(hWnd, WMSETICON, IntPtr.Zero, IntPtr.Zero);
            SendMessage(hWnd, WMSETICON, new IntPtr(1), IntPtr.Zero);

            SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWPNOMOVE | SWPNOSIZE | SWPNOZORDER | SWPNOACTIVATE | SWPFRAMECHANGED);
        }

        public static void SetWindowTitle(Window window, string title)
        {
            var hWnd = new WindowInteropHelper(window).Handle;
            SetWindowText(hWnd, title);
        }

        /// <summary>
        /// フレームのボタンを削除する
        /// </summary>
        /// <param name="window">対象のWindow</param>
        /// <remarks>利用していないが仕様変更の可能性のために残している</remarks>
        public static void RemoveFrameButton(Window window)
        {
            IntPtr handle = new WindowInteropHelper(window).Handle;
            int style = GetWindowLong(handle, GWLSTYLE);
            style = style & (~WSSYSMENU);
            SetWindowLong(handle, GWLSTYLE, style);
        }

        /// <summary>
        /// 指定されたウィンドウに関しての情報を取得する
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="nIndex">取得するデータの指定</param>
        /// <returns>成功時取得した数値、失敗時0（ただし、取得した値が0の場合は0）</returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>
        /// 指定されたウィンドウの属性を変更する
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="nIndex">変更するデータの指定</param>
        /// <param name="dwNewLong">新しい値</param>
        /// <returns>成功時、変更前の値、失敗時0（ただし、変更前の値が0の場合は0）</returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// 子ウィンドウ、ポップアップウィンドウ、またはトップレベルウィンドウのサイズ、位置、および Z オーダーを変更する
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="hWndInsertAfter"> Z オーダーを決めるためのウィンドウハンドル</param>
        /// <param name="x">ウィンドウの左上端の新しい x 座標</param>
        /// <param name="y">ウィンドウの左上端の新しい y 座標</param>
        /// <param name="cx">ウィンドウの新しい幅（ピクセル単位）</param>
        /// <param name="cy">ウィンドウの新しい高さ（ピクセル単位）</param>
        /// <param name="uFlags">ウィンドウのサイズと位置の変更に関するフラグ</param>
        /// <returns>成功時0以外、失敗時0</returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        /// <summary>
        /// SendMessage
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <param name="msg">送信するべきメッセージ</param>
        /// <param name="wParam">メッセージ特有の追加情報</param>
        /// <param name="lParam">メッセージの追加情報</param>
        /// <returns>メッセージ処理の結果</returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// メニューのハンドル取得
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <param name="bRevert">falseの場合はシステムメニューのハンドル、trueはデフォルトに戻す</param>
        /// <returns>メニューのハンドル</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        /// <summary>
        /// メニュー項目の削除
        /// </summary>
        /// <param name="hMenu">メニューのハンドル</param>
        /// <param name="uPosition">項目のID</param>
        /// <param name="uFlags">uPosition の値</param>
        /// <returns>成否</returns>
        [DllImport("user32.dll")]
        private static extern bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        /// <summary>
        /// ウィンドウタイトルの設定
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <param name="lpString">uPosition の値</param>
        /// <returns>成否</returns>
        [DllImport("user32.dll")]
        private static extern bool SetWindowText(IntPtr hWnd, string lpString);
    }
}
