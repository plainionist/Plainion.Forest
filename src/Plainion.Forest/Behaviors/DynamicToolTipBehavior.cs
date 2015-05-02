using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Plainion.Forest.Behaviors
{
    public class DynamicToolTipBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty ContentProperty =
             DependencyProperty.Register( "Content", typeof( string ),
             typeof( DynamicToolTipBehavior ), new FrameworkPropertyMetadata( null ) );

        public string Content
        {
            get { return ( string )GetValue( ContentProperty ); }
            set { SetValue( ContentProperty, value ); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.IsMouseDirectlyOverChanged += OnIsMouseDirectlyOverChanged;
        }

        private void OnIsMouseDirectlyOverChanged( object sender, DependencyPropertyChangedEventArgs e )
        {
            AssociatedObject.ToolTip = Content;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.IsMouseDirectlyOverChanged -= OnIsMouseDirectlyOverChanged;

            base.OnDetaching();
        }
    }
}
