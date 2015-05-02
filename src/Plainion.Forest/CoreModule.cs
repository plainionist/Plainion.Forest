using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Plainion.Forest.Views;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace Plainion.Forest
{
    [ModuleExport( typeof( CoreModule ) )]
    public class CoreModule : IModule
    {
        private CompositionContainer myContainer;

        [ImportingConstructor]
        public CoreModule( CompositionContainer container )
        {
            myContainer = container;
        }

        [Import]
        public IRegionManager RegionManager { get; set; }

        public void Initialize()
        {
            RegionManager.RegisterViewWithRegion( RegionNames.NodeDetails, typeof( NodeDetailsView ) );

            RegionManager.RegisterViewWithRegion( RegionNames.Planning, () =>
            {
                var view = myContainer.GetExportedValue<ExtendedTreeView>();
                view.Model.Scope = "Planning";
                view.Model.AutoExpandAllAfterLoaded = true;
                return view;
            } );

            // XXX: add at latest to have it selected initially :(
            RegionManager.RegisterViewWithRegion( RegionNames.Backlog, () =>
            {
                var view = myContainer.GetExportedValue<ExtendedTreeView>();
                view.Model.Scope = "Backlog";
                view.Model.ShowContentHint = true;
                return view;
            } );
        }
    }
}
