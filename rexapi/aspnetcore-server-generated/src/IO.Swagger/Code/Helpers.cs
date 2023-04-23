using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using Google.Apis.Bigquery.v2.Data;
using System.Diagnostics;


namespace IO.Swagger.Controllers
{
    /// <summary>
    /// class to handle Sanitization.
    /// </summary>
    public static class Sanitizer
    {
        /// <summary>
        /// Sanitize a string to be used as a package name.
        /// </summary>
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

        /// <summary>
        /// Sanitize a string to be used as a package name.
        /// </summary>
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

        /// <summary>
        /// Sanitize a string to be used as a package name.
        /// </summary>
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

        /// <summary>
        /// Sanitize a regex string to be used as a query.
        /// </summary>
        public static string SantizeRegex(string input)
        {
            //replace \' and ' with ''
            string escaped = input.Replace(@"\'", @"''");
            escaped = escaped.Replace("'", "''");
            //remove all SQL keywords
            string[] keywords = {"DROP", "DELETE", "INSERT", "UPDATE", "SELECT", "CREATE", "ALTER", "TRUNCATE", "EXEC", "EXECUTE", "GRANT", "REVOKE", "COMMIT", "ROLLBACK", "SAVE", "RESTORE", "BACKUP", "MERGE", "OPEN", "CLOSE", "FETCH", "PRINT", "RAISERROR", "READTEXT", "WRITETEXT", "DBCC", "USE", "WITH", "SET", "DECLARE", "GO", "IF", "ELSE", "WHILE", "BREAK", "CONTINUE", "TRY", "CATCH", "THROW", "WAITFOR", "PRINT", "RAISERROR", "READTEXT", "WRITETEXT", "DBCC", "USE", "WITH", "SET", "DECLARE", "GO", "IF", "ELSE", "WHILE", "BREAK", "CONTINUE", "TRY", "CATCH", "THROW", "WAITFOR", "PRINT", "RAISERROR", "READTEXT", "WRITETEXT", "DBCC", "USE", "WITH", "SET", "DECLARE", "GO", "IF", "ELSE", "WHILE", "BREAK", "CONTINUE", "TRY", "CATCH", "THROW", "WAITFOR", "PRINT", "RAISERROR", "READTEXT", "WRITETEXT", "DBCC", "USE", "WITH", "SET", "DECLARE", "GO", "IF", "ELSE", "WHILE", "BREAK", "CONTINUE", "TRY", "CATCH", "THROW", "WAITFOR", "PRINT", "RAISERROR", "READTEXT", "WRITETEXT", "DBCC", "USE", "WITH", "SET", "DECLARE", "GO", "IF", "ELSE", "WHILE", "BREAK", "CONTINUE", "TRY", "CATCH", "THROW", "WAITFOR", "PRINT", "RAISERROR", "READTEXT", "WRITETEXT", "DBCC", "USE", "WITH", "SET", "DECLARE", "GO", "IF", "ELSE", "WHILE", "BREAK", "CONTINUE", "TRY", "CATCH", "THROW", "WAITFOR", "PRINT", "RAISERROR", "READTEXT"};
            for (int i = 0; i < keywords.Length; i++)
            {
                escaped = escaped.Replace(keywords[i], "");
            }
            escaped = escaped.Replace("DROP TABLES", "");
            escaped = escaped.Replace(";", "");
            return escaped;
        }

        /// <summary>
        /// Generate a new ID.
        /// </summary>
        public static string GenerateNewID()
        {
            //generate a new ID
            string id = Guid.NewGuid().ToString();
            return id;
        }
    }

    /// <summary>
    /// class to handle hashing.
    /// </summary>
    public static class Hasher
    {
        /// <summary>
        /// Hash a string.
        /// </summary>
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


    /// <summary>
    /// The class to authenticate a user with a token
    /// </summary>
    public class TokenAuthenticator
    {
        /// <summary>
        /// The token of the user
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The username of the user
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The constructor for the TokenAuthenticator class
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The boolean to determine if the user is an admin
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// The token without the bearer
        /// </summary>
        public string Token_NoBearer { get; set; }

        /// <summary>
        /// The boolean to determine if the token is valid
        /// </summary>
        public bool IsTokenValid { get; set; }

        /// <summary>
        /// The factory to use to access the database
        /// </summary>
        public BigQueryFactory factory { get; set; }

        /// <summary>
        /// The results of the authentication attempt
        /// </summary>
        public enum AuthResults
        {
            /// <summary>
            /// The user does not exist
            /// </summary>
            NO_RESULTS,
            /// <summary>
            /// The user exists, but the password is incorrect
            /// </summary>
            WRONG_PASSWORD,
            /// <summary>
            /// The Token is over the limit
            /// </summary>
            TOKEN_OVERLIMIT,
            /// <summary>
            /// The Token is expired
            /// </summary>
            TOKEN_EXPIRED,
            /// <summary>
            /// The Token is invalid
            /// </summary>
            TOKEN_INVALID,
            /// <summary>
            /// The user exists, and the password is correct
            /// </summary>
            SUCCESS_USER,
            /// <summary>
            /// The user exists, and the password is correct
            /// </summary>
            SUCCESS_ADMIN
        }

        /// <summary>
        /// The results of the authentication attempt
        /// </summary>
        public enum AuthRefreshResults
        {
            /// <summary>
            /// The user does not exist
            /// </summary>
            FAILURE,
            /// <summary>
            /// The user exists, but the password is incorrect
            /// </summary>
            SUCCESS
        }



        /// <summary>
        /// The constructor for the TokenAuthenticator class
        /// </summary>
        public TokenAuthenticator()
        {
            factory = new BigQueryFactory();
        }

        /// <summary>
        /// Generate a token from the credentials
        /// </summary>
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

        /// <summary>
        /// Validate the token
        /// </summary>
        public AuthResults ValidateToken(string token)
        {
            //lookup token and check credentials
            string query = $"SELECT * FROM `package-registry-461.userData.users` WHERE token = '{token}' LIMIT 1";
            factory.SetQuery(query);
            BigQueryResults result = factory.ExecuteQuery();
            BigQueryRow row = result.FirstOrDefault();

            if (result.TotalRows == 0)
            {
                IsTokenValid = false;
                return AuthResults.NO_RESULTS;
            }

            //check if token is expired or overlimit
            if (Int64.Parse(row["numuses"].ToString()) < 0)
            {
                IsTokenValid = false;
                return AuthResults.TOKEN_OVERLIMIT;
            }
            DateTime foreignTokenExpiration = DateTime.Parse(row["dateexpired"].ToString());
            if (foreignTokenExpiration == null)
            {
                Console.WriteLine("ERROR ValidateToken: Token expiration date is null");
                IsTokenValid = false;
                return AuthResults.TOKEN_INVALID;
            }
            if (foreignTokenExpiration < DateTime.Now)
            {
                IsTokenValid = false;
                return AuthResults.TOKEN_EXPIRED;
            }

            IsTokenValid = true;
            //get username

            if (row["username"].ToString() == null)
            {
                Console.WriteLine("ERROR ValidateToken: Username is null");
                IsTokenValid = false;
                return AuthResults.TOKEN_INVALID;
            } 
            else
            {
                Username = row["username"].ToString();
            }
            //check if admin or user
            if (row["admin"].ToString() == "True")
            {
                IsAdmin = true;
                return AuthResults.SUCCESS_ADMIN;
            }
            else
            {
                IsAdmin = false;
                return AuthResults.SUCCESS_USER;
            }
        }

        /// <summary>
        /// Validate the credentials
        /// </summary>
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
            if (foreignToken == null)
            {
                return AuthResults.TOKEN_INVALID;
            }
            if (foreignToken != Token)
            {
                return AuthResults.TOKEN_INVALID;
            }

            //check if token is expired
            DateTime foreignTokenExpiration = default(DateTime);
            if (row["dateexpired"] != null)
            {
                foreignTokenExpiration = DateTime.Parse(row["dateexpired"].ToString());
                Console.WriteLine(foreignTokenExpiration.ToShortTimeString());
            }
            else
            {
                Console.WriteLine("ERROR ValidateCredentials: Token expiration date is null");
                return AuthResults.TOKEN_EXPIRED;
            }
            if (foreignTokenExpiration < DateTime.Now)
            {
                return AuthResults.TOKEN_EXPIRED;
            }

            //check if token is over limit
            int? foreignTokenUses = int.Parse(row["numuses"].ToString());
            if (foreignTokenUses == null)
            {
                return AuthResults.TOKEN_OVERLIMIT;
            }
            Console.WriteLine(foreignTokenUses);
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

        /// <summary>
        /// Updates the user's token expiration and uses
        /// </summary>
        public AuthRefreshResults UpdateUserDateRefreshToken()
        {
            //if token is valid, update token expiration and uses (now-10 hours, 1000)
            string query = $"UPDATE `package-registry-461.userData.users` SET dateexpired = TIMESTAMP_ADD(CURRENT_TIMESTAMP(), INTERVAL 10 HOUR), numuses = 1000 WHERE Username = '{Username}'";
            factory.SetQuery(query);
            BigQueryResults result = factory.ExecuteQuery();
            if (result == null)
            {
                Console.WriteLine("UpdateUserDateRefreshToken result is null");
                return AuthRefreshResults.FAILURE;
            }
            if (result.TotalRows == 0)
            {
                return AuthRefreshResults.FAILURE;
            }
            return AuthRefreshResults.SUCCESS;
        }

        /// <summary>
        /// Adds a user to the database if they don't already exist
        /// </summary>
        public AuthRefreshResults AddUserToDatabaseIfNotExists(string token)
        {
            //if token is valid, update token expiration and uses (now-10 hours, 1000)
            string query = $"INSERT INTO `package-registry-461.userData.users` (token, Username, Password, perms, dateexpired, numuses, Admin) VALUES ('{token}', '{Username}', '{Password}', '', TIMESTAMP_ADD(CURRENT_TIMESTAMP(), INTERVAL 10 HOUR), 1000, {IsAdmin})";
            factory.SetQuery(query);
            BigQueryResults result = factory.ExecuteQuery();
            if (result.TotalRows == 0)
            {
                return AuthRefreshResults.FAILURE;
            }
            return AuthRefreshResults.SUCCESS;
        }

        /// <summary>
        /// Decrements the number of uses for a token
        /// </summary>
        public AuthRefreshResults DecrementNumUsesForToken(string token)
        {

            string query = $"UPDATE `package-registry-461.userData.users` SET numuses = numuses - 1 WHERE token = '{token}'";
            factory.SetQuery(query);
            BigQueryResults result = factory.ExecuteQuery();
            if (result.TotalRows == 0)
            {
                Console.WriteLine("DecrementNumUsesForToken result is null");
                return AuthRefreshResults.FAILURE;
            }
            return AuthRefreshResults.SUCCESS;
        }


        /// <summary>
        /// Verifies that the token is valid
        /// </summary>
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

        /// <summary>
        /// Sets username
        /// </summary>
        public void setUsername(string username)
        {
            this.Username = username;
        }

        /// <summary>
        /// Sets password
        /// </summary>
        public void setPassword(string password)
        {
            this.Password = password;
        }


        /// <summary>
        /// Sets admin
        /// </summary>
        public void setAdmin(bool? admin)
        {
            this.IsAdmin = admin ?? false;
        }

        /// <summary>
        /// Returns username
        /// </summary>
        public string getUsername()
        {
            return this.Username;
        }

        /// <summary>
        /// Returns password
        /// </summary>
        public string getPassword()
        {
            return this.Password;
        }

        /// <summary>
        /// Returns true if user is admin, false if not
        /// </summary>
        public bool getAdmin()
        {
            return this.IsAdmin;
        }
    }

    /// <summary>
    /// Class for encoding and decoding base64 strings
    /// </summary>
    public class Base64Encoder
    {
        /// <summary>
        /// Encodes a file to a base64 string
        /// </summary>
        public static string Encode(string filename)
        {
            var plainTextBytes = System.IO.File.ReadAllBytes(filename);
            var encoded = System.Convert.ToBase64String(plainTextBytes);
            return encoded.Replace(" ", "").Replace("\n", "").Replace("\r", "");
        }

        /// <summary>
        /// Decodes a base64 string to a file
        /// </summary>
        public static void Decode(string base64String, string filename)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64String);
            System.IO.File.WriteAllBytes(filename, base64EncodedBytes);
            return;
        }



    }
}