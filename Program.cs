using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumLoginTest.Pages;
using SeleniumLoginTest.Services;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Serilog;

public class Program
{
    public static IServiceProvider Init()
    {
        // 初始化 Serilog 日志系统
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console() // ✅ 输出到控制台
            .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        
        var services = new ServiceCollection();

        // 注册 WebDriver 单例
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--headless");       // 无界面
        chromeOptions.AddArgument("--disable-gpu");    // 某些系统下需要
        chromeOptions.AddArgument("--window-size=1920,1080"); // 避免某些元素不可见
        chromeOptions.AddArgument("--no-sandbox");     // 对某些Linux环境有用
        chromeOptions.AddArgument("--disable-dev-shm-usage"); // 内存限制问题
        chromeOptions.AddArgument("--incognito");

        services.AddSingleton<IWebDriver>(new ChromeDriver(chromeOptions));

        // 注册页面对象和服务
        services.AddTransient<LoginPage>();
        services.AddSingleton<SeleniumLoginTest.Services.DriverService>();
        
        
        // 注册 ExtentReports 单例
        services.AddSingleton<ExtentReports>(provider =>
        {
            // 获取项目根目录
            string projectDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!
                .Parent!.Parent!.Parent!.FullName;

            string reportDir = Path.Combine(projectDir, "Reports");
            Directory.CreateDirectory(reportDir);

            string reportPath = Path.Combine(reportDir, "Spark.html");

            var spark = new ExtentSparkReporter(reportPath);
            var extent = new ExtentReports();
            extent.AttachReporter(spark);

            return extent;
        });
        
        

        return services.BuildServiceProvider();
    }
}