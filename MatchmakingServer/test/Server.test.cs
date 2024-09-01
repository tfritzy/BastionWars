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
            Assert.AreEqual("[::1]:7250", server.ConnectedHosts.First());
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
            var server = new Server(new HttpClient());
            var register = new Register
            {
                Port = "7250",
            };
            server.HandleRegisterHost("127.0.0.1", register);
            server.HandleRegisterHost("::1", register);

            Assert.AreEqual(2, server.ConnectedHosts.Count);
            Assert.AreEqual("127.0.0.1:7250", server.ConnectedHosts[0]);
            Assert.AreEqual("[::1]:7250", server.ConnectedHosts[1]);
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
            Oneof_HostServerToMatchmaker found = new Oneof_HostServerToMatchmaker()
            {
                GameAvailableOnPort = new()
                {
                    PlayerId = "plyr_001",
                    Port = "6001",
                    GameId = "game_001"
                }
            };
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupAnyRequest()
                .ReturnsResponse(HttpStatusCode.OK, found.ToByteArray(), "application/x-protobuf");
            var httpClient = handler.CreateClient();
            httpClient.BaseAddress = new Uri("http://testhost.com");
            var server = new Server(httpClient);
            var resp = server.HandleRegisterHost("::1", new Register { Port = "7250" });

            var response = await server.HandleSearchForGame(BuildSearchBody());
            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual("plyr_001", response.Body!.PlayerId);
            Assert.AreEqual("ws://[::1]:6001", response.Body!.Address);
            Assert.AreEqual("game_001", response.Body!.GameId);
        }

        [TestMethod]
        public async Task HandleSearch_CleansUpDeadHost()
        {
            Oneof_HostServerToMatchmaker found = new Oneof_HostServerToMatchmaker()
            {
                GameAvailableOnPort = new()
                {
                    PlayerId = "plyr_001",
                    Port = "8250",
                    GameId = "game_001"
                }
            };
            var handler = new Mock<HttpMessageHandler>();
            handler
                .SetupRequest(HttpMethod.Post, "http://[::1]:7250/place-player")
                .ReturnsResponse(HttpStatusCode.BadGateway);
            handler
                .SetupRequest(HttpMethod.Post, "http://[::1]:8250/place-player")
                .ReturnsResponse(
                    HttpStatusCode.OK,
                    found.ToByteArray(),
                    "application/x-protobuf");
            var httpClient = handler.CreateClient();
            var server = new Server(httpClient);
            server.HandleRegisterHost("::1", new Register { Port = "7250" });
            server.HandleRegisterHost("::1", new Register { Port = "8250" });

            Assert.AreEqual(2, server.ConnectedHosts.Count);
            var response = await server.HandleSearchForGame(BuildSearchBody());
            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual("plyr_001", response.Body!.PlayerId);
            Assert.AreEqual("ws://[::1]:8250", response.Body!.Address);
            Assert.AreEqual("game_001", response.Body!.GameId);
            Assert.AreEqual(1, server.ConnectedHosts.Count);
        }

        [TestMethod]
        public async Task HandleSearch_HandlesNoHosts()
        {
            var handler = new Mock<HttpMessageHandler>();
            var httpClient = handler.CreateClient();
            var server = new Server(httpClient);

            var response = await server.HandleSearchForGame(BuildSearchBody());
            Assert.AreEqual(500, response.StatusCode);
        }

        [TestMethod]
        public async Task HandleSearch_NoPlayerNoGame()
        {
            var handler = new Mock<HttpMessageHandler>();
            var httpClient = handler.CreateClient();
            var server = new Server(httpClient);

            var response = await server.HandleSearchForGame(BuildSearchBody(""));
            Assert.AreEqual(400, response.StatusCode);
        }

        private static Oneof_PlayerToMatchmaker BuildSearchBody(string playerId = "plyr_001")
        {
            return new()
            {
                PlayerId = playerId,
                SearchForGame = new SearchForGame
                {
                    Ranked = true,
                }
            };
        }
    }
}
