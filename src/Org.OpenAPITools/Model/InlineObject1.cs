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
    /// InlineObject1
    /// </summary>
    [DataContract]
    public partial class InlineObject1 : IEquatable<InlineObject1>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InlineObject1" /> class.
        /// </summary>
        /// <param name="twoFactCode">twoFactCode.</param>
        public InlineObject1(string twoFactCode = default(string))
        {
            this.TwoFactCode = twoFactCode;
        }

        /// <summary>
        /// Gets or Sets TwoFactCode
        /// </summary>
        [DataMember(Name = "twoFactCode", EmitDefaultValue = false)]
        public string TwoFactCode { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class InlineObject1 {\n");
            sb.Append("  TwoFactCode: ").Append(TwoFactCode).Append("\n");
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
            return this.Equals(input as InlineObject1);
        }

        /// <summary>
        /// Returns true if InlineObject1 instances are equal
        /// </summary>
        /// <param name="input">Instance of InlineObject1 to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(InlineObject1 input)
        {
            if (input == null)
                return false;

            return
                (
                    this.TwoFactCode == input.TwoFactCode ||
                    (this.TwoFactCode != null &&
                    this.TwoFactCode.Equals(input.TwoFactCode))
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
                if (this.TwoFactCode != null)
                    hashCode = hashCode * 59 + this.TwoFactCode.GetHashCode();
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
