using Grpc.Net.ClientFactory;
using GrpcService.SharedCode;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.ClientFactory;
using ProtoBuf.Grpc.Configuration;

namespace GrpcClient;

public class Tests
{
    [Fact]
    public async Task TestSurrogates()
    {
        var app = new WebApplicationFactory<Program>();

        var clientServiceCollection = new ServiceCollection();
        clientServiceCollection
            .AddProtobufSurrogates()
            .AddCodeFirstGrpcClient<IGreeterService>(nameof(IGreeterService), config =>
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

    [Fact]
    public void ObtainSerializer()
    {
        var app = new WebApplicationFactory<Program>();

        var clientServiceCollection = new ServiceCollection();
        clientServiceCollection
            .AddProtobufSurrogates()
            .AddCodeFirstGrpcClient<IGreeterService>(nameof(IGreeterService), config =>
            {
                config.Address = app.Server.BaseAddress;
                config.ChannelOptionsActions.Add(option => option.HttpHandler = app.Server.CreateHandler());
            });

        using var clientServiceProvider = clientServiceCollection.BuildServiceProvider();

        var binder = clientServiceProvider.GetRequiredService<BinderConfiguration>();
        var reply = new HelloReply
        {
            DateTimeOffset = DateTimeOffset.UtcNow,
            Message = "Test"
        };

        var marshaller = binder.GetMarshaller<HelloReply>();

        var bytes = marshaller.Serializer(reply);
        var clone = marshaller.Deserializer(bytes);

        Assert.Equivalent(reply, clone);
    }
}