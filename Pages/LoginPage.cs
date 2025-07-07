using OpenQA.Selenium;

namespace SeleniumLoginTest.Pages
{
    /**
     *   Page Object Model  (POM)
     *      每一个网页页面都对应一个类 Page Class，这个类封装了该页面上的所有元素 和 操作方法
     *
     *      测试逻辑 Test 和 页面元素定位 就解耦了
     *      The test logic and the UI element locators are decoupled
     */
    public class LoginPage
    {
        public readonly IWebDriver driver;
    
        // Constuctor
        public LoginPage(IWebDriver driver)
        {
            this.driver = driver;
        }
        
        private IWebElement UsernameInput => driver.FindElement(By.Id("username"));
        private IWebElement PasswordInput => driver.FindElement(By.Id("password"));
        private IWebElement LoginButton => driver.FindElement(By.CssSelector("button[type='submit']"));

        
        // Actions
        public void EnterUsername(string username)
        {
            UsernameInput.Clear();
            UsernameInput.SendKeys(username);
        }

        public void EnterPassword(string password)
        {
            PasswordInput.Clear();
            PasswordInput.SendKeys(password);
        }

        public void ClickLogin()
        {
            LoginButton.Click();
        }
        
        public bool IsUsernameEnteredCorrectly(string expected)
        {
            var actual = driver.FindElement(By.Id("username")).GetAttribute("value");
            return actual == expected;
        }

        public bool IsPasswordEnteredCorrectly(string expected)
        {
            var actual = driver.FindElement(By.Id("password")).GetAttribute("value");
            return actual == expected;
        }

    }
  
};

