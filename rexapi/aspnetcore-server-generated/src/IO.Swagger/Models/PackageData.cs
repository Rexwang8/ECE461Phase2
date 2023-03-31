/*
 * ECE 461 - Spring 2023 - Project 2
 *
 * API for ECE 461/Spring 2023/Project 2: A Trustworthy Module Registry
 *
 * OpenAPI spec version: 2.0.0
 * Contact: davisjam@purdue.edu
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace IO.Swagger.Models
{ 
    /// <summary>
    /// This is a \&quot;union\&quot; type. - On package upload, either Content or URL should be set. - On package update, exactly one field should be set. - On download, the Content field should be set.
    /// </summary>
    [DataContract]
    public partial class PackageData : IEquatable<PackageData>
    { 
        /// <summary>
        /// Package contents. This is the zip file uploaded by the user. (Encoded as text using a Base64 encoding).  This will be a zipped version of an npm package&#x27;s GitHub repository, minus the \&quot;.git/\&quot; directory.\&quot; It will, for example, include the \&quot;package.json\&quot; file that can be used to retrieve the project homepage.  See https://docs.npmjs.com/cli/v7/configuring-npm/package-json#homepage.
        /// </summary>
        /// <value>Package contents. This is the zip file uploaded by the user. (Encoded as text using a Base64 encoding).  This will be a zipped version of an npm package&#x27;s GitHub repository, minus the \&quot;.git/\&quot; directory.\&quot; It will, for example, include the \&quot;package.json\&quot; file that can be used to retrieve the project homepage.  See https://docs.npmjs.com/cli/v7/configuring-npm/package-json#homepage.</value>

        [DataMember(Name="Content")]
        public string Content { get; set; }

        /// <summary>
        /// Package URL (for use in public ingest).
        /// </summary>
        /// <value>Package URL (for use in public ingest).</value>

        [DataMember(Name="URL")]
        public string URL { get; set; }

        /// <summary>
        /// A JavaScript program (for use with sensitive modules).
        /// </summary>
        /// <value>A JavaScript program (for use with sensitive modules).</value>

        [DataMember(Name="JSProgram")]
        public string JSProgram { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class PackageData {\n");
            sb.Append("  Content: ").Append(Content).Append("\n");
            sb.Append("  URL: ").Append(URL).Append("\n");
            sb.Append("  JSProgram: ").Append(JSProgram).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((PackageData)obj);
        }

        /// <summary>
        /// Returns true if PackageData instances are equal
        /// </summary>
        /// <param name="other">Instance of PackageData to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(PackageData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    Content == other.Content ||
                    Content != null &&
                    Content.Equals(other.Content)
                ) && 
                (
                    URL == other.URL ||
                    URL != null &&
                    URL.Equals(other.URL)
                ) && 
                (
                    JSProgram == other.JSProgram ||
                    JSProgram != null &&
                    JSProgram.Equals(other.JSProgram)
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
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                    if (Content != null)
                    hashCode = hashCode * 59 + Content.GetHashCode();
                    if (URL != null)
                    hashCode = hashCode * 59 + URL.GetHashCode();
                    if (JSProgram != null)
                    hashCode = hashCode * 59 + JSProgram.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(PackageData left, PackageData right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PackageData left, PackageData right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
