using System;
using System.Collections.Generic;

namespace Core.Entities
{
    /// <summary>
    /// 共通保存情報を表すクラス
    /// </summary>
    public class ApplicationRuntime
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="nextVersionInstaller">次バージョンインストーラ情報</param>
        public ApplicationRuntime(Installer nextVersionInstaller)
        {
            this.NextVersionInstaller = nextVersionInstaller;
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        public ApplicationRuntime()
        {
        }

        /// <summary>
        /// 次バージョンインストーラ情報
        /// </summary>
        public Installer NextVersionInstaller { get; set; }
    }
}
