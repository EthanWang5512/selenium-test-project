using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Helpers
{
    public static class ScreenshotHelper
    {
        public static string TakeScreenshot(IWebDriver driver, string directoryPath, string fileName)
        {
            // 修剪路径，防止意外空格或换行
            directoryPath = directoryPath.Trim();

            // 确保目录存在
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // 拼接完整路径，含文件名
            string fullPath = Path.Combine(directoryPath, fileName);

            var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot.SaveAsFile(fullPath);

            return fullPath;
        }

    }
}
