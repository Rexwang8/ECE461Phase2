using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;


namespace IO.Swagger.Controllers
{
    public static class Sanitizer
    {
        public static string SanitizeString(string input)
        {
            if (input == null)
            {
                return null;
            }
            // Replace all non-alphanumeric characters with underscores
            string NoalphaNumeric = Regex.Replace(input, @"[^a-zA-Z0-9_\-\.]", "_");
            // Replace all multiple underscores with a single underscore
            string NoMultipleUnderscores = Regex.Replace(NoalphaNumeric, @"_+", "_");
            // Remove leading and trailing underscores
            string NoLeadingTrailingUnderscores = NoMultipleUnderscores.Trim('_');
            // Remove leading and trailing dashes
            string NoLeadingTrailingDashes = NoLeadingTrailingUnderscores.Trim('-');
            // cast to lowercase
            string Lowercase = NoLeadingTrailingDashes.ToLower();
            return Lowercase;
        }

        public static bool VerifyTokenSanitized(string token)
        {
            if (token == null)
            {
                return false;
            }
            if (token.Length != 162)
            {
                return false;
            }
            if (token.Any(c => !char.IsLetterOrDigit(c)))
            {
                return false;
            }
            if (token.Any(c => char.IsUpper(c)))
            {
                return false;
            }
            //contains no "bearer " at the beginning
            if (!token.StartsWith("bearer "))
            {
                return false;
            }
            return true;
        }

        public static bool VerifyPackageNameSafe(string name)
        {
            //check for null
            if (name == null)
            {
                return false;
            }
            //check blank
            if (name == "")
            {
                return false;
            }

            //check for invalid characters
            if (name.Any(c => !char.IsLetterOrDigit(c)))
            {
                return false;
            }
            return true;
        }
    }

    public static class Hasher
    {
        public static string HashString(string input)
        {
            if (input == null)
            {
                return null;
            }
            // hash string with sha256
            var sha256 = System.Security.Cryptography.SHA256.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = sha256.ComputeHash(inputBytes);
            string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            //append hash to itself 5x
            for (int i = 0; i < 5; i++)
            {
                hash = hash + hash;
            }

            //truncate to 155 characters
            hash = hash.Substring(0, 155);

            //permute with ceasar cipher
            string permuted = "";
            for (int i = 0; i < hash.Length; i++)
            {
                char c = hash[i];
                int shift = i % 13;
                if (c >= 'a' && c <= 'z')
                {
                    c = (char)(c + shift);
                    if (c > 'z')
                    {
                        c = (char)(c - 26);
                    }
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    c = (char)(c + shift);
                    if (c > 'Z')
                    {
                        c = (char)(c - 26);
                    }
                }

                permuted += c;
            }
            return permuted;
        }
    }
}