using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogTool.Core.Aigc.Tuning
{
    public class PromptConsts
    {
        public const string DescriptionGenerationMsg = "你是一个摘要生成工具，你需要解释我发送给你的内容，不要换行，不要超过200字，只需要介绍文章的内容，不需要提出建议和缺少的东西，只包含纯文本，不要用Markdown格式。请用中文回答，文章内容为：";

    }
}
