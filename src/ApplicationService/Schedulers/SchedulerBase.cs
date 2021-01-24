using System;
using System.Timers;
using Core.Interfaces;
using NLog;

namespace ApplicationService.Schedulers
{
    /// <summary>
    /// 例外発生時の通知処理
    /// </summary>
    /// <param name="exception">発生した例外</param>
    public delegate void ExceptionNotify(Exception exception);

    /// <summary>
    /// スケジューリング処理基底クラス
    /// </summary>
    public abstract class SchedulerBase
    {
        /// <summary>
        /// ロガー
        /// </summary>
        protected static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// 例外発生時の通知処理
        /// </summary>
        private ExceptionNotify exceptionNotify;

        /// <summary>
        /// タイマーオブジェクト
        /// </summary>
        private Timer timer;

        /// <summary>
        /// イベント処理が実行中かどうか
        /// </summary>
        /// <remarks>実行中の場合true, そうでない場合はfalse</remarks>
        private bool isExecuting;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="interval">実行間隔(ミリ秒)</param>
        /// <param name="exceptionNotify">例外発生時の通知処理</param>
        /// <param name="resourceWrapper">文言の取得を行うインスタンス</param>
        protected SchedulerBase(double interval, ExceptionNotify exceptionNotify, IResourceWrapper resourceWrapper)
        {
            this.timer = new Timer();
            this.timer.Interval = interval;
            this.timer.Elapsed += new ElapsedEventHandler(this.TimerElapsed);
            this.exceptionNotify = exceptionNotify;
            this.isExecuting = false;
            this.ResourceWrapper = resourceWrapper;
        }

        /// <summary>
        /// 文言の取得を行うインスタンス
        /// </summary>
        protected IResourceWrapper ResourceWrapper { get; private set; }

        /// <summary>
        /// スケジュール処理の開始
        /// </summary>
        public void Start()
        {
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
            if (this.isExecuting)
            {
                // 既に実行中の場合は処理をスキップする。
                Logger.Info(this.ResourceWrapper.GetString("LOG_INFO_SchedulerBase_TimerElapsed_ProcessSkip"));
                return;
            }

            try
            {
                this.isExecuting = true;
                this.ScheduledEvent();
            }
            catch (Exception exception)
            {
                this.exceptionNotify(exception);
            }
            finally
            {
                this.isExecuting = false;
            }
        }
    }
}
