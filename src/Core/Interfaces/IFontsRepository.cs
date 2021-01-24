using System.Collections.Generic;
using System.IO;
using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// フォント情報を格納するリポジトリのインターフェイス
    /// </summary>
    public interface IFontsRepository
    {
        /// <summary>
        /// インストールフォント情報を取得する
        /// </summary>
        /// <param name="type">取得するフォントの適用種別</param>
        /// <returns>フォント情報リスト</returns>
        IList<InstallFont> GetInstallFontInformations(string deviceId, VaildFontType type);

        /// <summary>
        /// フォントをダウンロードする
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="fontId">フォントID</param>
        public FileStream DownloadFonts(string deviceId, string fontId);
    }
}
