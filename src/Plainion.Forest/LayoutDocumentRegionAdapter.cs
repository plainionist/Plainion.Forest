using System;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;
using Plainion;
using Xceed.Wpf.AvalonDock.Layout;

namespace Plainion.Forest
{
    [Export( typeof( LayoutDocumentRegionAdapter ) )]
    public class LayoutDocumentRegionAdapter : RegionAdapterBase<LayoutDocument>
    {
        [ImportingConstructor]
        public LayoutDocumentRegionAdapter( IRegionBehaviorFactory regionBehaviorFactory )
            : base( regionBehaviorFactory )
        {
        }

        protected override void Adapt( IRegion region, LayoutDocument regionTarget )
        {
            Contract.RequiresNotNull( regionTarget, "regionTarget" );

            region.ActiveViews.CollectionChanged += delegate
            {
                var view = region.ActiveViews.FirstOrDefault();
                if (view ==null)
                {
                    return;
                }

                if( regionTarget.Content == null )
                {
                    regionTarget.Content = view;
                }
                else
                {
                    var contentControl = regionTarget.Content as ContentControl;
                    if( contentControl != null )
                    {
                        Contract.Requires( contentControl.Content == null, "ContentControl.Content must be null if ContentControl is provided" );
                        contentControl.Content = view;
                    
                        // Content in a tabcontrol is only visible once it is selected/visible
                        regionTarget.IsSelected = true;
                    }
                    else
                    {
                        throw new NotSupportedException( string.Format( "Child of type '{0}' not supported", contentControl.GetType() ) );
                    }
                }
            };

            region.Views.CollectionChanged += ( sender, e ) =>
                {
                    if( e.Action == NotifyCollectionChangedAction.Add && region.ActiveViews.Count() == 0 )
                    {
                        region.Activate( e.NewItems[ 0 ] );
                    }
                };
        }

        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }
    }
}
