﻿namespace Core.Entities
{
    /// <summary>
    /// レスポンスコード
    /// </summary>
    public enum ResponseCode
    {
        /// <summary>正常</summary>
        Succeeded = 0,

        /// <summary>２要素認証要求</summary>
        TwoFAIsRequired = 100,

        /// <summary>引数不正</summary>
        InvalidArgument = 1001,

        /// <summary>認証エラー</summary>
        AuthenticationFailed = 1002,

        /// <summary>２要素認証コード有効期限切れエラー</summary>
        TwoFACodeHasExpired = 1003,

        /// <summary>認証連続失敗(アカウントロック)得エラー</summary>
        AcountLocked = 1004,

        /// <summary>同時使用デバイス数の上限エラー</summary>
        MaximumNumberOfDevicesInUse = 1005,

        /// <summary>契約終了済みエラー</summary>
        ContractExpired = 1006,

        /// <summary>アクセストークン有効期限切れエラー</summary>
        AccessTokenExpired = 1007,

        /// <summary>リフレッシュトークン有効期限切れエラー</summary>
        RefreshTokenExpired = 1008,
    }
}
