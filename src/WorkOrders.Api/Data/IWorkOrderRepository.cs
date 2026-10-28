using WorkOrders.Api.Models;

namespace WorkOrders.Api.Data;

public interface IWorkOrderRepository
{
    IEnumerable<WorkOrder> GetAll(WorkOrderStatus? status = null);
    WorkOrder? GetById(int id);
    WorkOrder Add(WorkOrder workOrder);
    WorkOrder? UpdateStatus(int id, WorkOrderStatus status);
}
