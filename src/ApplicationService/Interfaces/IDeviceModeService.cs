using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationService.Interfaces
{
    /// <summary>
    /// デバイスモードに関する処理を行うサービスのインターフェイス
    /// </summary>
    public interface IDeviceModeService
    {
        /// <summary>
        /// 起動時/24時間チェックを行う
        /// </summary>
        IList<string> FixedTermCheck(bool isStartup);

        IList<string> UninstallFonts(IList<string> uninstallFontList, string messageHead);

        IList<string> InstallFonts(string tempPath);

        IDictionary<string, int> LetsNameKindMap();

        IDictionary<int, string> LetsKindNameMap();

        void DeleteFolder(string targetDirectoryPath);
    }
}
