using System.Dynamic;

namespace Domain.Models
{
    public class ShapedEntity
    {
        public Guid Id { get; set; }
        public  ExpandoObject Entity { get; set; } = new ExpandoObject();
    }
}
