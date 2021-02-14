using System;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Memory;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Infrastructure.API
{
    /// <summary>
    /// お客様情報を扱うAPIリポジトリ
    /// </summary>
    public class CustomerAPIRepository : APIRepositoryBase, ICustomerRepository
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        public CustomerAPIRepository(APIConfiguration apiConfiguration)
            : base(apiConfiguration)
        {
        }

        /// <inheritdoc/>
        public Core.Entities.Customer GetCustomer()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// お客様情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <returns>お客様情報</returns>
        /// <remarks>FUNCTION_08_02_01</remarks>>
        public Core.Entities.Customer GetCustomer(string deviceId)
        {
            Core.Entities.Customer response = new Core.Entities.Customer();
            this.ApiParam[APIParam.DeviceId] = deviceId;

            var memory = new VolatileSettingMemoryRepository().GetVolatileSetting();
            this.ApiParam[APIParam.AccessToken] = memory.AccessToken;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallGetCustomerApi);

                // 戻り値のセット（個別処理）
                var ret = new CustomerResponse(this.ApiResponse);
                if (ret.Code == (int)ResponseCode.Succeeded)
                {
                    response.MailAddress = ret.Data.MailAddress;
                    response.FirstName = ret.Data.FirstName;
                    response.LastName = ret.Data.LastName;
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

        /// <inheritdoc/>
        public void SaveCustomer(Core.Entities.Customer customer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// お客様情報取得の呼び出し
        /// </summary>
        private void CallGetCustomerApi()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.AccessToken = (string)this.ApiParam[APIParam.AccessToken];
            CustomerApi apiInstance = new CustomerApi(config);
            this.ApiResponse = apiInstance.GetCustomer((string)this.ApiParam[APIParam.DeviceId], (string)this.ApiParam[APIParam.UserAgent]);
        }
    }
}
