using System.Text.Json.Serialization;

namespace BlogTool.Core.Aigc.DashScope;

/// <summary>
/// Helper class representing task performance metrics.
/// </summary>
public record TaskMetrics
{
    /// <summary>
    /// Gets or initializes the total number of tasks.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("TOTAL")]
    public int Total { get; init; }

    /// <summary>
    /// Gets or initializes the number of tasks that succeeded.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("SUCCEEDED")]
    public int Succeeded { get; init; }

    /// <summary>
    /// Gets or initializes the number of tasks that failed.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("FAILED")]
    public int Failed { get; init; }
}
