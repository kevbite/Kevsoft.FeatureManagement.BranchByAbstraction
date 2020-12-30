using System.Threading.Tasks;

namespace Kevsoft.FeatureManagement.BranchByAbstraction
{
    public interface IBranchFactory<TInterface>
    {
        Task<TInterface> Create();
    }
}