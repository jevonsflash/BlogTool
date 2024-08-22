using System.Text.Json.Serialization;

namespace BlogTool.Core.Aigc.DashScope;

/// <summary>
/// Represents the output section of the Text to Image response.
/// </summary>
public class DashScopeTask
{
    /// <summary>
    /// Gets or sets the job ID of the asynchronous task for the current request.
    /// The actual job result needs to be obtained through the asynchronous task query interface.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("task_id")]
    public string TaskId { get; init; } = null!;

    /// <summary>
    /// Gets or sets the job status after submitting the asynchronous task.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("task_status")]
    public DashScopeTaskStatus TaskStatus { get; init; }
}
