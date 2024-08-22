using BlogTool.Core.Aigc.DashScope;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace BlogTool.Core.Aigc.DashScope.TextGeneration;

/// <summary>
/// LLM models clients that supports Qwen/open source LLMs.
/// </summary>
public class TextGenerationClient
{
    internal TextGenerationClient(DashScopeClient parent)
    {
        Parent = parent;
    }

    internal DashScopeClient Parent { get; }

    /// <summary>
    /// Sends a chat interaction to the DashScope large language model and returns a response.
    /// </summary>
    /// <param name="model">
    /// The specified model identifier for processing the chat interaction, known models:
    /// <list type="bullet">
    /// <item>
    /// Qwen series, options:
    /// <list type="bullet">
    /// <item>qwen-turbo</item>
    /// <item>qwen-plus</item>
    /// <item>qwen-max</item>
    /// <item>qwen-max-1201</item>
    /// <item>qwen-max-longcontext</item>
    /// </list>
    /// </item>
    /// <item>
    /// Qwen open source series, options:
    /// <list type="bullet">
    /// <item>qwen-72b-chat</item>
    /// <item>qwen-14b-chat</item>
    /// <item>qwen-7b-chat</item>
    /// <item>qwen-1.8b-longcontext-chat</item>
    /// <item>qwen-1.8b-chat</item>
    /// </list>
    /// </item>
    /// </list>
    /// </param>
    /// <param name="messages">A read-only list of chat messages representing the conversation history.</param>
    /// <param name="parameters">Optional parameters to customize the chat behavior.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <see cref="ResponseWrapper{TOutput, TUsage}"/> object with the <see cref="ChatOutput"/> as the result of the interaction and 
    /// <see cref="ChatTokenUsage"/> that provides metadata about the token usage for the conversation.
    /// </returns>
    public async Task<ResponseWrapper<ChatOutput, ChatTokenUsage>> Chat(string model, IReadOnlyList<ChatMessage> messages, ChatParameters parameters = null, CancellationToken cancellationToken = default)
    {
        var content = JsonConvert.SerializeObject(RequestWrapper.Create(model, new
        {
            messages,
        }, parameters), new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });
        HttpRequestMessage httpRequest = new(HttpMethod.Post, @"https://dashscope.aliyuncs.com/api/v1/services/aigc/text-generation/generation")
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };
        HttpResponseMessage resp = await Parent.HttpClient.SendAsync(httpRequest, cancellationToken);
        return await DashScopeClient.ReadResponse<ResponseWrapper<ChatOutput, ChatTokenUsage>>(resp, cancellationToken);
    }
}
