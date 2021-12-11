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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Org.OpenAPITools.Model
{
    /// <summary>
    /// AuthenticationAccountData
    /// </summary>
    [DataContract]
    public partial class AuthenticateAccountData : IEquatable<AuthenticateAccountData>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenRefreshTokenData" /> class.
        /// </summary>
        /// <param name="groupType">groupType.</param>
        /// <param name="offlineDeviceId">offlineDeviceId.</param>
        /// <param name="licenseDecryptionKey">licenseDecryptionKey.</param>
        /// <param name="indefiniteAccessToken">indefiniteAccessToken.</param>
        public AuthenticateAccountData(int groupType = default(int), string offlineDeviceId = default(string), string licenseDecryptionKey = default(string), string indefiniteAccessToken = default(string))
        {
            this.GroupType = groupType;
            this.OfflineDeviceId = offlineDeviceId;
            this.LicenseDecryptionKey = licenseDecryptionKey;
            this.IndefiniteAccessToken = indefiniteAccessToken;
        }

        /// <summary>
        /// Gets or Sets GroupType
        /// </summary>
        [DataMember(Name = "groupType", EmitDefaultValue = false)]
        public int GroupType { get; set; }

        /// <summary>
        /// Gets or Sets OfflineDeviceId
        /// </summary>
        [DataMember(Name = "offlineDeviceId", EmitDefaultValue = false)]
        public string OfflineDeviceId { get; set; }

        /// <summary>
        /// Gets or Sets LicenseDecryptionKey
        /// </summary>
        [DataMember(Name = "licenseDecryptionKey", EmitDefaultValue = false)]
        public string LicenseDecryptionKey { get; set; }

        /// <summary>
        /// Gets or Sets IndefiniteAccessToken
        /// </summary>
        [DataMember(Name = "indefiniteAccessToken", EmitDefaultValue = false)]
        public string IndefiniteAccessToken { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class AccessTokenRefreshTokenData {\n");
            sb.Append("  GroupType: ").Append(GroupType).Append("\n");
            sb.Append("  OfflineDeviceId: ").Append(OfflineDeviceId).Append("\n");
            sb.Append("  LicenseDecryptionKey: ").Append(LicenseDecryptionKey).Append("\n");
            sb.Append("  IndefiniteAccessToken: ").Append(IndefiniteAccessToken).Append("\n");
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
            return this.Equals(input as AuthenticateAccountData);
        }

        /// <summary>
        /// Returns true if AccessTokenRefreshTokenData instances are equal
        /// </summary>
        /// <param name="input">Instance of AccessTokenRefreshTokenData to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(AuthenticateAccountData input)
        {
            if (input == null)
                return false;

            return
                (
                    this.GroupType == input.GroupType
                ) &&
                (
                    this.OfflineDeviceId == input.OfflineDeviceId ||
                    (this.OfflineDeviceId != null &&
                    this.OfflineDeviceId.Equals(input.OfflineDeviceId))
                ) &&
                (
                    this.LicenseDecryptionKey == input.LicenseDecryptionKey ||
                    (this.LicenseDecryptionKey != null &&
                    this.LicenseDecryptionKey.Equals(input.LicenseDecryptionKey))
                ) &&
                (
                    this.IndefiniteAccessToken == input.IndefiniteAccessToken ||
                    (this.IndefiniteAccessToken != null &&
                    this.IndefiniteAccessToken.Equals(input.IndefiniteAccessToken))
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
                hashCode = hashCode * 59 + this.GroupType.GetHashCode();
                if (this.OfflineDeviceId != null)
                    hashCode = hashCode * 59 + this.OfflineDeviceId.GetHashCode();
                if (this.LicenseDecryptionKey != null)
                    hashCode = hashCode * 59 + this.LicenseDecryptionKey.GetHashCode();
                if (this.IndefiniteAccessToken != null)
                    hashCode = hashCode * 59 + this.IndefiniteAccessToken.GetHashCode();
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
