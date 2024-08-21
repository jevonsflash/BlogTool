using System.Collections.Generic;

namespace BlogTool.Core.Markdown
{
    public class MarkdownCreator
    {
        private GetMarkdownOption _option;
        private IMarkdownProvider markdownCreatorProvider;

        public ICollection<IMarkdown> Create(params object[] objects)
        {
            var result = markdownCreatorProvider.GetMarkdowns(_option, objects);
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
