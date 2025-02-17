using Data.Models.DTOs.Permission.Response;
using MediatR;

namespace Services.Queries.Permission
{
    public record GetPermissionsQuery() : IRequest<List<PermissionResponse>>;
}
