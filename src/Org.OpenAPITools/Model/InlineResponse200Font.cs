/* 
 * フォント配信サービス
 *
 * フォント配信サービスのインタフェース仕様です。
 *
 * The version of the OpenAPI document: 1.0.0
 * 
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using OpenAPIDateConverter = Org.OpenAPITools.Client.OpenAPIDateConverter;

namespace Org.OpenAPITools.Model
{
    /// <summary>
    /// InlineResponse200Data
    /// </summary>
    [DataContract]
    public partial class InlineResponse200Font :  IEquatable<InlineResponse200Font>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InlineResponse200Font" /> class.
        /// </summary>
        /// <param name="userFontId">userFontId.</param>
        /// <param name="activateFlg">activateFlg.</param>
        /// <param name="fontId">fontId.</param>
        /// <param name="displayFontName">displayFontName.</param>
        /// <param name="fileName">fileName.</param>
        /// <param name="fileSize">fileSize.</param>
        /// <param name="version">version.</param>
        /// <param name="needFontVersionUpdate">needFontVersionUpdate.</param>
        /// <param name="isAvailableFont">isAvailableFont.</param>
        /// <param name="isFreemium">isFreemium.</param>
        /// <param name="contractIds">contractIds.</param>
        public InlineResponse200Font(string userFontId = default(string), bool activateFlg = default(bool), string fontId = default(string), string displayFontName = default(string), string fileName = default(string), int fileSize = default(int), string version = default(string), bool needFontVersionUpdate = default(bool), bool isAvailableFont = default(bool), bool isFreemium = default(bool), List<string> contractIds = default(List<string>))
        {
            this.UserFontId = userFontId;
            this.ActivateFlg = activateFlg;
            this.FontId = fontId;
            this.DisplayFontName = displayFontName;
            this.FileName = fileName;
            this.FileSize = fileSize;
            this.Version = version;
            this.NeedFontVersionUpdate = needFontVersionUpdate;
            this.IsAvailableFont = isAvailableFont;
            this.IsFreemium = isFreemium;
            this.ContractIds = contractIds;
        }

        /// <summary>
        /// Gets or Sets UserFontId
        /// </summary>
        [DataMember(Name="userFontId", EmitDefaultValue=false)]
        public string UserFontId { get; set; }

        /// <summary>
        /// Gets or Sets ActivateFlg
        /// </summary>
        [DataMember(Name="activateFlg", EmitDefaultValue=false)]
        public bool ActivateFlg { get; set; }

        /// <summary>
        /// Gets or Sets FontId
        /// </summary>
        [DataMember(Name = "fontId", EmitDefaultValue = false)]
        public string FontId { get; set; }

        /// <summary>
        /// Gets or Sets DisplayFontName
        /// </summary>
        [DataMember(Name="displayFontName", EmitDefaultValue=false)]
        public string DisplayFontName { get; set; }

        /// <summary>
        /// Gets or Sets FileName
        /// </summary>
        [DataMember(Name="fileName", EmitDefaultValue=false)]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or Sets FileSize
        /// </summary>
        [DataMember(Name="fileSize", EmitDefaultValue=false)]
        public int FileSize { get; set; }

        /// <summary>
        /// Gets or Sets Version
        /// </summary>
        [DataMember(Name="version", EmitDefaultValue=false)]
        public string Version { get; set; }

        /// <summary>
        /// Gets or Sets NeedFontVersionUpdate
        /// </summary>
        [DataMember(Name="needFontVersionUpdate", EmitDefaultValue=false)]
        public bool NeedFontVersionUpdate { get; set; }

        /// <summary>
        /// Gets or Sets IsAvailableFont
        /// </summary>
        [DataMember(Name="isAvailableFont", EmitDefaultValue=false)]
        public bool IsAvailableFont { get; set; }

        /// <summary>
        /// Gets or Sets IsFreemium
        /// </summary>
        [DataMember(Name="isFreemium", EmitDefaultValue=false)]
        public bool IsFreemium { get; set; }

        /// <summary>
        /// Gets or Sets ContractIds
        /// </summary>
        [DataMember(Name="contractIds", EmitDefaultValue=false)]
        public List<string> ContractIds { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class InlineResponse200Data {\n");
            sb.Append("  UserFontId: ").Append(UserFontId).Append("\n");
            sb.Append("  ActivateFlg: ").Append(ActivateFlg).Append("\n");
            sb.Append("  FontId: ").Append(FontId).Append("\n");
            sb.Append("  DisplayFontName: ").Append(DisplayFontName).Append("\n");
            sb.Append("  FileName: ").Append(FileName).Append("\n");
            sb.Append("  FileSize: ").Append(FileSize).Append("\n");
            sb.Append("  Version: ").Append(Version).Append("\n");
            sb.Append("  NeedFontVersionUpdate: ").Append(NeedFontVersionUpdate).Append("\n");
            sb.Append("  IsAvailableFont: ").Append(IsAvailableFont).Append("\n");
            sb.Append("  IsFreemium: ").Append(IsFreemium).Append("\n");
            sb.Append("  ContractIds: ").Append(ContractIds).Append("\n");
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
            return this.Equals(input as InlineResponse200Font);
        }

        /// <summary>
        /// Returns true if InlineResponse200Data instances are equal
        /// </summary>
        /// <param name="input">Instance of InlineResponse200Data to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(InlineResponse200Font input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.UserFontId == input.UserFontId ||
                    (this.UserFontId != null &&
                    this.UserFontId.Equals(input.UserFontId))
                ) && 
                (
                    this.ActivateFlg == input.ActivateFlg ||
                    this.ActivateFlg.Equals(input.ActivateFlg)
                ) &&
                (
                    this.FontId == input.FontId ||
                    (this.FontId != null &&
                    this.FontId.Equals(input.FontId))
                ) &&
                (
                    this.DisplayFontName == input.DisplayFontName ||
                    (this.DisplayFontName != null &&
                    this.DisplayFontName.Equals(input.DisplayFontName))
                ) && 
                (
                    this.FileName == input.FileName ||
                    (this.FileName != null &&
                    this.FileName.Equals(input.FileName))
                ) && 
                (
                    this.FileSize == input.FileSize ||
                    this.FileSize.Equals(input.FileSize)
                ) && 
                (
                    this.Version == input.Version ||
                    (this.Version != null &&
                    this.Version.Equals(input.Version))
                ) && 
                (
                    this.NeedFontVersionUpdate == input.NeedFontVersionUpdate ||
                    this.NeedFontVersionUpdate.Equals(input.NeedFontVersionUpdate)
                ) && 
                (
                    this.IsAvailableFont == input.IsAvailableFont ||
                    this.IsAvailableFont.Equals(input.IsAvailableFont)
                ) && 
                (
                    this.IsFreemium == input.IsFreemium ||
                    this.IsFreemium.Equals(input.IsFreemium)
                ) && 
                (
                    this.ContractIds == input.ContractIds ||
                    this.ContractIds != null &&
                    input.ContractIds != null &&
                    this.ContractIds.SequenceEqual(input.ContractIds)
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
                if (this.UserFontId != null)
                    hashCode = hashCode * 59 + this.UserFontId.GetHashCode();
                hashCode = hashCode * 59 + this.ActivateFlg.GetHashCode();
                if (this.DisplayFontName != null)
                    hashCode = hashCode * 59 + this.DisplayFontName.GetHashCode();
                if (this.FileName != null)
                    hashCode = hashCode * 59 + this.FileName.GetHashCode();
                hashCode = hashCode * 59 + this.FileSize.GetHashCode();
                if (this.Version != null)
                    hashCode = hashCode * 59 + this.Version.GetHashCode();
                hashCode = hashCode * 59 + this.NeedFontVersionUpdate.GetHashCode();
                hashCode = hashCode * 59 + this.IsAvailableFont.GetHashCode();
                hashCode = hashCode * 59 + this.IsFreemium.GetHashCode();
                if (this.ContractIds != null)
                    hashCode = hashCode * 59 + this.ContractIds.GetHashCode();
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
