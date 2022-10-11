using RestSharp;
using System.Net;
using static System.Net.WebRequestMethods;

namespace Homework2._2
{
    [TestClass]
    public class Homework2Point2Test
    {
        private static RestClient restClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string PetEndpoint = "pet";

        private static string GetURL(string enpoint) => $"{BaseURL}{enpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetModel> cleanUpList = new List<PetModel>();

        [TestInitialize]
        public void TestInitialize()
        {
            restClient = new RestClient();
        }

        //Create a cleanup method using DELETE request ​
        [TestCleanup]
        public async Task TestCleanUp()
        {
            foreach (var data in cleanUpList)
            {
                var restRequest = new RestRequest(GetURI($"{PetEndpoint}/{data.id}"));
                await restClient.DeleteAsync(restRequest);
            }
        }

        [TestMethod]
        public async Task CreatePetMethod()
        {
            //Request Payload should contain pet name, category, photo URLS, tags, and status​
            PetModel petData = new PetModel();
            petData.id = 123123;
            petData.category = new Category { id = 0, name = "string" };
            petData.name = "Whitey";
            petData.photoUrls = new string[] { "test1", "test2" };
            petData.tags = new Tag[] { new Tag() { id = 0, name = "string" } };
            petData.status = "available";

            var postRestRequest = new RestRequest(GetURI(PetEndpoint)).AddJsonBody(petData);
            var postRestResponse = await restClient.ExecutePostAsync(postRestRequest);
            var x = postRestResponse;

            #region GetPet
            var restRequest = new RestRequest(GetURI($"{PetEndpoint}/{petData.id}"), Method.Get);
            var restResponse = await restClient.ExecuteAsync<PetModel>(restRequest);
            #endregion

            
            
            #region Assertions
            // Add an Assertion for HTTP Status Code​
            Assert.AreEqual(HttpStatusCode.OK, restResponse.StatusCode, "Status code is not equal to 200");
            //Add an Assertion if pet name, category, photo URLS, tags, and status are correctly reflected
            Assert.AreEqual(petData.name, restResponse.Data.name, "Pet Name did not match.");
            Assert.AreEqual(petData.category.name, restResponse.Data.category.name, "Category did not match.");
            
            for (int i = 0; i < restResponse.Data.photoUrls.Count(); i++)
            {
                Assert.AreEqual(petData.photoUrls[i], restResponse.Data.photoUrls[i], "Photourl did not match.");
            }
            for (int y = 0; y < restResponse.Data.tags.Count(); y++)
            {
                Assert.AreEqual(petData.tags[y].name, restResponse.Data.tags[y].name, "Tags did not match.");
            }
            
            Assert.AreEqual(petData.status, restResponse.Data.status, "Status did not match.");
            #endregion

            #region CleanUp
            cleanUpList.Add(petData);
            #endregion
        }
    }
}