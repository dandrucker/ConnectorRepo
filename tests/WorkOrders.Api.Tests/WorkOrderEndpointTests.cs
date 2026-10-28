using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using WorkOrders.Api.Models;

namespace WorkOrders.Api.Tests;

public class WorkOrderEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _client;

    public WorkOrderEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateWorkOrder_ReturnsCreated()
    {
        var request = new CreateWorkOrderRequest
        {
            PipelineSegment = "SEG-101",
            Description = "Replace corroded valve",
            Priority = WorkOrderPriority.High
        };

        var response = await _client.PostAsJsonAsync("/workorders", request, JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<WorkOrder>(JsonOptions);
        Assert.NotNull(created);
        Assert.Equal(WorkOrderStatus.Open, created!.Status);
    }

    [Fact]
    public async Task CreateWorkOrder_MissingSegment_ReturnsBadRequest()
    {
        var request = new CreateWorkOrderRequest { Description = "No segment" };

        var response = await _client.PostAsJsonAsync("/workorders", request, JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetWorkOrder_UnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/workorders/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStatus_Completed_SetsCompletedUtc()
    {
        var create = await _client.PostAsJsonAsync("/workorders", new CreateWorkOrderRequest
        {
            PipelineSegment = "SEG-202",
            Description = "Inspect cathodic protection",
            Priority = WorkOrderPriority.Medium
        }, JsonOptions);
        var created = await create.Content.ReadFromJsonAsync<WorkOrder>(JsonOptions);

        var response = await _client.PutAsJsonAsync($"/workorders/{created!.Id}/status",
            new UpdateStatusRequest { Status = WorkOrderStatus.Completed }, JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<WorkOrder>(JsonOptions);
        Assert.Equal(WorkOrderStatus.Completed, updated!.Status);
        Assert.NotNull(updated.CompletedUtc);
    }
}
