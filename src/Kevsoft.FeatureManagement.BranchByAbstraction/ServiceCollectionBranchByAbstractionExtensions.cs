using System;
using Kevsoft.FeatureManagement.BranchByAbstraction;
using Microsoft.FeatureManagement;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionBranchByAbstractionExtensions
    {
        /// <summary>
        /// Configures a service collection with a given feature to switch the type based on the branch by abstraction techniques. 
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="featureName">The name of the feature in which to toggle the type.</param>
        /// <param name="featureSnapshot">If to use a snapshot of feature state to ensure consistency across a given request.</param>
        /// <typeparam name="TInterface">The shared interface type which is implemented on both implementations.</typeparam>
        /// <typeparam name="TFeatureEnabledImpl">The type to use when the feature is enabled.</typeparam>
        /// <typeparam name="TFeatureDisabledImpl">The type to use when the feature is disabled.</typeparam>
        /// <returns></returns>
        public static IServiceCollection AddBranchByAbstraction<TInterface, TFeatureEnabledImpl, TFeatureDisabledImpl>(this IServiceCollection services,
            string featureName, bool featureSnapshot = true)
            where TFeatureEnabledImpl : class, TInterface
            where TFeatureDisabledImpl : class, TInterface
        {
            services.AddTransient<TFeatureEnabledImpl>();
            services.AddSingleton<Func<TFeatureEnabledImpl>>(x => x.GetRequiredService<TFeatureEnabledImpl>);
            
            services.AddTransient<TFeatureDisabledImpl>();
            services.AddSingleton<Func<TFeatureDisabledImpl>>(x => x.GetRequiredService<TFeatureDisabledImpl>);
            
            IFeatureManager GetFeatureManagerService(IServiceProvider provider)
            {
                return featureSnapshot switch
                {
                    true => provider.GetRequiredService<IFeatureManagerSnapshot>(),
                    false =>provider.GetRequiredService<IFeatureManager>(),
                };
            }
            
            services.AddTransient<IBranchFactory<TInterface>>(provider =>
                new BranchFactory<TInterface, TFeatureEnabledImpl, TFeatureDisabledImpl>(
                    GetFeatureManagerService(provider),
                    provider.GetRequiredService<Func<TFeatureEnabledImpl>>(),
                    provider.GetRequiredService<Func<TFeatureDisabledImpl>>(),
                    featureName
                ));

            return services;
        }
        
    }
}