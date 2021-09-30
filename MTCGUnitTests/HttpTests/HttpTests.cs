using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTCG.Http;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MTCGUnitTests.HttpTests
{
    [TestClass]
    public class GetTests
    {
        [TestMethod]
        public void GetRequest()
        {
            // Start http server
            HttpClient server = new(80);

            string requestUrl = "http://127.0.0.1/user";
            SendGetRequest(requestUrl);

            // Recieve data
            HttpRequest request = server.GetHttpRequest();

            Assert.AreEqual(requestUrl, request.Url.AbsoluteUri);

            server.Close();
        }

        Task SendGetRequest(string url)
        {
            // For sending get requests (TESTING ONLY)
            System.Net.Http.HttpClient _httpClient = new();
            return _httpClient.GetAsync(url);
        }
    }
        
    [TestClass]
    public class PostTests
    {
        [TestMethod]
        public void PostRequest()
        {
            // Start http server
            HttpClient server = new(80);

            // Data to be sent
            SomeData clientData = new(24, "coolUser");
            SendPostRequest("http://127.0.0.1/somedata", clientData);

            // Recieve data
            HttpRequest request = server.GetHttpRequest();
            SomeData serverData = SomeData.Deserialize(request.RequestBody);

            // Send response
            Assert.AreEqual(clientData.Username, serverData.Username);
            Assert.AreEqual(clientData.ID, serverData.ID);
            Assert.AreEqual(clientData.Serialize().Length, request.RequestBody.Length);

            server.Close();
        }

        [TestMethod]
        public void PostResponse()
        {
            // Data to be sent
            SomeData data = new(26, "coolUsers");
            Task.Run(() => AnswerPostResponse(data));

            // For sending post requests (TESTING ONLY)
            System.Net.Http.HttpClient _httpClient = new();
            Task<System.Net.Http.HttpResponseMessage> t1 = _httpClient.PostAsync(
                "http://127.0.0.1/somedata", 
                new System.Net.Http.StringContent(data.Serialize()));

            // Deserialize response
            Stream s = t1.Result.Content.ReadAsStream();
            byte[] b = new byte[s.Length];
            s.Read(b, 0, (int)s.Length);
            string response = System.Text.Encoding.UTF8.GetString(b);

            Assert.AreEqual(data.Serialize(), response);
        }

        Task SendPostRequest(string url, SomeData data)
        {
            // For sending post requests (TESTING ONLY)
            System.Net.Http.HttpClient _httpClient = new();
            return _httpClient.PostAsync(url, new System.Net.Http.StringContent(data.Serialize()));
        }

        void AnswerPostResponse(SomeData actualData)
        {
            // Start http server
            HttpClient server = new(80);            
            
            // Recieve data
            HttpRequest request = server.GetHttpRequest();

            // Send response and close the server
            server.SendHttpResponse(new HttpResponse(actualData.Serialize(), HttpStatusCode.Created, "application/json"), request);
            server.Close();
        }
    }

    /// <summary>
    /// Data for testing data transfer
    /// </summary>
    class SomeData
    {
        public uint ID { get; }
        public string Username { get; }

        public SomeData(uint id, string username)
        {
            ID = id;
            Username = username;
        }

        private SomeData() { }
        public static SomeData Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<SomeData>(json);
        }
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
