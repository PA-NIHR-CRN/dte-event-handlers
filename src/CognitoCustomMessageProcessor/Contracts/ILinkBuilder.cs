namespace CognitoCustomMessageProcessor.Contracts
{
    public interface ILinkBuilder
    {
        ILinkBuilder AddLink(string name, string baseUrl, string code, string email);
        string Build();
    }
}