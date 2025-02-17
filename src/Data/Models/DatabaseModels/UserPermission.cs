using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.DatabaseModels
{
    [Table("UserPermission")]
    public class UserPermission : BaseEntity
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public long PermissionId { get; set; }

        public User User { get; set; }

        public Permission Permission { get; set; }
    }
}
