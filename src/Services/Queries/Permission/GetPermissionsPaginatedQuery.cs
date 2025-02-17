using Data.Helpers;
using Data.Models.DTOs.Permission.Response;
using MediatR;

namespace Services.Queries.Permission
{
    public record GetPermissionsPaginatedQuery(int PageNumber, int PageSize) : IRequest<PaginatedList<PermissionResponse>>;
}
