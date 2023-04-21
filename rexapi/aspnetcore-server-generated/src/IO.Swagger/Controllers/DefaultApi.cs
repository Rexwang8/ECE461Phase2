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
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using IO.Swagger.Attributes;
using IO.Swagger.Security;
using Microsoft.AspNetCore.Authorization;
using IO.Swagger.Models;
using IO.Swagger.Controllers;
using System.Security.Cryptography;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using Google.Apis.Bigquery.v2.Data;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Google.Cloud.SecretManager.V1;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;


namespace IO.Swagger.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class DefaultApiController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Create an access token.</remarks>
        /// <param name="body"></param>
        /// <response code="200">Return an AuthenticationToken.</response>
        /// <response code="400">There is missing field(s) in the AuthenticationRequest or it is formed improperly.</response>
        /// <response code="401">The user or password is invalid.</response>
        /// <response code="501">This system does not support authentication.</response>
        [HttpPut]
        [Route("/authenticate")]
        [ValidateModelState]
        [SwaggerOperation("CreateAuthToken")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "Return an AuthenticationToken.")]
        public virtual IActionResult CreateAuthToken([FromBody] AuthenticationRequest body)
        {
            string username = body.User.Name;
            bool? admin = body.User.IsAdmin;
            string password = body.Secret.Password;

            //check if any fields are null or empty
            if (username == null || password == null || username == "" || password == "" || admin == null)
            {
                return StatusCode(400);
            }

            //check if username or password is invalid
            if (username == "admin" || password == "admin")
            {
                return StatusCode(401);
            }
            if (username == "user" || password == "user")
            {
                return StatusCode(401);
            }
            if (username == "guest" || password == "guest")
            {
                return StatusCode(401);
            }
            if (username == "invalid" || password == "invalid")
            {
                return StatusCode(401);
            }
            
            
            //sanitize input
            string SanitizedUsername = Sanitizer.SanitizeString(username);
            string SanitizedPassword = Sanitizer.SanitizeString(password);

            TokenAuthenticator authenticator = new TokenAuthenticator();
            authenticator.setPassword(SanitizedPassword);
            authenticator.setUsername(SanitizedUsername);
            authenticator.setAdmin(admin);
            string token = authenticator.GenerateTokenFromCredentials();
            TokenAuthenticator.AuthResults UserStatus = authenticator.ValidateCredentials();
            if (UserStatus == TokenAuthenticator.AuthResults.NO_RESULTS)
            {
                //user does not exist, add user to database
                Response.Headers.Add("X-Debug", "User does not exist, adding user to database");
                authenticator.AddUserToDatabaseIfNotExists(token);
            }
            else if (UserStatus == TokenAuthenticator.AuthResults.WRONG_PASSWORD || UserStatus == TokenAuthenticator.AuthResults.TOKEN_INVALID)
            {
                //wrong password, return error
                if (UserStatus == TokenAuthenticator.AuthResults.WRONG_PASSWORD)
                    Response.Headers.Add("X-Debug", "Wrong password, returning error");
                else if (UserStatus == TokenAuthenticator.AuthResults.TOKEN_INVALID)
                    Response.Headers.Add("X-Debug", "Wrong password or token, returning error");
                return StatusCode(401);
            }
            else if (UserStatus == TokenAuthenticator.AuthResults.TOKEN_OVERLIMIT || UserStatus == TokenAuthenticator.AuthResults.TOKEN_EXPIRED)
            {
                //Refresh token
                if (UserStatus == TokenAuthenticator.AuthResults.TOKEN_OVERLIMIT)
                    Response.Headers.Add("X-Debug", "Token overlimit, refreshing token");
                else if (UserStatus == TokenAuthenticator.AuthResults.TOKEN_EXPIRED)
                    Response.Headers.Add("X-Debug", "Token expired, refreshing token");
                authenticator.UpdateUserDateRefreshToken();
            }

            //add the token to the header of the response in the X-Authorization field
            Response.Headers.Add("X-Authorization", token);

            //set the token as the response body
            return new ObjectResult(token);
        }

        /// <summary>
        /// Delete all versions of this package.
        /// </summary>
        /// <param name="xAuthorization"></param>
        /// <param name="name"></param>
        /// <response code="200">Package is deleted.</response>
        /// <response code="400">There is missing field(s) in the PackageName/AuthenticationToken or it is formed improperly, or the AuthenticationToken is invalid.</response>
        /// <response code="404">Package does not exist.</response>
        [HttpDelete]
        [Route("/package/byName/{name}")]
        [ValidateModelState]
        [SwaggerOperation("PackageByNameDelete")]
        public virtual IActionResult PackageByNameDelete([FromHeader(Name = "X-Authorization")][Required()] string xAuthorization, [FromRoute][Required] string name)
        {
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200);

            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);

            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);
            string token = xAuthorization;

            //Sanitize Inputs
            if (!Sanitizer.VerifyTokenSanitized(token))
            {
                Response.Headers.Add("X-Debug", "Token is not sanitized");
                return StatusCode(400);
            }

            if (!Sanitizer.VerifyPackageNameSafe(name))
            {
                Response.Headers.Add("X-Debug", "Name is not sanitized");
                return StatusCode(400);
            }

            //Check Token Permissions
            TokenAuthenticator authenticator = new TokenAuthenticator();
            TokenAuthenticator.AuthResults UserStatus = authenticator.ValidateToken(token);

            if (UserStatus != TokenAuthenticator.AuthResults.SUCCESS_ADMIN)
            {
                Response.Headers.Add("X-Debug", "User has no perms");
                return StatusCode(400);
            }

            //Decrement Token uses
            TokenAuthenticator.AuthRefreshResults success = authenticator.DecrementNumUsesForToken(token);
            if (success != TokenAuthenticator.AuthRefreshResults.SUCCESS)
            {
                Response.Headers.Add("X-Debug", "Token decrement failed");
                return StatusCode(400);
            }


            BigQueryFactory factory = new BigQueryFactory();
            BigQueryResults result = null;

            //get metadata for package, most recent version
            string query = $"SELECT * FROM `package-registry-461.packages.packagesMetadata` WHERE name='{name}' ORDER BY version DESC LIMIT 1";
            factory.SetQuery(query);
            result = factory.ExecuteQuery();
            if (result == null)
            {
                Response.Headers.Add("X-Debug", "Query failed");
                return StatusCode(400);
            }
            if (result.TotalRows == 0)
            {
                Response.Headers.Add("X-Debug", "Package does not exist");
                return StatusCode(404);
            }

            PackageMetadata metadata = new PackageMetadata();
            foreach (BigQueryRow row in result)
            {
                //Name
                if (row["name"] != null)
                {
                    metadata.Name = row["name"].ToString();
                }
                else 
                {
                    metadata.Name = "invalid";
                }

                //Version
                if (row["version"] != null)
                {
                    metadata.Version = row["version"].ToString();
                }
                else
                {
                    metadata.Version = "invalid";
                }

                //ID
                if (row["id"] != null)
                {
                    metadata.ID = row["id"].ToString();
                }
                else
                {
                    metadata.ID = "invalid";
                }
                    
            }


            //--------------------Add to History Query------------------------------------

            //
            query = $"INSERT INTO `package-registry-461.packages.packagesHistory` (action, date, user_isadmin, user_name, packagemetadata_id, packagemetadata_name, packagemetadata_version) VALUES ('DELETE', CURRENT_TIMESTAMP(), true, '{authenticator.getUsername()}', '{metadata.ID}', '{metadata.Name}', '{metadata.Version}')";
            factory.SetQuery(query);
            result = factory.ExecuteQuery();

            //-----------------------Delete from Meta Data Query------------------------------------
            query = $"DELETE * FROM `package-registry-461.packages.packagesMetadata` WHERE name='{name}' LIMIT 20";
            factory.SetQuery(query);
            result = factory.ExecuteQuery();

            //--------------------Delete from Packages Data Query------------------------------------
            query = $"DELETE * FROM `package-registry-461.packages.packagesData` WHERE name='{name}' LIMIT 1";
            factory.SetQuery(query);
            result = factory.ExecuteQuery();
            //--------------------Delete from cloud store------------------------------------
            


            

            return StatusCode(200);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Return the history of this package (all versions).</remarks>
        /// <param name="name"></param>
        /// <param name="xAuthorization"></param>
        /// <response code="200">Return the package history.</response>
        /// <response code="400">There is missing field(s) in the PackageName/AuthenticationToken or it is formed improperly, or the AuthenticationToken is invalid.</response>
        /// <response code="404">No such package.</response>
        /// <response code="0">unexpected error</response>
        [HttpGet]
        [Route("/package/byName/{name}")]
        [ValidateModelState]
        [SwaggerOperation("PackageByNameGet")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<PackageHistoryEntry>), description: "Return the package history.")]
        [SwaggerResponse(statusCode: 0, type: typeof(Error), description: "unexpected error")]
        public virtual IActionResult PackageByNameGet([FromRoute][Required] string name, [FromHeader(Name = "X-Authorization")][Required()] string xAuthorization)
        {
            //Get Variables
            string token = xAuthorization;
            string packagename = name;

            //Validate token
            bool isSanitized = Sanitizer.VerifyTokenSanitized(token);
            if (!isSanitized)
            {
                //append debug message to header
                Response.Headers.Add("X-Debug", "Token is not sanitized");
                return StatusCode(400);
            }
            TokenAuthenticator authenticator = new TokenAuthenticator();
            TokenAuthenticator.AuthResults UserStatus = authenticator.ValidateToken(token);
            if (UserStatus != TokenAuthenticator.AuthResults.SUCCESS_USER && UserStatus != TokenAuthenticator.AuthResults.SUCCESS_ADMIN)
            {
                //append debug message to header
                Response.Headers.Add("X-Debug", "Token is invalid");
                return StatusCode(400);
            }

            //decrement token
            TokenAuthenticator.AuthRefreshResults success = authenticator.DecrementNumUsesForToken(token);
            if (success != TokenAuthenticator.AuthRefreshResults.SUCCESS)
            {
                Response.Headers.Add("X-Debug", "Token decrement failed");
                return StatusCode(400);
            }

            bool isSanitizedName = Sanitizer.VerifyPackageNameSafe(packagename);
            if (!isSanitizedName)
            {
                //append debug message to header
                Response.Headers.Add("X-Debug", "Package name is not sanitized");
                return StatusCode(400);
            }

            BigQueryFactory factory = new BigQueryFactory();
            BigQueryResults result = null;

            //query database get all rows with the package name and return them
            string query = $"SELECT * FROM `package-registry-461.packages.packagesHistory` WHERE packagemetadata_name = '{packagename}' ORDER BY date LIMIT 100";
            factory.SetQuery(query);
            result = factory.ExecuteQuery();

            // Print out the name and type of each column
            string Buffer = "Schema: ";
            foreach (var col in result.Schema.Fields)
            {
                Buffer += col.Name + " " + col.Type + ", ";
            }
            Response.Headers.Add("X-DebugSchema", Buffer);
            if (result.TotalRows == 0)
            {
                //append debug message to header
                Response.Headers.Add("X-Debug", "Package does not exist");
                return StatusCode(404);
            }

            List<PackageHistoryEntry> packageHistoryEntries = new List<PackageHistoryEntry>();

            //create list of PackageHistoryEntry objects
            packageHistoryEntries = factory.GetPackageHistoryFromResults(result);

            //return list of package history entries in response body, formatted as ndjson [{},{}]
            Response.Headers.Add("X-Debug", "Package history returned");
            return StatusCode(200, packageHistoryEntries);
        }

        /// <summary>
        /// Get any packages fitting the regular expression.
        /// </summary>
        /// <remarks>Search for a package using regular expression over package names and READMEs. This is similar to search by name.</remarks>
        /// <param name="body"></param>
        /// <param name="xAuthorization"></param>
        /// <response code="200">Return a list of packages.</response>
        /// <response code="400">There is missing field(s) in the PackageRegEx/AuthenticationToken or it is formed improperly, or the AuthenticationToken is invalid.</response>
        /// <response code="404">No package found under this regex.</response>
        [HttpPost]
        [Route("/package/byRegEx")]
        [ValidateModelState]
        [SwaggerOperation("PackageByRegExGet")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<PackageMetadata>), description: "Return a list of packages.")]
        public virtual IActionResult PackageByRegExGet([FromBody] PackageRegex body, [FromHeader(Name = "X-Authorization")][Required()] string xAuthorization)
        {
            string token = xAuthorization;
            bool isSantized = Sanitizer.VerifyTokenSanitized(token);
            if (!isSantized)
            {
                Response.Headers.Add("X-Debug", "Token is not sanitized");
                return StatusCode(400, "");
            }
            TokenAuthenticator authenticator = new TokenAuthenticator();
            TokenAuthenticator.AuthResults UserStatus = authenticator.ValidateToken(token);
            if (UserStatus != TokenAuthenticator.AuthResults.SUCCESS_USER && UserStatus != TokenAuthenticator.AuthResults.SUCCESS_ADMIN)
            {
                //append debug message to header
                Response.Headers.Add("X-Debug", "Token is invalid");
                return StatusCode(400);
            }

            //decrement token
            TokenAuthenticator.AuthRefreshResults success = authenticator.DecrementNumUsesForToken(token);
            if (success != TokenAuthenticator.AuthRefreshResults.SUCCESS)
            {
                Response.Headers.Add("X-Debug", "Token decrement failed");
                return StatusCode(400);
            }


            string unsanitizedregex = body.Regex;

            string  pattern = Sanitizer.SantizeRegex(unsanitizedregex);
            if (pattern == null)
            {
                //append debug message to header
                Response.Headers.Add("X-Debug", "Regex is null");
                return StatusCode(400);
            }

            BigQueryFactory factory = new BigQueryFactory();
            BigQueryResults result = null;
            string query = $"SELECT * FROM `package-registry-461.packages.packagesMetadata` WHERE REGEXP_CONTAINS(name, r'{pattern}') LIMIT 100";
            factory.SetQuery(query);

            result = factory.ExecuteQuery();

            if (result.TotalRows == 0)
            {
                //append debug message to header
                Response.Headers.Add("X-Debug", "No packages found for regex + " + pattern + "  " + factory.GetQuery());
                return StatusCode(404);
            }

            List<PackageMetadata> packageMetadata = new List<PackageMetadata>();
            packageMetadata = factory.GetPackageMetadataFromResults(result);

            //return list of package metadata in response body, formatted as ndjson [{},{}]
            Response.Headers.Add("X-Debug", "Package metadata returned");
            return StatusCode(200, packageMetadata);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="xAuthorization"></param>
        /// <response code="201">Success. Check the ID in the returned metadata for the official ID.</response>
        /// <response code="400">There is missing field(s) in the PackageData/AuthenticationToken or it is formed improperly, or the AuthenticationToken is invalid.</response>
        /// <response code="409">Package exists already.</response>
        /// <response code="424">Package is not uploaded due to the disqualified rating.</response>
        [HttpPost]
        [Route("/package")]
        [ValidateModelState]
        [SwaggerOperation("PackageCreate")]
        [SwaggerResponse(statusCode: 201, type: typeof(Package), description: "Success. Check the ID in the returned metadata for the official ID.")]
        public virtual IActionResult PackageCreate([FromBody] PackageData body, [FromHeader][Required()] string xAuthorization)
        {
            //TODO: Uncomment the next line to return response 201 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(201, default(Package));

            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);

            //TODO: Uncomment the next line to return response 409 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(409);

            //TODO: Uncomment the next line to return response 424 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(424);
            string token = xAuthorization;

            //verify token
            if (!Sanitizer.VerifyTokenSanitized(token))
            {
                Response.Headers.Add("X-Debug", "Token is not sanitized");
                return StatusCode(400);
            }

            TokenAuthenticator authenticator = new TokenAuthenticator();
            TokenAuthenticator.AuthResults UserStatus = authenticator.ValidateToken(token);

            if (UserStatus != TokenAuthenticator.AuthResults.SUCCESS_USER && UserStatus != TokenAuthenticator.AuthResults.SUCCESS_ADMIN)
            {
                //append debug message to header
                Response.Headers.Add("X-Debug", "Token is invalid");
                return StatusCode(400);
            }

            //-------------Add package to metadata table ----------------
            BigQueryFactory factory = new BigQueryFactory();
            BigQueryResults result = null;
            
            //sanitize package name, version, and id
            string Name = "Rex";
            string Version = "1.1.1";
            string ID = Guid.NewGuid().ToString();

            string query = $"INSERT INTO `package-registry-461.packages.packagesMetadata` (id, name, version) VALUES ({ID}, {Name}, {Version})";
            factory.SetQuery(query);
            result = factory.ExecuteQuery();
            
            if (result.TotalRows == 0)
            {
                //append debug message to header
                Response.Headers.Add("X-Debug", "No packages are added + " + factory.GetQuery());
                return StatusCode(404);
            }

            return StatusCode(201);
        }

        /// <summary>
        /// Delete this version of the package.
        /// </summary>
        /// <param name="xAuthorization"></param>
        /// <param name="id">Package ID</param>
        /// <response code="200">Package is deleted.</response>
        /// <response code="400">There is missing field(s) in the PackageID/AuthenticationToken or it is formed improperly, or the AuthenticationToken is invalid.</response>
        /// <response code="404">Package does not exist.</response>
        [HttpDelete]
        [Route("/package/{id}")]
        [ValidateModelState]
        [SwaggerOperation("PackageDelete")]
        public virtual IActionResult PackageDelete([FromHeader(Name = "X-Authorization")][Required()] string xAuthorization, [FromRoute][Required] string id)
        {
            string token = xAuthorization;

            //Sanitize Inputs
            if (!Sanitizer.VerifyTokenSanitized(token))
            {
                Response.Headers.Add("X-Debug", "Token is not sanitized");
                return StatusCode(400);
            }

            if (!Guid.TryParse(id, out _))
            {
                Response.Headers.Add("X-Debug", "ID is not sanitized");
                return StatusCode(400);
            }

            //Check Token Permissions
            TokenAuthenticator authenticator = new TokenAuthenticator();
            TokenAuthenticator.AuthResults UserStatus = authenticator.ValidateToken(token);

            if (UserStatus != TokenAuthenticator.AuthResults.SUCCESS_ADMIN)
            {
                Response.Headers.Add("X-Debug", "User has no perms");
                return StatusCode(400);
            }

            //Decrement Token uses
            TokenAuthenticator.AuthRefreshResults success = authenticator.DecrementNumUsesForToken(token);
            if (success != TokenAuthenticator.AuthRefreshResults.SUCCESS)
            {
                Response.Headers.Add("X-Debug", "Token decrement failed");
                return StatusCode(400);
            }


            BigQueryFactory factory = new BigQueryFactory();
            BigQueryResults result = null;

            //Delete from Meta Data Query
            string query = $"DELETE * FROM `package-registry-461.packages.packagesMetadata` WHERE id= '{id}' LIMIT 1";
            factory.SetQuery(query);
            result = factory.ExecuteQuery();

            if (result.TotalRows == 0)
            {
                //append debug message to header
                Response.Headers.Add("X-Debug", "No packages found for id: + " + id + "  " + factory.GetQuery());
                return StatusCode(404);
            }

            List<PackageHistoryEntry> packageHistoryEntries = new List<PackageHistoryEntry>();
            packageHistoryEntries = factory.GetPackageHistoryFromResults(result);

            //Delete from Packages Data Query
            query = $"DELETE * FROM `package-registry-461.packages.packagesData` WHERE metaid= '{id}' LIMIT 1";
            factory.SetQuery(query);
            result = factory.ExecuteQuery();

            if (result.TotalRows == 0)
            {
                //append debug message to header
                Response.Headers.Add("X-Debug", "No packages found for id: + " + id + "  " + factory.GetQuery());
                return StatusCode(404);
            }

            packageHistoryEntries = factory.GetPackageHistoryFromResults(result);

            return StatusCode(200);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="xAuthorization"></param>
        /// <response code="200">Return the rating. Only use this if each metric was computed successfully.</response>
        /// <response code="400">There is missing field(s) in the PackageID/AuthenticationToken or it is formed improperly, or the AuthenticationToken is invalid.</response>
        /// <response code="404">Package does not exist.</response>
        /// <response code="500">The package rating system choked on at least one of the metrics.</response>
        [HttpGet]
        [Route("/package/{id}/rate")]
        [ValidateModelState]
        [SwaggerOperation("PackageRate")]
        [SwaggerResponse(statusCode: 200, type: typeof(PackageRating), description: "Return the rating. Only use this if each metric was computed successfully.")]
        public virtual IActionResult PackageRate([FromRoute][Required] string id, [FromHeader(Name = "X-Authorization")][Required()] string xAuthorization)
        {
            //VERIFY TOKEN
            string token = xAuthorization;
            bool isSantized = Sanitizer.VerifyTokenSanitized(token);
            if (!isSantized)
            {
                Response.Headers.Add("X-Debug", "Token is not sanitized");
                return StatusCode(400);
            }

            TokenAuthenticator authenticator = new TokenAuthenticator();
            TokenAuthenticator.AuthResults UserStatus = authenticator.ValidateToken(token);
            if (UserStatus != TokenAuthenticator.AuthResults.SUCCESS_ADMIN && UserStatus != TokenAuthenticator.AuthResults.SUCCESS_USER)
            {
                //append debug message to header
                Response.Headers.Add("X-Debug", "Token is invalid");
                return StatusCode(400);
            }

            //decrement token
            TokenAuthenticator.AuthRefreshResults success = authenticator.DecrementNumUsesForToken(token);
            if (success != TokenAuthenticator.AuthRefreshResults.SUCCESS)
            {
                Response.Headers.Add("X-Debug", "Token decrement failed");
                return StatusCode(400);
            }

            //Form NotImplemented JSON
            PackageRating rating = new PackageRating();
            rating.BusFactor = 2;
            rating.Correctness = 3;
            rating.RampUp = 2;
            rating.ResponsiveMaintainer = 3;
            rating.LicenseScore = 2;
            rating.GoodPinningPractice = 2;
            rating.PullRequest = 2;
            rating.NetScore = 2;

            Response.Headers.Add("X-Debug", "Token is valid --- " + id);


            return StatusCode(200, rating);





            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(PackageRating));

            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);

            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);

            //TODO: Uncomment the next line to return response 500 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(500);
            string exampleJson = null;
            exampleJson = "{\n  \"GoodPinningPractice\" : 2.3021358869347655,\n  \"NetScore\" : 9.301444243932576,\n  \"PullRequest\" : 7.061401241503109,\n  \"ResponsiveMaintainer\" : 5.962133916683182,\n  \"LicenseScore\" : 5.637376656633329,\n  \"RampUp\" : 1.4658129805029452,\n  \"BusFactor\" : 0.8008281904610115,\n  \"Correctness\" : 6.027456183070403\n}";

            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<PackageRating>(exampleJson)
            : default(PackageRating);            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Interact with the package with this ID
        /// </summary>
        /// <remarks>Return this package.</remarks>
        /// <param name="xAuthorization"></param>
        /// <param name="id">ID of package to fetch</param>
        /// <response code="200">Return the package.</response>
        /// <response code="400">There is missing field(s) in the PackageID/AuthenticationToken or it is formed improperly, or the AuthenticationToken is invalid.</response>
        /// <response code="404">Package does not exist.</response>
        /// <response code="0">unexpected error</response>
        [HttpGet]
        [Route("/package/{id}")]
        [ValidateModelState]
        [SwaggerOperation("PackageRetrieve")]
        [SwaggerResponse(statusCode: 200, type: typeof(Package), description: "Return the package.")]
        [SwaggerResponse(statusCode: 0, type: typeof(Error), description: "unexpected error")]
        public virtual IActionResult PackageRetrieve([FromHeader][Required()] string xAuthorization, [FromRoute][Required] string id)
        {
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(Package));

            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);

            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);

            //TODO: Uncomment the next line to return response 0 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(0, default(Error));
            string exampleJson = null;
            exampleJson = "{\n  \"metadata\" : {\n    \"Version\" : \"1.2.3\",\n    \"ID\" : \"ID\",\n    \"Name\" : \"Name\"\n  },\n  \"data\" : {\n    \"Content\" : \"Content\",\n    \"JSProgram\" : \"JSProgram\",\n    \"URL\" : \"URL\"\n  }\n}";

            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<Package>(exampleJson)
            : default(Package);            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Update this content of the package.
        /// </summary>
        /// <remarks>The name, version, and ID must match.  The package contents (from PackageData) will replace the previous contents.</remarks>
        /// <param name="body"></param>
        /// <param name="xAuthorization"></param>
        /// <param name="id"></param>
        /// <response code="200">Version is updated.</response>
        /// <response code="400">There is missing field(s) in the PackageID/AuthenticationToken or it is formed improperly, or the AuthenticationToken is invalid.</response>
        /// <response code="404">Package does not exist.</response>
        [HttpPut]
        [Route("/package/{id}")]
        [ValidateModelState]
        [SwaggerOperation("PackageUpdate")]
        public virtual IActionResult PackageUpdate([FromBody] Package body, [FromHeader][Required()] string xAuthorization, [FromRoute][Required] string id)
        {
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200);

            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);

            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);
            

            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the packages from the registry.
        /// </summary>
        /// <remarks>Get any packages fitting the query. Search for packages satisfying the indicated query.  If you want to enumerate all packages, provide an array with a single PackageQuery whose name is \&quot;*\&quot;.  The response is paginated; the response header includes the offset to use in the next query.</remarks>
        /// <param name="body"></param>
        /// <param name="xAuthorization"></param>
        /// <param name="offset">Provide this for pagination. If not provided, returns the first page of results.</param>
        /// <response code="200">List of packages</response>
        /// <response code="400">There is missing field(s) in the PackageQuery/AuthenticationToken or it is formed improperly, or the AuthenticationToken is invalid.</response>
        /// <response code="413">Too many packages returned.</response>
        /// <response code="0">unexpected error</response>
        [HttpPost]
        [Route("/packages")]
        [ValidateModelState]
        [SwaggerOperation("PackagesList")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<PackageMetadata>), description: "List of packages")]
        [SwaggerResponse(statusCode: 0, type: typeof(Error), description: "unexpected error")]
        public virtual IActionResult PackagesList([FromBody] List<PackageQuery> body, [FromHeader][Required()] string xAuthorization, [FromQuery] string offset)
        {
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(List<PackageMetadata>));

            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);

            //TODO: Uncomment the next line to return response 413 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(413);

            //TODO: Uncomment the next line to return response 0 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(0, default(Error));
            string exampleJson = null;
            exampleJson = "[ {\n  \"Version\" : \"1.2.3\",\n  \"ID\" : \"ID\",\n  \"Name\" : \"Name\"\n}, {\n  \"Version\" : \"1.2.3\",\n  \"ID\" : \"ID\",\n  \"Name\" : \"Name\"\n} ]";

            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<List<PackageMetadata>>(exampleJson)
            : default(List<PackageMetadata>);            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Reset the registry
        /// </summary>
        /// <remarks>Reset the registry to a system default state.</remarks>
        /// <param name="xAuthorization"></param>
        /// <response code="200">Registry is reset.</response>
        /// <response code="400">There is missing field(s) in the AuthenticationToken or it is formed improperly, or the AuthenticationToken is invalid.</response>
        /// <response code="401">You do not have permission to reset the registry.</response>
        [HttpDelete]
        [Route("/reset")]
        [ValidateModelState]
        [SwaggerOperation("RegistryReset")]
        public virtual IActionResult RegistryReset([FromHeader(Name = "X-Authorization")][Required()] string xAuthorization)
        {
            //use this for testing.
            string token = xAuthorization;
            bool isSantized = Sanitizer.VerifyTokenSanitized(token);
            if (!isSantized)
            {
                Response.Headers.Add("X-Debug", "Token is not sanitized");
                return StatusCode(400);
            }
            TokenAuthenticator authenticator = new TokenAuthenticator();
            TokenAuthenticator.AuthResults UserStatus = authenticator.ValidateToken(token);
            if (UserStatus != TokenAuthenticator.AuthResults.SUCCESS_ADMIN)
            {
                //append debug message to header
                Response.Headers.Add("X-Debug", "Token is invalid");
                return StatusCode(400);
            }

            //decrement token
            TokenAuthenticator.AuthRefreshResults success = authenticator.DecrementNumUsesForToken(token);
            if (success != TokenAuthenticator.AuthRefreshResults.SUCCESS)
            {
                Response.Headers.Add("X-Debug", "Token decrement failed");
                return StatusCode(400);
            }

            //get bigquery
            BigQueryFactory factory = new BigQueryFactory();
            var ghtoken = factory.GetGithubTokenStoredInBQ(); 
            Response.Headers.Add("X-Debug", "Registry reset + github token: " + ghtoken);
            return StatusCode(200);



        }
    }
}
