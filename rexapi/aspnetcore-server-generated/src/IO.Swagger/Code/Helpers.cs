using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using Google.Apis.Bigquery.v2.Data;


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
            string tokenmain = token.Substring(7);

            if (tokenmain.Any(c => !char.IsLetterOrDigit(c)))
            {
                return false;
            }
            if (tokenmain.Any(c => char.IsUpper(c)))
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
    
        public static Regex SanitizedCompiledRegex(string input)
        {
            String sanitized = Regex.Escape(input);
            Regex InputRegex = new Regex(sanitized);
            return InputRegex;
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

    public class TokenAuthenticator
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public string Token_NoBearer { get; set; }

        public BigQueryFactory factory { get; set; }

        public enum AuthResults 
        {
            NO_RESULTS,
            WRONG_PASSWORD,
            TOKEN_OVERLIMIT,
            TOKEN_EXPIRED,
            TOKEN_INVALID,
            SUCCESS_USER,
            SUCCESS_ADMIN
        }

        public enum AuthRefreshResults
        {
            FAILURE,
            SUCCESS
        }
    


        public TokenAuthenticator()
        {
            factory = new BigQueryFactory();
        }

        public string GenerateTokenFromCredentials()
        {
            if (Username == null || Password == null)
            {
                return null;
            }
            if (Username == "" || Password == "")
            {
                return null;
            }
            if (Username.Length > 50 || Password.Length > 50)
            {
                return null;
            }
            if (Username.Any(c => !char.IsLetterOrDigit(c)))
            {
                return null;
            }
            if (Password.Any(c => !char.IsLetterOrDigit(c)))
            {
                return null;
            }
            if (Username.Any(c => char.IsUpper(c)))
            {
                return null;
            }
            if (Password.Any(c => char.IsUpper(c)))
            {
                return null;
            }
            if (Username.Length < 3 || Password.Length < 3)
            {
                return null;
            }
            if (Username == "admin" || Password == "admin")
            {
                return null;
            }
            if (Username == "guest" || Password == "guest")
            {
                return null;
            }


            string basestring = $"START -- SanitizedUsername: {Username} SanitizedPassword: {Password} Admin: {IsAdmin} -- END";
            Token_NoBearer = Hasher.HashString(basestring);
            Token = "bearer " + Token_NoBearer;

            return Token;
        }

        public AuthResults ValidateToken(string token)
        {
            //lookup token and check credentials
            string query = $"SELECT * FROM `package-registry-461.userData.users` WHERE token = '{token}' LIMIT 1";
            factory.SetQuery(query);
            BigQueryResults result = factory.ExecuteQuery();
            BigQueryRow row = result.FirstOrDefault();

            if (result.TotalRows == 0)
            {
                return AuthResults.NO_RESULTS;
            }

            //check if token is expired or overlimit
            if (Int64.Parse(row["numuses"].ToString()) < 0)
            {
                return AuthResults.TOKEN_OVERLIMIT;
            }
            DateTime foreignTokenExpiration = DateTime.Parse(row["dateissued"].ToString());
            if (foreignTokenExpiration < DateTime.Now)
            {
                return AuthResults.TOKEN_EXPIRED;
            }

            //check if admin or user
            if (row["admin"].ToString() == "True")
            {
                return AuthResults.SUCCESS_ADMIN;
            }
            else
            {
                return AuthResults.SUCCESS_USER;
            }
        }

        public AuthResults ValidateCredentials()
        {
            string query = $"SELECT * FROM `package-registry-461.userData.users` WHERE Username = '{Username}' LIMIT 1";
            factory.SetQuery(query);
            BigQueryResults result = factory.ExecuteQuery();

            BigQueryRow row = result.FirstOrDefault();


            if (result.TotalRows == 0)
            {
                return AuthResults.NO_RESULTS;
            }
            
            //check if password matches
            string foreignPassword = row["password"].ToString();
            if (foreignPassword != Password)
            {
                return AuthResults.WRONG_PASSWORD;
            }

            //check if token matches
            string foreignToken = row["token"].ToString();
            if (foreignToken != Token)
            {
                return AuthResults.TOKEN_INVALID;
            }

            //check if token is expired
            DateTime foreignTokenExpiration = DateTime.Parse(row["dateissued"].ToString());
            if (foreignTokenExpiration < DateTime.Now)
            {
                return AuthResults.TOKEN_EXPIRED;
            }

            //check if token is over limit
            int foreignTokenUses = int.Parse(row["numuses"].ToString());
            if (foreignTokenUses <= 0)
            {
                return AuthResults.TOKEN_OVERLIMIT;
            }

            //check if admin
            bool foreignAdmin = bool.Parse(row["admin"].ToString());
            if (foreignAdmin)
            {
                return AuthResults.SUCCESS_ADMIN;
            }
            else
            {
                return AuthResults.SUCCESS_USER;
            }

        }

        public AuthRefreshResults UpdateUserDateRefreshToken()
        {
            //if token is valid, update token expiration and uses (now-10 hours, 1000)
            string query = $"UPDATE `package-registry-461.userData.users` SET dateissued = TIMESTAMP_SUB(CURRENT_TIMESTAMP(), INTERVAL 10 HOUR), numuses = 1000 WHERE Username = '{Username}'";
            factory.SetQuery(query);
            BigQueryResults result = factory.ExecuteQuery();
            if (result.TotalRows == 0)
            {
                return AuthRefreshResults.FAILURE;
            }
            return AuthRefreshResults.SUCCESS;
        }

        public AuthRefreshResults AddUserToDatabaseIfNotExists()
        {
            //if token is valid, update token expiration and uses (now-10 hours, 1000)
            string query = $"INSERT INTO `package-registry-461.userData.users` (Username, Password, dateissued, numuses, Admin) VALUES ('{Username}', '{Password}', TIMESTAMP_SUB(CURRENT_TIMESTAMP(), INTERVAL 10 HOUR), 1000, {IsAdmin})";
            factory.SetQuery(query);
            BigQueryResults result = factory.ExecuteQuery();
            if (result.TotalRows == 0)
            {
                return AuthRefreshResults.FAILURE;
            }
            return AuthRefreshResults.SUCCESS;
        }

        public bool VerifyToken()
        {
            if (Token == null)
            {
                return false;
            }
            if (Token.Length != 162)
            {
                return false;
            }
            string tokenmain = Token.Substring(7);

            if (tokenmain.Any(c => !char.IsLetterOrDigit(c)))
            {
                return false;
            }
            if (tokenmain.Any(c => char.IsUpper(c)))
            {
                return false;
            }
            //contains no "bearer " at the beginning
            if (!Token.StartsWith("bearer "))
            {
                return false;
            }
            return true;
        }

        public void setUsername(string username)
        {
            this.Username = username;
        }
        public void setPassword(string password)
        {
            this.Password = password;
        }
        public void setAdmin(bool? admin)
        {
            this.IsAdmin = admin ?? false;
        }
    }
}