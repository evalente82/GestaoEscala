
using NovosAprovados.Tools;

namespace NovosAprovados
{
    public class Config
    {
        static IConfigurationRoot CurrentConfiguration { get; set; }

        static Config()
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            CurrentConfiguration = builder.Build();
        }
        public static string GetValue(string key) => (String.IsNullOrEmpty(EnvHelper.Get(key)) ? CurrentConfiguration.GetValue<string>(key) : EnvHelper.Get(key));
        public static T GetValue<T>(string key) => (String.IsNullOrEmpty(EnvHelper.Get(key)) ? CurrentConfiguration.GetValue<T>(key) : (T)Convert.ChangeType(EnvHelper.Get(key), typeof(T)));
    }
}