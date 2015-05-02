using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Plainion.Forest.Events;
using Plainion.Forest.Model;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using Plainion.AppFw.Wpf.Services;

namespace Plainion.Forest.ViewModels
{
    [Export( typeof( NodeDetailsViewModel ) )]
    public class NodeDetailsViewModel : BindableBase
    {
        private INode myNode;
        private bool myIsSelected;
        private bool myIsCaptionFocused;
        private IProjectService<Project> myProjectService;

        [ImportingConstructor]
        public NodeDetailsViewModel( IEventAggregator eventAggregator, IProjectService<Project> projectService )
        {
            myProjectService = projectService;

            eventAggregator.GetEvent<NodeSelectedEvent>().Subscribe( node => SetNode( node ) );
            eventAggregator.GetEvent<NodeActivatedEvent>().Subscribe( node => ActivateNode( node ) );
        }

        private void ActivateNode( INode node )
        {
            SetNode( node );
            IsSelected = true;

            Task.Run( () =>
                {
                    Thread.Sleep( 250 );
                    Dispatcher.CurrentDispatcher.Invoke( new Action( () => IsCaptionFocused = true ) );
                } );
        }

        private void SetNode( INode node )
        {
            myNode = node;

            OnPropertyChanged( "IsEnabled" );
            OnPropertyChanged( "Caption" );
            OnPropertyChanged( "Content" );
            OnPropertyChanged( "CreatedAt" );
            OnPropertyChanged( "LastModifiedAt" );
        }

        public string Caption
        {
            get { return myNode != null ? myNode.Caption : string.Empty; }
            set { myProjectService.Project.ChangeCaption( myNode, value ); }
        }

        public bool IsEnabled
        {
            get { return myNode != null; }
        }

        public string Title
        {
            get { return "Editor"; }
        }

        // needed for tab selection
        public bool IsSelected
        {
            get { return myIsSelected; }
            set { SetProperty( ref myIsSelected, value ); }
        }

        public bool IsCaptionFocused
        {
            get { return myIsCaptionFocused; }
            set { SetProperty( ref myIsCaptionFocused, value ); }
        }

        public string Content
        {
            get { return myNode != null ? myNode.Content : string.Empty; }
            set { myProjectService.Project.ChangeContent( myNode, value ); }
        }

        public DateTime CreatedAt
        {
            get { return myNode != null ? myNode.CreatedAt : DateTime.Now; }
        }

        public DateTime LastModifiedAt
        {
            get { return myNode != null ? myNode.LastModifiedAt : DateTime.Now; }
        }
    }
}
