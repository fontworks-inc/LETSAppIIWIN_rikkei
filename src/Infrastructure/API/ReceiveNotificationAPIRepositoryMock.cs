using Core.Interfaces;

namespace Infrastructure.API
{
    /// <summary>
    /// 通知受信機能を格納するリポジトリのクラス
    /// </summary>
    public class ReceiveNotificationAPIRepositoryMock : APIRepositoryBase, IReceiveNotificationRepository
    {
        /// <summary>
        /// ユーザ別ステータス情報を格納するリポジトリ
        /// </summary>
        private readonly IUserStatusRepository userStatusRepository;

        /// <summary>
        /// フォントのアクティベート通知のサービス
        /// </summary>
        /// <remarks>フォントアクティベートを受信後、該当するメソッドを呼ぶことで通知する</remarks>
        private readonly IFontNotificationService fontNotificationService;

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        /// <param name="fontNotificationService">フォントのアクティベート通知のサービス</param>
        public ReceiveNotificationAPIRepositoryMock(
            APIConfiguration apiConfiguration,
            IUserStatusRepository userStatusRepository,
            IFontNotificationService fontNotificationService)
            : base(apiConfiguration)
        {
            this.userStatusRepository = userStatusRepository;
            this.fontNotificationService = fontNotificationService;
        }

        /// <summary>
        /// SSE接続を開始する
        /// </summary>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>SSE接続の成否：常に接続</returns>
        public bool Start(string accessToken)
        {
            return true;
        }

        /// <summary>
        /// 接続中か確認する
        /// </summary>
        /// <returns>接続中か：常に接続</returns>
        public bool IsConnected()
        {
            return true;
        }

        /// <summary>
        /// SSE接続を停止する
        /// </summary>
        public void Stop()
        {
        }
    }
}
