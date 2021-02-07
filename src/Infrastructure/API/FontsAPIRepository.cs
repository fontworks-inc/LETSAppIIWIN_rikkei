using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Memory;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Infrastructure.API
{
    /// <summary>
    ///  フォント情報リポジトリのインターフェイス
    /// </summary>
    public class FontsAPIRepository : APIRepositoryBase, IFontsRepository
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        public FontsAPIRepository(APIConfiguration apiConfiguration)
            : base(apiConfiguration)
        {
        }

        /// <summary>
        /// フォントをダウンロードする
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="fontId">フォントID</param>
        /// <remarks>FUNCTION_08_06_03(フォントダウンロードAPI)</remarks>
        /// <returns>フォントファイルストリーム</returns>
        public FileStream DownloadFonts(string deviceId, string fontId)
        {
            FileStream response = null;

            this.ApiParam[APIParam.DeviceId] = deviceId;
            this.ApiParam[APIParam.FontId] = fontId;

            var memory = new VolatileSettingMemoryRepository().GetVolatileSetting();
            this.ApiParam[APIParam.AccessToken] = memory.AccessToken;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallFontsDownload);

                // 戻り値のセット（個別処理）
                var ret = (FileStream)this.ApiResponse;
                response = ret;
            }
            catch (ApiException)
            {
                // 通信に失敗or通信しなかった
                throw;
            }

            return response;
        }

        /// <inheritdoc/>
        public IList<InstallFont> GetInstallFontInformations(string deviceId, VaildFontType type)
        {
            IList<InstallFont> response = new List<InstallFont>();

            this.ApiParam[APIParam.DeviceId] = deviceId;
            switch (type)
            {
                case VaildFontType.AvailableFonts:
                    this.ApiParam[APIParam.Contains] = "availableFonts";
                    break;

                case VaildFontType.DeletedFonts:
                    this.ApiParam[APIParam.Contains] = "deletedFonts";
                    break;

                case VaildFontType.Both:
                default:
                    this.ApiParam[APIParam.Contains] = "availableFonts,deletedFonts";
                    break;
            }

            var memory = new VolatileSettingMemoryRepository().GetVolatileSetting();
            this.ApiParam[APIParam.AccessToken] = memory.AccessToken;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallGetInstallFonts);

                // 戻り値のセット（個別処理）
                var ret = (InlineResponse200)this.ApiResponse;
                if (ret.Code == (int)ResponseCode.Succeeded)
                {
                    if (ret.Data != null)
                    {
                        foreach (InlineResponse200Font d in ret.Data.Fonts)
                        {
                            InstallFont f = new InstallFont(
                                d.UserFontId,
                                d.ActivateFlg,
                                d.FontId,
                                d.DisplayFontName,
                                d.FileName,
                                d.FileSize,
                                d.Version,
                                d.NeedFontVersionUpdate,
                                d.IsAvailableFont,
                                d.IsFreemium,
                                d.ContractIds);
                            response.Add(f);
                        }
                    }
                }
                else
                {
                    throw new ApiException(ret.Code, ret.Message);
                }
            }
            catch (ApiException)
            {
                // 通信に失敗or通信しなかった
                throw;
            }

            return response;
        }

        /// <summary>
        /// フォントダウンロードの取得呼び出し
        /// </summary>
        private void CallFontsDownload()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.AccessToken = (string)this.ApiParam[APIParam.AccessToken];
            FontApi apiInstance = new FontApi(config);
            this.ApiResponse = apiInstance.GetFont((string)this.ApiParam[APIParam.DeviceId], config.UserAgent, (string)this.ApiParam[APIParam.FontId]);
        }

        /// <summary>
        /// フォントダウンロードの取得呼び出し
        /// </summary>
        private void CallGetInstallFonts()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.AccessToken = (string)this.ApiParam[APIParam.AccessToken];
            FontApi apiInstance = new FontApi(config);
            this.ApiResponse = apiInstance.GetInstallFonts((string)this.ApiParam[APIParam.DeviceId], config.UserAgent, (string)this.ApiParam[APIParam.Contains]);
        }
    }
}
