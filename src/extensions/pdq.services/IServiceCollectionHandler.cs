using Microsoft.Extensions.DependencyInjection;

namespace pdq.services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IServiceCollectionHandler
	{
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		IServiceCollection AsScoped();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		IServiceCollection AsSingleton();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		IServiceCollection AsTransient();
	}
}

