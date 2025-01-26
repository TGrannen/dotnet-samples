using AngleSharp.Dom;

namespace BlazorServer.Tests.Extensions;

public static class BUnitExtensions
{
    public static IElement FindByDataTestId(this IRenderedFragment component, string dataTestId) => component.Find($"[data-test-id='{dataTestId}']");

    public static IElement FindByDataTestClass(this IRenderedFragment component, string dataTestClass) =>
        component.Find($"[data-test-class='{dataTestClass}']");

    public static IRefreshableElementCollection<IElement> FindAllByDataTestClass(this IRenderedFragment component,
        string dataTestClass) => component.FindAll($"[data-test-class='{dataTestClass}']");

    public static IElement? AsElement(this IRenderedFragment component) => component.Nodes.QuerySelector("*");

    public static void ShouldHaveTag(this IElement? element, string expected)
    {
        element.ShouldNotBeNull();
        var tag = element.LocalName;
        tag.ShouldBe(expected, customMessage: "Expected {context:IElement} {0} to be {1}{reason}, but found {2}.");
    }
}