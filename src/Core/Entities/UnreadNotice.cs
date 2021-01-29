using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    /// <summary>
    /// 未読お知らせ情報を表すクラス
    /// </summary>
    public class UnreadNotice
    {
        /// <summary>
        /// 未読お知らせ件数
        /// </summary>
        public int Toal { get; set; } = -1;

        /// <summary>
        /// 未読お知らせ有無
        /// </summary>
        public bool ExistsLatestNotice { get; set; } = false;
    }
}
