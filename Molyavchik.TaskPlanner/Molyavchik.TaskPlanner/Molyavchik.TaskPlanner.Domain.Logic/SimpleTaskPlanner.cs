using Molyavchik.TaskPlanner.Domain.Models;
using Molyavchik.TaskPlanner.DataAccess.Abstractions;

namespace Molyavchik.TaskPlanner.Domain.Logic
{
    public class SimpleTaskPlanner
    {
        private readonly IWorkItemRepository _repository;

        public SimpleTaskPlanner(IWorkItemRepository repository)
        {
            _repository = repository;
        }

        public WorkItem[] CreatePlan()
        {
            var items = _repository
                .GetAll()
                .Where(wi => !wi.IsCompleted)
                .ToList();

            items.Sort(CompareWorkItems);
            return items.ToArray();
        }

        private static int CompareWorkItems(WorkItem firstItem, WorkItem secondItem)
        {
            // Сортування за Priority (спадання)
            var priorityComparison = secondItem.Priority.CompareTo(firstItem.Priority);
            if (priorityComparison != 0)
            {
                return priorityComparison;
            }

            // Сортування за DueDate (зростання)
            var dateComparison = firstItem.DueDate.CompareTo(secondItem.DueDate);
            if (dateComparison != 0)
            {
                return dateComparison;
            }

            // Сортування за Title (алфавітний порядок)
            return string.Compare(firstItem.Title, secondItem.Title);
        }
    }
}
