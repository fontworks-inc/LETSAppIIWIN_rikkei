using Core.Entities;

namespace OS.Interfaces
{
    /// <summary>
    /// フォントアクティベートサービスを表すインターフェイス
    /// </summary>
    public interface IFontActivationService
    {
        /// <summary>
        /// フォントをインストールする
        /// </summary>
        /// <param name="font">対象フォント</param>
        /// <returns>true:インストール成功、false:インストール失敗</returns>
        bool Install(Font font);

        /// <summary>
        /// フォントチェンジメッセージを送信する
        /// </summary>
        void BroadcastFont();

        /// <summary>
        /// フォントをアンインストールする
        /// </summary>
        /// <param name="font">対象フォント</param>
        void Uninstall(Font font);

        /// <summary>
        /// フォントを削除する
        /// </summary>
        /// <param name="font">対象フォント</param>
        /// <returns>実削除を行ったらtrueを返す</returns>
        bool Delete(Font font);

        /// <summary>
        /// フォントをアクティベートする
        /// </summary>
        /// <param name="font">対象フォント</param>
        /// <returns>アクティベートに成功したらtrueを返す</returns>
        bool Activate(Font font);

        /// <summary>
        /// フォントをディアクティベートする
        /// </summary>
        /// <param name="font">対象フォント</param>
        void Deactivate(Font font);

        /// <summary>
        /// フォントレジストリを削除する
        /// </summary>
        /// <param name="font">対象フォント</param>
        void DelRegistory(Font font);

        /// <summary>
        /// フォント設定ファイルの対象フォントを削除対象にする
        /// </summary>
        /// <param name="font">対象フォント</param>
        /// <param name="isRemove">true:削除対象</param>
        void RemoveTargetSettings(Font font, bool isRemove = true);

        /// <summary>
        /// フォントをインストールする(デバイスモード)
        /// </summary>
        /// <param name="fontPath">対象フォント</param>
        /// <returns>true:インストール成功、false:インストール失敗</returns>
        DeviceModeFontInfo InstallDeviceMode(string fontPath);

        /// <summary>
        /// フォントをアンインストールする(デバイスモード)
        /// </summary>
        /// <param name="letsKind">削除対象LETS種別</param>
        void UninstallDeviceMode(int letsKind);
    }
}
