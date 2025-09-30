using Molyavchik.TaskPlanner.Domain.Models;
using Molyavchik.TaskPlanner.DataAccess.Abstractions;
using Newtonsoft.Json;

namespace Molyavchik.TaskPlanner.DataAccess.FileStorage
{
    public class FileWorkItemsRepository : IWorkItemRepository
    {
        private static readonly string FileName = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "work-items.json"
        );

        private readonly Dictionary<Guid, WorkItem> _workItems;

        public FileWorkItemsRepository()
        {
            if (File.Exists(FileName) && new FileInfo(FileName).Length > 0)
            {
                var json = File.ReadAllText(FileName);
                var items = JsonConvert.DeserializeObject<WorkItem[]>(json) ?? Array.Empty<WorkItem>();
                _workItems = items.ToDictionary(wi => wi.Id, wi => wi);
            }
            else
            {
                _workItems = new Dictionary<Guid, WorkItem>();
            }
        }

        public Guid Add(WorkItem workItem)
        {
            var copy = workItem.Clone();
            copy.Id = Guid.NewGuid();
            _workItems[copy.Id] = copy;
            return copy.Id;
        }

        public WorkItem Get(Guid id)
        {
            return _workItems.TryGetValue(id, out var workItem) ? workItem.Clone() : null;
        }

        public WorkItem[] GetAll()
        {
            return _workItems.Values.Select(wi => wi.Clone()).ToArray();
        }

        public bool Update(WorkItem workItem)
        {
            if (!_workItems.ContainsKey(workItem.Id))
                return false;

            _workItems[workItem.Id] = workItem.Clone();
            return true;
        }

        public bool Remove(Guid id)
        {
            return _workItems.Remove(id);
        }

        public void SaveChanges()
        {
            var items = _workItems.Values.ToArray();
            var json = JsonConvert.SerializeObject(items, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(FileName, json);
        }
    }
}