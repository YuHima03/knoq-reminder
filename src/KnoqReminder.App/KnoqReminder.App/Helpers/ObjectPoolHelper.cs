using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;

namespace KnoqReminder.App.Helpers;

static class ObjectPoolHelper
{
    public static IServiceCollection AddDefaultObjectPool(this IServiceCollection services)
    {
        services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.TryAddSingleton(typeof(ObjectPool<>), typeof(DefaultObjectPool<>));
        return services;
    }
}

sealed class DefaultObjectPool<T>(ObjectPoolProvider provider) : ObjectPool<T>
    where T : class
{
    readonly ObjectPool<T> _innerPool = provider.Create(new PooledObjectPolicySlim<T>());

    public override T Get() => _innerPool.Get();

    public override void Return(T obj) => _innerPool.Return(obj);
}

file sealed class PooledObjectPolicySlim<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : IPooledObjectPolicy<T>
    where T : class
{
    static readonly Func<T> DefaultConstructor = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();

    public T Create() => DefaultConstructor.Invoke();

    public bool Return(T obj) => true;
}
