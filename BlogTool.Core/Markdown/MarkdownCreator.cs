using System.Collections.Generic;

namespace BlogTool.Core.Markdown
{
    public class MarkdownCreator
    {
        private GetMarkdownOption _option;
        private IMarkdownProvider markdownCreatorProvider;

        public ICollection<IMarkdown> Create()
        {
            var result = markdownCreatorProvider.GetMarkdowns(_option);
            return result;
        }



        public MarkdownCreator(GetMarkdownOption option, IMarkdownProvider markdownCreatorProvider) : this()
        {
            SetMarkdownProvider(option, markdownCreatorProvider);

        }

        public MarkdownCreator()
        {

        }


        public void SetMarkdownProvider(GetMarkdownOption option, IMarkdownProvider markdownCreatorProvider)
        {
            _option = option == null ? new GetMarkdownOption() : option;
            this.markdownCreatorProvider = markdownCreatorProvider;


        }
    }
}
