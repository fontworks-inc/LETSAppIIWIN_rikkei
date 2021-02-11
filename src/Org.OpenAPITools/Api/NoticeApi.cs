/* 
 * フォント配信サービス
 *
 * フォント配信サービスのインタフェース仕様です。
 *
 * The version of the OpenAPI document: 1.0.0
 * 
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;
using System;

namespace Org.OpenAPITools.Api
{

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface INoticeApiSync : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// お知らせ情報取得API
        /// </summary>
        /// <remarks>
        /// ユーザの未読（&#x3D;会員サイトのお知らせ一覧画面にて表示されてない）お知らせの有無と件数を返却する。 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="userAgent">LETS/{アプリバージョン} ({OSタイプ} {OSバージョン})</param>
        /// <returns>Object</returns>
        Object GetNotice(string X_LETS_DEVICEID, string userAgent);

        /// <summary>
        /// お知らせ情報取得API
        /// </summary>
        /// <remarks>
        /// ユーザの未読（&#x3D;会員サイトのお知らせ一覧画面にて表示されてない）お知らせの有無と件数を返却する。 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="userAgent">LETS/{アプリバージョン} ({OSタイプ} {OSバージョン})</param>
        /// <returns>ApiResponse of Object</returns>
        ApiResponse<Object> GetNoticeWithHttpInfo(string X_LETS_DEVICEID, string userAgent);
        /// <summary>
        /// お知らせ画面URLの取得API
        /// </summary>
        /// <remarks>
        /// 会員サイトの「MYP_01_02 お知らせ画面」のURLを返却する。 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="userAgent">LETS/{アプリバージョン} ({OSタイプ} {OSバージョン})</param>
        /// <returns>UrlResponse</returns>
        UrlResponse GetNoticeUrl(string X_LETS_DEVICEID, string userAgent);

        /// <summary>
        /// お知らせ画面URLの取得API
        /// </summary>
        /// <remarks>
        /// 会員サイトの「MYP_01_02 お知らせ画面」のURLを返却する。 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="userAgent">LETS/{アプリバージョン} ({OSタイプ} {OSバージョン})</param>
        /// <returns>ApiResponse of UrlResponse</returns>
        ApiResponse<UrlResponse> GetNoticeUrlWithHttpInfo(string X_LETS_DEVICEID, string userAgent);
        #endregion Synchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface INoticeApiAsync : IApiAccessor
    {
        #region Asynchronous Operations
        /// <summary>
        /// お知らせ情報取得API
        /// </summary>
        /// <remarks>
        /// ユーザの未読（&#x3D;会員サイトのお知らせ一覧画面にて表示されてない）お知らせの有無と件数を返却する。 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="userAgent">LETS/{アプリバージョン} ({OSタイプ} {OSバージョン})</param>
        /// <returns>Task of Object</returns>
        System.Threading.Tasks.Task<Object> GetNoticeAsync(string X_LETS_DEVICEID, string userAgent);

        /// <summary>
        /// お知らせ情報取得API
        /// </summary>
        /// <remarks>
        /// ユーザの未読（&#x3D;会員サイトのお知らせ一覧画面にて表示されてない）お知らせの有無と件数を返却する。 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="userAgent">LETS/{アプリバージョン} ({OSタイプ} {OSバージョン})</param>
        /// <returns>Task of ApiResponse (Object)</returns>
        System.Threading.Tasks.Task<ApiResponse<Object>> GetNoticeAsyncWithHttpInfo(string X_LETS_DEVICEID, string userAgent);
        /// <summary>
        /// お知らせ画面URLの取得API
        /// </summary>
        /// <remarks>
        /// 会員サイトの「MYP_01_02 お知らせ画面」のURLを返却する。 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="userAgent">LETS/{アプリバージョン} ({OSタイプ} {OSバージョン})</param>
        /// <returns>Task of UrlResponse</returns>
        System.Threading.Tasks.Task<UrlResponse> GetNoticeUrlAsync(string X_LETS_DEVICEID, string userAgent);

        /// <summary>
        /// お知らせ画面URLの取得API
        /// </summary>
        /// <remarks>
        /// 会員サイトの「MYP_01_02 お知らせ画面」のURLを返却する。 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="userAgent">LETS/{アプリバージョン} ({OSタイプ} {OSバージョン})</param>
        /// <returns>Task of ApiResponse (UrlResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<UrlResponse>> GetNoticeUrlAsyncWithHttpInfo(string X_LETS_DEVICEID, string userAgent);
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface INoticeApi : INoticeApiSync, INoticeApiAsync
    {

    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public partial class NoticeApi : INoticeApi
    {
        private Org.OpenAPITools.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoticeApi"/> class.
        /// </summary>
        /// <returns></returns>
        public NoticeApi() : this((string)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoticeApi"/> class.
        /// </summary>
        /// <returns></returns>
        public NoticeApi(String basePath)
        {
            this.Configuration = Org.OpenAPITools.Client.Configuration.MergeConfigurations(
                Org.OpenAPITools.Client.GlobalConfiguration.Instance,
                new Org.OpenAPITools.Client.Configuration { BasePath = basePath }
            );
            this.Client = new Org.OpenAPITools.Client.ApiClient(this.Configuration.BasePath);
            this.AsynchronousClient = new Org.OpenAPITools.Client.ApiClient(this.Configuration.BasePath);
            this.ExceptionFactory = Org.OpenAPITools.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoticeApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public NoticeApi(Org.OpenAPITools.Client.Configuration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            this.Configuration = Org.OpenAPITools.Client.Configuration.MergeConfigurations(
                Org.OpenAPITools.Client.GlobalConfiguration.Instance,
                configuration
            );
            this.Client = new Org.OpenAPITools.Client.ApiClient(this.Configuration.BasePath);
            this.AsynchronousClient = new Org.OpenAPITools.Client.ApiClient(this.Configuration.BasePath);
            ExceptionFactory = Org.OpenAPITools.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoticeApi"/> class
        /// using a Configuration object and client instance.
        /// </summary>
        /// <param name="client">The client interface for synchronous API access.</param>
        /// <param name="asyncClient">The client interface for asynchronous API access.</param>
        /// <param name="configuration">The configuration object.</param>
        public NoticeApi(Org.OpenAPITools.Client.ISynchronousClient client, Org.OpenAPITools.Client.IAsynchronousClient asyncClient, Org.OpenAPITools.Client.IReadableConfiguration configuration)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (asyncClient == null) throw new ArgumentNullException("asyncClient");
            if (configuration == null) throw new ArgumentNullException("configuration");

            this.Client = client;
            this.AsynchronousClient = asyncClient;
            this.Configuration = configuration;
            this.ExceptionFactory = Org.OpenAPITools.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// The client for accessing this underlying API asynchronously.
        /// </summary>
        public Org.OpenAPITools.Client.IAsynchronousClient AsynchronousClient { get; set; }

        /// <summary>
        /// The client for accessing this underlying API synchronously.
        /// </summary>
        public Org.OpenAPITools.Client.ISynchronousClient Client { get; set; }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public String GetBasePath()
        {
            return this.Configuration.BasePath;
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public Org.OpenAPITools.Client.IReadableConfiguration Configuration { get; set; }

        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public Org.OpenAPITools.Client.ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }
                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        /// <summary>
        /// お知らせ情報取得API ユーザの未読（&#x3D;会員サイトのお知らせ一覧画面にて表示されてない）お知らせの有無と件数を返却する。 
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="userAgent">LETS/{アプリバージョン} ({OSタイプ} {OSバージョン})</param>
        /// <returns>Object</returns>
        public Object GetNotice(string X_LETS_DEVICEID, string userAgent)
        {
            Org.OpenAPITools.Client.ApiResponse<Object> localVarResponse = GetNoticeWithHttpInfo(X_LETS_DEVICEID, userAgent);
            return localVarResponse.Data;
        }

        /// <summary>
        /// お知らせ情報取得API ユーザの未読（&#x3D;会員サイトのお知らせ一覧画面にて表示されてない）お知らせの有無と件数を返却する。 
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="userAgent">LETS/{アプリバージョン} ({OSタイプ} {OSバージョン})</param>
        /// <returns>ApiResponse of Object</returns>
        public Org.OpenAPITools.Client.ApiResponse<Object> GetNoticeWithHttpInfo(string X_LETS_DEVICEID, string userAgent)
        {
            // verify the required parameter 'X_LETS_DEVICEID' is set
            if (X_LETS_DEVICEID == null)
                throw new Org.OpenAPITools.Client.ApiException(400, "Missing required parameter 'X_LETS_DEVICEID' when calling NoticeApi->GetNotice");

            // verify the required parameter 'userAgent' is set
            if (userAgent == null)
                throw new Org.OpenAPITools.Client.ApiException(400, "Missing required parameter 'userAgent' when calling NoticeApi->GetNotice");

            Org.OpenAPITools.Client.RequestOptions localVarRequestOptions = new Org.OpenAPITools.Client.RequestOptions();

            String[] _contentTypes = new String[] {
            };

            // to determine the Accept header
            String[] _accepts = new String[] {
                "application/json; charset=utf-8"
            };

            var localVarContentType = Org.OpenAPITools.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null) localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);

            var localVarAccept = Org.OpenAPITools.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null) localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            localVarRequestOptions.HeaderParameters.Add("X-LETS-DEVICEID", Org.OpenAPITools.Client.ClientUtils.ParameterToString(X_LETS_DEVICEID)); // header parameter
            localVarRequestOptions.HeaderParameters.Add("User-Agent", Org.OpenAPITools.Client.ClientUtils.ParameterToString(userAgent)); // header parameter

            // authentication (BearerAuth) required
            // bearer authentication required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarRequestOptions.HeaderParameters.Add("Authorization", "Bearer " + this.Configuration.AccessToken);
            }

            // make the HTTP request
            var localVarResponse = this.Client.Get<Object>("/api/v1/notice", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("GetNotice", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        /// お知らせ情報取得API ユーザの未読（&#x3D;会員サイトのお知らせ一覧画面にて表示されてない）お知らせの有無と件数を返却する。 
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="userAgent">LETS/{アプリバージョン} ({OSタイプ} {OSバージョン})</param>
        /// <returns>Task of Object</returns>
        public async System.Threading.Tasks.Task<Object> GetNoticeAsync(string X_LETS_DEVICEID, string userAgent)
        {
            Org.OpenAPITools.Client.ApiResponse<Object> localVarResponse = await GetNoticeAsyncWithHttpInfo(X_LETS_DEVICEID, userAgent);
            return localVarResponse.Data;

        }

        /// <summary>
        /// お知らせ情報取得API ユーザの未読（&#x3D;会員サイトのお知らせ一覧画面にて表示されてない）お知らせの有無と件数を返却する。 
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="userAgent">LETS/{アプリバージョン} ({OSタイプ} {OSバージョン})</param>
        /// <returns>Task of ApiResponse (Object)</returns>
        public async System.Threading.Tasks.Task<Org.OpenAPITools.Client.ApiResponse<Object>> GetNoticeAsyncWithHttpInfo(string X_LETS_DEVICEID, string userAgent)
        {
            // verify the required parameter 'X_LETS_DEVICEID' is set
            if (X_LETS_DEVICEID == null)
                throw new Org.OpenAPITools.Client.ApiException(400, "Missing required parameter 'X_LETS_DEVICEID' when calling NoticeApi->GetNotice");

            // verify the required parameter 'userAgent' is set
            if (userAgent == null)
                throw new Org.OpenAPITools.Client.ApiException(400, "Missing required parameter 'userAgent' when calling NoticeApi->GetNotice");


            Org.OpenAPITools.Client.RequestOptions localVarRequestOptions = new Org.OpenAPITools.Client.RequestOptions();

            String[] _contentTypes = new String[] {
            };

            // to determine the Accept header
            String[] _accepts = new String[] {
                "application/json; charset=utf-8"
            };

            foreach (var _contentType in _contentTypes)
                localVarRequestOptions.HeaderParameters.Add("Content-Type", _contentType);

            foreach (var _accept in _accepts)
                localVarRequestOptions.HeaderParameters.Add("Accept", _accept);

            localVarRequestOptions.HeaderParameters.Add("X-LETS-DEVICEID", Org.OpenAPITools.Client.ClientUtils.ParameterToString(X_LETS_DEVICEID)); // header parameter
            localVarRequestOptions.HeaderParameters.Add("User-Agent", Org.OpenAPITools.Client.ClientUtils.ParameterToString(userAgent)); // header parameter

            // authentication (BearerAuth) required
            // bearer authentication required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarRequestOptions.HeaderParameters.Add("Authorization", "Bearer " + this.Configuration.AccessToken);
            }

            // make the HTTP request

            var localVarResponse = await this.AsynchronousClient.GetAsync<Object>("/api/v1/notice", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("GetNotice", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        /// お知らせ画面URLの取得API 会員サイトの「MYP_01_02 お知らせ画面」のURLを返却する。 
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="userAgent">LETS/{アプリバージョン} ({OSタイプ} {OSバージョン})</param>
        /// <returns>UrlResponse</returns>
        public UrlResponse GetNoticeUrl(string X_LETS_DEVICEID, string userAgent)
        {
            Org.OpenAPITools.Client.ApiResponse<UrlResponse> localVarResponse = GetNoticeUrlWithHttpInfo(X_LETS_DEVICEID, userAgent);
            return localVarResponse.Data;
        }

        /// <summary>
        /// お知らせ画面URLの取得API 会員サイトの「MYP_01_02 お知らせ画面」のURLを返却する。 
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="userAgent">LETS/{アプリバージョン} ({OSタイプ} {OSバージョン})</param>
        /// <returns>ApiResponse of UrlResponse</returns>
        public Org.OpenAPITools.Client.ApiResponse<UrlResponse> GetNoticeUrlWithHttpInfo(string X_LETS_DEVICEID, string userAgent)
        {
            // verify the required parameter 'X_LETS_DEVICEID' is set
            if (X_LETS_DEVICEID == null)
                throw new Org.OpenAPITools.Client.ApiException(400, "Missing required parameter 'X_LETS_DEVICEID' when calling NoticeApi->GetNoticeUrl");

            // verify the required parameter 'userAgent' is set
            if (userAgent == null)
                throw new Org.OpenAPITools.Client.ApiException(400, "Missing required parameter 'userAgent' when calling NoticeApi->GetNoticeUrl");

            Org.OpenAPITools.Client.RequestOptions localVarRequestOptions = new Org.OpenAPITools.Client.RequestOptions();

            String[] _contentTypes = new String[] {
            };

            // to determine the Accept header
            String[] _accepts = new String[] {
                "application/json; charset=utf-8"
            };

            var localVarContentType = Org.OpenAPITools.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null) localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);

            var localVarAccept = Org.OpenAPITools.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null) localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            localVarRequestOptions.HeaderParameters.Add("X-LETS-DEVICEID", Org.OpenAPITools.Client.ClientUtils.ParameterToString(X_LETS_DEVICEID)); // header parameter
            localVarRequestOptions.HeaderParameters.Add("User-Agent", Org.OpenAPITools.Client.ClientUtils.ParameterToString(userAgent)); // header parameter

            // authentication (BearerAuth) required
            // bearer authentication required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarRequestOptions.HeaderParameters.Add("Authorization", "Bearer " + this.Configuration.AccessToken);
            }

            // make the HTTP request
            var localVarResponse = this.Client.Get<UrlResponse>("/api/v1/screens/notice/url", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("GetNoticeUrl", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        /// お知らせ画面URLの取得API 会員サイトの「MYP_01_02 お知らせ画面」のURLを返却する。 
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="userAgent">LETS/{アプリバージョン} ({OSタイプ} {OSバージョン})</param>
        /// <returns>Task of UrlResponse</returns>
        public async System.Threading.Tasks.Task<UrlResponse> GetNoticeUrlAsync(string X_LETS_DEVICEID, string userAgent)
        {
            Org.OpenAPITools.Client.ApiResponse<UrlResponse> localVarResponse = await GetNoticeUrlAsyncWithHttpInfo(X_LETS_DEVICEID, userAgent);
            return localVarResponse.Data;

        }

        /// <summary>
        /// お知らせ画面URLの取得API 会員サイトの「MYP_01_02 お知らせ画面」のURLを返却する。 
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="userAgent">LETS/{アプリバージョン} ({OSタイプ} {OSバージョン})</param>
        /// <returns>Task of ApiResponse (UrlResponse)</returns>
        public async System.Threading.Tasks.Task<Org.OpenAPITools.Client.ApiResponse<UrlResponse>> GetNoticeUrlAsyncWithHttpInfo(string X_LETS_DEVICEID, string userAgent)
        {
            // verify the required parameter 'X_LETS_DEVICEID' is set
            if (X_LETS_DEVICEID == null)
                throw new Org.OpenAPITools.Client.ApiException(400, "Missing required parameter 'X_LETS_DEVICEID' when calling NoticeApi->GetNoticeUrl");

            // verify the required parameter 'userAgent' is set
            if (userAgent == null)
                throw new Org.OpenAPITools.Client.ApiException(400, "Missing required parameter 'userAgent' when calling NoticeApi->GetNoticeUrl");


            Org.OpenAPITools.Client.RequestOptions localVarRequestOptions = new Org.OpenAPITools.Client.RequestOptions();

            String[] _contentTypes = new String[] {
            };

            // to determine the Accept header
            String[] _accepts = new String[] {
                "application/json; charset=utf-8"
            };

            foreach (var _contentType in _contentTypes)
                localVarRequestOptions.HeaderParameters.Add("Content-Type", _contentType);

            foreach (var _accept in _accepts)
                localVarRequestOptions.HeaderParameters.Add("Accept", _accept);

            localVarRequestOptions.HeaderParameters.Add("X-LETS-DEVICEID", Org.OpenAPITools.Client.ClientUtils.ParameterToString(X_LETS_DEVICEID)); // header parameter
            localVarRequestOptions.HeaderParameters.Add("User-Agent", Org.OpenAPITools.Client.ClientUtils.ParameterToString(userAgent)); // header parameter

            // authentication (BearerAuth) required
            // bearer authentication required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarRequestOptions.HeaderParameters.Add("Authorization", "Bearer " + this.Configuration.AccessToken);
            }

            // make the HTTP request

            var localVarResponse = await this.AsynchronousClient.GetAsync<UrlResponse>("/api/v1/screens/notice/url", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("GetNoticeUrl", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

    }
}
