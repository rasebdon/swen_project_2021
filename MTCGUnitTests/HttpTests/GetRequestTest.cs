using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTCG.Http;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace HttpTests
{
    [TestClass]
    public class GetTests
    {
        [TestMethod]
        public void GetRequest()
        {
            // Self programmed socket
            HttpSocket socket = new(80);
            socket.Start();

            string requestUrl = "http://127.0.0.1/user";
            SendGetRequest(requestUrl);

            // Recieve data
            HttpRequest request = socket.GetRequest();

            Assert.AreEqual(requestUrl, request.Url.AbsoluteUri);

            socket.Close();
        }

        Task SendGetRequest(string url)
        {
            // For sending get requests (TESTING ONLY)
            HttpClient _httpClient = new();
            return _httpClient.GetAsync(url);
        }
    }
        
    [TestClass]
    public class PostTests
    {
        [TestMethod]
        public void PostRequest()
        {
            HttpSocket socket = new(80);
            socket.Start();

            // Data to be sent
            SomeData clientData = new(24, "coolUser");
            SendPostRequest("http://127.0.0.1/somedata", clientData);

            // Recieve data
            HttpRequest request = socket.GetRequest();
            SomeData serverData = SomeData.Deserialize(request.RequestBody);

            Assert.AreEqual(clientData.Username, serverData.Username);
            Assert.AreEqual(clientData.ID, serverData.ID);
            Assert.AreEqual(clientData.Serialize().Length, request.RequestBody.Length);

            socket.Close();
        }

        Task SendPostRequest(string url, SomeData data)
        {
            // For sending post requests (TESTING ONLY)
            HttpClient _httpClient = new();
            return _httpClient.PostAsync(url, new StringContent(data.Serialize()));
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
