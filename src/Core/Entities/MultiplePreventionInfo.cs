using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Core.Entities
{
    /// <summary>
    /// 多重起動防止情報を表すクラス
    /// </summary>
    public class MultiplePreventionInfo
    {
        /// <summary>
        /// 多重起動防止Mutex名
        /// </summary>
        public string MutexName { get; set; } = "LETS_EXE_MULTIPLE_PREVENTION_MUTEX";

        /// <summary>
        /// Mutex情報
        /// </summary>
        public Mutex MutexInfo { get; set; } = null;

        /// <summary>
        /// Mutex所有権
        /// </summary>
        public bool HasHandle { get; set; } = false;
    }
}
