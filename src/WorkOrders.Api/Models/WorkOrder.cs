namespace WorkOrders.Api.Models;

public enum WorkOrderPriority
{
    Low,
    Medium,
    High,
    Critical
}

public enum WorkOrderStatus
{
    Open,
    InProgress,
    Completed,
    Cancelled
}

public class WorkOrder
{
    public int Id { get; set; }
    public string PipelineSegment { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public WorkOrderPriority Priority { get; set; }
    public WorkOrderStatus Status { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? CompletedUtc { get; set; }
}

public class CreateWorkOrderRequest
{
    public string PipelineSegment { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public WorkOrderPriority Priority { get; set; }
}

public class UpdateStatusRequest
{
    public WorkOrderStatus Status { get; set; }
}
