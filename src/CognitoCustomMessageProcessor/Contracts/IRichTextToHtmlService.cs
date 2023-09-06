using CognitoCustomMessageProcessor.Models;

namespace CognitoCustomMessageProcessor.Contracts;

public interface IRichTextToHtmlService
{
    string Convert(RichTextNode richTextNode);
}