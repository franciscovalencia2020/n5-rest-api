using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.DatabaseModels
{
    [Table("Permission")]
    public class Permission : BaseEntity
    {
        [Required]
        public long PermissionTypeId { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }

        public PermissionType PermissionType { get; set; }
    }
}
