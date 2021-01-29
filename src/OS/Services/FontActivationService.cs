﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Win32;
using NLog;
using OS.Interfaces;

namespace OS.Services
{
    /// <summary>
    /// フォントアクティベートサービスを表すクラス
    /// </summary>
    public class FontActivationService : IFontActivationService
    {
        /// <summary>
        /// ウィンドウのハンドル
        /// </summary>
        private const int HWNDBROADCAST = 0xffff;

        /// <summary>
        /// フォント更新メッセージ
        /// </summary>
        private const int WMFONTCHANGE = 0x001D;

        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// フォント変更フラグ
        /// </summary>
        private static bool fontChange = false;

        /// <summary>
        /// Registryのフォントパス
        /// </summary>
        private readonly string registryFontsPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts";

        /// <summary>
        /// ユーザ別フォント情報を格納するリポジトリ
        /// </summary>
        private readonly IUserFontsSettingRepository userFontsSettingRepository;

        /// <summary>
        /// ユーザー配下のフォントフォルダ
        /// </summary>
        private readonly string fontDir = string.Empty;

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="userFontsSettingRepository">ユーザ別フォント情報を格納するリポジトリ</param>
        public FontActivationService(IUserFontsSettingRepository userFontsSettingRepository)
        {
            this.userFontsSettingRepository = userFontsSettingRepository;

            // ユーザー配下のローカルフォルダ
            var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            this.fontDir = @$"{local}\Microsoft\Windows\Fonts";
        }

        /// <summary>
        /// フォントをインストールする
        /// </summary>
        /// <param name="font">対象フォント</param>
        /// <returns>インストールが成功した場合はtrueを返す</returns>
        public bool Install(Font font)
        {
            try
            {
                Logger.Debug("Install:Path(Source)=" + font.Path);
                var destFileName = this.MoveFile(font.Path);
                font.Path = destFileName;
                Logger.Debug("Install:Path(Dest)=" + font.Path);

                // アクティベート実施
                bool activate = this.Activate(font);
                Logger.Debug("Activated:" + font.Path);

                return activate;
            }
            catch (Exception e)
            {
                Logger.Debug("Install:Exception:" + e.Message + "\n" + e.StackTrace);
            }

            Logger.Debug("Install:false:" + font.Path);
            return false;
        }

        /// <summary>
        /// フォントをアンインストールする
        /// </summary>
        /// <param name="font">対象フォント</param>
        public void Uninstall(Font font)
        {
            // ディアクティベート実施
            this.Deactivate(font);

            // フォント一覧の削除対象をTRUEに設定
            this.RemoveTargetSettings(font);
        }

        /// <summary>
        /// フォントをアクティベートする
        /// </summary>
        /// <param name="font">対象フォント</param>
        /// <returns>アクティベートに成功したらtrueを返す</returns>
        public bool Activate(Font font)
        {
            Logger.Debug("Activate:" + font.Path);

            var fontPath = font.Path;

            try
            {
                // フォント名の取得と登録
                using (var pfc = new System.Drawing.Text.PrivateFontCollection())
                {
                    pfc.AddFontFile(fontPath);
                    if (pfc.Families.Length != 0)
                    {
                        // フォント名
                        var fontName = pfc.Families[0].Name;
                        Logger.Debug("Activate:fontName=" + fontName);

                        // レジストリに登録
                        this.AddRegistry(fontName, fontPath);

                        // 登録したデータをユーザ別フォント情報にも保存
                        font.IsActivated = true;
                        font.RegistryKey = fontName;
                        this.AddSettings(font, fontName);
                    }
                }

                // 削除してから追加：既に追加されてた場合、2重に登録された形になるため。
                RemoveFontResource(fontPath);
                Logger.Debug(string.Format("Activate:RemoveFontResource:" + fontPath, string.Empty));

                // 追加：失敗時は0が戻り、成功時には追加されたフォント数が戻る（フォントは太字や斜体などがあるため1とは限らない）
                var result = AddFontResource(fontPath);
                Logger.Debug("Activate:AddFontResource:" + fontPath + " result=" + result.ToString());

                // 起動中の他のアプリケーションに通知
                if (result > 0)
                {
                    fontChange = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("Activate:\n" + ex.StackTrace);
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public void BroadcastFont()
        {
            if (fontChange)
            {
                // 起動中の他のアプリケーションに通知
                fontChange = false;
                SendMessageTimeout((IntPtr)HWNDBROADCAST, WMFONTCHANGE, IntPtr.Zero, IntPtr.Zero, 0x2, 10 * 1000, IntPtr.Zero);
            }
        }

        /// <summary>
        /// フォントをディアクティベートする
        /// </summary>
        /// <param name="font">対象フォント</param>
        public void Deactivate(Font font)
        {
            // レジストリから除外
            this.ReleaseRegistry(font.RegistryKey);

            // ユーザ別フォント情報にも保存
            font.IsActivated = false;
            this.ReleaseSettings(font);

            // 削除
            var result = RemoveFontResource(font.Path);
            Logger.Debug("Deactivate:RemoveFontResource:" + font.Path);

            // 起動中の他のアプリケーションに通知
            if (result > 0)
            {
                //SendMessage((IntPtr)HWNDBROADCAST, WMFONTCHANGE, 0, 0);
                fontChange = true;
            }
        }

        /// <summary>
        /// フォント追加
        /// </summary>
        /// <param name="lpFileName">フォント名称</param>
        /// <returns>成功時：追加されたフォントの数、失敗時：0</returns>
        [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
        private static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        /// <summary>
        /// フォント削除
        /// </summary>
        /// <param name="lpFileName">フォント名称</param>
        /// <returns>成功時：0以外、失敗時：0</returns>
        [DllImport("gdi32.dll", EntryPoint = "RemoveFontResourceW", SetLastError = true)]
        private static extern int RemoveFontResource([In][MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        /// <summary>
        /// SendMessage
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <param name="msg">送信するべきメッセージ</param>
        /// <param name="wParam">メッセージ特有の追加情報</param>
        /// <param name="lParam">メッセージの追加情報</param>
        /// <returns>メッセージ処理の結果</returns>
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, int fuFlags, int uTimeout, IntPtr lpdwResult);

        /// <summary>
        /// ファイルをフォントフォルダに移動する
        /// </summary>
        /// <param name="srcPath">移動元のファイルパス</param>
        /// <returns>移動先のファイルパス</returns>
        private string MoveFile(string srcPath)
        {
            // フォルダが存在しなければ作成する。
            if (!Directory.Exists(this.fontDir))
            {
                Directory.CreateDirectory(this.fontDir);
            }

            // 移動先のファイルパス
            var destFileName = Path.Combine(this.fontDir, Path.GetFileName(srcPath));

            // 移動＋上書き
            try
            {
                File.Move(srcPath, destFileName, true);
            }
            catch (UnauthorizedAccessException ex)
            {
                // フォントフォルダのファイルに上書きできないので無視する
                Logger.Debug("MoveFile:" + ex.StackTrace);
            }

            return destFileName;
        }

        /// <summary>
        /// レジストリに登録する
        /// </summary>
        /// <param name="key">キー：フォント名</param>
        /// <param name="value">値：フォントファイルパス</param>
        private void AddRegistry(string key, string value)
        {
            Logger.Debug("AddRegistry(Before):key=" + this.registryFontsPath + ":" + key + " value=" + value);
            var regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(this.registryFontsPath, true);
            Logger.Debug("AddRegistry(Before):regkey=" + regkey.ToString());
            regkey.SetValue(key, value);
            Logger.Debug("AddRegistry(After):key=" + key + " value=" + value);
            regkey.Close();
        }

        public bool IsExistRegistry(string key)
        {
            RegistryKey regkey = null;
            try
            {
                regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(this.registryFontsPath, true);
                if (regkey != null)
                {
                    if (string.IsNullOrEmpty(key) == false)
                    {
                        if (regkey.GetValue(key) != null)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (regkey != null)
                {
                    regkey.Close();
                }
            }
            return false;
        }

        /// <summary>
        /// フォント設定ファイルにキーを追加する
        /// </summary>
        /// <param name="font">対象フォント</param>
        /// <param name="key">キー：フォント名</param>
        private void AddSettings(Font font, string key)
        {
            var settings = this.userFontsSettingRepository.GetUserFontsSetting();
            var target = settings.Fonts.FirstOrDefault(f => f.Path == font.Path);
            if (target != null)
            {
                // 登録済（ディアクティベート⇒アクティベート）の場合
                //font.IsActivated = true;
                //font.RegistryKey = key;
                //target = font;
                target.IsActivated = true;
                target.RegistryKey = key;
            }
            else
            {
                // 新規追加の場合
                settings.Fonts.Add(font);
            }

            // 保存
            this.userFontsSettingRepository.SaveUserFontsSetting(settings);
        }

        /// <summary>
        /// レジストリから除外する
        /// </summary>
        /// <param name="key">キー：フォント名</param>
        private void ReleaseRegistry(string key)
        {
            try
            {
                var regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(this.registryFontsPath, true);
                regkey.DeleteValue(key);
            }
            catch (Exception)
            {
                //  無視
            }
        }

        /// <summary>
        /// フォント設定ファイルからキーを除外する
        /// </summary>
        /// <param name="font">対象フォント</param>
        private void ReleaseSettings(Font font)
        {
            var settings = this.userFontsSettingRepository.GetUserFontsSetting();

            var target = settings.Fonts.FirstOrDefault(f => f.Path == font.Path);
            if (target != null)
            {
                // 登録済（アクティベート⇒ディアクティベート）の場合
                //font.IsActivated = false;
                //font.RegistryKey = string.Empty;
                //target = font;
                target.IsActivated = false;
                target.RegistryKey = string.Empty;
            }

            // 保存
            this.userFontsSettingRepository.SaveUserFontsSetting(settings);
        }

        /// <summary>
        /// フォント設定ファイルの対象フォントを削除対象にする
        /// </summary>
        /// <param name="font">対象フォント</param>
        private void RemoveTargetSettings(Font font)
        {
            var settings = this.userFontsSettingRepository.GetUserFontsSetting();

            var target = settings.Fonts.FirstOrDefault(f => f.Path == font.Path);
            if (target != null)
            {
                //font.IsRemove = true;
                //target = font;
                target.IsRemove = true;
            }

            // 保存
            this.userFontsSettingRepository.SaveUserFontsSetting(settings);
        }
    }
}
