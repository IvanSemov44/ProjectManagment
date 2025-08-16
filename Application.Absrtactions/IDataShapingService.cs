using Domain.Models;

namespace Application.Absrtactions
{
    public interface IDataShapingService<T>
    {
        IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string properties);
        ShapedEntity ShapeData(T entity, string properties);
    }
}