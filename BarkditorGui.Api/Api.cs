using System.Text;
using System.Text.Json;
using System.Net.Sockets;

namespace BarkditorGui.Api;

public static partial class Api
{
    // TODO: make port cli argument
    private static int _port { get; set; }

    public static void SetPort(int port)
    {
        _port = port;
    }

    private static RpcResponse<T> _makeRpcCall<T>(string method, string[] args)
    {
        if (_port == 0)
        {
            _port = 5000;
        }

        string req = JsonSerializer.Serialize(new RpcRequest {
            Method = method,
            Params = args,
            Id     = 0,
        });

        using TcpClient client = new TcpClient();
        client.Connect("localhost", _port);

        using NetworkStream networkStream = client.GetStream();
        networkStream.ReadTimeout = 2000;

        using StreamReader reader = new StreamReader(networkStream, Encoding.UTF8);

        byte[] bytes = Encoding.UTF8.GetBytes(req);
        networkStream.Write(bytes, 0, bytes.Length);

        byte[] data = new byte[256];
        int n = networkStream.Read(data, 0, data.Length);
        string responseData = Encoding.ASCII.GetString(data, 0, n);

        RpcResponse res = JsonSerializer.Deserialize<RpcResponse<T>>(responseData.Replace("\n", ""));
        return res;
    }
}
