using System.Collections.Generic;

namespace CognitoCustomMessageProcessor.Models;

public class RichTextNode
{
    public string NodeType { get; set; }
    public Dictionary<string, object> Data { get; set; }
    public List<RichTextNode> Content { get; set; }
    public string Value { get; set; }
}