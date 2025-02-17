using Data.Models.DTOs.Permission.Response;
using MediatR;

namespace Services.Commands.Permission
{
    public record UpdatePermissionCommand(long Id, string Description, long PermissionTypeId) : IRequest<PermissionResponse>;
}
