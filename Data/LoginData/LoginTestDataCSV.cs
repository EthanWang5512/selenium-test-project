using SeleniumLoginTest.Data;

namespace SeleniumLoginTest.CSVData
{
    public static class LoginCSVDataProvider
    {
        public static IEnumerable<LoginTestData> GetLoginDataFromCsv()
        {
            // 获取项目根目录
            // 拼接 CSV 文件路径（假设在 /Data/LoginData.csv） 这里需要设置配置文件
            // <ItemGroup>
            //     <None Update="Data\LoginData.csv">
            //     <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
            //     </None>
            // </ItemGroup>

            string csvPath = Path.Combine(AppContext.BaseDirectory, "Data", "LoginData", "LoginData.csv");

            // 读取并解析
            var lines = File.ReadAllLines(csvPath);
            foreach (var line in lines.Skip(1)) // 跳过表头
            {
                if (string.IsNullOrWhiteSpace(line)) continue; // 跳过空行
                
                var parts = line.Split(',');
                
                
                yield return new LoginTestData { Username = parts[0], Password = parts[1], success = "true".Equals(parts[2]) };
            }
        }
    }

};

