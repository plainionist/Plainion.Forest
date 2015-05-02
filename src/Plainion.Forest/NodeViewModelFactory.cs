using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Plainion.Forest.Model;
using Plainion.Forest.ViewModels;

namespace Plainion.Forest
{
    class NodeViewModelFactory : INodeViewModelFactory
    {
        private CompositionContainer myContainer;

        [ImportingConstructor]
        public NodeViewModelFactory( CompositionContainer container )
        {
            myContainer = container;
        }

        public NodeViewModel Create( INode node )
        {
            var catalog = new TypeCatalog( typeof( NodeViewModel ) );

            using ( var scope = new CompositionContainer( catalog, myContainer ) )
            {
                scope.ComposeExportedValue( node );

                return scope.GetExportedValue<NodeViewModel>();
            }
        }

    }
}
