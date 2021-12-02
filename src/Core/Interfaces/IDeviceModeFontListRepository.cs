using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// フォント情報一覧(デバイスモード時)を格納するリポジトリのインターフェイス
    /// </summary>
    public interface IDeviceModeFontListRepository
    {
        /// <summary>
        /// フォント情報一覧(デバイスモード時)を取得する
        /// </summary>
        /// <returns>フォント情報一覧(デバイスモード時)</returns>
        DeviceModeFontList GetDeviceModeFontList();

        /// <summary>
        /// フォント情報一覧(デバイスモード時)を保存する
        /// </summary>
        /// <param name="deviceModeFontList">フォント情報一覧(デバイスモード時)</param>
        void SaveDeviceModeFontList(DeviceModeFontList deviceModeFontList);
    }
}
