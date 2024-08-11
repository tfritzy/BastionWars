using System.Diagnostics;
using Helpers;

namespace HostServer;

public struct GameInstanceDetails
{
    public Task Task;
    public int Port;
    public string Id;
}