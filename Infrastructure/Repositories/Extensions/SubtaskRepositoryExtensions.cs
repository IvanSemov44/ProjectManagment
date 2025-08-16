using Domain.Models;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.Extensions
{
    public static class SubtaskRepositoryExtensions
    {
        public static IQueryable<Subtask> Filter(this IQueryable<Subtask> subtasks, string? title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                return subtasks.Where(x => x.Title.Contains(title));
            }
            return subtasks;
        }

        public static IQueryable<Subtask> Search(this IQueryable<Subtask> subtasks, string? searchTerm)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var cleanTerm = searchTerm.Trim().ToLower();

                return subtasks.Where(x => x.Title.ToLower().Contains(cleanTerm) ||
                                           x.Description.ToLower().Contains(cleanTerm));
            }
            return subtasks;
        }
        public static IQueryable<Subtask> Sort(this IQueryable<Subtask> subtasks, string? sortBy, string? sortOrder)
        {
            Expression<Func<Subtask, object>> key = sortBy?.ToLower() switch
            {
                "title" => subtask => subtask.Title,
                "description" => subtask => subtask.Description,
                _ => subtask => subtask.Id
            };

            if (sortOrder is "desc")
            {
                return subtasks.OrderByDescending(key);
            }
            return subtasks.OrderBy(key);
        }
    }
}
