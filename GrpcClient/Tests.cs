using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.ClientFactory;

namespace GrpcClient;

public class Tests
{
    [Fact]
    public async Task TestSurrogates()
    {
        var app = new WebApplicationFactory<Program>();

        var clientServiceCollection = new ServiceCollection();
        clientServiceCollection.AddCodeFirstGrpcClient<IGreeterService>(nameof(IGreeterService), config =>
        {
            config.Address = app.Server.BaseAddress;
            config.ChannelOptionsActions.Add(option => option.HttpHandler = app.Server.CreateHandler());
        });

        using var clientServiceProvider = clientServiceCollection.BuildServiceProvider();
        var serviceFactory = clientServiceProvider.GetRequiredService<GrpcClientFactory>();
        var greeterClient = serviceFactory.CreateClient<IGreeterService>(nameof(IGreeterService));

        var reply = await greeterClient.SayHelloAsync();

        Assert.Equal("Hello world", reply.Message);
        Assert.NotEqual(default, reply.DateTimeOffset);
    }
}
