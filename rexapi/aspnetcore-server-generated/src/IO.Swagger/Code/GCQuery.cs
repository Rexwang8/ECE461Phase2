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

        public List<PackageMetadata> GetPackageMetadataFromResults(BigQueryResults results)
        {
            List<PackageMetadata> packageMetadataList = new List<PackageMetadata>();
            foreach (BigQueryRow row in results)
            {
                PackageMetadata packageMetadata = new PackageMetadata();
                if (row["name"] != null)
                {
                    packageMetadata.Name = row["name"].ToString();
                }
                else
                {
                    packageMetadata.Name = "Unknown";
                }

                if (row["version"] != null)
                {
                    packageMetadata.Version = row["version"].ToString();
                }
                else
                {
                    packageMetadata.Version = "Unknown";
                }

                if (row["id"] != null)
                {
                    packageMetadata.ID = row["id"].ToString();
                }
                else
                {
                    packageMetadata.ID = "Unknown";
                }

                packageMetadataList.Add(packageMetadata);
            }
            return packageMetadataList;
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
                User user = new User();
                if (row["user_name"] != null)
                {
                    user.Name = row["user_name"].ToString();
                }
                else
                {
                    user.Name = "Unknown";
                }

                if (row["user_isadmin"] != null)
                {
                    user.IsAdmin = bool.Parse(row["user_isadmin"].ToString());
                }
                else
                {
                    user.IsAdmin = false;
                }

                PackageMetadata packageMetadata = new PackageMetadata();
                if (row["packagemetadata_name"] != null)
                {
                    packageMetadata.Name = row["packagemetadata_name"].ToString();
                }
                else
                {
                    packageMetadata.Name = "Unknown";
                }

                if (row["packagemetadata_version"] != null)
                {
                    packageMetadata.Version = row["packagemetadata_version"].ToString();
                }
                else
                {
                    packageMetadata.Version = "Unknown";
                }

                if (row["packagemetadata_id"] != null)
                {
                    packageMetadata.ID = row["packagemetadata_id"].ToString();
                }
                else
                {
                    packageMetadata.ID = "Unknown";
                }

                packageHistoryEntry.User = user;
                packageHistoryEntry.PackageMetadata = packageMetadata;

                

                //set action
                if (row["action"] != null)
                {
                    switch (row["action"].ToString())
                    {
                        case "CREATE":
                            packageHistoryEntry.Action = PackageHistoryEntry.ActionEnum.CREATEEnum;
                            break;
                        case "UPDATE":
                            packageHistoryEntry.Action = PackageHistoryEntry.ActionEnum.UPDATEEnum;
                            break;
                        case "DOWNLOAD":
                            packageHistoryEntry.Action = PackageHistoryEntry.ActionEnum.DOWNLOADEnum;
                            break;
                        case "RATE":
                            packageHistoryEntry.Action = PackageHistoryEntry.ActionEnum.RATEEnum;
                            break;
                        default:
                            packageHistoryEntry.Action = PackageHistoryEntry.ActionEnum.CREATEEnum;
                            break;
                    }
                    
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