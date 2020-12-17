namespace Core.Entities
{
    /// <summary>
    /// アプリケーション設定情報を表すクラス
    /// </summary>
    public class ApplicationSetting
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        public ApplicationSetting()
        {
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="fontDeliveryServerUri">フォント配信サーバURI</param>
        /// <param name="notificationServerUri">通知サーバURI</param>
        /// <param name="communicationRetryCount">通信リトライ回数</param>
        /// <param name="fixedTermConfirmationInterval">定期確認間隔</param>
        /// <param name="fontCalculationFactor">フォント容量計算係数</param>
        public ApplicationSetting(
            string fontDeliveryServerUri,
            string notificationServerUri,
            int communicationRetryCount,
            int fixedTermConfirmationInterval,
            int fontCalculationFactor)
        {
            this.FontDeliveryServerUri = fontDeliveryServerUri;
            this.NotificationServerUri = notificationServerUri;
            this.CommunicationRetryCount = communicationRetryCount;
            this.FixedTermConfirmationInterval = fixedTermConfirmationInterval;
            this.FontCalculationFactor = fontCalculationFactor;
        }

        /// <summary>
        /// フォント配信サーバURI
        /// </summary>
        public string FontDeliveryServerUri { get; set; }

        /// <summary>
        /// 通知サーバURI
        /// </summary>
        public string NotificationServerUri { get; set; }

        /// <summary>
        /// 通信リトライ回数
        /// </summary>
        /// <remarks>通信エラー時のリトライ回数</remarks>
        /// <example>10</example>
        public int CommunicationRetryCount { get; set; }

        /// <summary>
        /// 定期確認間隔
        /// </summary>
        /// <remarks>定期確認を実行する間隔</remarks>
        /// <example>1800</example>
        public int FixedTermConfirmationInterval { get; set; }

        /// <summary>
        /// フォント容量計算係数
        /// </summary>
        /// <remarks>フォントインストール可能容量の計算係数</remarks>
        public int FontCalculationFactor { get; set; }
    }
}
