using System.Text;
using CognitoCustomMessageProcessor.Builders;
using CognitoCustomMessageProcessor.Models;
using NUnit.Framework;

namespace CognitoCustomMessageProcessorTests.Builders;

public class LinkBuilderTests
{
    [Test]
    public void AddLink_ShouldAddLinkToList()
    {
        // Arrange
        var linkBuilder = new LinkBuilder();
        var name = "TestLink";
        var baseUrl = "http://example.com";
        var code = "123abc";
        var userId = "user1";

        // Act
        linkBuilder.AddLink(name, baseUrl, code, userId);
        var result = linkBuilder.Build();

        // Assert
        var expectedLink = new Link { Name = name, BaseUrl = baseUrl, Code = code, UserId = userId }.ToString() + "</br>";
        StringAssert.Contains(expectedLink, result);
    }

    [Test]
    public void Build_ShouldReturnCorrectHtmlString()
    {
        // Arrange
        var linkBuilder = new LinkBuilder();
        linkBuilder.AddLink("Link1", "http://example.com", "code1", "user1");
        linkBuilder.AddLink("Link2", "http://example.org", "code2", "user2");

        // Act
        var result = linkBuilder.Build();

        // Assert
        var expectedStringBuilder = new StringBuilder();
        expectedStringBuilder.Append(new Link { Name = "Link1", BaseUrl = "http://example.com", Code = "code1", UserId = "user1" }.ToString() + "</br>");
        expectedStringBuilder.Append(new Link { Name = "Link2", BaseUrl = "http://example.org", Code = "code2", UserId = "user2" }.ToString() + "</br>");

        Assert.AreEqual(expectedStringBuilder.ToString(), result);
    }

    [Test]
    public void Build_ShouldReturnEmptyStringIfNoLinks()
    {
        // Arrange
        var linkBuilder = new LinkBuilder();

        // Act
        var result = linkBuilder.Build();

        // Assert
        Assert.IsEmpty(result);
    }
}
