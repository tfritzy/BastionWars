using System.IO;
using System.Net;
using System.Text;
using Schema;

namespace MatchmakingServer.Tests
{
    [TestClass]
    public class ServerTests
    {
        [TestMethod]
        public void HandleRegisterHost_AllowlistedIP_Returns200()
        {
            var server = new Server();
            var register = new Register
            {
                Port = "7250",
            };
            var resp = server.HandleRegisterHost("::1", register);

            Assert.AreEqual(200, resp.StatusCode);
            Assert.AreEqual(1, server.ConnectedHosts.Count);
            Assert.AreEqual("::1:7250", server.ConnectedHosts.First());
        }

        [TestMethod]
        public void HandleRegisterHost_NotAllowlistedIP_Returns400()
        {
            var server = new Server();
            var register = new Register
            {
                Port = "7250",
            };
            var resp = server.HandleRegisterHost("192.168.0.1", register);

            Assert.AreEqual(400, resp.StatusCode);
            Assert.AreEqual(0, server.ConnectedHosts.Count);
        }
    }
}
