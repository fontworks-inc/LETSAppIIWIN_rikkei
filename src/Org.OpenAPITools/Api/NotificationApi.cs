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
using System;

namespace Org.OpenAPITools.Api
{

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface INotificationApiSync : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// クライアントアプリ通知API
        /// </summary>
        /// <remarks>
        /// 本IFはServerSentEventプロトコルにて実装されることを想定している。 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="connection"></param>
        /// <param name="cacheControl"></param>
        /// <param name="lastEventID"> (optional)</param>
        /// <returns>string</returns>
        string Notification(string X_LETS_DEVICEID, string connection, string cacheControl, int? lastEventID = default(int?));

        /// <summary>
        /// クライアントアプリ通知API
        /// </summary>
        /// <remarks>
        /// 本IFはServerSentEventプロトコルにて実装されることを想定している。 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="connection"></param>
        /// <param name="cacheControl"></param>
        /// <param name="lastEventID"> (optional)</param>
        /// <returns>ApiResponse of string</returns>
        ApiResponse<string> NotificationWithHttpInfo(string X_LETS_DEVICEID, string connection, string cacheControl, int? lastEventID = default(int?));
        #endregion Synchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface INotificationApiAsync : IApiAccessor
    {
        #region Asynchronous Operations
        /// <summary>
        /// クライアントアプリ通知API
        /// </summary>
        /// <remarks>
        /// 本IFはServerSentEventプロトコルにて実装されることを想定している。 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="connection"></param>
        /// <param name="cacheControl"></param>
        /// <param name="lastEventID"> (optional)</param>
        /// <returns>Task of string</returns>
        System.Threading.Tasks.Task<string> NotificationAsync(string X_LETS_DEVICEID, string connection, string cacheControl, int? lastEventID = default(int?));

        /// <summary>
        /// クライアントアプリ通知API
        /// </summary>
        /// <remarks>
        /// 本IFはServerSentEventプロトコルにて実装されることを想定している。 
        /// </remarks>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="connection"></param>
        /// <param name="cacheControl"></param>
        /// <param name="lastEventID"> (optional)</param>
        /// <returns>Task of ApiResponse (string)</returns>
        System.Threading.Tasks.Task<ApiResponse<string>> NotificationAsyncWithHttpInfo(string X_LETS_DEVICEID, string connection, string cacheControl, int? lastEventID = default(int?));
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface INotificationApi : INotificationApiSync, INotificationApiAsync
    {

    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public partial class NotificationApi : INotificationApi
    {
        private Org.OpenAPITools.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationApi"/> class.
        /// </summary>
        /// <returns></returns>
        public NotificationApi() : this((string)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationApi"/> class.
        /// </summary>
        /// <returns></returns>
        public NotificationApi(String basePath)
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
        /// Initializes a new instance of the <see cref="NotificationApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public NotificationApi(Org.OpenAPITools.Client.Configuration configuration)
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
        /// Initializes a new instance of the <see cref="NotificationApi"/> class
        /// using a Configuration object and client instance.
        /// </summary>
        /// <param name="client">The client interface for synchronous API access.</param>
        /// <param name="asyncClient">The client interface for asynchronous API access.</param>
        /// <param name="configuration">The configuration object.</param>
        public NotificationApi(Org.OpenAPITools.Client.ISynchronousClient client, Org.OpenAPITools.Client.IAsynchronousClient asyncClient, Org.OpenAPITools.Client.IReadableConfiguration configuration)
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
        /// クライアントアプリ通知API 本IFはServerSentEventプロトコルにて実装されることを想定している。 
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="connection"></param>
        /// <param name="cacheControl"></param>
        /// <param name="lastEventID"> (optional)</param>
        /// <returns>string</returns>
        public string Notification(string X_LETS_DEVICEID, string connection, string cacheControl, int? lastEventID = default(int?))
        {
            Org.OpenAPITools.Client.ApiResponse<string> localVarResponse = NotificationWithHttpInfo(X_LETS_DEVICEID, connection, cacheControl, lastEventID);
            return localVarResponse.Data;
        }

        /// <summary>
        /// クライアントアプリ通知API 本IFはServerSentEventプロトコルにて実装されることを想定している。 
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="connection"></param>
        /// <param name="cacheControl"></param>
        /// <param name="lastEventID"> (optional)</param>
        /// <returns>ApiResponse of string</returns>
        public Org.OpenAPITools.Client.ApiResponse<string> NotificationWithHttpInfo(string X_LETS_DEVICEID, string connection, string cacheControl, int? lastEventID = default(int?))
        {
            // verify the required parameter 'X_LETS_DEVICEID' is set
            if (X_LETS_DEVICEID == null)
                throw new Org.OpenAPITools.Client.ApiException(400, "Missing required parameter 'X_LETS_DEVICEID' when calling NotificationApi->Notification");

            // verify the required parameter 'connection' is set
            if (connection == null)
                throw new Org.OpenAPITools.Client.ApiException(400, "Missing required parameter 'connection' when calling NotificationApi->Notification");

            // verify the required parameter 'cacheControl' is set
            if (cacheControl == null)
                throw new Org.OpenAPITools.Client.ApiException(400, "Missing required parameter 'cacheControl' when calling NotificationApi->Notification");

            Org.OpenAPITools.Client.RequestOptions localVarRequestOptions = new Org.OpenAPITools.Client.RequestOptions();

            String[] _contentTypes = new String[] {
            };

            // to determine the Accept header
            String[] _accepts = new String[] {
                "text/event-stream; charset=utf-8",
                "application/json; charset=utf-8"
            };

            var localVarContentType = Org.OpenAPITools.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null) localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);

            var localVarAccept = Org.OpenAPITools.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null) localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            localVarRequestOptions.HeaderParameters.Add("X-LETS-DEVICEID", Org.OpenAPITools.Client.ClientUtils.ParameterToString(X_LETS_DEVICEID)); // header parameter
            localVarRequestOptions.HeaderParameters.Add("Connection", Org.OpenAPITools.Client.ClientUtils.ParameterToString(connection)); // header parameter
            localVarRequestOptions.HeaderParameters.Add("Cache-Control", Org.OpenAPITools.Client.ClientUtils.ParameterToString(cacheControl)); // header parameter
            if (lastEventID != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Last-Event-ID", Org.OpenAPITools.Client.ClientUtils.ParameterToString(lastEventID)); // header parameter
            }

            // authentication (BearerAuth) required
            // bearer authentication required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarRequestOptions.HeaderParameters.Add("Authorization", "Bearer " + this.Configuration.AccessToken);
            }

            // make the HTTP request
            var localVarResponse = this.Client.Get<string>("/notifications", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("Notification", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        /// クライアントアプリ通知API 本IFはServerSentEventプロトコルにて実装されることを想定している。 
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="connection"></param>
        /// <param name="cacheControl"></param>
        /// <param name="lastEventID"> (optional)</param>
        /// <returns>Task of string</returns>
        public async System.Threading.Tasks.Task<string> NotificationAsync(string X_LETS_DEVICEID, string connection, string cacheControl, int? lastEventID = default(int?))
        {
            Org.OpenAPITools.Client.ApiResponse<string> localVarResponse = await NotificationAsyncWithHttpInfo(X_LETS_DEVICEID, connection, cacheControl, lastEventID);
            return localVarResponse.Data;

        }

        /// <summary>
        /// クライアントアプリ通知API 本IFはServerSentEventプロトコルにて実装されることを想定している。 
        /// </summary>
        /// <exception cref="Org.OpenAPITools.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="X_LETS_DEVICEID">デバイス固有のIDを設定する。</param>
        /// <param name="connection"></param>
        /// <param name="cacheControl"></param>
        /// <param name="lastEventID"> (optional)</param>
        /// <returns>Task of ApiResponse (string)</returns>
        public async System.Threading.Tasks.Task<Org.OpenAPITools.Client.ApiResponse<string>> NotificationAsyncWithHttpInfo(string X_LETS_DEVICEID, string connection, string cacheControl, int? lastEventID = default(int?))
        {
            // verify the required parameter 'X_LETS_DEVICEID' is set
            if (X_LETS_DEVICEID == null)
                throw new Org.OpenAPITools.Client.ApiException(400, "Missing required parameter 'X_LETS_DEVICEID' when calling NotificationApi->Notification");

            // verify the required parameter 'connection' is set
            if (connection == null)
                throw new Org.OpenAPITools.Client.ApiException(400, "Missing required parameter 'connection' when calling NotificationApi->Notification");

            // verify the required parameter 'cacheControl' is set
            if (cacheControl == null)
                throw new Org.OpenAPITools.Client.ApiException(400, "Missing required parameter 'cacheControl' when calling NotificationApi->Notification");


            Org.OpenAPITools.Client.RequestOptions localVarRequestOptions = new Org.OpenAPITools.Client.RequestOptions();

            String[] _contentTypes = new String[] {
            };

            // to determine the Accept header
            String[] _accepts = new String[] {
                "text/event-stream; charset=utf-8",
                "application/json; charset=utf-8"
            };

            foreach (var _contentType in _contentTypes)
                localVarRequestOptions.HeaderParameters.Add("Content-Type", _contentType);

            foreach (var _accept in _accepts)
                localVarRequestOptions.HeaderParameters.Add("Accept", _accept);

            localVarRequestOptions.HeaderParameters.Add("X-LETS-DEVICEID", Org.OpenAPITools.Client.ClientUtils.ParameterToString(X_LETS_DEVICEID)); // header parameter
            localVarRequestOptions.HeaderParameters.Add("Connection", Org.OpenAPITools.Client.ClientUtils.ParameterToString(connection)); // header parameter
            localVarRequestOptions.HeaderParameters.Add("Cache-Control", Org.OpenAPITools.Client.ClientUtils.ParameterToString(cacheControl)); // header parameter
            if (lastEventID != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Last-Event-ID", Org.OpenAPITools.Client.ClientUtils.ParameterToString(lastEventID)); // header parameter
            }

            // authentication (BearerAuth) required
            // bearer authentication required
            if (!String.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                localVarRequestOptions.HeaderParameters.Add("Authorization", "Bearer " + this.Configuration.AccessToken);
            }

            // make the HTTP request

            var localVarResponse = await this.AsynchronousClient.GetAsync<string>("/notifications", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("Notification", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

    }
}
