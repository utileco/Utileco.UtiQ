using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Utileco.UtiQ.DI
{
    public class UtiQServiceConfiguration
    {
        public Func<Type, bool> TypeEvaluator { get; set; } = t => true;

        public Type UtiQImplementationType { get; set; } = typeof(Utiqueue);

        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;

        public RequestExceptionActionProcessorStrategy RequestExceptionActionProcessorStrategy { get; set; }
            = RequestExceptionActionProcessorStrategy.ApplyForUnhandledExceptions;

        internal List<Assembly> AssembliesToRegister { get; } = new();

        public List<ServiceDescriptor> BehaviorsToRegister { get; } = new();

        public UtiQServiceConfiguration RegisterServicesFromAssemblyContaining<T>()
            => RegisterServicesFromAssemblyContaining(typeof(T));

        public UtiQServiceConfiguration RegisterServicesFromAssemblyContaining(Type type)
            => RegisterServicesFromAssemblyContaining(type.Assembly);

        public UtiQServiceConfiguration RegisterServicesFromAssemblyContaining(Assembly assembly)
        {
            AssembliesToRegister.Add(assembly);
            return this;
        }

        public UtiQServiceConfiguration RegisterServicesFromAssemblyContaining(params Assembly[] assemblies)
        {
            AssembliesToRegister.AddRange(assemblies);
            return this;
        }

        public UtiQServiceConfiguration RegisterServicesFromAssembly(Assembly assembly)
        {
            AssembliesToRegister.Add(assembly);

            return this;
        }

        public UtiQServiceConfiguration RegisterServicesFromAssemblies(
            params Assembly[] assemblies)
        {
            AssembliesToRegister.AddRange(assemblies);

            return this;
        }

        public UtiQServiceConfiguration AddBehavior<TServiceType, TImplementationType>(ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            => AddBehavior(typeof(TServiceType), typeof(TImplementationType), serviceLifetime);

        public UtiQServiceConfiguration AddBehavior(Type serviceType, Type implementatinType, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            BehaviorsToRegister.Add(new ServiceDescriptor(serviceType, implementatinType, serviceLifetime));
            return this;
        }

        public UtiQServiceConfiguration AddOpenBehavior(Type openBehaviorType, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            if (!openBehaviorType.IsGenericType)
            {
                throw new InvalidOperationException($"{openBehaviorType.Name} must be generic");
            }

            var implementedGenericInterfaces = openBehaviorType.GetInterfaces().Where(i => i.IsGenericType).Select(i => i.GetGenericTypeDefinition());
            var implementedOpenBehaviorInterfaces = new HashSet<Type>(implementedGenericInterfaces.Where(i => i == typeof(IPipelineBehavior<,>)));

            if (implementedOpenBehaviorInterfaces.Count == 0)
            {
                throw new InvalidOperationException($"{openBehaviorType.Name} must implement {typeof(IPipelineBehavior<,>).FullName}");
            }

            foreach (var openBehaviorInterface in implementedOpenBehaviorInterfaces)
            {
                BehaviorsToRegister.Add(new ServiceDescriptor(openBehaviorInterface, openBehaviorType, serviceLifetime));
            }

            return this;
        }
    }
}
