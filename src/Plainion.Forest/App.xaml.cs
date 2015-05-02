using System.Windows;
using System.Windows.Threading;

namespace Plainion.Forest
{
    public partial class App : Application
    {
        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );

            new Bootstrapper().Run();
        }
    }
}
