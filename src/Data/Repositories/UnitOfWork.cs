using Data.Interfaces;

namespace Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RestApiN5DbContext _context;

        public IUserRepository Users { get; }
        public IPermissionTypeRepository PermissionsType { get; }
        public IPermissionRepository Permissions { get; }
        public IUserPermissionRepository UserPermissions { get; }

        public UnitOfWork(RestApiN5DbContext context, IUserRepository userRepository, IPermissionTypeRepository permissionTypeRepository, IPermissionRepository permissionRepository, IUserPermissionRepository userPermissionRepository)
        {
            _context = context;
            Users = userRepository;
            PermissionsType = permissionTypeRepository;
            Permissions = permissionRepository;
            UserPermissions = userPermissionRepository;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
