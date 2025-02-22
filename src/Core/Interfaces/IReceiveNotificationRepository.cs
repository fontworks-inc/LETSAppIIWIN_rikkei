﻿using System;

namespace Core.Interfaces
{
    /// <summary>
    ///  通知受信機能を格納するリポジトリのインターフェイス
    /// </summary>
    public interface IReceiveNotificationRepository
    {
        /// <summary>
        /// SSE接続を開始する
        /// </summary>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>SSE接続の成否</returns>
        bool Start(string accessToken, string deviceid);

        /// <summary>
        /// 接続中か確認する
        /// </summary>
        /// <returns>接続中か</returns>
        bool IsConnected();

        /// <summary>
        /// 接続した時間を返す
        /// </summary>
        /// <returns>接続開始日時</returns>
        public DateTime ConnectedTime();

        /// <summary>
        /// 接続時に使用したアクセストークンを返す
        /// </summary>
        /// <returns>接続開始時アクセストークン</returns>
        public string ConnectedAccessToken();

        /// <summary>
        /// SSE接続を停止する
        /// </summary>
        void Stop();
    }
}
