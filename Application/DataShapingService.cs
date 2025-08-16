using Application.Absrtactions;
using Domain.Models;
using System.Dynamic;
using System.Reflection;

namespace Application
{
    public class DataShapingService<T> : IDataShapingService<T> where T : class
    {
        private readonly PropertyInfo[] _properties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);
        public IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string properties)
        {
            return entities.Select(entity => ShapeData(entity, properties));
        }

        public ShapedEntity ShapeData(T entity, string properties)
        {
            var shapedEntity = new ShapedEntity();
            var entityObject = new ExpandoObject();

            var requestedProperites = ParseProperties(properties);

            foreach (var property in _properties)
            {
                if (requestedProperites.Contains(property.Name, StringComparer.OrdinalIgnoreCase))
                {
                    var propertyValue = property.GetValue(entity);
                    entityObject.TryAdd(property.Name, propertyValue);
                }
            }

            shapedEntity.Entity = entityObject;
            shapedEntity.Id = (Guid)_properties
                .First(p=>p.Name.Equals("Id",StringComparison.OrdinalIgnoreCase))
                .GetValue(entity)!;

            return shapedEntity;
        }

        private string[] ParseProperties(string properties)
        {
            if (string.IsNullOrEmpty(properties))
            {
                return _properties
                    .Where(p => !p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    .Select(p => p.Name)
                    .ToArray();
            }

            return properties
                .Split(",", StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
