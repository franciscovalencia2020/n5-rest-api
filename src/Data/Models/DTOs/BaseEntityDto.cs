namespace Data.Models.DTOs
{
    public class BaseEntityDto
    {
        public long Id { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
