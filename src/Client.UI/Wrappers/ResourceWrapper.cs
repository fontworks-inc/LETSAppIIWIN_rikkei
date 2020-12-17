using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Core.Interfaces;

namespace Client.UI.Wrappers
{
    /// <summary>
    /// ResourceWrapper クラス
    /// </summary>
    public class ResourceWrapper : IResourceWrapper
    {
        /// <summary>
        /// ResourceManager
        /// </summary>
        private ResourceManager resourceManager;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        public ResourceWrapper()
        {
            var assembly = Assembly.GetExecutingAssembly();
            this.resourceManager = new ResourceManager("Client.UI.Resources.Resource", assembly);
        }

        /// <summary>
        /// リソースIDを指定して ImageSource を取得する
        /// </summary>
        /// <param name="resourceID">リソースID</param>
        /// <returns>ImageSource</returns>
        public ImageSource GetImageSource(string resourceID)
        {
            ImageSource source = null;

            try
            {
                Bitmap bitmap = (Bitmap)this.resourceManager.GetObject(resourceID);
                MemoryStream stream = new MemoryStream();
                bitmap.Save(stream, ImageFormat.Png);

                BitmapDecoder decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                source = decoder.Frames[0];
            }
            catch (Exception)
            {
            }

            return source;
        }

        /// <summary>
        /// リソースIDを指定して文字列を取得する
        /// </summary>
        /// <param name="resourceID">リソースID</param>
        /// <returns>文字列</returns>
        public string GetString(string resourceID)
        {
            return this.resourceManager.GetString(resourceID);
        }

        /// <summary>
        /// リソースIDを指定してアイコンを取得する
        /// </summary>
        /// <param name="resourceID">リソースID</param>
        /// <returns>アイコン</returns>
        public Icon GetIcon(string resourceID)
        {
            return (Icon)this.resourceManager.GetObject(resourceID);
        }
    }
}
