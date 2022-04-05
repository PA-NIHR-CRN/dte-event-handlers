namespace CognitoCustomMessageProcessor.Models
{
    public class Link
    {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }

        public override string ToString()
        {
            var link = $"{BaseUrl}?code={Code}&email={Email}";
            return @$"<a href=""{link}"">{Name ?? link}</a>";
        }
    }
}