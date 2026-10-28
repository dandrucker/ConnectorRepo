using System.Collections.Concurrent;
using WorkOrders.Api.Models;

namespace WorkOrders.Api.Data;

public class InMemoryWorkOrderRepository : IWorkOrderRepository
{
    private readonly ConcurrentDictionary<int, WorkOrder> _workOrders = new();
    private int _nextId = 0;

    public IEnumerable<WorkOrder> GetAll(WorkOrderStatus? status = null)
    {
        var query = _workOrders.Values.AsEnumerable();
        if (status.HasValue)
        {
            query = query.Where(w => w.Status == status.Value);
        }
        return query.OrderBy(w => w.Id).ToList();
    }

    public WorkOrder? GetById(int id)
    {
        _workOrders.TryGetValue(id, out var workOrder);
        return workOrder;
    }

    public WorkOrder Add(WorkOrder workOrder)
    {
        workOrder.Id = Interlocked.Increment(ref _nextId);
        _workOrders[workOrder.Id] = workOrder;
        return workOrder;
    }

    public WorkOrder? UpdateStatus(int id, WorkOrderStatus status)
    {
        if (!_workOrders.TryGetValue(id, out var workOrder))
        {
            return null;
        }

        workOrder.Status = status;
        workOrder.CompletedUtc = status == WorkOrderStatus.Completed ? DateTime.UtcNow : null;
        return workOrder;
    }
}
