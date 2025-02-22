﻿using System.Collections.Generic;
using Core.Entities;

namespace ApplicationService.Interfaces
{
    /// <summary>
    /// フォント管理に関する処理を行うサービスのインターフェイス
    /// </summary>
    public interface IFontManagerService
    {
        /// <summary>
        /// エラーダイアログ表示用のイベント
        /// </summary>
        /// <param name="text">本文</param>
        /// <param name="caption">タイトル</param>
        public delegate void ShowErrorDialogEvent(string text, string caption);

        /// <summary>
        /// ダウンロード開始時のイベント
        /// </summary>
        /// <param name="font">フォント</param>
        /// <param name="compFileSize">ダウンロード済みファイルサイズ</param>
        /// <param name="totalFileSize">合計ファイルサイズ</param>
        public delegate void FontStartDownloadEvent(InstallFont font, double compFileSize, double totalFileSize);

        /// <summary>
        /// ダウンロード完了時のイベント
        /// </summary>
        /// <param name="fontList">フォントリスト</param>
        public delegate void FontDownloadCompletedEvent(IList<InstallFont> fontList);

        /// <summary>
        /// ダウンロード失敗時のイベント
        /// </summary>
        /// <param name="font">失敗したフォント</param>
        public delegate void FontDownloadFailedEvent(InstallFont font);

        /// <summary>
        /// ダウンロード中断時のイベント
        /// </summary>
        /// <param name="font">中断したフォント</param>
        public delegate void FontDownloadCancelledEvent(InstallFont font);

        /// <summary>
        /// フォント同期完了時ののイベント
        /// </summary>
        public delegate void FontUpdateComplateEvent();

        /// <summary>
        /// フォントの同期処理を実施する
        /// </summary>
        /// <param name="font">アクティベート対象フォント</param>
        /// <remarks>アクティベート通知からの同期処理</remarks>
        void Synchronize(ActivateFont font);

        /// <summary>
        /// フォントの同期処理を実施する
        /// </summary>
        /// <param name="startUp">起動時かどうか</param>
        /// <remarks>アクティベート通知以外からの同期処理</remarks>
        void Synchronize(bool startUp);

        /// <summary>
        /// ダウンロード実行中フラグ取得
        /// </summary>
        /// <returns>起動時ダウンロード実行終了フラグ</returns>
        bool GetIsFirstDownloadCompleted();

        /// <summary>
        /// 契約切れフォントのディアクティベート
        /// </summary>
        /// <param name="contracts">契約情報</param>
        void DeactivateExpiredFonts(IList<Contract> contracts);

        /// <summary>
        /// フォント：フォント一覧の一括ディアクティベート
        /// </summary>
        /// <remarks>保存ファイル内のアクティベートフォントを一括でディアクティベートする</remarks>
        void DeactivateSettingFonts();

        /// <summary>
        /// ディアクティベートされたフォントの削除
        /// </summary>
        void UninstallDeactivatedFonts();

        /// <summary>
        /// フォントのディアクティベート
        /// </summary>
        /// <param name="fontId">ディアクティベートするフォントID</param>
        void DeactivateFont(string fontId);

        /// <summary>
        /// フォント：フォント一覧のチェック
        /// </summary>
        void CheckFontsList();

        /// <summary>
        /// フォント：フォント一覧の更新
        /// </summary>
        /// <param name="userFontsDir">ユーザーフォントのディレクトリ</param>
        void UpdateFontsList(string userFontsDir);

        /// <summary>
        /// フォントチェンジメッセージを送信する
        /// </summary>
        void BroadcastFontChange();

        /// <summary>
        /// サーバより削除されたフォント情報の一覧を取得する
        /// </summary>
        /// <returns>サーバより削除されたフォント情報の一覧</returns>
        IList<InstallFont> GetDeletedFontInformations();

        /// <summary>
        /// フォントをアンインストールする
        /// </summary>
        /// <param name="font">対象フォント</param>
        void Uninstall(Font font);
    }
}