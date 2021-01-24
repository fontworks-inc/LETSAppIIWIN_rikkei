using System;
using System.Collections.Generic;
using System.IO;
using ApplicationService.Interfaces;
using Core.Interfaces;

namespace ApplicationService.Fonts
{
    /// <summary>
    /// FileStreamの処理を継承したフォントファイル出力用クラス
    /// </summary>
    public class LetsFileStream : FileStream
    {
        /// <summary>
        /// アクティベート通知以外から呼ばれるコンストラクタ
        /// </summary>
        /// <param name="path">ファイルパス</param>
        public LetsFileStream(string path)
            : base(path, FileMode.Create, FileAccess.Write, FileShare.None)
        {
        }

        /// <summary>
        /// 進捗イベント
        /// </summary>
        public event EventHandler Progress;

        /// <summary>
        /// 現在のサイズ（バイト数）
        /// </summary>
        public long CurrentSize { get; private set; }

        /// <summary>
        /// ファイル ストリームにバイトブロックを書き込む
        /// </summary>
        /// <param name="array">書き込むデータを格納しているバッファー</param>
        /// <param name="offset">array 内のバイト オフセット</param>
        /// <param name="count">書き込む最大バイト数</param>
        public override void Write(byte[] array, int offset, int count)
        {
            base.Write(array, offset, count);
            this.CurrentSize += count;
            var progress = this.Progress;
            if (progress != null)
            {
                progress(this, EventArgs.Empty);
            }
        }
    }
}
