using System;
using System.Threading.Tasks;
using Microsoft.FeatureManagement;

namespace Kevsoft.FeatureManagement.BranchByAbstraction
{
    public class BranchFactory<TInterface, TImpl1, TImpl2>
        : IBranchFactory<TInterface>
        where TImpl1 : class, TInterface
        where TImpl2 : class, TInterface
    {
        private readonly IFeatureManager _featureManager;
        private readonly Func<TImpl1> _factory1;
        private readonly Func<TImpl2> _factory2;
        private readonly string _featureName;

        public BranchFactory(
            IFeatureManager featureManager,
            Func<TImpl1> factory1,
            Func<TImpl2> factory2,
            string featureName)
        {
            _featureManager = featureManager;
            _factory1 = factory1;
            _factory2 = factory2;
            _featureName = featureName;
        }

        public async Task<TInterface> Create()
        {
            if (await _featureManager.IsEnabledAsync(_featureName))
            {
                return _factory1();
            }

            return _factory2();
        }
    }
}