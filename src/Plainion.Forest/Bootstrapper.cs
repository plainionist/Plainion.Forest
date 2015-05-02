using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Plainion.Forest.Model;
using Microsoft.Practices.Prism.Regions;
using Plainion.AppFw.Wpf;
using Plainion.AppFw.Wpf.Services;
using Plainion.AppFw.Wpf.ViewModels;
using Plainion.Prism.Interactivity;
using Xceed.Wpf.AvalonDock.Layout;

namespace Plainion.Forest
{
    public class Bootstrapper : BootstrapperBase<Shell>
    {
        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

            // Prism automatically loads the module with that line
            AggregateCatalog.Catalogs.Add( new AssemblyCatalog( GetType().Assembly ) );
            AggregateCatalog.Catalogs.Add( new AssemblyCatalog( typeof( PopupWindowActionRegionAdapter ).Assembly ) );

            AggregateCatalog.Catalogs.Add( new TypeCatalog(
                typeof( PersistenceService<Project> ),
                typeof( ProjectLifecycleViewModel<Project> ),
                typeof( TitleViewModel<Project> )
                ) );
        }

        protected override CompositionContainer CreateContainer()
        {
            var container = base.CreateContainer();

            // required to parameterize the TreeViewModel
            container.ComposeExportedValue( container );
            container.ComposeExportedValue<INodeViewModelFactory>( new NodeViewModelFactory( container ) );

            return container;
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            var mappings = base.ConfigureRegionAdapterMappings();
            mappings.RegisterMapping( typeof( LayoutDocument ), this.Container.GetExportedValue<LayoutDocumentRegionAdapter>() );
            return mappings;
        }
    }
}
