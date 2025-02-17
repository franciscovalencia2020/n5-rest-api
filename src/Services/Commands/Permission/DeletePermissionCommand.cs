using MediatR;

namespace Services.Commands.Permission
{
    public record DeletePermissionCommand(long Id) : IRequest<bool>;
}
