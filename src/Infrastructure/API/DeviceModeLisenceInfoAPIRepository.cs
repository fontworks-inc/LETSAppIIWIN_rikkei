using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text.Json;
using Core.Entities;
using Core.Interfaces;
using Newtonsoft.Json;
using NLog;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Infrastructure.API
{
    public class DeviceModeLisenceInfoAPIRepository : APIRepositoryBase, IDeviceModeLicenseInfoRepository
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");


        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        public DeviceModeLisenceInfoAPIRepository(APIConfiguration apiConfiguration)
            : base(apiConfiguration)
        {
        }

        public DeviceModeLicenseInfo GetDeviceModeLicenseInfo()
        {
            throw new NotImplementedException();
        }

        public DeviceModeLicenseInfo GetDeviceModeLicenseInfo(bool fromOnline, string offlineDeviceId, string indefiniteAccessToken, string licenceFileKeyPath, string licenseDecryptionKey)
        {
            return this.GetUpdateLicense(offlineDeviceId, indefiniteAccessToken, licenceFileKeyPath, licenseDecryptionKey);
        }

        public void SaveDeviceModeLicenseInfo(DeviceModeLicenseInfo setting)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 契約情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>契約情報の集合体</returns>
        public DeviceModeLicenseInfo GetUpdateLicense(string offlineDeviceId, string indefiniteAccessToken, string licenceFileKeyPath, string licenseDecryptionKey)
        {
            UpdateLicenseResponse response = new UpdateLicenseResponse();
            DeviceModeLicenseInfo deviceModeLicenseInfo = new DeviceModeLicenseInfo();

            // APIの引数の値をセット(個別処理)
            this.ApiParam[APIParam.OfflineDeviceId] = offlineDeviceId;
            this.ApiParam[APIParam.IndefiniteAccessToken] = indefiniteAccessToken;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallUpdateLicenseAPI);

                // 戻り値のセット（個別処理）
                // 戻り値のセット（個別処理）
                Logger.Info(string.Format("DeviceModeLisenceInfoAPIRepository:GetUpdateLicense 戻り値のセット（個別処理）", string.Empty));
                var ret = (UpdateLicenseResponse)this.ApiResponse;
                response.Code = ret.Code;
                response.Message = ret.Message;
                if (ret.Code == (int)ResponseCode.Succeeded)
                {
                    try
                    {
                        // ライセンスキーを解凍する
                        UpdateLicenseData updateLicenseData = ret.Data;

                        byte[] encrypted = Convert.FromBase64String(updateLicenseData.LicenseKey);

                        // AES復号化を行う
                        RijndaelManaged aes = new RijndaelManaged();
                        aes.BlockSize = 128;
                        aes.KeySize = 128;
                        aes.Padding = PaddingMode.Zeros;
                        aes.Mode = CipherMode.ECB;
                        //aes.Key = encrypted;
                        //aes.Key = Convert.FromBase64String("WCQIbAarS92xCjO5sL1JKcmHPu/DsUAXs5cYjSJ7AEs=");
                        aes.Key = Convert.FromBase64String(licenseDecryptionKey);

                        ICryptoTransform decryptor = aes.CreateDecryptor();

                        byte[] planeText = new byte[encrypted.Length];

                        MemoryStream memoryStream = new MemoryStream(encrypted);
                        CryptoStream cryptStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                        cryptStream.Read(planeText, 0, planeText.Length);
                        string jsonText = System.Text.Encoding.UTF8.GetString(planeText);

                        int lastClosingParenthesis = jsonText.LastIndexOf('}');
                        if (lastClosingParenthesis > 0)
                        {
                            jsonText = jsonText.Substring(0, lastClosingParenthesis + 1);
                        }

                        deviceModeLicenseInfo = this.CreateLicenseInfoFromJsonText(jsonText);
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug(ex.StackTrace);
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

            return deviceModeLicenseInfo;
        }

        public DeviceModeLicenseInfo CreateLicenseInfoFromJsonText(string jsonText)
        {
            DeviceModeLicenseInfo deviceModeLicenseInfo = new DeviceModeLicenseInfo();

            LETSLicenseKey letsLicenseKey = new LETSLicenseKey();
            letsLicenseKey = JsonConvert.DeserializeObject<LETSLicenseKey>(jsonText);

            deviceModeLicenseInfo.DeviceModeLicenceList = new List<DeviceModeLicense>();
            foreach (var letsLicense in letsLicenseKey.Licenses)
            {
                DeviceModeLicense deviceModeLicense = new DeviceModeLicense();
                deviceModeLicense.LetsKind = (int)letsLicense.LetsTypeId;
                deviceModeLicense.LetsKindName = letsLicense.LetsTypeName;
                deviceModeLicense.ExpireDate = letsLicense.ExpirationDate;
                deviceModeLicenseInfo.DeviceModeLicenceList.Add(deviceModeLicense);
            }
            deviceModeLicenseInfo.ZipPassword = letsLicenseKey.ZipPassword;

            return deviceModeLicenseInfo;

        }

        /// <summary>
        /// 契約情報取得の呼び出し
        /// </summary>
        private void CallUpdateLicenseAPI()
        {
            Logger.Debug("DeviceModeLisenceInfoAPIRepository:CallAuthenticateAccountApi Enter");
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.WebProxy = this.APIConfiguration.GetWebProxy(this.BasePath);
            ContractApi apiInstance = new ContractApi(config);
            var body = new InlineObjectUpdateLicense();
            InlineObjectUpdateLicense inlineObjectUpdateLicense = new InlineObjectUpdateLicense();
            inlineObjectUpdateLicense.OfflineDeviceId = (string)this.ApiParam[APIParam.OfflineDeviceId];
            inlineObjectUpdateLicense.IndefiniteAccessToken = (string)this.ApiParam[APIParam.IndefiniteAccessToken];
            Logger.Debug("DeviceModeLisenceInfoAPIRepository:CallAuthenticateAccountApi apiInstance.AuthenticateAccount:Before");
            this.ApiResponse = apiInstance.UpdateLicense((string)this.ApiParam[APIParam.UserAgent], inlineObjectUpdateLicense);
            Logger.Debug("DeviceModeLisenceInfoAPIRepository:CallAuthenticateAccountApi apiInstance.AuthenticateAccount:After");
        }

        [DataContract]
        internal class LETSLicenseKey
        {
            public LETSLicenseKey()
            {
                this.DeviceId = string.Empty;
                this.Licenses = null;
                this.ZipPassword = string.Empty;
            }

            public LETSLicenseKey(string deviceId = default(string), List<LETSLicense> licenses = default(List<LETSLicense>), string zipPassword = default(string))
            {
                this.DeviceId = deviceId;
                this.Licenses = licenses;
                this.ZipPassword = zipPassword;
            }

            [DataMember(Name = "device_id", EmitDefaultValue = false)]
            public string DeviceId { get; set; }
            [DataMember(Name = "licenses", EmitDefaultValue = false)]
            public List<LETSLicense> Licenses { get; set; }
            [DataMember(Name = "zip_password", EmitDefaultValue = false)]
            public string ZipPassword { get; set; }
        }

        [DataContract]
        internal class LETSLicense
        {
            public LETSLicense()
            {
                this.LetsTypeId = 0;
                this.LetsTypeName = string.Empty;
                this.ExpirationDate = DateTime.Now;
            }

            public LETSLicense(int letsTypeId = default(int), string letsTypeName = default(string), DateTime expirationDate = default(DateTime))
            {
                this.LetsTypeId = letsTypeId;
                this.LetsTypeName = letsTypeName;
                this.ExpirationDate = expirationDate;
            }

            [DataMember(Name = "lets_type_id", EmitDefaultValue = false)]
            public int LetsTypeId { get; set; }
            [DataMember(Name = "lets_type_name", EmitDefaultValue = false)]
            public string LetsTypeName { get; set; }
            [DataMember(Name = "expiration_date", EmitDefaultValue = false)]
            public DateTime ExpirationDate { get; set; }
        }
    }
}
