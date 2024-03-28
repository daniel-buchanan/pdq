using System;
using System.Linq;
using decaf.common;
using decaf.common.Connections;
using decaf.common.Exceptions;
using decaf.common.Logging;
using decaf.common.Options;
using decaf.common.Utilities;
using decaf.common.Utilities.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace decaf
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add decaf to your <see cref="IServiceCollection"/>, setting the default options.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add to.</param>
        /// <returns></returns>
        public static IDecafOrmServiceCollection AddDecaf(this IServiceCollection services)
            => AddDecaf(services, new DecafOptions());

        /// <summary>
        /// Add decaf to your <see cref="IServiceCollection"/>, providing an action
        /// which will resolve the options.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add to.</param>
        /// <param name="options">An <see cref="Action{T}"/> that resolves the decaf options.</param>
        /// <returns></returns>
        public static IDecafOrmServiceCollection AddDecaf(
            this IServiceCollection services,
            Action<IDecafOptionsBuilder> options)
        {
            var b = new DecafOptionsBuilder(services);
            options(b);
            return AddDecaf(services, b.Build());
        }

        /// <summary>
        /// Add decaf to your <see cref="IServiceCollection"/>, providing an instance
        /// of pre-configured decaf options to be used.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add to.</param>
        /// <param name="options">A set of pre-configured <see cref="DecafOptions"/>.</param>
        /// <returns></returns>
        public static IDecafOrmServiceCollection AddDecaf(
            this IServiceCollection services,
            DecafOptions options)
        {
            ValidateOptions(options);

            services.AddSingleton(options);
            services.AddTransient<IDecaf, Decaf>();
            services.AddSingleton<IHashProvider, HashProvider>();
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddSingleton<IReflectionHelper, ReflectionHelper>();
            services.AddSingleton<IExpressionHelper, ExpressionHelper>();

            if (options.InjectUnitOfWork)
            {
                services.Add(new ServiceDescriptor(typeof(IUnitOfWork), GetUnitOfWork, options.UnitOfWorkLifetime));
            }

            if (services.FirstOrDefault(sd => sd.ServiceType == typeof(ILoggerProxy)) == null)
            {
                services.AddSingleton<IStdOutputWrapper, StdOutputWrapper>();
                services.AddScoped<ILoggerProxy, DefaultLoggerProxy>();
            }

            return DecafOrmServiceCollection.Create(services);
        }

        private static IUnitOfWork GetUnitOfWork(IServiceProvider p)
        {
            var factory = p.GetRequiredService<IUnitOfWorkFactory>();
            var connectionDetails = p.GetRequiredService<IConnectionDetails>();
            return factory.Create(connectionDetails);
        }

        private static void ValidateOptions(DecafOptions options)
        {
            if (options == null)
            {
                throw new DecafOptionsInvalidException($"The provided {nameof(options)} is NULL.");
            }
        }
    }
}

