namespace BarkditorGui.Api;

public static partial class Api
{
    public static bool Heartbeat()
    {
        RpcResponse res = _makeRpcCall<bool>("API.Heartbeat", new string[] {""});
        return res.Result == "true" ? true : false;
    }
}
