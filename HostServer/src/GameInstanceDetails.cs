using System.Diagnostics;
using Helpers;

namespace HostServer;

public struct GameInstanceDetails
{
    public Task Task;
    public string Port;
    public string Id;
}