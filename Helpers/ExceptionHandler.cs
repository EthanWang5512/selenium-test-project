using OpenQA.Selenium;
using AventStack.ExtentReports;
using Serilog;


namespace Helpers;

public class ExceptionHandler
{
    public static void HandleElementNotFound(IWebDriver driver, ExtentTest test, string message, string screenshotDir, Exception ex)
    {
        string screenshot = ScreenshotHelper.TakeScreenshot(driver, screenshotDir, "error.png");
        test.Fail(message, MediaEntityBuilder.CreateScreenCaptureFromPath(screenshot).Build());
        Log.Error(ex, message);
        test.Log(Status.Info, "Screenshot saved.");
        
        Assert.Fail(message);
    }
}