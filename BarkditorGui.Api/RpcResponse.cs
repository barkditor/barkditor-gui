using System.Text.Json.Serialization;

namespace BarkditorGui.Api;

public class RpcResponse<T>
{
    [JsonPropertyName("result")]
    public T      Result { get; init; }

    [JsonPropertyName("error")]
    public string Error  { get; init; }

    [JsonPropertyName("id")]
    public int    Id     { get; init; }
}
