using Domain.Models;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.Extensions
{
    public static class ProjectRepositoryExtensions
    {
        public static IQueryable<Project> Filter(this IQueryable<Project> projects, string? name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                return projects.Where(x => x.Name.Contains(name));
            }
            return projects;
        }

        public static IQueryable<Project> Search(this IQueryable<Project> projects, string? searchTerm)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var cleanTerm = searchTerm.Trim().ToLower();

                return projects.Where(x => x.Name.ToLower().Contains(cleanTerm) ||
                                           x.Description.ToLower().Contains(cleanTerm));
            }

            return projects;
        }

        public static IQueryable<Project> Sort(this IQueryable<Project> projects, string? sortBy, string? sortOrder)
        {
            Expression<Func<Project, object>> key = sortBy?.ToLower() switch
            {
                "name" => project => project.Name,
                "description" => project => project.Description,
                _ => project => project.Id
            };

            if (sortOrder is "desc")
            {
                return projects.OrderByDescending(key);
            }
            return projects.OrderBy(key);
        }
    }
}
