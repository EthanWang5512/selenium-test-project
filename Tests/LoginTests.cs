
using OpenQA.Selenium;

using OpenQA.Selenium.Remote;
// using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;

using OpenQA.Selenium.Support.UI;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Serilog;
using FluentAssertions;
using Helpers;
using SeleniumLoginTest.Pages;
using SeleniumLoginTest.Data;
using SeleniumLoginTest.CSVData;
using SeleniumLoginTest.Services;
using Microsoft.Extensions.DependencyInjection;




using NUnit.Framework;
// using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;


namespace SeleniumLoginTest
{
  // NUnit框架下要给类加 [TestFixture] Attribute
  
  // [TestFixture("chrome")]
  // [TestFixture("firefox")]
  // [TestFixture("edge")]
  // [Parallelizable(ParallelScope.All)] // 允许测试用例并行运行（需要NUnit3以上）
  [TestFixture]
  public class LoginTests
  {
    // private string browser;
    
    // 声明全局变量
    // IWebDriver? driver;
    ExtentSparkReporter spark;
    String reportDir;
    
    private IServiceProvider? serviceProvider;
    private SeleniumLoginTest.Services.DriverService? driverService;
    private LoginPage? loginPage;
    private ExtentReports? extent;
    
    [OneTimeSetUp]
    public void InitReport()
    {
      serviceProvider = Program.Init();
      driverService = serviceProvider.GetService<SeleniumLoginTest.Services.DriverService>();
      loginPage = serviceProvider.GetService<LoginPage>();
      
      // TODO 这里要改进 兼容CI/CD
      extent = serviceProvider.GetService<ExtentReports>();
      
      string projectDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!
        .Parent!.Parent!.Parent!.FullName;
      reportDir = Path.Combine(projectDir, "Reports");
      Directory.CreateDirectory(reportDir);
      
      string reportPath = Path.Combine(reportDir, "Spark.html");
      
      spark = new ExtentSparkReporter(reportPath);
      extent.AttachReporter(spark);
      
      // 配置Serilog
      Logger.Init();
      Log.Information("🚀 启动测试");
    }
    
    // 第一步 SetUp
    // [SetUp]
    // public void Setup()
    // {
    //   // 指定用Chrome浏览器
    //   var service = ChromeDriverService.CreateDefaultService();
    //   var options = new ChromeOptions();
    //   options.AddArgument("--headless"); // 如果你希望看到浏览器过程可以注释掉这一行
    //   
    //   //  启用日志   这个是系统级的日志 很复杂 可以改用extentreport第三方
    //   // service.LogPath = "chromedriver.log"; 
    //   
    //   driver = new ChromeDriver(service, options);
    //   driver.Manage().Window.Maximize(); // 最大化窗口 防止元素丢失
    
      // 用指定浏览器进行测试  需要开启Selenium Grid
      // var gridUrl = new Uri("http://192.168.68.50:4444"); // Selenium Grid Hub 地址
      // // var options = new ChromeOptions();
      // var options = new FirefoxOptions();
      // options.AddArgument("--headless"); // 如果你希望无头运行
      //
      // // RemoteWebDriver 连接到 Selenium Grid
      // driver = new RemoteWebDriver(gridUrl, options.ToCapabilities(), TimeSpan.FromSeconds(60));

      
      // 在Selenium Grid里同时进行多浏览器测试
      // var gridUrl = new Uri("http://192.168.68.50:4444");
      // DriverOptions options;
      //
      // switch (browser.ToLower())
      // {
      //   case "chrome":
      //     var chromeOptions = new ChromeOptions();
      //     chromeOptions.AddArgument("--headless");
      //     options = chromeOptions;
      //     break;
      //
      //   case "firefox":
      //     var firefoxOptions = new FirefoxOptions();
      //     firefoxOptions.AddArgument("--headless");
      //     options = firefoxOptions;
      //     break;
      //
      //   case "edge":
      //     var edgeOptions = new EdgeOptions();
      //     edgeOptions.AddArgument("--headless");
      //     options = edgeOptions;
      //     break;
      //
      //   default:
      //     throw new ArgumentException("Unsupported browser");
      // }
      //
      // driver = new RemoteWebDriver(gridUrl, options);

    // }
    

    // 第二步 Test 对前端进行互动测试
    // [Test]
    // 数据驱动 Data-Driven Testing, DDT
    // Test logic and data are decoupled
    
    // 第一种方法 直接写test data
    // [Test]
    // [TestCase("tomsmith", "SuperSecretPassword!")]
    // [TestCase("admin", "123456!")]
    // [TestCase("user", "123456!")]
    
    // 第二种方法 封装起来 做一个数据类
    // [Test, TestCaseSource(typeof(LoginDataProvider), nameof(LoginDataProvider.LoginTestData))]
    
    // 第三种方法  提取csv文件  在第二种方法的基础上提取外部文件
    [Test, TestCaseSource(typeof(LoginCSVDataProvider), nameof(LoginCSVDataProvider.GetLoginDataFromCsv))]
    public void TestSuccessfulLogin(LoginTestData data)
    {
      var username = data.Username;
      var password = data.Password;
      var success = data.success;
      
      // TODO:  这里可以decouple 套用其他的report工具
      var test = extent.CreateTest($"TestSuccessfulLogin - {username}").Info("Starting login");

      Log.Information($"TestSuccessfulLogin - {username}");
      driverService?.Open("https://the-internet.herokuapp.com/login");
      
      
      // 选取对应元素 完成动作
      try
      {
        Log.Information("Entered username");
        loginPage.EnterUsername(username);
        test.Log(Status.Pass, "Entered username");
        
        
        Log.Information("Entered password");
        loginPage.EnterPassword(password);
        test.Log(Status.Pass, "Entered password");
        
        Log.Information("Submitted login form");
        loginPage.ClickLogin();
        test.Log(Status.Pass, "Submitted login form");
        
      }
      catch (NoSuchElementException ex)
      {
        ExceptionHandler.HandleElementNotFound(driverService.GetDriver(), test, ErrorReason.ElementNotFound, reportDir, ex);
      }

      // 等待响应
      
      // 等待2秒，让页面有时间跳转或加载
      // System.Threading.Thread.Sleep(2000);   // 要被WebDriverWait替代
      // string flashText = driver.FindElement(By.Id("flash")).Text;

      // WebDriverWait 可以不用死等  用箭头函数 不用写等待条件 直接获取元素内容
      // var wait = new WebDriverWait(driverService.GetDriver(), TimeSpan.FromSeconds(10));
      // var flash = wait.Until(d => d.FindElement(By.Id("flash")));
      
     

      // 断言声明
      // 1.单断言   如果有失败，测试会立即停止，后续断言不执行
      // FluentAssertions

      Log.Information("Check username and password");
      string flashText = "";
      IWebElement flash;
      
      try
      {
        flash = driverService.GetWait(10).Until(d => d.FindElement(By.Id("flash")));
      }
      catch (WebDriverTimeoutException ex)
      {
        Log.Error("❌ 等待 #flash 元素超时，页面可能未加载成功：{Message}", ex.Message);
        test.Fail("等待提示信息超时，页面可能加载失败或提示元素不存在");
        ExceptionHandler.HandleElementNotFound(driverService.GetDriver(), test, ErrorReason.ElementNotFound, reportDir, ex);
        throw;
      }
      
      try
      {
        
        flashText = flash.Text;
        Log.Information("页面提示信息：{Text}", flashText);
        test.Info("页面提示信息：" + flashText);

        if (success)
        {
          flashText.Should().Contain("You logged into a secure area!", because: "登录失败，提示信息不正确");
          Log.Information("Login success message verified.");
          test.Log(Status.Pass, "Login success message verified.");
        }
        else
        {
          flashText.Should().Contain("invalid", because: "预期登录失败，但提示信息不正确");
          Log.Information("登录失败提示验证通过");
          test.Pass("登录失败提示验证通过");
        }
          
      }
      catch (Exception e)
      {
        // Log.Warning($"❌ 登录失败，提示信息：- {flashText}");
        
        test.Fail("登录失败，提示不正确");
        throw;
      }
      
      
      
      
      
      
      
      // 2.多断言   如果前面的断言有失败，测试会自动进行
      //   FluentAssertion不能做 只能用Assert
      // string yep = "yep";
      // string greeting = "Hello, World";
      // Assert.Multiple(() =>
      // {
      //   Assert.IsTrue(2 > 1);
      //   // Assert.IsFalse(2 > 1);
      //   Assert.AreEqual("yep", yep);
      //   Assert.AreNotEqual("nope", yep);
      //   Assert.IsNull(null);
      //   // Assert.IsNotNull(null);
      //
      //   // 上面的都可以用Assert.That(value, constraints) 来代替
      //   Assert.That(greeting, Does.Contain("World"));
      //   Assert.That(greeting, Is.Not.Null.And.Not.Empty);
      //
      // });
      
      
    }

    // 最后一步 收尾释放资源
    // [TearDown]
    // public void TearDown()
    // {
    //   driver?.Quit();
    //   driver?.Dispose();
    // }

    [OneTimeTearDown]
    public void CloseReport()
    {
      driverService?.GetDriver().Quit();
      extent.Flush();
      Log.Information("🧹 Test finished");
    }
  }
}

/*
你目前的自动化测试学习已经打下了很扎实的基础，涵盖了：
   
   * Selenium + NUnit 测试框架
   * POM（Page Object Model）设计模式
   * 数据驱动测试（DDT）
   * 测试报告（ExtentReports）
   * Selenium Grid 分布式测试
   
   接下来可以从以下几个方向深入，具体取决于你的目标（比如找工作、做企业项目、提升架构能力等）。下面是系统性的推荐学习路径：
   
   ---
   
   ### ✅ **1. 提高测试用例的可维护性和可扩展性**
   
   #### 建议学习内容：
   
   * **依赖注入**（如 .NET 的 `Microsoft.Extensions.DependencyInjection`）
   * **配置管理**（如 appsettings.json + `IConfiguration`）
   * **日志记录**（如 `Serilog`）
   
   #### 推荐理由：
   
   实际项目需要良好的项目结构，方便维护、扩展和团队协作。
   
   ---
   
   ### ✅ **2. CI/CD 自动化集成测试流程**
   
   #### 建议学习内容：
   
   * **GitHub Actions / GitLab CI / Azure DevOps / Jenkins**
   * 将你的 Selenium 测试集成到 CI 流程中
   * 在 CI 中运行测试并生成报告（HTML + Screenshot）
   
   学习 Git + GitHub 基础（如熟悉可跳过）
   
   学习一个 CI 工具：推荐 GitHub Actions（免费、简单），或 Jenkins（企业常用）
   
   创建工作流 .yml 文件，让每次 push/pull request 时自动执行 dotnet test
   
   集成 ExtentReports 输出 + 保存 artifact
   
   
   #### 推荐理由：
   
   企业项目中测试用例需要能自动运行在 pipeline 里，这是必备技能。
   
   ---
   
   ### ✅ **3. 学习 BDD 测试：SpecFlow**
   
   #### 建议学习内容：
   
   * Gherkin 语法（Given/When/Then）
   * 使用 SpecFlow 编写与业务无缝对接的自动化脚本
   * SpecFlow + NUnit + Selenium 集成
   
   #### 推荐理由：
   
   许多团队使用 BDD 方法来让产品经理、测试和开发协同工作。
   
   ---
   
   ### ✅ **4. 更复杂的等待机制和异常处理**
   
   #### 建议深入：
   
   * `FluentWait` / 自定义重试机制
   * 测试失败截图 + 自动重跑失败用例（例如使用 NUnit Retry）
   
   #### 推荐理由：
   
   提高测试稳定性、避免 flaky tests 是自动化测试的重点难点。
   
   ---
   
   ### ✅ **5. API 自动化测试**
   
   #### 学习内容：
   
   * 使用 **RestSharp** 或 **HttpClient**
   * 学习如何写 API 测试用例（GET/POST/PUT/DELETE）
   * 学习如何做 UI + API 组合测试
   
   #### 推荐理由：
   
   现代测试不止 UI 测试，很多逻辑直接验证 API 更快更稳定。
   
   ---
   
   ### ✅ **6. 学习测试数据管理与Mock**
   
   #### 学习内容：
   
   * 使用 **Faker** 或 **Bogus** 生成测试数据
   * 使用 **WireMock.NET** 模拟第三方 API
   * 数据准备、清理、隔离
   
   #### 推荐理由：
   
   测试不是简单地跑用例，更重要的是构建稳定的数据环境。
   
   ---
   
   ### ✅ **7. 学习并行测试 / 多线程**
   
   #### 学习内容：
   
   * NUnit 的 `[Parallelizable]` 属性
   * 配合 Selenium Grid 或本地运行多浏览器测试
   
   #### 推荐理由：
   
   加快测试执行速度是提升 CI 效率的重要手段。
   
   ---
   
   ### ✅ **8. 掌握浏览器自动化以外的测试场景**
   
   例如：
   
   | 类型    | 技术             |
   | ----- | -------------- |
   | 手机自动化 | Appium         |
   | 桌面应用  | WinAppDriver   |
   | 性能测试  | JMeter / k6    |
   | 安全测试  | OWASP ZAP 简单集成 |
   
   ---
   
   ### 🎯 推荐学习顺序（你目前的基础下）：
   
   ```text
   1. CI/CD 集成自动化测试流程
   2. SpecFlow + BDD 测试
   3. API 自动化测试（RestSharp）
   4. Mock、测试数据管理
   5. 并行测试优化
   6. 移动端自动化（Appium）
   7. 性能 / 安全测试（可选）
   ```
   
   ---
   
   如果你愿意，我可以帮你规划一个 **逐周的学习计划**，根据你的时间安排每周的目标、任务和实践内容。是否需要我为你制定一个？
   
       
*/