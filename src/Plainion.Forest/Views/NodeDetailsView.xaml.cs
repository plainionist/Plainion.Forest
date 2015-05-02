using System.ComponentModel.Composition;
using System.Windows.Controls;
using Plainion.Forest.ViewModels;

namespace Plainion.Forest.Views
{
    /// <summary>
    /// Interaction logic for NodeDetailsView.xaml
    /// </summary>
    [Export( typeof( NodeDetailsView ) )]
    public partial class NodeDetailsView : UserControl
    {
        [ImportingConstructor]
        public NodeDetailsView( NodeDetailsViewModel model )
        {
            InitializeComponent();

            DataContext = model;
        }
    }
}
