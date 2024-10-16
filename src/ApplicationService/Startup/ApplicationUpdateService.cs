using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using ApplicationService.Interfaces;
using Core.Entities;
using Core.Interfaces;
using NLog;

namespace ApplicationService.Startup
{
    /// <summary>
    /// プログラムのアップデートを行うサービス
    /// </summary>
    public class ApplicationUpdateService : IApplicationUpdateService
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// 文言の取得を行うインスタンス
        /// </summary>
        private IResourceWrapper resourceWrapper = null;

        /// <summary>
        /// メモリで保持する情報を格納するリポジトリ
        /// </summary>
        private IVolatileSettingRepository volatileSettingRepository;

        /// <summary>
        /// クライアントアプリの起動Ver情報のファイルリポジトリ
        /// </summary>
        private IClientApplicationVersionRepository clientApplicationVersionFileRepository;

        /// <summary>
        /// 共通保存情報を格納するリポジトリ
        /// </summary>
        private IApplicationRuntimeRepository applicationRuntimeRepository;

        /// <summary>
        /// 指定のプロセスを実施するサービス
        /// </summary>
        private IStartProcessService startProcessService;

        /// <summary>
        /// インスタンスを初期化する
        /// 指定のプロセスを実施するサービス
        /// </summary>
        public ApplicationUpdateService()
        {
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="resourceWrapper">文言の取得を行うインスタンス</param>
        /// <param name="volatileSettingRepository">メモリで保持する情報を格納するリポジトリ</param>
        /// <param name="clientApplicationVersionFileRepository">クライアントアプリの起動Ver情報のファイルリポジトリ</param>
        /// <param name="applicationRuntimeRepository">共通保存情報を格納するリポジトリ</param>
        /// <param name="startProcessService">指定のプロセスを実施するサービス</param>
        public ApplicationUpdateService(
            IResourceWrapper resourceWrapper,
            IVolatileSettingRepository volatileSettingRepository,
            IClientApplicationVersionRepository clientApplicationVersionFileRepository,
            IApplicationRuntimeRepository applicationRuntimeRepository,
            IStartProcessService startProcessService)
        {
            this.resourceWrapper = resourceWrapper;
            this.volatileSettingRepository = volatileSettingRepository;
            this.clientApplicationVersionFileRepository = clientApplicationVersionFileRepository;
            this.applicationRuntimeRepository = applicationRuntimeRepository;
            this.startProcessService = startProcessService;
        }

        /// <summary>
        /// プログラムのアップデートを実施
        /// </summary>
        /// <param name="changeUIEvent">UI(メニュー・アイコン)を変更するためのイベント</param>
        public void Update()
        {
            Logger.Debug($"Update:Enter");

            //// アプリ実行ユーザの所属グループに「管理者グループ（SID : S-1-5-32-544）」が含まれているかチェックする
            //AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            //WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
            //List<Claim> userClaims = new List<Claim>(principal.UserClaims);
            //bool isAdministrator = userClaims.Where(claim => claim.Value.Equals("S-1-5-32-544")).Any();
            //if (!isAdministrator)
            //{
            //    // 管理者グループではない場合、エラーを出力して処理を終了する
            //    throw new UnauthorizedAccessException(this.resourceWrapper.GetString("FUNC_01_02_16_ERR_StartAsAdministrators"));
            //}

            // アップデータのディレクトリパスを作成する
            string directoryPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
            Logger.Debug($"Update:directoryPath={directoryPath}");

            // 第一引数にバージョン、第二引数に起動指定バージョン
            List<string> argList = new List<string>();
            argList.Add(this.applicationRuntimeRepository.GetApplicationRuntime().NextVersionInstaller.Version);
            string runVersion = this.clientApplicationVersionFileRepository.GetClientApplicationVersion().Version;
            if (string.IsNullOrEmpty(runVersion))
            {
                Logger.Debug($"Update:起動指定バージョンが取れないときは、アップデートバージョンを渡す");
                runVersion = this.applicationRuntimeRepository.GetApplicationRuntime().NextVersionInstaller.Version;
            }

            argList.Add(runVersion);

            // 実行権限を管理者権限とし、プログラムアップデータを実行する
            Logger.Debug($"Update:StartProcessAdministrator={directoryPath}/LETSUpdater.exe");
            this.startProcessService.StartProcessAdministrator(directoryPath, "LETSUpdater.exe", argList.ToArray());

            // メモリ上に「プログラムアップデート中」を設定する
            VolatileSetting volatileSetting = this.volatileSettingRepository.GetVolatileSetting();
            volatileSetting.IsUpdating = true;

            // 共通保存に「アップデータ実行」を記録する
            Logger.Debug($"Update:共通保存に「アップデータ実行」を記録する:DownloadStatus={DownloadStatus.Update}");
            ApplicationRuntime applicationRuntime = this.applicationRuntimeRepository.GetApplicationRuntime();
            applicationRuntime.NextVersionInstaller.DownloadStatus = DownloadStatus.Update;
            this.applicationRuntimeRepository.SaveApplicationRuntime(applicationRuntime);
        }
    }
}
