using System.Collections.Generic;
using System.Text;
using CognitoCustomMessageProcessor.Contracts;
using CognitoCustomMessageProcessor.Models;

namespace CognitoCustomMessageProcessor.Builders
{
    public class LinkBuilder : ILinkBuilder
    {
        private readonly List<Link> _links = new ();
        
        public ILinkBuilder AddLink(string name, string baseUrl, string code, string userId)
        {
            var link = new Link
            {
                Name = name, BaseUrl = baseUrl, Code = code, UserId = userId
            };

            _links.Add(link);

            return this;
        }

        public string Build()
        {
            var sb = new StringBuilder();

            foreach (var link in _links)
            {
                sb.Append($"{link}</br>");
            }

            return sb.ToString();
        }
    }
}
