using GrpcService.SharedCode;
using ProtoBuf.Grpc.Server;
using ProtoBuf.Meta;
using System.Runtime.Serialization;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

RuntimeTypeModel.Default.RegisterProtobufSurrogates();

builder.Services
    //.AddProtobufSurrogates()
    .AddCodeFirstGrpc();

var app = builder.Build();

app.MapGrpcService<GreeterService>();

await app.RunAsync();

[DataContract]
public class HelloReply
{
    [DataMember(Order = 1)]
    public string? Message { get; set; }

    [DataMember(Order = 2)]
    public DateTimeOffset DateTimeOffset { get; set; }
}

[ServiceContract]
public interface IGreeterService
{
    [OperationContract]
    Task<HelloReply> SayHelloAsync();
}

public class GreeterService : IGreeterService
{
    public Task<HelloReply> SayHelloAsync() =>
        Task.FromResult(new HelloReply { Message = "Hello world", DateTimeOffset = DateTimeOffset.UtcNow });
}

public partial class Program { }