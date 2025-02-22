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
    /// ContractsDataContracts
    /// </summary>
    [DataContract]
    public partial class ContractsDataContracts : IEquatable<ContractsDataContracts>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContractsDataContracts" /> class.
        /// </summary>
        /// <param name="contractId">contractId.</param>
        /// <param name="contractEndDate">contractEndDate.</param>
        public ContractsDataContracts(string contractId = default(string), string contractEndDate = default(string))
        {
            this.ContractId = contractId;
            this.ContractEndDate = contractEndDate;
        }

        /// <summary>
        /// Gets or Sets ContractId
        /// </summary>
        [DataMember(Name = "contractId", EmitDefaultValue = false)]
        public string ContractId { get; set; }

        /// <summary>
        /// Gets or Sets ContractEndDate
        /// </summary>
        [DataMember(Name = "contractEndDate", EmitDefaultValue = false)]
        public string ContractEndDate { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ContractsDataContracts {\n");
            sb.Append("  ContractId: ").Append(ContractId).Append("\n");
            sb.Append("  ContractEndDate: ").Append(ContractEndDate).Append("\n");
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
            return this.Equals(input as ContractsDataContracts);
        }

        /// <summary>
        /// Returns true if ContractsDataContracts instances are equal
        /// </summary>
        /// <param name="input">Instance of ContractsDataContracts to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ContractsDataContracts input)
        {
            if (input == null)
                return false;

            return
                (
                    this.ContractId == input.ContractId ||
                    (this.ContractId != null &&
                    this.ContractId.Equals(input.ContractId))
                ) &&
                (
                    this.ContractEndDate == input.ContractEndDate ||
                    (this.ContractEndDate != null &&
                    this.ContractEndDate.Equals(input.ContractEndDate))
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
                if (this.ContractId != null)
                    hashCode = hashCode * 59 + this.ContractId.GetHashCode();
                if (this.ContractEndDate != null)
                    hashCode = hashCode * 59 + this.ContractEndDate.GetHashCode();
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
