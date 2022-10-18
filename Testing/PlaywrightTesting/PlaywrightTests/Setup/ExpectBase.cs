namespace PlaywrightTests.Setup;

public abstract class ExpectBase
{
    public ILocatorAssertions Expect(ILocator locator) => Assertions.Expect(locator);

    public IPageAssertions Expect(IPage page) => Assertions.Expect(page);

    public IAPIResponseAssertions Expect(IAPIResponse response) => Assertions.Expect(response);
}