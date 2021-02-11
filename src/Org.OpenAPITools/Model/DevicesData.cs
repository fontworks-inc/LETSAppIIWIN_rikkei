/* 
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
    /// DevicesData
    /// </summary>
    [DataContract]
    public partial class DevicesData : IEquatable<DevicesData>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DevicesData" /> class.
        /// </summary>
        /// <param name="userDeviceId">userDeviceId.</param>
        /// <param name="deviceId">deviceId.</param>
        /// <param name="hostname">hostname.</param>
        /// <param name="osUserName">osUserName.</param>
        /// <param name="osType">osType.</param>
        /// <param name="osVersion">osVersion.</param>
        public DevicesData(string userDeviceId = default(string), string deviceId = default(string), string hostname = default(string), string osUserName = default(string), string osType = default(string), string osVersion = default(string))
        {
            this.UserDeviceId = userDeviceId;
            this.DeviceId = deviceId;
            this.Hostname = hostname;
            this.OsUserName = osUserName;
            this.OsType = osType;
            this.OsVersion = osVersion;
        }

        /// <summary>
        /// Gets or Sets UserDeviceId
        /// </summary>
        [DataMember(Name = "userDeviceId", EmitDefaultValue = false)]
        public string UserDeviceId { get; set; }

        /// <summary>
        /// Gets or Sets DeviceId
        /// </summary>
        [DataMember(Name = "deviceId", EmitDefaultValue = false)]
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets or Sets Hostname
        /// </summary>
        [DataMember(Name = "hostname", EmitDefaultValue = false)]
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or Sets OsUserName
        /// </summary>
        [DataMember(Name = "osUserName", EmitDefaultValue = false)]
        public string OsUserName { get; set; }

        /// <summary>
        /// Gets or Sets OsType
        /// </summary>
        [DataMember(Name = "osType", EmitDefaultValue = false)]
        public string OsType { get; set; }

        /// <summary>
        /// Gets or Sets OsVersion
        /// </summary>
        [DataMember(Name = "osVersion", EmitDefaultValue = false)]
        public string OsVersion { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class DevicesData {\n");
            sb.Append("  UserDeviceId: ").Append(UserDeviceId).Append("\n");
            sb.Append("  DeviceId: ").Append(DeviceId).Append("\n");
            sb.Append("  Hostname: ").Append(Hostname).Append("\n");
            sb.Append("  OsUserName: ").Append(OsUserName).Append("\n");
            sb.Append("  OsType: ").Append(OsType).Append("\n");
            sb.Append("  OsVersion: ").Append(OsVersion).Append("\n");
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
            return this.Equals(input as DevicesData);
        }

        /// <summary>
        /// Returns true if DevicesData instances are equal
        /// </summary>
        /// <param name="input">Instance of DevicesData to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(DevicesData input)
        {
            if (input == null)
                return false;

            return
                (
                    this.UserDeviceId == input.UserDeviceId ||
                    (this.UserDeviceId != null &&
                    this.UserDeviceId.Equals(input.UserDeviceId))
                ) &&
                (
                    this.DeviceId == input.DeviceId ||
                    (this.DeviceId != null &&
                    this.DeviceId.Equals(input.DeviceId))
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
                ) &&
                (
                    this.OsType == input.OsType ||
                    (this.OsType != null &&
                    this.OsType.Equals(input.OsType))
                ) &&
                (
                    this.OsVersion == input.OsVersion ||
                    (this.OsVersion != null &&
                    this.OsVersion.Equals(input.OsVersion))
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
                if (this.UserDeviceId != null)
                    hashCode = hashCode * 59 + this.UserDeviceId.GetHashCode();
                if (this.DeviceId != null)
                    hashCode = hashCode * 59 + this.DeviceId.GetHashCode();
                if (this.Hostname != null)
                    hashCode = hashCode * 59 + this.Hostname.GetHashCode();
                if (this.OsUserName != null)
                    hashCode = hashCode * 59 + this.OsUserName.GetHashCode();
                if (this.OsType != null)
                    hashCode = hashCode * 59 + this.OsType.GetHashCode();
                if (this.OsVersion != null)
                    hashCode = hashCode * 59 + this.OsVersion.GetHashCode();
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
