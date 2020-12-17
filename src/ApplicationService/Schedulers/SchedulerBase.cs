using System;
using System.Timers;

namespace ApplicationService.Schedulers
{
    /// <summary>
    /// スケジューリング処理基底クラス
    /// </summary>
    public abstract class SchedulerBase
    {
        /// <summary>
        /// タイマーオブジェクト
        /// </summary>
        private Timer timer;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="interval">実行間隔(ミリ秒)</param>
        protected SchedulerBase(double interval)
        {
            this.timer = new Timer();
            this.timer.Interval = interval;
            this.timer.Elapsed += new ElapsedEventHandler(this.TimerElapsed);
            this.timer.Start();
        }

        /// <summary>
        /// 継承したクラスで定義するスケジュール処理
        /// </summary>
        protected abstract void ScheduledEvent();

        /// <summary>
        /// 定期間隔で実行されるイベント
        /// </summary>
        /// <param name="sender">送信元</param>
        /// <param name="e">イベントオブジェクト</param>
        private void TimerElapsed(object sender, EventArgs e)
        {
            try
            {
                this.ScheduledEvent();
            }
            catch (Exception)
            {
                // 例外を握りつぶし、処理を継続する。ロギングも行う。
            }
        }
    }
}
