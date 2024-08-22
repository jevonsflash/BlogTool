using System.Text.Json.Serialization;

namespace BlogTool.Core.Aigc.DashScope;

/// <summary>
/// Generic base response class for image request async task.
/// </summary>
public record ResponseWrapper<TOutput, TUsage>
{
    /// <summary>
    /// The identifier corresponds to each individual request.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("request_id")]
    public required string RequestId { get; init; }

    /// <summary>
    /// The processed task status response associated with the respective request.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("output")]
    public required TOutput Output { get; init; }

    /// <summary>
    /// Usage of the request.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("usage")]
    public TUsage? Usage { get; init; }
}
