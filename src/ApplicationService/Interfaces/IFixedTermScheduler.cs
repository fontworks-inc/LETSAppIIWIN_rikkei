namespace ApplicationService.Interfaces
{
    /// <summary>
    /// 定期確認処理を行うスケジューラのインターフェース
    /// </summary>
    public interface IFixedTermScheduler
    {
        /// <summary>
        /// スケジュール処理の開始
        /// </summary>
        void Start();
    }
}
