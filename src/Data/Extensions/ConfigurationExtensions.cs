using Microsoft.Extensions.Configuration;

namespace Data.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetJwtSecretKey(this IConfiguration configuration) =>
            configuration.GetSection("JwtSettings").GetValue<string>("SecretKey");

        public static string GetElasticSearchSetting(this IConfiguration configuration, string key) =>
            configuration.GetSection("ElasticSearch").GetValue<string>(key);

        public static string GetKafkaSetting(this IConfiguration configuration, string key) =>
            configuration.GetSection("Kafka").GetValue<string>(key);
    }
}
