using BlazorServer.Areas.Counter.Pages;
using BlazorServer.Tests.Extensions;
using NUnit.Framework;

namespace BlazorServer.Tests;

public class Tests : BunitTestContext
{
    [Test]
    public void CounterComponent_ShouldMatchMarkup()
    {
        // Arrange
        using var ctx = new Bunit.TestContext();

        // Act
        var cut = ctx.RenderComponent<Counter>();

        // Assert
        cut.MarkupMatches(@"
<h1>Counter</h1>
<h6 class=""mud-typography mud-typography-h6"">MudBlazor is ????</h6>
<button  type=""button"" class=""mud-button-root mud-button mud-button-filled mud-button-filled-primary mud-button-filled-size-medium mud-ripple""  >
  <span class=""mud-button-label"">Click Me</span>
</button>
");
    }

    [Test]
    public void CounterComponent_ShouldUpdateText_WhenButtonClicked()
    {
        // Arrange
        using var ctx = new Bunit.TestContext();
        var cut = ctx.RenderComponent<Counter>();
        var buttonElement = cut.Find("button");

        // Act
        buttonElement.Click();
        buttonElement.Click();
        buttonElement.Click();

        cut.Instance.Text.ShouldBe("Awesome x 3");
    }

    [Test]
    public void CounterComponent_ShouldHaveHeaderWithTextContent()
    {
        using var ctx = new Bunit.TestContext();

        var cut = ctx.RenderComponent<Counter>();

        var asElement = cut.AsElement();
        asElement.ShouldNotBeNull();
        asElement.ShouldHaveTag("h1");
        asElement.TextContent.ShouldBe("Counter");
    }
}