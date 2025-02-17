using Data.Models.DTOs.Permission.Response;
using MediatR;

namespace Services.Queries.Permission
{
    public record GetPermissionByIdQuery(long Id) : IRequest<PermissionResponse>;
}
