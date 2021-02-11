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
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Org.OpenAPITools.Model
{
    /// <summary>
    /// InlineResponse200Data
    /// </summary>
    [DataContract]
    public partial class InlineResponse200Data : IEquatable<InlineResponse200Data>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InlineResponse200Data" /> class.
        /// </summary>
        /// <param name="fonts">fonts.</param>
        /// <param name="hasNext">hasNext.</param>
        public InlineResponse200Data(List<InlineResponse200Font> fonts = default(List<InlineResponse200Font>), bool hasNext = default(bool))
        {
            this.Fonts = fonts;
            this.HasNext = hasNext;
        }

        /// <summary>
        /// Gets or Sets fonts
        /// </summary>
        [DataMember(Name = "fonts", EmitDefaultValue = false)]
        public List<InlineResponse200Font> Fonts { get; set; }

        /// <summary>
        /// Gets or Sets HasNext
        /// </summary>
        [DataMember(Name = "hasNext", EmitDefaultValue = false)]
        public bool HasNext { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class InlineResponse200Data {\n");
            sb.Append("  Fonts: ").Append(Fonts).Append("\n");
            sb.Append("  HasNext: ").Append(HasNext).Append("\n");
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
            return this.Equals(input as InlineResponse200Data);
        }

        /// <summary>
        /// Returns true if InlineResponse200Data instances are equal
        /// </summary>
        /// <param name="input">Instance of InlineResponse200Data to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(InlineResponse200Data input)
        {
            if (input == null)
                return false;

            return
                (
                    this.Fonts == input.Fonts ||
                    this.Fonts != null &&
                    input.Fonts != null &&
                    this.Fonts.SequenceEqual(input.Fonts)
                ) &&
                (
                    this.HasNext == input.HasNext ||
                    this.HasNext.Equals(input.HasNext)
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
                hashCode = hashCode * 59 + this.HasNext.GetHashCode();
                if (this.Fonts != null)
                    hashCode = hashCode * 59 + this.Fonts.GetHashCode();
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
