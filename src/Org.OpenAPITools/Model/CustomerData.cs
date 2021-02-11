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
    /// CustomerData
    /// </summary>
    [DataContract]
    public partial class CustomerData : IEquatable<CustomerData>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerData" /> class.
        /// </summary>
        /// <param name="mailAddress">mailAddress.</param>
        /// <param name="lastName">lastName.</param>
        /// <param name="firstName">firstName.</param>
        public CustomerData(string mailAddress = default(string), string lastName = default(string), string firstName = default(string))
        {
            this.MailAddress = mailAddress;
            this.LastName = lastName;
            this.FirstName = firstName;
        }

        /// <summary>
        /// Gets or Sets MailAddress
        /// </summary>
        [DataMember(Name = "mailAddress", EmitDefaultValue = false)]
        public string MailAddress { get; set; }

        /// <summary>
        /// Gets or Sets LastName
        /// </summary>
        [DataMember(Name = "lastName", EmitDefaultValue = false)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or Sets FirstName
        /// </summary>
        [DataMember(Name = "firstName", EmitDefaultValue = false)]
        public string FirstName { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CustomerData {\n");
            sb.Append("  MailAddress: ").Append(MailAddress).Append("\n");
            sb.Append("  LastName: ").Append(LastName).Append("\n");
            sb.Append("  FirstName: ").Append(FirstName).Append("\n");
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
            return this.Equals(input as CustomerData);
        }

        /// <summary>
        /// Returns true if CustomerData instances are equal
        /// </summary>
        /// <param name="input">Instance of CustomerData to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(CustomerData input)
        {
            if (input == null)
                return false;

            return
                (
                    this.MailAddress == input.MailAddress ||
                    (this.MailAddress != null &&
                    this.MailAddress.Equals(input.MailAddress))
                ) &&
                (
                    this.LastName == input.LastName ||
                    (this.LastName != null &&
                    this.LastName.Equals(input.LastName))
                ) &&
                (
                    this.FirstName == input.FirstName ||
                    (this.FirstName != null &&
                    this.FirstName.Equals(input.FirstName))
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
                if (this.MailAddress != null)
                    hashCode = hashCode * 59 + this.MailAddress.GetHashCode();
                if (this.LastName != null)
                    hashCode = hashCode * 59 + this.LastName.GetHashCode();
                if (this.FirstName != null)
                    hashCode = hashCode * 59 + this.FirstName.GetHashCode();
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
