using Data.Models.DTOs.Permission.Response;
using MediatR;

namespace Services.Commands.Permission
{
    public record CreatePermissionCommand(string Description, long PermissionTypeId) : IRequest<PermissionResponse>;
}
