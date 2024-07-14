using System.ComponentModel.DataAnnotations;

namespace Order_System.models
{
    public class BaseEntity
    {

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}