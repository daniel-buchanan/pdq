using Microsoft.Extensions.DependencyInjection;

namespace pdq.services
{
	public static class ServiceCollectionExtensions
	{
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollectionHandler AddPdqService<TEntity>(this IServiceCollection services)
            where TEntity : class, IEntity => ServiceCollectionHandler<TEntity>.Create(services);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollectionHandler AddPdqService<TEntity, TKey>(this IServiceCollection services)
            where TEntity : class, IEntity<TKey> => ServiceCollectionHandler<TEntity, TKey>.Create(services);
    }
}

