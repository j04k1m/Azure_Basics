using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace DatabasLab4
{
    class Program
    {
        private const string EndpointUrl = "https://azurepractise.documents.azure.com:443/";
        private const string PrimaryKey = "FwJ5WzGBH3jFawW7VHZuyFb7C05FNXfj0drbsqueh620yYpq9wkYOtmdfVOBgUiGEGalFuklslVYmF4sd03HXA==";
        private DocumentClient client;

        static void Main(string[] args)
        {
            try
            {
                Program p = new Program();
                p.InitialCreate().Wait();
                p.Menu();
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }

        private async Task InitialCreate()
        {
            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);

            await this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = "AzureCosmosPractiseDB" });

            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("AzureCosmosPractiseDB"), new DocumentCollection { Id = "User" });
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("AzureCosmosPractiseDB"), new DocumentCollection { Id = "ReviewQueue" });
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("AzureCosmosPractiseDB"), new DocumentCollection { Id = "ApprovedPictures" });

            //Create

            //this.ExecuteSimpleQuery("AzureCosmosPractiseDB", "PractiseCollection");


            /*Update 
            andersenFamily.Children[0].Grade = 6; 

            await this.ReplaceFamilyDocument("AzureCosmosPractiseDB", "PractiseCollection", "Andersen.1", andersenFamily);

            this.ExecuteSimpleQuery("AzureCosmosPractiseDB", "PractiseCollection"); */

            //Delete Document
            //await this.DeleteFamilyDocument("AzureCosmosPractiseDB", "PractiseCollection", "Andersen.1");

            //Clean up/delete the database
            //await this.client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri("AzureCosmosPractiseDB"));
        }

        private async Task CreateUser()
        {
            Console.WriteLine("Enter Id: ");
            string id = Console.ReadLine();
            Console.WriteLine("Enter name: ");
            string name = Console.ReadLine();
            Console.WriteLine("Enter email: ");
            string email = Console.ReadLine();
            Console.WriteLine("Enter profile picture URL: ");
            string profilepictureURL = Console.ReadLine();
            Console.WriteLine("Enter picture Id: ");
            string pictureid = Console.ReadLine();

            User newUser = new User
            {
                id = id,
                name = name,
                email = email,
            };

            ReviewPictureQueue newReviewPicture = new ReviewPictureQueue
            {
                id = pictureid,
                ReviewPicture = profilepictureURL
            };

            await this.CreateUserDocumentIfNotExists("AzureCosmosPractiseDB", "User", newUser);
            await this.CreateReviewDocumentIfNotExists("AzureCosmosPractiseDB", "ReviewQueue", newReviewPicture);
        }

        public void Menu()
        {
            Console.WriteLine("");
            Console.WriteLine("Press 1 to add User");
            Console.WriteLine("Press 2 to check users");
            Console.WriteLine("Press 3 to check review queue");
            Console.WriteLine("Press 4 to exit");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                CreateUser().Wait();
                Menu();
            }
            if (choice == "2")
            {
                ViewUsers();
                Menu();
            }
            if (choice == "3")
            {
                ViewReviewQueue();
                Menu();
            }
            if (choice == "4")
            {

            }
        }

        public void ViewReviewQueue()
        {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };
            Console.WriteLine("\nPictures in review: ");
            IQueryable<ReviewPictureQueue> rq = this.client.CreateDocumentQuery<ReviewPictureQueue>(UriFactory.CreateDocumentCollectionUri("AzureCosmosPractiseDB", "ReviewQueue"),
            "SELECT * FROM ReviewQueue", queryOptions);

            foreach (var item in rq)
            {
                Console.WriteLine(item.ReviewPicture);
            }
        }

        public void ViewUsers()
        {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };
            Console.WriteLine("\nUsers: ");
            IQueryable<User> rq = this.client.CreateDocumentQuery<User>(UriFactory.CreateDocumentCollectionUri("AzureCosmosPractiseDB", "User"),
            "SELECT * FROM User", queryOptions);

            foreach (var item in rq)
            {
                Console.WriteLine(item.name);
            }
        }

        private async Task CreateUserDocumentIfNotExists(string databaseName, string collectionName, User user)
        {
            try
            {
                await this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, user.name));
                this.WriteToConsoleAndPromptToContinue("Found {0}", user.name);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), user);
                    this.WriteToConsoleAndPromptToContinue("Created user {0}", user.name);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateReviewDocumentIfNotExists(string databaseName, string collectionName, ReviewPictureQueue reviewPicture)
        {
            try
            {
                await this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, reviewPicture.ReviewPicture));
                this.WriteToConsoleAndPromptToContinue("Found {0}", reviewPicture.ReviewPicture);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), reviewPicture);
                    this.WriteToConsoleAndPromptToContinue("Picture sent for review: {0}", reviewPicture.ReviewPicture);
                }
                else
                {
                    throw;
                }
            }
        }

        private void WriteToConsoleAndPromptToContinue(string format, params object[] args)
        {
            Console.WriteLine(format, args);
            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }


    }
}
