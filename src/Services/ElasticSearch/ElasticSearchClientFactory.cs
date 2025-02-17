using Nest;
using Microsoft.Extensions.Configuration;
using Data.Extensions;

namespace Services.ElasticSearch
{
    public class ElasticSearchClientFactory
    {
        private readonly IElasticClient _client;
        private readonly string _elasticSearchUrl;
        private readonly string _elasticSearchUsername;
        private readonly string _elasticSearchPassword;

        public ElasticSearchClientFactory(IConfiguration configuration)
        {
            _elasticSearchUrl = configuration.GetElasticSearchSetting("Url");
            _elasticSearchUsername = configuration.GetElasticSearchSetting("Username");
            _elasticSearchPassword = configuration.GetElasticSearchSetting("Password");


            var settings = new ConnectionSettings(new Uri(_elasticSearchUrl))
                .BasicAuthentication(_elasticSearchUsername, _elasticSearchPassword)
                .DefaultIndex("permissions");

            _client = new ElasticClient(settings);
        }

        public IElasticClient GetClient() => _client;
    }
}
