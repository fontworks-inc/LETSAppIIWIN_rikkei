using System.Drawing;
using System.Windows.Media;

namespace Core.Interfaces
{
    /// <summary>
    /// ResourceWrapper のインタフェース
    /// </summary>
    public interface IResourceWrapper
    {
        /// <summary>
        /// リソースIDを指定して ImageSource を取得する
        /// </summary>
        /// <param name="resourceID">リソースID</param>
        /// <returns>ImageSource</returns>
        public ImageSource GetImageSource(string resourceID);

        /// <summary>
        /// リソースIDを指定して文字列を取得する
        /// </summary>
        /// <param name="resourceID">リソースID</param>
        /// <returns>文字列</returns>
        public string GetString(string resourceID);

        /// <summary>
        /// リソースIDを指定してアイコンを取得する
        /// </summary>
        /// <param name="resourceID">リソースID</param>
        /// <returns>アイコン</returns>
        public Icon GetIcon(string resourceID);
    }
}
