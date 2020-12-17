using System.Collections.Generic;

namespace Core.Entities
{
    /// <summary>
    /// ユーザ別フォント情報を表すクラス
    /// </summary>
    public class UserFontsSetting
    {
        /// <summary>
        /// フォント一覧
        /// </summary>
        public IList<Font> Fonts { get; set; } = new List<Font>();
    }
}
