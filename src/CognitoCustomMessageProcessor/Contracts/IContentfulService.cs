using System.Globalization;
using System.Threading.Tasks;
using CognitoCustomMessageProcessor.Models;

namespace CognitoCustomMessageProcessor.Contracts;

public interface IContentfulService
{
    Task<ContentfulEmail> GetContentfulEmailAsync(string entryId, string locale);
    Task<ContentfulEmailResponse> GetEmailContentAsync(string emailName, CultureInfo selectedLocale, string link = null);
}