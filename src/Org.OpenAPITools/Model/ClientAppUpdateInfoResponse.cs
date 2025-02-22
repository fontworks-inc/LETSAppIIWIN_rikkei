﻿/* 
 * フォント配信サービス
 *
 * フォント配信サービスのインタフェース仕様です。
 *
 * The version of the OpenAPI document: 1.0.0
 * 
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;


namespace Org.OpenAPITools.Model
{
    /// <summary>
    /// ClientAppUpdateInfoResponse
    /// </summary>
    [DataContract]
    public partial class ClientAppUpdateInfoResponse : IEquatable<ClientAppUpdateInfoResponse>, IValidatableObject
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientAppUpdateInfoResponse" /> class.
        /// </summary>
        public ClientAppUpdateInfoResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientAppUpdateInfoResponse" /> class.
        /// </summary>
        /// <param name="code">code.</param>
        /// <param name="message">message.</param>
        /// <param name="data">data.</param>
        public ClientAppUpdateInfoResponse(int code = default(int), string message = default(string), ClientAppUpdateInfoData data = default(ClientAppUpdateInfoData))
        {
            this.Code = code;
            this.Message = message;
            this.Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientAppUpdateInfoResponse" /> class.
        /// </summary>
        /// <param name="apiResponse">apiResponse.</param>
        public ClientAppUpdateInfoResponse(object apiResponse)
        {
            ClientAppUpdateInfoResponse response =
            JsonConvert.DeserializeObject<ClientAppUpdateInfoResponse>(apiResponse.ToString());
            this.Code = response.Code;
            this.Message = response.Message;
            this.Data = response.Data;
        }

        /// <summary>
        /// Gets or Sets Code
        /// </summary>
        [DataMember(Name = "code", EmitDefaultValue = false)]
        public int Code { get; set; }

        /// <summary>
        /// Gets or Sets Message
        /// </summary>
        [DataMember(Name = "message", EmitDefaultValue = false)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or Sets Data
        /// </summary>
        [DataMember(Name = "data", EmitDefaultValue = false)]
        public ClientAppUpdateInfoData Data { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ClientAppUpdateInfoResponse {\n");
            sb.Append("  Code: ").Append(Code).Append("\n");
            sb.Append("  Message: ").Append(Message).Append("\n");
            sb.Append("  Data: ").Append(Data).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as ClientAppUpdateInfoResponse);
        }

        /// <summary>
        /// Returns true if ClientAppUpdateInfoResponse instances are equal
        /// </summary>
        /// <param name="input">Instance of ClientAppUpdateInfoResponse to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ClientAppUpdateInfoResponse input)
        {
            if (input == null)
                return false;

            return
                (
                    this.Code == input.Code ||
                    this.Code.Equals(input.Code)
                ) &&
                (
                    this.Message == input.Message ||
                    (this.Message != null &&
                    this.Message.Equals(input.Message))
                ) &&
                (
                    this.Data == input.Data ||
                    this.Data != null &&
                    input.Data != null &&
                    this.Data.Equals(input.Data)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                hashCode = hashCode * 59 + this.Code.GetHashCode();
                if (this.Message != null)
                    hashCode = hashCode * 59 + this.Message.GetHashCode();
                if (this.Data != null)
                    hashCode = hashCode * 59 + this.Data.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
