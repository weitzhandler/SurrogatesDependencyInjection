using ProtoBuf.Grpc.Configuration;
using ProtoBuf.Meta;

namespace GrpcService.SharedCode;

public static class SharedCode
{
    public static IServiceCollection AddProtobufSurrogates(this IServiceCollection services)
    {
        var model = RuntimeTypeModel.Create();
        model.RegisterProtobufSurrogates();

        var marshallerFactory = ProtoBufMarshallerFactory.Create(model, ProtoBufMarshallerFactory.Options.None);
        var binderConfiguration = BinderConfiguration.Create([marshallerFactory]);

        return services.AddSingleton(binderConfiguration);
    }

    public static void RegisterProtobufSurrogates(this RuntimeTypeModel model)
    {
        RegisterDateTimeOffsetSurrogate(model);
    }

    private static void RegisterDateTimeOffsetSurrogate(RuntimeTypeModel model)
    {
        model.SetSurrogate<DateTimeOffset, DateTimeOffsetSurrogate>(
            underlyingToSurrogate: DateTimeOffsetSurrogate.DateTimeOffsetToSurrogate,
            surrogateToUnderlying: DateTimeOffsetSurrogate.SurrogateToDateTimeOffset);
    }

    private record DateTimeOffsetSurrogate(long Ticks)
    {
        public static DateTimeOffsetSurrogate DateTimeOffsetToSurrogate(DateTimeOffset dateTimeOffset) =>
            new(dateTimeOffset.Ticks);
        public static DateTimeOffset SurrogateToDateTimeOffset(DateTimeOffsetSurrogate surrogate) =>
            new(surrogate.Ticks, TimeSpan.Zero);
    }
}