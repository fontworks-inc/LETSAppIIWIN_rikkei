namespace ApplicationService.Schedulers
{
    /// <summary>
    /// ログイン状態確認処理のスケジューリング処理基底クラス
    /// </summary>
    public class ConfirmLoginStatusScheduler : SchedulerBase
    {
        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="interval">実行間隔(ミリ秒)</param>
        /// <param name="confirmLoginStatusFunction">ログイン状態確認処理メソッド</param>
        public ConfirmLoginStatusScheduler(double interval, ConfirmLoginStatus confirmLoginStatusFunction)
            : base(interval)
        {
            this.ConfirmLoginStatusFunction = confirmLoginStatusFunction;
        }

        /// <summary>
        /// ログイン状態確認処理のdelegate
        /// </summary>
        public delegate void ConfirmLoginStatus();

        /// <summary>
        /// ログイン状態確認処理
        /// </summary>
        private ConfirmLoginStatus ConfirmLoginStatusFunction { get; set; }

        /// <summary>
        /// 定期間隔で実行されるイベント
        /// </summary>
        protected override void ScheduledEvent()
        {
            this.ConfirmLoginStatusFunction();
        }
    }
}
