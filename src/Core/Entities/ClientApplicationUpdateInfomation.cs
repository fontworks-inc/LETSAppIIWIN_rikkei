using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    /// <summary>
    /// クライアントアプリケーションの更新情報を表すクラス
    /// </summary>
    public class ClientApplicationUpdateInfomation
    {
        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        public ClientApplicationUpdateInfomation()
        {
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="clientApplicationVersion">クライアントアプリケーションの起動バージョン情報を表すクラス</param>
        /// <param name="applicationUpdateType">強制/任意</param>
        public ClientApplicationUpdateInfomation(ClientApplicationVersion clientApplicationVersion, bool applicationUpdateType)
        {
            this.ClientApplicationVersion = clientApplicationVersion;
            this.ApplicationUpdateType = applicationUpdateType;
        }

        /// <summary>
        /// クライアントアプリケーションの起動バージョン情報を表すクラス
        /// </summary>
        public ClientApplicationVersion ClientApplicationVersion { get; set; } = new ClientApplicationVersion();

        /// <summary>
        /// 強制/任意
        /// </summary>
        /// <remarks>強制の時にtrue, 任意の時にfalseを返す</remarks>
        public bool ApplicationUpdateType { get; set; } = false;
    }
}
