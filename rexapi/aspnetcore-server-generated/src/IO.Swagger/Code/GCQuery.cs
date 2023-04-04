using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;


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
            try
            {
                //execute query
                BigQueryResults results = client.ExecuteQuery(query, parameters: null);
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
}

}