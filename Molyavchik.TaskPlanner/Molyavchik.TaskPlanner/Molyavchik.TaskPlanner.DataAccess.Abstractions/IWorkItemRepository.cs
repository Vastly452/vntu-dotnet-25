using Molyavchik.TaskPlanner.Domain.Models;
using Molyavchik.TaskPlanner.Domain.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Molyavchik.TaskPlanner.DataAccess.Abstractions
{
    public interface IWorkItemRepository
    {
        Guid Add(WorkItem workItem);
        WorkItem Get(Guid id);
        WorkItem[] GetAll();
        bool Update(WorkItem workItem);
        bool Remove(Guid id);
        void SaveChanges();
    }
}