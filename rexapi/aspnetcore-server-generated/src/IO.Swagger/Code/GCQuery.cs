using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using Newtonsoft.Json;
using IO.Swagger.Models;


namespace IO.Swagger.Controllers
{
    public class BigQueryFactory
    {
        //client
        private BigQueryClient client;

        //query
        private string query;

        //init 
        public BigQueryFactory()
        {
            //init from default service account
            var credentials = GoogleCredential.GetApplicationDefault();
            client = BigQueryClient.Create("package-registry-461", credentials);
        }

        //execute query
        public BigQueryResults ExecuteQuery()
        {
            //execute query
            BigQueryResults results = null;
            try
            {
                //execute query
                results = client.ExecuteQuery(query, parameters: null);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error executing query: " + e.Message);
                return null;
            }


            //return results
            return results;
        }

        //set query
        public void SetQuery(string query)
        {
            this.query = query;
        }

        //get query
        public string GetQuery()
        {
            return this.query;
        }

        //print results
        public void PrintResults(BigQueryResults results)
        {
            try
            {
                //print results
                foreach (BigQueryRow row in results)
                {
                    foreach (var field in row.Schema.Fields)
                    {
                        Console.WriteLine("{0}: {1}", field.Name, row[field.Name]);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error printing results: " + e.Message);
            }
        }

        public List<PackageHistoryEntry> GetPackageHistoryFromResults(BigQueryResults results)
        {
            //init list
            List<PackageHistoryEntry> packageHistory = new List<PackageHistoryEntry>();

            //loop through results
            foreach (BigQueryRow row in results)
            {
                //init package history entry
                PackageHistoryEntry packageHistoryEntry = new PackageHistoryEntry();

                //set date
                if (row["date"] != null)
                {
                    packageHistoryEntry.Date = DateTime.Parse(row["date"].ToString());
                }
                else
                {
                    packageHistoryEntry.Date = DateTime.Now;
                }


                //set user
                packageHistoryEntry.User = new User();
                BigQueryRow userRecord = row["user"]as BigQueryRow;
                if (userRecord != null)
                {
                    packageHistoryEntry.User.Name = userRecord["name"].ToString();
                    packageHistoryEntry.User.IsAdmin = bool.Parse(userRecord["isadmin"].ToString());
                }
                else
                {
                    packageHistoryEntry.User.Name = "Unknown";
                    packageHistoryEntry.User.IsAdmin = false;
                }

                


                

                //set package metadata
                packageHistoryEntry.PackageMetadata = new PackageMetadata();
                BigQueryRow packageMetadataRecord = row["packagemetadata"] as BigQueryRow;

                if (packageMetadataRecord != null)
                {
                    packageHistoryEntry.PackageMetadata.Name = packageMetadataRecord["name"].ToString();
                    packageHistoryEntry.PackageMetadata.Version = packageMetadataRecord["version"].ToString();
                    packageHistoryEntry.PackageMetadata.ID = packageMetadataRecord["id"].ToString();
                }
                else
                {
                    packageHistoryEntry.PackageMetadata.Name = "Unknown";
                    packageHistoryEntry.PackageMetadata.Version = "Unknown";
                    packageHistoryEntry.PackageMetadata.ID = "Unknown";
                }

                

                //set action
                if (row["action"] != null)
                {
                    packageHistoryEntry.Action = (PackageHistoryEntry.ActionEnum)Enum.Parse(typeof(PackageHistoryEntry.ActionEnum), row["action"].ToString());
                }
                else
                {
                    packageHistoryEntry.Action = PackageHistoryEntry.ActionEnum.CREATEEnum;
                }

                //add to list
                packageHistory.Add(packageHistoryEntry);
            }

            //return list
            return packageHistory;
        }

}

}