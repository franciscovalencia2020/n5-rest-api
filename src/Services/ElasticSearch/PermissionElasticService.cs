using Nest;
using Data.Models.DatabaseModels;
using Data.Helpers;
using Services.ElasticSearch.Interfaces;

namespace Services.ElasticSearch
{
    public class PermissionElasticService : IPermissionElasticService
    {
        private readonly IElasticClient _elasticClient;

        public PermissionElasticService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task IndexPermissionAsync(Permission permission)
        {
            await _elasticClient.IndexDocumentAsync(permission);
        }

        public async Task<Permission> GetPermissionByIdAsync(long id)
        {
            var response = await _elasticClient.GetAsync<Permission>(id, idx => idx.Index("permissions"));
            return response.Source;
        }

        public async Task<IEnumerable<Permission>> GetAllPermissionsAsync()
        {
            var searchResponse = await _elasticClient.SearchAsync<Permission>(s => s.Index("permissions").MatchAll());
            return searchResponse.Documents;
        }

        public async Task UpdatePermissionAsync(Permission permission)
        {
            await _elasticClient.UpdateAsync<Permission>(permission.Id, u => u.Index("permissions").Doc(permission));
        }

        public async Task DeletePermissionAsync(long id)
        {
            await _elasticClient.DeleteAsync<Permission>(id, d => d.Index("permissions"));
        }

        public async Task<PaginatedList<Permission>> GetPaginatedPermissionsAsync(int pageNumber, int pageSize)
        {
            var response = await _elasticClient.SearchAsync<Permission>(s => s
                .Index("permissions")
                .From((pageNumber - 1) * pageSize)
                .Size(pageSize)
                .MatchAll());

            var list = response.Documents.ToList();
            return new PaginatedList<Permission>(list, (int)response.Total, pageNumber, pageSize);
        }

        public async Task<IEnumerable<Permission>> SearchPermissionsAsync(string query)
        {
            var response = await _elasticClient.SearchAsync<Permission>(s => s
                .Index("permissions")
                .Query(q => q.QueryString(d => d.Query(query))));

            return response.Documents;
        }

        public async Task DeletePermissionsAsync(IEnumerable<Permission> permissions)
        {
            var bulkRequest = new BulkRequest("permissions")
            {
                Operations = permissions.Select(permission => new BulkDeleteOperation<Permission>(permission.Id) as IBulkOperation).ToList()
            };

            var bulkResponse = await _elasticClient.BulkAsync(bulkRequest);

            if (bulkResponse.Errors)
            {
                throw new Exception($"Error deleting permissions: {string.Join(", ", bulkResponse.ItemsWithErrors.Select(e => e.Error.Reason))}");
            }
        }
    }
}
