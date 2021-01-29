using System.Diagnostics;
using System.IO;
using ApplicationService.Interfaces;
using Core.Entities;

namespace ApplicationService.Startup
{
    /// <summary>
    /// プログラムからバージョンを取得するサービス
    /// </summary>
    public class ApplicationVersionService : IApplicationVersionService
    {
        /// <summary>
        /// クライアントアプリケーションのファイルパス
        /// </summary>
        private string clientApplicationPath = null;

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        public ApplicationVersionService()
        {
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="clientApplicationPath">クライアントアプリケーションのファイルパス</param>
        public ApplicationVersionService(string clientApplicationPath)
        {
            this.clientApplicationPath = clientApplicationPath;
        }

        /// <summary>
        /// プログラムのバージョンを取得する
        /// </summary>
        /// <returns>バージョン</returns>
        public string GetVerison()
        {
            if (this.clientApplicationPath == null || !File.Exists(this.clientApplicationPath))
            {
                return string.Empty;
            }

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(this.clientApplicationPath);
            return versionInfo.ProductVersion;
        }

        /// <summary>
        /// 指定したバージョンのプログラムフォルダパスを取得する
        /// </summary>
        /// <param name="targetVersion">バージョン</param>
        /// <returns>プログラムフォルダのパス</returns>
        public string GetTargetVerisonDirectory(string targetVersion)
        {
            if (this.clientApplicationPath == null)
            {
                return string.Empty;
            }

            string parentDirectory = Directory.GetParent(this.clientApplicationPath).Parent.FullName;
            return Path.Combine(parentDirectory, "LETS-Ver" + targetVersion);
        }
    }
}