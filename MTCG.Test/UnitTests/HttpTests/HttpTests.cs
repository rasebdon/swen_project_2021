using MTCG.BL.Http;
using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttpServer = MTCG.BL.Http.HttpServer;

namespace MTCH.Test.UnitTests.HttpTest
{
    public class HttpServerTest
    {
        private HttpServer _server;
        private HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            // Arrange client and server
            _server = new(IPAddress.Any, 8080, false);
            _client = new();
        }

        [Test]
        public void GetRequest()
        {
            // Arrange
            string requestUrl = "http://127.0.0.1:8080/someget";

            // Act
            _client.GetAsync(requestUrl);
            HttpRequest request = _server.GetHttpRequest();

            // Assert
            Assert.AreEqual("someget", request.Resources[1]);
        }

        [Test]
        public void PostRequest()
        {
            // Arrange
            string requestUrl = "http://127.0.0.1:8080/somedata";
            // Data to be sent
            SomeData clientData = new(24, "coolUser");

            _client.PostAsync(requestUrl, new StringContent(clientData.Serialize()));

            // Recieve data
            HttpRequest request = _server.GetHttpRequest();
            SomeData serverData = SomeData.Deserialize(request.RequestBody);

            // Send response
            Assert.AreEqual(clientData.Username, serverData.Username);
            Assert.AreEqual(clientData.ID, serverData.ID);
            Assert.AreEqual(clientData.Serialize().Length, request.RequestBody.Length);
        }

        [Test]
        public void PostResponse()
        {
            // Arrange
            string requestUrl = "http://127.0.0.1:8080/somedata";
            // Data to be sent
            SomeData data = new(26, "coolUsers");

            // Act
            HttpRequest request;
            Task.Run(() =>
            {
                // Recieve data
                request = _server.GetHttpRequest();
                // Send response
                _server.SendHttpResponse(new HttpResponse(data.Serialize(), MTCG.BL.Http.HttpStatusCode.Created, "application/json"), request);
            });

            // For sending post requests (TESTING ONLY)
            HttpResponseMessage t1 = _client.PostAsync(requestUrl,
                new StringContent(data.Serialize())).Result;

            // Deserialize response
            Stream s = t1.Content.ReadAsStream();
            byte[] b = new byte[s.Length];
            s.Read(b, 0, (int)s.Length);
            string response = System.Text.Encoding.UTF8.GetString(b);

            Assert.AreEqual(data.Serialize(), response);
        }

        [TearDown]
        public void TearDown()
        {
            _server.Close();
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
