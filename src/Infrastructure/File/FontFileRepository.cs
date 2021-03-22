using System;
using System.Runtime.InteropServices;
using Core.Interfaces;

namespace Infrastructure.File
{
    /// <summary>
    /// フォント情報のリポジトリクラス
    /// </summary>
    public class FontFileRepository : IFontFileRepository
    {
        /// <summary>
        /// 文言の取得を行うインスタンス
        /// </summary>
        private readonly IResourceWrapper resourceWrapper;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="resourceWrapper">文言の取得を行うインスタンス</param>
        public FontFileRepository(IResourceWrapper resourceWrapper)
        {
            this.resourceWrapper = resourceWrapper;
        }

        /// <summary>
        /// フォントの内部情報を取得する。
        /// </summary>
        /// <param name="fontFilePath">対象のフォントファイルパス</param>
        /// <returns>成功した場合は取得したフォントの内部情報を返し、失敗した場合は空の内部情報を返す</returns>
        public FontIdInfo GetFontInfo(string fontFilePath)
        {
            // 識別子確認のDLLを介し、情報を取得する
            var info = default(FontIdInfo);

            string outValid = string.Empty.PadLeft(1);
            IntPtr outGotIdInfo = Marshal.AllocCoTaskMem(Marshal.SizeOf(info));

            int ret = f_get_id(fontFilePath, outValid, outGotIdInfo);

            // 正常終了の0以外の場合
            if (ret != 0)
            {
                FontIdInfo errFontIdInfo = default(FontIdInfo);
                errFontIdInfo.NameInfo.Ids.FontId = string.Empty;
                errFontIdInfo.DeviceId = string.Empty;
                errFontIdInfo.UserId = string.Empty;
                errFontIdInfo.NameInfo.UniqueName = string.Empty;
                errFontIdInfo.NameInfo.Version = string.Empty;
                return errFontIdInfo;
            }

            var fontIdInfo = (FontIdInfo)Marshal.PtrToStructure(outGotIdInfo, info.GetType());
            bool isLets = outValid.CompareTo("\u0001") == 0;
            if (!isLets)
            {
                fontIdInfo.NameInfo.Ids.FontId = string.Empty;
            }

            return fontIdInfo;
        }

        /// <summary>
        /// フォント識別情報を取得する
        /// </summary>
        /// <param name="fontPath">フォントファイルパス</param>
        /// <param name="valid">（out)ユーザーフォントである場合には1、そうでない場合には0を返す</param>
        /// <param name="fontInfo">（out)フォント情報へのポインタを返す</param>
        /// <returns>正常終了の場合は0を返し、エラーが発生した場合は負の数を返す。</returns>
        [DllImport("GetID_Core.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "dllのメソッド名のため、名変更不可")]
        private static extern int f_get_id(string fontPath, string valid, IntPtr fontInfo);
    }
}