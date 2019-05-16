namespace BusinessLogic
{
    public class Content : IContent
    {
        public Content(string rawContent)
        {
            RawContent = rawContent;
        }

        /// <inheritdoc />
        public string RawContent { get; }
    }
}