using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumLoginTest.Services
{
    public class DriverService
    {
        private readonly IWebDriver driver;

        public DriverService(IWebDriver driver)
        {
            this.driver = driver;
            driver.Manage().Window.Maximize();
        }

        public void Open(string url)
        {
            driver.Navigate().GoToUrl(url);
        }
        
        public WebDriverWait GetWait(int seconds = 10)
        {
            return new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));
        }

        public IWebDriver GetDriver() => driver;
    }
}