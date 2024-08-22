using System.Text.Json.Serialization;

namespace BlogTool.Core.Aigc.DashScope;

/// <summary>
/// Additional information regarding the resource usage of the task.
/// </summary>
public class ImageTaskUsage
{
    /// <summary>
    /// Image count
    /// </summary>
    [Newtonsoft.Json.JsonProperty("image_count")]
    public int ImageCount { get; set; }
}
