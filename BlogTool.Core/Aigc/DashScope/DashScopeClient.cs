using BlogTool.Core.Aigc.DashScope.StableDiffusion;
using BlogTool.Core.Aigc.DashScope.TextGeneration;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BlogTool.Core.Aigc.DashScope;

/// <summary>
/// Represents a client for interacting with the DashScope API.
/// </summary>
public class DashScopeClient : IDisposable, IAigcClient
{
    public const string Name = "DashScope";

    internal readonly HttpClient HttpClient = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="DashScopeClient"/> class with the specified API key.
    /// </summary>
    /// <param name="apiKey">The API key used for authentication.</param>
    /// <param name="httpClient">The HTTP client used for making requests. If null, a new instance of <see cref="System.Net.Http.HttpClient"/> will be created.</param>
    [SetsRequiredMembers]
    public DashScopeClient(string apiKey, HttpClient httpClient = null)
    {
        HttpClient = httpClient ?? new HttpClient();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        StableDiffusion = new StableDiffusionClient(this);
        TextGeneration = new TextGenerationClient(this);
    }



    /// <summary>
    /// The Stable Diffusion API provides a series of AI models that can be used to generate images from text.
    /// </summary>
    public StableDiffusionClient StableDiffusion { get; }


    /// <summary>
    /// LLM models clients that supports Qwen/open source LLMs.
    /// </summary>
    public TextGenerationClient TextGeneration { get; }

    /// <summary>
    /// Queries the status of a task using the specified task ID.
    /// </summary>
    /// <param name="taskId">The ID of the task to query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task status response.</returns>
    public async Task<TaskStatusResponse> QueryTaskStatus(string taskId, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage resp = await HttpClient.GetAsync($@"https://dashscope.aliyuncs.com/api/v1/tasks/{taskId}", cancellationToken);
        return await ReadWrapperResponse<TaskStatusResponse>(resp, cancellationToken);
    }

    /// <summary>
    /// Disposes the underlying HTTP client.
    /// </summary>
    public void Dispose() => HttpClient.Dispose();

    internal async Task<T> ReadWrapperResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        return (await ReadResponse<ResponseWrapper<T, ImageTaskUsage>>(response, cancellationToken)).Output;
    }

    internal static async Task<T> ReadResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw new DashScopeException(await response.Content.ReadAsStringAsync());
        }

        try
        {
            var debug = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(debug)!;
        }
        catch (Exception e) when (e is NotSupportedException or JsonException)
        {
            throw new DashScopeException($"Failed to convert the following JSON into {typeof(T).Name}: {await response.Content.ReadAsStringAsync()}", e);
        }
    }
}
