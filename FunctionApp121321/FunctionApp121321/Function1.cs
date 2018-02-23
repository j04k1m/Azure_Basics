using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace FunctionApp121321
{
    public static class Function1
    {

        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {

            string EndpointUrl = "https://azurepractise.documents.azure.com:443/";
            string PrimaryKey = "FwJ5WzGBH3jFawW7VHZuyFb7C05FNXfj0drbsqueh620yYpq9wkYOtmdfVOBgUiGEGalFuklslVYmF4sd03HXA==";
            DocumentClient client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);

            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;

            string asdf = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "asdf", true) == 0)
                .Value;

           /* var ReviewQueue = (from x in req.GetQueryNameValuePairs() 
                                  where x.Key == "ReviewQueue"
                                  select x.Value).FirstOrDefault(); */

            string mode = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "mode", true) == 0)
                .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            name = name ?? data?.name;


            if (mode == "viewReviewQueue")
            {
                var reviewQpictures = UsersInReviewQueue();

                return req.CreateResponse(HttpStatusCode.OK, reviewQpictures, "application/json");
            }
            else
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "empty or invalid");
            }
        }
        
        private static List<ReviewQueue> UsersInReviewQueue()
        {
            string EndpointUrl = "https://azurepractise.documents.azure.com:443/";
            string PrimaryKey = "FwJ5WzGBH3jFawW7VHZuyFb7C05FNXfj0drbsqueh620yYpq9wkYOtmdfVOBgUiGEGalFuklslVYmF4sd03HXA==";
            DocumentClient client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);

            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };
            
            IQueryable<ReviewQueue> userSql = client.CreateDocumentQuery<ReviewQueue>(UriFactory.CreateDocumentCollectionUri("AzureCosmosPractiseDB", "ReviewQueue"),
            "SELECT * FROM ReviewQueue", queryOptions);

            var AllPicturesInReview = userSql.ToList();

            return AllPicturesInReview;
        } 

        public class ReviewQueue
        {
            public string ReviewPicture { get; set; }
        }
    }
}
