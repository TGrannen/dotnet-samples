# Playwright Testing

[Playwright](https://playwright.dev/dotnet) was created specifically to accommodate the needs of end-to-end testing.
Playwright supports all modern rendering engines including Chromium, WebKit, and Firefox. Test on Windows, Linux, and
macOS, locally or on CI, headless or headed with native mobile emulation. Playwright comes with auto-wait built in
meaning it waits for elements to be ~~~~actionable prior to performing actions. Playwright provides the Expect function
to write assertions.

## Test Generation

Playwright comes with the ability to generate tests out of the box and is a great way to quickly get started with
testing. It will open two windows, a browser window where you interact with the website you wish to test and the
Playwright Inspector window where you can record your tests, copy the tests, clear your tests as well as change the
language of your tests.

https://playwright.dev/dotnet/docs/codegen#running-codegen

```shell
# From within the PlaywrightTests directory
pwsh bin/Debug/net6.0/playwright.ps1 codegen URL_TO_LAUNCH
```

## Debugging Tests

```dotenv
#Launches Browser with Playwright in Debug mode
PWDEBUG=1
```

## [Trace Viewer](https://playwright.dev/dotnet/docs/trace-viewer-intro)

Playwright Trace Viewer is a GUI tool that lets you explore recorded Playwright traces of your tests meaning you can go
back and forward though each action of your test and visually see what was happening during each action.

### Recording

```csharp
await using var browser = playwright.Chromium.LaunchAsync();
await using var context = await browser.NewContextAsync();

// Start tracing before creating / navigating a page.
await context.Tracing.StartAsync(new()
{
  Screenshots = true,
  Snapshots = true,
  Sources = true
});

var page = context.NewPageAsync();
await page.GotoAsync("https://playwright.dev");

// Stop tracing and export it into a zip archive.
await context.Tracing.StopAsync(new()
{
  Path = "trace.zip"
});
```

#### Opening the trace

You can open the saved trace using Playwright CLI or in your browser on trace.playwright.dev.

```shell
# From within the PlaywrightTests directory
pwsh bin/Debug/net6.0/playwright.ps1 show-trace bin/Debug/net6.0/trace.zip
```
