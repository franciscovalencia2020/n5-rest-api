using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.DatabaseModels
{
    [Table("PermissionType")]
    public class PermissionType : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
    }
}
