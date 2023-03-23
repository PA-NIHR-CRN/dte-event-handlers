namespace CognitoCustomMessageProcessor.Models
{
    public class Link
    {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string Code { get; set; }
        public string UserId { get; set; }

        public override string ToString()
        {
            var link = $"{BaseUrl}?code={Code}&userId={UserId}";
            return @$"<a href=""{link}"">{Name ?? link}</a>";
        }
    }
}