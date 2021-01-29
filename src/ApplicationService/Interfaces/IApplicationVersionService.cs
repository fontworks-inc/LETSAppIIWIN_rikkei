using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationService.Interfaces
{
    /// <summary>
    /// プログラムからバージョンを取得するサービスのインターフェース
    /// </summary>
    public interface IApplicationVersionService
    {
        /// <summary>
        /// プログラムのバージョンを取得する
        /// </summary>
        /// <returns>バージョン</returns>
        string GetVerison();

        /// <summary>
        /// 指定したバージョンのプログラムフォルダパスを取得する
        /// </summary>
        /// <param name="targetVersion">バージョン</param>
        /// <returns>プログラムフォルダのパス</returns>
        string GetTargetVerisonDirectory(string targetVersion);
    }
}