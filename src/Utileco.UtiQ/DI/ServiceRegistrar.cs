using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Utileco.UtiQ.DI
{
    public static class ServiceRegistrar
    {
        public static void AddUtiQClasses(IServiceCollection services, UtiQServiceConfiguration configuration)
        {
            var assembliesToScan = configuration.AssembliesToRegister.Distinct().ToArray();

            ConnectImplementationsToTypesClosing(typeof(ICommandHandler<>), services, assembliesToScan, false, configuration);
            ConnectImplementationsToTypesClosing(typeof(ICommandHandler<,>), services, assembliesToScan, false, configuration);
            ConnectImplementationsToTypesClosing(typeof(IQueryHandler<,>), services, assembliesToScan, false, configuration);

            var multiOpenInterfaces = new[]
            {
                typeof(ICommandHandler<>),
                typeof(ICommandHandler<,>),
                typeof(IQueryHandler<,>)
            };

            foreach (var multiOpenInterface in multiOpenInterfaces)
            {
                var ar = multiOpenInterface.GetGenericArguments().Length;

                var concretions = assembliesToScan
                    .SelectMany(x => x.DefinedTypes)
                    .Where(type => type.FindInterfacesThatClose(multiOpenInterface).Any())
                    .Where(type => type.IsConcrete() && type.IsOpenGeneric())
                    .Where(type => type.GetGenericArguments().Length == ar)
                    .Where(configuration.TypeEvaluator)
                    .ToList();

                foreach (var type in concretions)
                {
                    services.AddTransient(multiOpenInterface, type);
                }
            }
        }

        private static void ConnectImplementationsToTypesClosing(Type openRequestInterface,
            IServiceCollection services,
            IEnumerable<Assembly> assembliesToScan,
            bool addIfAlreadyExists,
            UtiQServiceConfiguration configuration)
        {
            var concretions = new List<Type>();
            var interfaces = new List<Type>();
            foreach (var type in assembliesToScan.SelectMany(a => a.DefinedTypes).Where(t => !t.IsOpenGeneric()).Where(configuration.TypeEvaluator))
            {
                var interfaceTypes = type.FindInterfacesThatClose(openRequestInterface).ToArray();
                if (!interfaceTypes.Any()) continue;

                if (type.IsConcrete())
                {
                    concretions.Add(type);
                }

                foreach (var interfaceType in interfaceTypes)
                {
                    interfaces.Fill(interfaceType);
                }
            }

            foreach (var @interface in interfaces)
            {
                var exactMatches = concretions.Where(x => x.CanBeCastTo(@interface)).ToList();
                if (addIfAlreadyExists)
                {
                    foreach (var type in exactMatches)
                    {
                        services.AddTransient(@interface, type);
                    }
                }
                else
                {
                    if (exactMatches.Count > 1)
                    {
                        exactMatches.RemoveAll(m => !IsMatchingWithInterface(m, @interface));
                    }

                    foreach (var type in exactMatches)
                    {
                        services.TryAddTransient(@interface, type);
                    }
                }

                if (!@interface.IsOpenGeneric())
                {
                    AddConcretionsThatCouldBeClosed(@interface, concretions, services);
                }
            }
        }

        private static bool IsMatchingWithInterface(Type? handlerType, Type handlerInterface)
        {
            if (handlerType == null || handlerInterface == null)
            {
                return false;
            }

            if (handlerType.IsInterface)
            {
                if (handlerType.GenericTypeArguments.SequenceEqual(handlerInterface.GenericTypeArguments))
                {
                    return true;
                }
            }
            else
            {
                return IsMatchingWithInterface(handlerType.GetInterface(handlerInterface.Name), handlerInterface);
            }

            return false;
        }

        private static void AddConcretionsThatCouldBeClosed(Type @interface, List<Type> concretions, IServiceCollection services)
        {
            foreach (var type in concretions
                         .Where(x => x.IsOpenGeneric() && x.CouldCloseTo(@interface)))
            {
                try
                {
                    services.TryAddTransient(@interface, type.MakeGenericType(@interface.GenericTypeArguments));
                }
                catch (Exception)
                {
                }
            }
        }

        private static bool CouldCloseTo(this Type openConcretion, Type closedInterface)
        {
            var openInterface = closedInterface.GetGenericTypeDefinition();
            var arguments = closedInterface.GenericTypeArguments;

            var concreteArguments = openConcretion.GenericTypeArguments;
            return arguments.Length == concreteArguments.Length && openConcretion.CanBeCastTo(openInterface);
        }

        private static bool CanBeCastTo(this Type pluggedType, Type pluginType)
        {
            if (pluggedType == null) return false;

            if (pluggedType == pluginType) return true;

            return pluginType.IsAssignableFrom(pluggedType);
        }

        private static bool IsOpenGeneric(this Type type)
        {
            return type.IsGenericTypeDefinition || type.ContainsGenericParameters;
        }

        private static IEnumerable<Type> FindInterfacesThatClose(this Type pluggedType, Type templateType)
        {
            return FindInterfacesThatClosesCore(pluggedType, templateType).Distinct();
        }

        private static IEnumerable<Type> FindInterfacesThatClosesCore(Type pluggedType, Type templateType)
        {
            if (pluggedType == null) yield break;

            if (!pluggedType.IsConcrete()) yield break;

            if (templateType.IsInterface)
            {
                foreach (
                    var interfaceType in
                    pluggedType.GetInterfaces()
                        .Where(type => type.IsGenericType && (type.GetGenericTypeDefinition() == templateType)))
                {
                    yield return interfaceType;
                }
            }
            else if (pluggedType.BaseType!.IsGenericType &&
                     (pluggedType.BaseType!.GetGenericTypeDefinition() == templateType))
            {
                yield return pluggedType.BaseType!;
            }

            if (pluggedType.BaseType == typeof(object)) yield break;

            foreach (var interfaceType in FindInterfacesThatClosesCore(pluggedType.BaseType!, templateType))
            {
                yield return interfaceType;
            }
        }

        private static bool IsConcrete(this Type type)
        {
            return !type.IsAbstract && !type.IsInterface;
        }

        private static void Fill<T>(this IList<T> list, T value)
        {
            if (list.Contains(value)) return;
            list.Add(value);
        }

        public static void AddRequiredServices(IServiceCollection services, UtiQServiceConfiguration serviceConfiguration)
        {
            services.TryAdd(new ServiceDescriptor(typeof(IUtiQ), serviceConfiguration.UtiQImplementationType, serviceConfiguration.Lifetime));

            foreach (var serviceDescriptor in serviceConfiguration.BehaviorsToRegister)
            {
                services.TryAddEnumerable(serviceDescriptor);
            }
        }

        private static void RegisterBehaviorIfImplementationsExist(IServiceCollection services, Type behaviorType, Type subBehaviorType)
        {
            var hasAnyRegistrationsOfSubBehaviorType = services
                .Select(service => service.ImplementationType)
                .OfType<Type>()
                .SelectMany(type => type.GetInterfaces())
                .Where(type => type.IsGenericType)
                .Select(type => type.GetGenericTypeDefinition())
                .Any(type => type == subBehaviorType);

            if (hasAnyRegistrationsOfSubBehaviorType)
            {
                services.TryAddEnumerable(new ServiceDescriptor(typeof(IPipelineBehavior<,>), behaviorType, ServiceLifetime.Transient));
            }
        }
    }
}
