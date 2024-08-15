using System.IO;
using System.Net;
using System.Text;
using Google.Protobuf;
using Microsoft.AspNetCore.Identity.Data;
using Moq;
using Moq.Contrib.HttpClient;
using Schema;

namespace MatchmakingServer.Tests
{
    [TestClass]
    public class ServerTests
    {
        [TestMethod]
        public void HandleRegisterHost_AllowlistedIP_Returns200()
        {
            var server = new Server(new HttpClient());
            var register = new Register
            {
                Port = "7250",
            };
            var resp = server.HandleRegisterHost("::1", register);

            Assert.AreEqual(200, resp.StatusCode);
            Assert.AreEqual(1, server.ConnectedHosts.Count);
            Assert.AreEqual("::1:7250", server.ConnectedHosts.First());
            Assert.AreEqual("7250", resp.Body!.Port);
        }

        [TestMethod]
        public void HandleRegisterHost_NotAllowlistedIP_Returns400()
        {
            var server = new Server(new HttpClient());
            var register = new Register
            {
                Port = "7250",
            };
            var resp = server.HandleRegisterHost("192.168.0.1", register);

            Assert.AreEqual(400, resp.StatusCode);
            Assert.AreEqual(0, server.ConnectedHosts.Count);
        }

        [TestMethod]
        public void HandleRegisterHost_ParsesBothIpv4AndIpv6()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void HandleRegisterHost_DoestAddHostMultipleTimes()
        {
            var server = new Server(new HttpClient());
            var register = new Register { Port = "7250" };
            var resp = server.HandleRegisterHost("::1", register);
            Assert.AreEqual(200, resp.StatusCode);
            resp = server.HandleRegisterHost("::1", register);
            Assert.AreEqual(200, resp.StatusCode);
            Assert.AreEqual(1, server.ConnectedHosts.Count);
        }

        [TestMethod]
        public async Task HandleSearch_PlacesPlayerInConnectedHost()
        {
            GameFoundForPlayer response = new GameFoundForPlayer
            {
                PlayerId = "plyr_001",
                Address = "::1:6001",
                GameId = "game_001"
            };
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupAnyRequest()
                .ReturnsResponse(HttpStatusCode.OK, response.ToByteArray(), "application/x-protobuf");
            var httpClient = handler.CreateClient();
            httpClient.BaseAddress = new Uri("http://testhost.com");
            var server = new Server(httpClient);
            var resp = server.HandleRegisterHost("::1", new Register { Port = "7250" });

            SearchForGame searchForGame = new SearchForGame
            {
                Ranked = true,
            };

            await server.HandleSearchForGame("plyr_001", searchForGame);
        }
    }
}
