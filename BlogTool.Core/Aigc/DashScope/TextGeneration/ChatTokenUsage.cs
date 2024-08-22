using System.Text.Json.Serialization;

namespace BlogTool.Core.Aigc.DashScope.TextGeneration;

/// <summary>
/// Token usage of the chat request.
/// </summary>
public record ChatTokenUsage
{
    /// <summary>
    /// Output token count of generated text.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("output_tokens")]
    public int OutputTokens { get; init; }

    /// <summary>
    /// Input token count of messages.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("input_tokens")]
    public int InputTokens { get; init; }
}