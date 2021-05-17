using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Core.Entities;
using Core.Interfaces;
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
        /// ユーザ別ステータス情報を格納するリポジトリ
        /// </summary>
        private readonly IUserStatusRepository userStatusRepository = null;

        /// <summary>
        /// フォント内部情報を格納するリポジトリ
        /// </summary>
        private readonly IFontFileRepository fontInfoRepository = null;

        /// <summary>
        /// ユーザー配下のフォントフォルダ
        /// </summary>
        private readonly string fontDir = string.Empty;

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="userFontsSettingRepository">ユーザ別フォント情報を格納するリポジトリ</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        public FontActivationService(IUserFontsSettingRepository userFontsSettingRepository, IUserStatusRepository userStatusRepository, IFontFileRepository fontInfoRepository)
        {
            this.userFontsSettingRepository = userFontsSettingRepository;
            this.userStatusRepository = userStatusRepository;
            this.fontInfoRepository = fontInfoRepository;

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
            if (!this.userStatusRepository.GetStatus().IsLoggingIn)
            {
                return false;
            }

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

            this.Delete(font);
        }

        /// <inheritdoc/>
        public void Delete(Font font)
        {
            // フォントファイルの削除を試みる
            try
            {
                if (File.Exists(font.Path))
                {
                    File.Delete(font.Path);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // フォント一覧の削除対象をTRUEに設定
                this.RemoveTargetSettings(font);
            }
            catch (Exception ex)
            {
                Logger.Debug("FontActivationService:Uninstall:" + ex.StackTrace);
            }
        }

        /// <summary>
        /// フォントをアクティベートする
        /// </summary>
        /// <param name="font">対象フォント</param>
        /// <returns>アクティベートに成功したらtrueを返す</returns>
        public bool Activate(Font font)
        {
            Logger.Debug("Activate:" + font.Path);

            if (!this.userStatusRepository.GetStatus().IsLoggingIn)
            {
                return false;
            }

            var fontPath = font.Path;

            try
            {
                // フォント名の取得
                string fontName = this.GetFontName(font);

                // レジストリに登録
                this.AddRegistry(fontName, fontPath);

                // 登録したデータをユーザ別フォント情報にも保存
                font.IsActivated = true;
                font.RegistryKey = fontName;
                this.AddSettings(font, fontName);

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
                PostMessage((IntPtr)HWNDBROADCAST, WMFONTCHANGE, 0x0, 0x0);
            }
        }

        /// <summary>
        /// フォントをディアクティベートする
        /// </summary>
        /// <param name="font">対象フォント</param>
        public void Deactivate(Font font)
        {
            // レジストリから除外
            if (string.IsNullOrEmpty(font.RegistryKey))
            {
                string fontName = this.GetFontName(font);
                this.ReleaseRegistry(fontName);
            }
            else
            {
                this.ReleaseRegistry(font.RegistryKey);
            }

            // ユーザ別フォント情報にも保存
            font.IsActivated = false;
            this.ReleaseSettings(font);

            // 削除
            var result = RemoveFontResource(font.Path);
            Logger.Debug("Deactivate:RemoveFontResource:" + font.Path);

            // 起動中の他のアプリケーションに通知
            fontChange = true;
        }

        /// <summary>
        /// フォント名取得
        /// </summary>
        private string GetFontName(Font font)
        {
            try
            {
                // 識別子確認のDLLを介し、情報を取得する
                string filepath = font.Path;
                if (File.Exists(filepath))
                {
                    var fontIdInfo = this.fontInfoRepository.GetFontInfo(font.Path);
                    string fontname = fontIdInfo.NameInfo.UniqueName;
                    string ext = Path.GetExtension(font.Path).ToLower();
                    if (ext.CompareTo(".ttc") == 0)
                    {
                        fontname += "(TTC)";
                    }

                    return fontname;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
            }

            return string.Empty;
        }

        /// <summary>
        /// フォント追加
        /// </summary>
        /// <param name="lpFileName">フォント名称</param>
        /// <returns>成功時：追加されたフォントの数、失敗時：0</returns>
        [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "<保留中>")]
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
        private static extern int PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

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
            var regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(this.registryFontsPath, true);

            // 同じフォントパスに対して別のレジストリがあれば削除する
            string[] keys = regkey.GetValueNames();
            foreach (string k in keys)
            {
                string v = (string)regkey.GetValue(k);
                if (v.Equals(value))
                {
                    if (!k.Equals(key))
                    {
                        regkey.DeleteValue(k);
                    }
                }
            }

            regkey.SetValue(key, value);
            regkey.Close();
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
                // 無視
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
                target.IsActivated = false;
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
                target.IsRemove = true;
            }

            // 保存
            this.userFontsSettingRepository.SaveUserFontsSetting(settings);
        }
    }
}
