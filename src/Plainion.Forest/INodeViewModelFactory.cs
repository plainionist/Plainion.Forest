using Plainion.Forest.Model;
using Plainion.Forest.ViewModels;

namespace Plainion.Forest
{
    public interface INodeViewModelFactory
    {
        NodeViewModel Create( INode node );
    }
}
