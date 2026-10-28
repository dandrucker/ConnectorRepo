using System.Text.Json.Serialization;
using WorkOrders.Api.Data;
using WorkOrders.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IWorkOrderRepository, InMemoryWorkOrderRepository>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

app.MapGet("/", () => "Pipeline Maintenance Work Order Tracker");

app.MapGet("/workorders", (WorkOrderStatus? status, IWorkOrderRepository repo) =>
{
    return Results.Ok(repo.GetAll(status));
});

app.MapGet("/workorders/{id}", (int id, IWorkOrderRepository repo) =>
{
    var workOrder = repo.GetById(id);
    return workOrder is null ? Results.NotFound() : Results.Ok(workOrder);
});

app.MapPost("/workorders", (CreateWorkOrderRequest request, IWorkOrderRepository repo, ILogger<Program> logger) =>
{
    if (string.IsNullOrWhiteSpace(request.PipelineSegment))
    {
        return Results.BadRequest("PipelineSegment is required.");
    }

    if (string.IsNullOrWhiteSpace(request.Description))
    {
        return Results.BadRequest("Description is required.");
    }

    var workOrder = new WorkOrder
    {
        PipelineSegment = request.PipelineSegment,
        Description = request.Description,
        Priority = request.Priority,
        Status = WorkOrderStatus.Open,
        CreatedUtc = DateTime.UtcNow
    };

    repo.Add(workOrder);
    logger.LogInformation($"Created work order {workOrder.Id} for segment {workOrder.PipelineSegment}");

    return Results.Created($"/workorders/{workOrder.Id}", workOrder);
});

app.MapPut("/workorders/{id}/status", (int id, UpdateStatusRequest request, IWorkOrderRepository repo, ILogger<Program> logger) =>
{
    var workOrder = repo.UpdateStatus(id, request.Status);
    if (workOrder is null)
    {
        return Results.NotFound();
    }

    logger.LogInformation($"Work order {id} status changed to {request.Status}");
    return Results.Ok(workOrder);
});

app.Run();

public partial class Program { }
