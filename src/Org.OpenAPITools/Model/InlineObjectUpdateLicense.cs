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
    /// InlineObject
    /// </summary>
    [DataContract]
    public partial class InlineObjectUpdateLicense : IEquatable<InlineObjectUpdateLicense>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InlineObjectUpdateLicense" /> class.
        /// </summary>
        /// <param name="offlineDeviceId">mailAddress.</param>
        /// <param name="indefiniteAccessToken">password.</param>
        public InlineObjectUpdateLicense(string offlineDeviceId = default(string), string indefiniteAccessToken = default(string), string hostname = default(string), string osUserName = default(string))
        {
            this.OfflineDeviceId = offlineDeviceId;
            this.IndefiniteAccessToken = indefiniteAccessToken;
            this.Hostname = hostname;
            this.OsUserName = osUserName;
        }

        /// <summary>
        /// Gets or Sets MailAddress
        /// </summary>
        [DataMember(Name = "offlineDeviceId", EmitDefaultValue = false)]
        public string OfflineDeviceId { get; set; }

        /// <summary>
        /// Gets or Sets Password
        /// </summary>
        [DataMember(Name = "indefiniteAccessToken", EmitDefaultValue = false)]
        public string IndefiniteAccessToken { get; set; }

        /// <summary>
        /// Gets or Sets hostname
        /// </summary>
        [DataMember(Name = "hostname", EmitDefaultValue = false)]
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or Sets osUserName
        /// </summary>
        [DataMember(Name = "osUserName", EmitDefaultValue = false)]
        public string OsUserName { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class InlineObject {\n");
            sb.Append("  OfflineDeviceId: ").Append(OfflineDeviceId).Append("\n");
            sb.Append("  IndefiniteAccessToken: ").Append(IndefiniteAccessToken).Append("\n");
            sb.Append("  Hostname: ").Append(Hostname).Append("\n");
            sb.Append("  OsUserName: ").Append(OsUserName).Append("\n");
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
            return this.Equals(input as InlineObjectUpdateLicense);
        }

        /// <summary>
        /// Returns true if InlineObject instances are equal
        /// </summary>
        /// <param name="input">Instance of InlineObject to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(InlineObjectUpdateLicense input)
        {
            if (input == null)
                return false;

            return
                (
                    this.OfflineDeviceId == input.OfflineDeviceId ||
                    (this.OfflineDeviceId != null &&
                    this.OfflineDeviceId.Equals(input.OfflineDeviceId))
                ) &&
                (
                    this.IndefiniteAccessToken == input.IndefiniteAccessToken ||
                    (this.IndefiniteAccessToken != null &&
                    this.IndefiniteAccessToken.Equals(input.IndefiniteAccessToken))
                ) &&
                (
                    this.Hostname == input.Hostname ||
                    (this.Hostname != null &&
                    this.Hostname.Equals(input.Hostname))
                ) &&
                (
                    this.OsUserName == input.OsUserName ||
                    (this.OsUserName != null &&
                    this.OsUserName.Equals(input.OsUserName))
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
                if (this.OfflineDeviceId != null)
                    hashCode = hashCode * 59 + this.OfflineDeviceId.GetHashCode();
                if (this.IndefiniteAccessToken != null)
                    hashCode = hashCode * 59 + this.IndefiniteAccessToken.GetHashCode();
                if (this.Hostname != null)
                    hashCode = hashCode * 59 + this.Hostname.GetHashCode();
                if (this.OsUserName != null)
                    hashCode = hashCode * 59 + this.OsUserName.GetHashCode();
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
