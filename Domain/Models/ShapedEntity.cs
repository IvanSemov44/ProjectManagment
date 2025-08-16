using System.Dynamic;

namespace Domain.Models
{
    public class ShapedEntity
    {
        public Guid Id { get; set; }
        public List<Property> Properties { get; set; } = [];
    }
}
