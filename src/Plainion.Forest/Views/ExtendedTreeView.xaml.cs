using System.ComponentModel.Composition;
using System.Windows.Controls;
using Plainion.Forest.ViewModels;

namespace Plainion.Forest.Views
{
    [PartCreationPolicy( CreationPolicy.NonShared )]
    [Export( typeof( ExtendedTreeView ) )]
    public partial class ExtendedTreeView : UserControl
    {
        [ImportingConstructor]
        public ExtendedTreeView( TreeViewModel model )
        {
            InitializeComponent();

            Model = model;
            DataContext = model;
        }

        public TreeViewModel Model
        {
            get;
            private set;
        }
    }
}
