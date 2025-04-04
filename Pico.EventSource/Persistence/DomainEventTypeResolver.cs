using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Pico.EventSource.Models;

namespace Pico.EventSource.Persistence;

public class DomainEventTypeResolver : DefaultJsonTypeInfoResolver
{
    private static List<JsonDerivedType>? _derivedTypes;

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        if (jsonTypeInfo.Type != typeof(IDomainEvent)) return jsonTypeInfo;

        jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
        {
            TypeDiscriminatorPropertyName = "$event-type",
            IgnoreUnrecognizedTypeDiscriminators = true,
            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
        };

        IList<JsonDerivedType> eventTypes = GetAllEventTypes();

        foreach (JsonDerivedType jsonDerivedType in eventTypes)
        {
            jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(jsonDerivedType);
        }

        return jsonTypeInfo;
    }

    private static List<JsonDerivedType> GetAllEventTypes()
    {
        if (_derivedTypes != null) return _derivedTypes;

        _derivedTypes = [];

        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (Assembly assembly in assemblies)
        {
            IEnumerable<Type> eventTypes = assembly
                .GetTypes()
                .Where(type => typeof(IDomainEvent).IsAssignableFrom(type) && type is
                {
                    IsInterface: false,
                    IsAbstract: false
                });

            _derivedTypes.AddRange(eventTypes.Select(eventType => new JsonDerivedType(eventType, eventType.Name)));
        }

        return _derivedTypes;
    }
}