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
    /// NoticeResponseData
    /// </summary>
    [DataContract]
    public partial class NoticeResponseData : IEquatable<NoticeResponseData>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoticeResponseData" /> class.
        /// </summary>
        /// <param name="total">total.</param>
        /// <param name="existsLatestNotice">existsLatestNotice.</param>
        public NoticeResponseData(int total = default(int), bool existsLatestNotice = default(bool))
        {
            this.Total = total;
            this.ExistsLatestNotice = existsLatestNotice;
        }

        /// <summary>
        /// Gets or Sets total
        /// </summary>
        [DataMember(Name = "total", EmitDefaultValue = false)]
        public int Total { get; set; }

        /// <summary>
        /// Gets or Sets existsLatestNotice
        /// </summary>
        [DataMember(Name = "existsLatestNotice", EmitDefaultValue = false)]
        public bool ExistsLatestNotice { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class NoticeResponseData {\n");
            sb.Append("  Total: ").Append(Total).Append("\n");
            sb.Append("  ExistsLatestNotice: ").Append(ExistsLatestNotice).Append("\n");
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
            return this.Equals(input as NoticeResponseData);
        }

        /// <summary>
        /// Returns true if NoticeResponseData instances are equal
        /// </summary>
        /// <param name="input">Instance of NoticeResponseData to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(NoticeResponseData input)
        {
            if (input == null)
                return false;

            return
                (
                    this.Total == input.Total ||
                    this.Total.Equals(input.Total)
                ) &&
                (
                    this.ExistsLatestNotice == input.ExistsLatestNotice ||
                    this.ExistsLatestNotice.Equals(input.ExistsLatestNotice)
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
                hashCode = hashCode * 59 + this.Total.GetHashCode();
                hashCode = hashCode * 59 + this.ExistsLatestNotice.GetHashCode();
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
