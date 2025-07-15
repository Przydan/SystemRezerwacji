using System.Net;
using System.Text.Json;
using Application.Interfaces.Persistence;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using Shared.DTOs.Resource;
using WebApp.Services;
using Xunit; // <--- DODANA LINIA
using TestContext = Bunit.TestContext;

namespace WebApp.Tests;

public class ResourceTypeServiceTests : IDisposable
{
    private readonly TestContext _ctx;
    private readonly Mock<HttpMessageHandler> _mockHttpHandler;
    private readonly IResourceTypeService _sut; // System Under Test

    public ResourceTypeServiceTests()
    {
        _ctx = new TestContext();
        _mockHttpHandler = new Mock<HttpMessageHandler>();

        var client = new HttpClient(_mockHttpHandler.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        _ctx.Services.AddSingleton(client);
        _ctx.Services.AddScoped<IResourceTypeService, ResourceTypeService>();

        _sut = _ctx.Services.GetRequiredService<IResourceTypeService>();
    }

    [Fact]
    public async Task GetAllResourceTypesAsync_ShouldReturnResourceTypes_WhenApiCallIsSuccessful()
    {
        // ARRANGE
        var expectedResourceTypes = new List<ResourceTypeDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Sala Konferencyjna", Description = "Duża sala" },
            new() { Id = Guid.NewGuid(), Name = "Projektor", Description = "Przenośny projektor" }
        };
        
        var jsonResponse = JsonSerializer.Serialize(expectedResourceTypes);

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().EndsWith("api/resourcetypes")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // ACT
        var result = await _sut.GetAllResourceTypesAsync();

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Sala Konferencyjna", result[0].Name);
    }
    
    public void Dispose()
    {
        _ctx.Dispose();
    }
}