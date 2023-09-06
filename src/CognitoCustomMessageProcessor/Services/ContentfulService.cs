using System.Globalization;
using System.Threading.Tasks;
using CognitoCustomMessageProcessor.Content;
using CognitoCustomMessageProcessor.Contracts;
using CognitoCustomMessageProcessor.Models;
using Contentful.Core;
using Contentful.Core.Search;
using HandlebarsDotNet;

namespace CognitoCustomMessageProcessor.Services;

public class ContentfulService: IContentfulService
{
    private readonly IContentfulClient _client;
    private readonly IRichTextToHtmlService _richTextToHtmlConverter;

    public ContentfulService(IContentfulClient client, IRichTextToHtmlService richTextToHtmlConverter)
    {
        _client = client;
        _richTextToHtmlConverter = richTextToHtmlConverter;
    }

    public async Task<ContentfulEmail> GetContentfulEmailAsync(string entryId, string locale = "en-GB")
    {
        var entry = await _client.GetEntry(entryId, new QueryBuilder<ContentfulEmail>().LocaleIs(locale));
        return entry;
    }

    public async Task<ContentfulEmailResponse> GetEmailContentAsync(string emailName, CultureInfo selectedLocale, string link = null)
    {
        var contentfulEmail = await GetContentfulEmailAsync(emailName, selectedLocale.ToString());
        string htmlContent = _richTextToHtmlConverter.Convert(contentfulEmail.EmailBody);

        var htmlBody = CustomMessageEmail.GetCustomMessageHtml().Replace("###BODY_REPLACE###", htmlContent);
        
        if (link != null)
        {
            var template = Handlebars.Compile(htmlBody);
            var data = new
            {
                Link = link
            };
            htmlBody = template(data);
        }

        return new ContentfulEmailResponse
        {
            EmailSubject = contentfulEmail.EmailSubject,
            EmailBody = htmlBody
        };
    }
}