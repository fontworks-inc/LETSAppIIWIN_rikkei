using System;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using Core.Interfaces;

namespace Infrastructure.API
{
    /// <summary>
    /// API接続に利用する設定情報
    /// </summary>
    public class APIConfiguration : IAPIConfiguration
    {
        /// <summary>
        /// 強制ログアウトを実施するイベント
        /// </summary>
        public delegate void ForceLogoutEvent();

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="basePath">API接続先のベースURL</param>
        /// <param name="notifyBasePath">通知サーバーのURL</param>
        /// <param name="fixedTermConfirmationInterval">定期確認間隔</param>
        /// <param name="communicationRetryCount">リトライ回数</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "<保留中>")]
        public APIConfiguration(string basePath, string notifyBasePath, int fixedTermConfirmationInterval, int communicationRetryCount, IProxyAuthSettingRepository proxyAuthSettingRepository)
        {
            this.BasePath = basePath;
            this.NotifyBasePath = notifyBasePath;
            this.FixedTermConfirmationInterval = fixedTermConfirmationInterval;
            this.CommunicationRetryCount = communicationRetryCount;
            this.ProxyAuthSettingRepository = proxyAuthSettingRepository;
        }

        /// <summary>
        /// API接続先のベースURL
        /// </summary>
        /// <example>"http://api/v1</example>
        public string BasePath { get; }

        /// <summary>
        /// 通知サーバーのURL
        /// </summary>
        public string NotifyBasePath { get; }

        /// <summary>
        /// 定期確認間隔
        /// </summary>
        public int FixedTermConfirmationInterval { get; }

        /// <summary>
        /// リトライ回数
        /// </summary>
        public int CommunicationRetryCount { get; }

        /// <summary>
        /// プロキシ認証情報
        /// </summary>
        public IProxyAuthSettingRepository ProxyAuthSettingRepository { get; }

        /// <summary>
        /// 強制ログアウトを実施するイベント
        /// </summary>
        public ForceLogoutEvent ForceLogout { get; set; }

        /// <summary>
        /// プロキシを取得する
        /// </summary>
        /// <param name="targeturi">接続先URI</param>
        /// <returns>プロキシ情報</returns>
        public IWebProxy GetWebProxy(string targeturl)
        {
            IWebProxy webproxy = WebRequest.GetSystemWebProxy();
            if (!webproxy.IsBypassed(new Uri(this.BasePath)))
            {
                var proxyAuth = this.ProxyAuthSettingRepository.GetSetting();
                if (!string.IsNullOrEmpty(proxyAuth.ID))
                {
                    webproxy.Credentials = new NetworkCredential(proxyAuth.ID, proxyAuth.Password);
                }
                else
                {
                    webproxy.Credentials = CredentialCache.DefaultCredentials;
                }

                return webproxy;
            }

            return null;
        }

        /// <summary>
        /// UseerAgentを取得する
        /// </summary>
        /// <param name="apppath">アプリケーションパス</param>
        /// <returns>UserAgent</returns>
        public string GetUserAgent(string apppath)
        {
            // アプリバージョンの取得
            var appver = string.Empty;

            if (System.IO.File.Exists(apppath))
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(apppath);
                appver = versionInfo.ProductVersion;
            }

            System.Diagnostics.Process pro = new System.Diagnostics.Process();

            pro.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            pro.StartInfo.Arguments = @"/c ver";
            pro.StartInfo.CreateNoWindow = true;
            pro.StartInfo.UseShellExecute = false;
            pro.StartInfo.RedirectStandardOutput = true;

            pro.Start();
            string output = pro.StandardOutput.ReadToEnd();

            MatchCollection matches = Regex.Matches(output, @"\d+\.\d+\.\d+(\.\d+)?");
            foreach (Match match in matches)
            {
                output = string.Concat("LETS/", appver, " (Win ", match.Value, ")");
            }

            return output;
        }
    }
}
