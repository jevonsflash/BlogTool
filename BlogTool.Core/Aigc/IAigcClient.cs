using BlogTool.Core.Aigc.DashScope.StableDiffusion;
using BlogTool.Core.Aigc.DashScope.TextGeneration;
using System.Threading;
using System.Threading.Tasks;

namespace BlogTool.Core.Aigc.DashScope
{
    public interface IAigcClient
    {
        StableDiffusionClient StableDiffusion { get; }
        TextGenerationClient TextGeneration { get; }

        Task<TaskStatusResponse> QueryTaskStatus(string taskId, CancellationToken cancellationToken = default);
    }
}